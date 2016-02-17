﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AnalysisManager.Core.Models;
using AnalysisManager.Models;

namespace AnalysisManager
{
    public static class UIUtility
    {
        public static IEnumerable<object> RemoveSelectedItems(DataGridView dgvItems, int checkColumn)
        {
            if (dgvItems == null)
            {
                return null;
            }

            dgvItems.CurrentCell = null;  //Force any changes to save

            var removeList = new List<DataGridViewRow>();
            for (int index = 0; index < dgvItems.Rows.Count; index++)
            {
                var item = dgvItems.Rows[index];
                var cell = item.Cells[checkColumn] as DataGridViewCheckBoxCell;
                if (cell != null && cell.Value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                {
                    removeList.Add(item);
                }
            }

            foreach (var item in removeList)
            {
                dgvItems.Rows.Remove(item);
            }

            return removeList.Select(x => x.Tag);
        }

        public static FileDialog FileDialogFactory(bool isOpenFile)
        {
            if (isOpenFile)
            {
                return new OpenFileDialog();
            }

            return new SaveFileDialog();
        }

        public static string GetFileName(string filter, bool isOpenFile = true)
        {
            FileDialog openFile = FileDialogFactory(isOpenFile);
            openFile.Filter = filter;
            if (DialogResult.OK == openFile.ShowDialog())
            {
                return openFile.FileName;
            }

            return null;
        }

        public static string GetAddInName()
        {
            return "Analysis Manager";
        }

        public static string GetVersionLabel()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0} v{1}.{2}.{3}", GetAddInName(), version.Major, version.Minor, version.Revision);
        }

        public static void WarningMessageBox(string text)
        {
            MessageBox.Show(text, GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public static string GetCopyright()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var copyright = GetAssemblyCustomAttribute(assembly, typeof(System.Reflection.AssemblyCopyrightAttribute));
            var company = GetAssemblyCustomAttribute(assembly, typeof (System.Reflection.AssemblyCompanyAttribute));
            return string.Format("{0} {1}", copyright, company);
        }

        private static string GetAssemblyCustomAttribute(System.Reflection.Assembly assembly, Type attributeType)
        {
            var attribute = assembly.CustomAttributes.FirstOrDefault(
                x => x.AttributeType == attributeType);
            if (attribute == null)
            {
                return string.Empty;
            }

            return attribute.ConstructorArguments.FirstOrDefault().Value.ToString();
        }

        public static void SetCachedAnnotation(List<Annotation> existingAnnotations, Annotation annotation)
        {
            var existingAnnotation = existingAnnotations.FirstOrDefault(x => x.Equals(annotation));
            if (existingAnnotation != null && existingAnnotation.CachedResult != null)
            {
                annotation.CachedResult = new List<CommandResult>(existingAnnotation.CachedResult);
            }
        }

        public static string Pluralize(this string singularForm, int howMany)
        {
            return singularForm.Pluralize(howMany, singularForm + "s");
        }

        public static string Pluralize(this string singularForm, int howMany, string pluralForm)
        {
            return howMany == 1 ? singularForm : pluralForm;
        }

        public static void ReportException(Exception exc, string userMessage, LogManager logger)
        {
            if (logger != null)
            {
                logger.WriteException(exc);
            }

            MessageBox.Show(userMessage, GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
