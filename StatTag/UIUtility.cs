using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using StatTag.Core.Models;
using StatTag.Models;
using Font = Microsoft.Office.Interop.Word.Font;

namespace StatTag
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
            return "StatTag";
        }

        public static string GetVersionLabel()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0} v{1}.{2}.{3}", GetAddInName(), version.Major, version.Minor, version.Revision);
        }

        public static void WarningMessageBox(string text, LogManager logger)
        {
            if (logger != null)
            {
                logger.WriteMessage(text);
            }
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

        public static void ReportException(Exception exc, string userMessage, LogManager logger)
        {
            if (logger != null)
            {
                logger.WriteException(exc);
            }

            MessageBox.Show(userMessage, GetAddInName(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public static string Pluralize(this string singularForm, int howMany)
        {
            return singularForm.Pluralize(howMany, singularForm + "s");
        }

        public static string Pluralize(this string singularForm, int howMany, string pluralForm)
        {
            return howMany == 1 ? singularForm : pluralForm;
        }

        public static IEnumerable<Annotation> GetCheckedAnnotationsFromListView(ListView listView)
        {
            return
                listView.CheckedItems.Cast<ListViewItem>()
                    .Where(x => x.Tag is Annotation)
                    .Select(x => x.Tag as Annotation);
        }

        public static System.Drawing.Font CreateScaledFont(System.Drawing.Font font, Graphics graphics)
        {
            //var dpiScale = Math.Min(graphics.DpiX, 120f);
            const float dpiScale = 96f;
            var scaledFont = new System.Drawing.Font(font.Name, 9.75f * dpiScale / graphics.DpiX, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            return scaledFont;
        }

        public static void BuildCodeFileActionColumn(List<CodeFile> files, DataGridView gridView, int columnIndex, bool forSingleAnnotation)
        {
            var actions = new List<GridDataItem>();
            actions.Add(GridDataItem.CreateActionItem(string.Empty, Constants.CodeFileActionTask.NoAction, null));
            foreach (var file in files)
            {
                actions.Add(GridDataItem.CreateActionItem(string.Format("Use file {0}", file.FilePath),
                    Constants.CodeFileActionTask.ChangeFile, file));
            }
            actions.Add(GridDataItem.CreateActionItem(string.Format("Remove {0} from document", (forSingleAnnotation ? "this annotation" : "all annotations in this code file")),
                Constants.CodeFileActionTask.RemoveAnnotations, null));
            actions.Add(GridDataItem.CreateActionItem("Link the missing code file to this document",
                Constants.CodeFileActionTask.ReAddFile, null));

            var column = gridView.Columns[columnIndex] as DataGridViewComboBoxColumn;
            column.DataSource = actions;
            column.DisplayMember = "Display";
            column.ValueMember = "Data";
        }
    }
}
