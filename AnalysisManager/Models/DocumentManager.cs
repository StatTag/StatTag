using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AnalysisManager.Core.Models;
using Microsoft.Office.Interop.Word;

namespace AnalysisManager.Models
{
    public class DocumentManager
    {
        public List<CodeFile> Files { get; set; }

        public const string ConfigurationAttribute = "Analysis Manager Configuration";

        public DocumentManager()
        {
            Files = new List<CodeFile>();
        }

        /// <summary>
        /// Provider a wrapper to check if a variable exists in the document.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        protected bool DocumentVariableExists(Variable variable)
        {
            // The Word interop doesn't provide a nice check mechanism, and uses exceptions instead.
            try
            {

                var value = variable.Value;
                return true;
            }
            catch (COMException exc)
            {
                return false;
            }
        }

        public void SaveFilesToDocument(Document document)
        {
            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                var attribute = CodeFile.SerializeList(Files);
                if (!DocumentVariableExists(variable))
                {
                    variables.Add(ConfigurationAttribute, attribute);
                }
                else
                {
                    variable.Value = attribute;
                }
            }
            finally
            {
                Marshal.ReleaseComObject(variables);
                Marshal.ReleaseComObject(variable);
            }
        }

        public void LoadFilesFromDocument(Document document)
        {
            var variables = document.Variables;
            var variable = variables[ConfigurationAttribute];
            try
            {
                Files = DocumentVariableExists(variable) ? CodeFile.DeserializeList(variable.Value) : new List<CodeFile>();
            }
            finally
            {
                Marshal.ReleaseComObject(variables);
                Marshal.ReleaseComObject(variable);
            }
        }
    }
}
