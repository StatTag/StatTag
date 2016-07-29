using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using StatTag.Core.Interfaces;
using Stata;
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
            return string.Format("{0} v{1}.{2}.{3}", GetAddInName(), version.Major, version.Minor, version.Build);
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

        public static IEnumerable<Tag> GetCheckedTagsFromListView(ListView listView)
        {
            return
                listView.CheckedItems.Cast<ListViewItem>()
                    .Where(x => x.Tag is Tag)
                    .Select(x => x.Tag as Tag);
        }

        public static System.Drawing.Font CreateScaledFont(System.Drawing.Font font, Graphics graphics)
        {
            //var dpiScale = Math.Min(graphics.DpiX, 120f);
            const float dpiScale = 96f;
            var scaledFont = new System.Drawing.Font(font.Name, 9.75f * dpiScale / graphics.DpiX, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            return scaledFont;
        }

        public static void BuildCodeFileActionColumn(List<CodeFile> files, DataGridView gridView, int columnIndex, bool forSingleTag)
        {
            var actions = new List<GridDataItem>
            {
                GridDataItem.CreateActionItem(string.Empty, Constants.CodeFileActionTask.NoAction, null)
            };
            foreach (var file in files)
            {
                actions.Add(GridDataItem.CreateActionItem(string.Format("Use file {0}", file.FilePath),
                    Constants.CodeFileActionTask.ChangeFile, file));
            }
            actions.Add(GridDataItem.CreateActionItem(string.Format("Remove {0} from document", (forSingleTag ? "this tag" : "all tags in this code file")),
                Constants.CodeFileActionTask.RemoveTags, null));
            actions.Add(GridDataItem.CreateActionItem("Re-link this missing code file to the document",
                Constants.CodeFileActionTask.ReAddFile, null));
            actions.Add(GridDataItem.CreateActionItem("Select another file to link...",
                Constants.CodeFileActionTask.SelectFile, null));

            var column = gridView.Columns[columnIndex] as DataGridViewComboBoxColumn;
            column.DataSource = actions;
            column.DisplayMember = "Display";
            column.ValueMember = "Data";
        }

        public static GridDataItem AddOptionToBuildCodeFileActionColumn(CodeFile file, DataGridView gridView, int columnIndex)
        {
            var action = GridDataItem.CreateActionItem(string.Format("Use file {0}", file.FilePath),
                Constants.CodeFileActionTask.ChangeFile, file);

            var column = gridView.Columns[columnIndex] as DataGridViewComboBoxColumn;
            var actions = (column.DataSource as List<GridDataItem>);
            var index = actions.FindIndex(x => x.Data.Action == Constants.CodeFileActionTask.RemoveTags);
            actions.Insert(index, action);
            column.DataSource = actions;
            return action;
        }

        public static void SetDialogTitle(Form form)
        {
            form.Text = string.Format("{0} - {1}", GetAddInName(), form.Text);
        }

        public static IResultCommandList GetResultCommandList(CodeFile file, string resultType)
        {
            if (file != null)
            {
                IResultCommandFormatter formatter = null;
                switch (file.StatisticalPackage)
                {
                    case Constants.StatisticalPackages.Stata:
                        formatter = new StataCommands();
                        break;
                }

                if (formatter != null)
                {
                    switch (resultType)
                    {
                        case Constants.TagType.Value:
                            return formatter.ValueResultCommands();
                        case Constants.TagType.Figure:
                            return formatter.FigureResultCommands();
                        case Constants.TagType.Table:
                            return formatter.TableResultCommands();
                    }
                }
            }

            return null;
        }

        public static System.Drawing.Font ToggleBoldFont(Control control, bool bold)
        {
            return new System.Drawing.Font(control.Font, bold ? FontStyle.Bold : FontStyle.Regular);
        }
    }
}
