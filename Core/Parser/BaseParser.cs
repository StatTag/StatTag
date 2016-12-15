using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatTag.Core.Interfaces;
using StatTag.Core.Models;

namespace StatTag.Core.Parser
{
    public abstract class BaseParser : ICodeFileParser
    {
        public abstract string CommentCharacter { get; }

        protected Regex StartTagRegEx = null;
        protected Regex EndTagRegEx = null;

        protected BaseParser()
        {
            SetupRegEx();
        }

        public abstract bool IsImageExport(string command);
        public abstract string GetImageSaveLocation(string command);
        public abstract bool IsValueDisplay(string command);
        public abstract string GetValueName(string command);
        public abstract bool IsTableResult(string command);
        public abstract string GetTableName(string command);

        /// <summary>
        /// Perform any necessary pre-processing on the original code file content in order
        /// to allow it to be executed.  This may change the number of lines, so don't assume
        /// that the output will have a 1:1 line mapping from the original.
        /// </summary>
        /// <param name="originalContent">The contents as read from the code file</param>
        /// <returns>A list of strings representing the code that should be executed</returns>
        public abstract List<string> PreProcessContent(List<string> originalContent);

        protected Match DetectTag(Regex tagRegex, string line)
        {
            if (line == null)
            {
                line = string.Empty;
            }

            return tagRegex.Match(line);
        }

        private void SetupRegEx()
        {
            if (StartTagRegEx == null)
            {
                StartTagRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}\s*{2}(.*)", CommentCharacter, Constants.TagTags.StartTag, Constants.TagTags.TagPrefix), RegexOptions.Singleline);
            }

            if (EndTagRegEx == null)
            {
                EndTagRegEx = new Regex(string.Format(@"\s*[\{0}]{{2,}}\s*{1}", CommentCharacter, Constants.TagTags.EndTag, Constants.TagTags.TagPrefix), RegexOptions.Singleline);
            }
        }

        public static string FormatCommandListAsNonCapturingGroup(string[] commands)
        {
            if (commands.Length == 0)
            {
                return string.Empty;
            }

            return string.Format("(?:{0})",
                string.Join("|", commands.Select(x => x.Replace(" ", "\\s+"))));
        }

        public Tag[] Parse(CodeFile file,
            int filterMode = Constants.ParserFilterMode.IncludeAll,
            List<Tag> tagsToRun = null)
        {
            SetupRegEx();

            var tags = new List<Tag>();
            if (file == null)
            {
                return tags.ToArray();
            }

            var lines = file.LoadFileContent();
            if (lines == null)
            {
                return tags.ToArray();
            }

            int? startIndex = null;
            Tag tag = null;
            for (int index = 0; index < lines.Count(); index++)
            {
                var line = lines[index].Trim();
                var match = StartTagRegEx.Match(line);
                if (match.Success)
                {
                    tag = new Tag()
                    {
                        LineStart = index,
                        CodeFile = file
                    };
                    startIndex = index;
                    ProcessTag(match.Groups[1].Value, tag);
                }
                else if (startIndex.HasValue)
                {
                    match = EndTagRegEx.Match(line);
                    if (match.Success)
                    {
                        tag.LineStart = startIndex;
                        tag.LineEnd = index;
                        tags.Add(tag);
                        startIndex = null;
                    }
                }
            }

            if (filterMode == Constants.ParserFilterMode.ExcludeOnDemand)
            {
                return tags.Where(x => x.RunFrequency == Constants.RunFrequency.Always).ToArray();
            }
            else if (filterMode == Constants.ParserFilterMode.TagList && tagsToRun != null)
            {
                return tags.Where(x => tagsToRun.Contains(x)).ToArray();
            }

            return tags.ToArray();
        }

        public List<ExecutionStep> GetExecutionSteps(CodeFile file,
            int filterMode = Constants.ParserFilterMode.IncludeAll,
            List<Tag> tagsToRun = null)
        {
            var executionSteps = new List<ExecutionStep>();
            var lines = PreProcessContent(file.LoadFileContent());
            if (lines == null || lines.Count == 0)
            {
                return executionSteps;
            }

            int? startIndex = null;
            var isSkipping = false;
            ExecutionStep step = null;
            for (var index = 0; index < lines.Count(); index++)
            {
                var line = lines[index].Trim();

                if (step == null)
                {
                    step = new ExecutionStep();
                }

                var match = StartTagRegEx.Match(line);
                if (match.Success)
                {
                    // If the previous code block had content, save it off and create a new one
                    if (step.Code.Count > 0)
                    {
                        executionSteps.Add(step);
                        step = new ExecutionStep();
                    }

                    var tag = new Tag() { CodeFile = file };
                    startIndex = index;
                    ProcessTag(match.Groups[1].Value, tag);
                    if (filterMode == Constants.ParserFilterMode.ExcludeOnDemand
                        && tag.RunFrequency == Constants.RunFrequency.OnDemand)
                    {
                        isSkipping = true;
                    }
                    else if (filterMode == Constants.ParserFilterMode.TagList
                             && tagsToRun != null
                             && !tagsToRun.Contains(tag))
                    {
                        isSkipping = true;
                    }
                    else
                    {
                        step.Type = Constants.ExecutionStepType.Tag;
                        step.Tag = tag;
                        step.Code.Add(line);
                    }
                }
                else if (startIndex.HasValue)
                {
                    if (!isSkipping)
                    {
                        step.Code.Add(line);
                    }
                    
                    match = EndTagRegEx.Match(line);
                    if (match.Success)
                    {
                        isSkipping = false;
                        startIndex = null;

                        if (step.Code.Count > 0)
                        {
                            executionSteps.Add(step);
                        }
                        step = new ExecutionStep();
                    }
                }
                else if (!isSkipping)
                {
                    step.Code.Add(line);
                }
            }

            if (step != null && step.Code.Count > 0)
            {
                executionSteps.Add(step);
            }

            return executionSteps;
        }

        protected void ProcessTag(string tagText, Tag tag)
        {
            if (tagText.StartsWith(Constants.TagType.Value))
            {
                tag.Type = Constants.TagType.Value;
                ValueParameterParser.Parse(tagText, tag);
            }
            else if (tagText.StartsWith(Constants.TagType.Figure))
            {
                tag.Type = Constants.TagType.Figure;
                FigureParameterParser.Parse(tagText, tag);
            }
            else if (tagText.StartsWith(Constants.TagType.Table))
            {
                tag.Type = Constants.TagType.Table;
                TableParameterParser.Parse(tagText, tag);
                ValueParameterParser.Parse(tagText, tag);
            }
            else
            {
                //throw new Exception("Unsupported tag type");
            }
        }

        protected string MatchRegexReturnGroup(string text, Regex regex, int groupNum)
        {
            var match = regex.Match(text);
            if (match.Success)
            {
                return match.Groups[groupNum].Value.Trim();
            }

            return string.Empty;
        }

        protected string[] GlobalMatchRegexReturnGroup(string text, Regex regex, int groupNum)
        {
            var matches = regex.Matches(text);
            if (matches.Count == 0)
            {
                return null;
            }

            var results = matches.OfType<Match>().Select(match => match.Groups[groupNum].Value.Trim()).ToList();
            return results.ToArray();
        }


    }
}
