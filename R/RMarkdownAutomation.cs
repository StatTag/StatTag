using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDotNet;
using StatTag.Core.Models;
using StatTag.Core.Parser;
using StatTag.Core.Utility;

namespace R
{
    public class RMarkdownAutomation : RAutomation
    {
        private const string TemporaryImageFileFolder = "STTemp-Figures";
        private const string TemporaryImageFileFilter = "*.png";
        private string TemporaryImageFilePath = "";

        public RMarkdownAutomation()
        {
            Parser = new RMarkdownParser();
            State = new StatPackageState();
        }

        public override bool Initialize(CodeFile file, LogManager logger)
        {
            // Once the base initialization is done, we know we have an active R engine that we can access
            if (!base.Initialize(file, logger))
            {
                return false;
            }

            // Initialize the temporary directory we'll use for figures
            var path = Path.GetDirectoryName(file.FilePath);
            if (!string.IsNullOrEmpty(path))
            {
                TemporaryImageFilePath = Path.Combine(path, TemporaryImageFileFolder);
                logger.WriteMessage(string.Format("Creating a temporary image folder at full path: {0}", TemporaryImageFilePath));
            }
            else
            {
                // If we don't know what the path is, we'll just create a relative path and hope for the best.
                TemporaryImageFilePath = string.Format(".\\{0}", TemporaryImageFileFolder);
                logger.WriteMessage(string.Format("Creating temporary image folder at relative path: {0}", TemporaryImageFilePath));
            }

            // Make sure the temp image folder is established.
            if (Directory.Exists(TemporaryImageFilePath))
            {
                logger.WriteMessage("Temporary image folder already exists");
            }
            else
            {
                logger.WriteMessage("Temporary image folder does not exist.  We will create it.");
                Directory.CreateDirectory(TemporaryImageFilePath);
                logger.WriteMessage("Created the temporary image folder");
            }

            // Now, set up the R environment so it uses the PNG graphic device by default (if no other device
            // is specified).
            logger.WriteMessage("Setting R option for default graphics device");
            RunCommands(new[]
            {
                string.Format("stattag_png = function() {{ png(filename=paste(\"{0}\\\\\", \"StatTagFigure%03d.png\", sep=\"\")) }}",
                    TemporaryImageFilePath.Replace("\\", "\\\\").Replace("\"", "\\\"")),
                "options(device=\"stattag_png\")"
            });
            logger.WriteMessage("Updated R option for default graphics device");

            return true;
        }

        /// <summary>
        /// Handler after a command has run to see if it should be processed as an image result.
        /// </summary>
        /// <param name="tag">Assumed to always be set, the tag we just processed results for</param>
        /// <param name="command">The code that was executed</param>
        /// <param name="result">Result from R for the command</param>
        /// <returns></returns>
        public override CommandResult HandleImageResult(Tag tag, string command, SymbolicExpression result)
        {
            // If this is recognized by the parser as an image export, we have all of the information contained
            // in the command to find the figure file.  We can rely on the base RAutomation implementation to
            // handle this.
            if (Parser.IsImageExport(command))
            {
                return base.HandleImageResult(tag, command, result);
            }

            if (!tag.Type.Equals(Constants.TagType.Figure))
            {
                return null;
            }

            // Make sure the graphics device is closed. We're going with the assumption that a device
            // was open and a figure was written out.
            RunCommand("dev.off()");

            // If we don't have the file specified normally, we will use our fallback mechanism of writing to a
            // temporary directory.
            var files = Directory.GetFiles(TemporaryImageFilePath, TemporaryImageFileFilter);
            if (files.Length == 0)
            {
                return null;
            }

            // Find the last file in the directory.  We anticipate there would normally only be 1, but since 
            // several commands could be run, we will just take the last one.
            var tempImageFile = files.Last();
            var correctedPath = Path.GetFullPath(Path.Combine(TemporaryImageFilePath, ".."));
            var imageFile = Path.Combine(correctedPath, string.Format("{0}.png", TagUtil.TagNameAsFileName(tag)));
            File.Copy(tempImageFile, imageFile, true);

            return new CommandResult() { FigureResult = imageFile };
        }

        protected override void PreEvaluateHook(string command, Tag tag = null)
        {
            // Before we run a figure, clean out our temporary directory.  That way we know any figure written out is
            // from the command about to be run.
            if (tag != null && tag.Type.Equals(Constants.TagType.Figure))
            {
                CleanTemporaryImageFolder();
            }
        }

        /// <summary>
        /// Helper method to clean out the temporary folder used for storing images
        /// </summary>
        /// <param name="deleteFolder"></param>
        private void CleanTemporaryImageFolder(bool deleteFolder = false)
        {
            if (Directory.Exists(TemporaryImageFilePath))
            {
                // TODO: Can we make this less brute-force?  We want to ensure we're not trying to access
                // a file that's associated with an open graphics device.
                RunCommand("graphics.off()");

                var files = Directory.GetFiles(TemporaryImageFilePath, TemporaryImageFileFilter);
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                if (deleteFolder)
                {
                    Directory.Delete(TemporaryImageFilePath);
                }
            }
        }

        /// <summary>
        /// Our cleanup for R Markdown involves removing the temporary folder we created for images.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            CleanTemporaryImageFolder(true);
        }
    }
}
