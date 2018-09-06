using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using R;
using SAS;
using StatTag.Core.Interfaces;
using Stata;
using StatTag.Core.Models;
using StatTag.Models;
using Font = Microsoft.Office.Interop.Word.Font;

namespace StatTag
{
    public static class UIUtility
    {
        public static IEnumerable<object> RemoveCheckedItems(DataGridView dgvItems, int checkColumn)
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

        public static string GetOpenFileName(string filter)
        {
            var files = GetOpenFileNames(filter, false);
            if (files != null && files.Length > 0)
            {
                return files.First();
            }

            return null;
        }

        public static string[] GetOpenFileNames(string filter, bool allowMultiple = true)
        {
            var openFile = new OpenFileDialog
            {
                Filter = filter,
                Multiselect = allowMultiple
            };

            if (DialogResult.OK == openFile.ShowDialog())
            {
                return openFile.FileNames;
            }

            return null;
        }

        public static string GetSaveFileName(string filter)
        {
            var dialog = new SaveFileDialog {Filter = filter};
            if (DialogResult.OK == dialog.ShowDialog())
            {
                return dialog.FileName;
            }

            return null;
        }

        public static string GetAddInName()
        {
            return "StatTag";
        }

        public static string GetVersionLabel()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return string.Format("{0} v{1}", GetAddInName(),
                GetAssemblyCustomAttribute(assembly, typeof (System.Reflection.AssemblyFileVersionAttribute)));
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

        /// <summary>
        /// Creates a new font to be scaled to 96dpi.
        /// </summary>
        /// <param name="font"></param>
        /// <param name="graphics"></param>
        /// <returns></returns>
        public static System.Drawing.Font CreateScaledFont(System.Drawing.Font font, Graphics graphics)
        {
            const float dpiScale = 96f;
            var scaledFont = new System.Drawing.Font(font.Name, font.Size * dpiScale / graphics.DpiX, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
            return scaledFont;
        }

        /// <summary>
        /// Helper function to recursively adjust control fonts to control for DPI scaling.
        /// </summary>
        /// <param name="container"></param>
        public static void ScaleContainerFont(Control container)
        {
            foreach (var control in container.Controls.OfType<Control>())
            {
                if (control.HasChildren)
                {
                    ScaleContainerFont(control);
                }
                else
                {
                    control.Font = UIUtility.CreateScaledFont(control.Font, control.CreateGraphics());
                }
            }

            container.Font = UIUtility.CreateScaledFont(container.Font, container.CreateGraphics());
        }

        /// <summary>
        /// Utility function to handle scaling fonts in all form controls to appear as if they did
        /// on 96dpi.  This is our (not optimal) workaround to DPI scaling issues.  Instead of
        /// scaling the UI, we force it to be as if we're still on 96dpi.
        /// </summary>
        /// <param name="form"></param>
        public static void ScaleFont(Form form)
        {
            form.AutoScaleMode = AutoScaleMode.None;
            ScaleContainerFont(form);
        }

        public static void ScaleFont(UserControl control)
        {
            control.AutoScaleMode = AutoScaleMode.None;
            ScaleContainerFont(control);
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
                    case Constants.StatisticalPackages.SAS:
                        formatter = new SASCommands();
                        break;
                    case Constants.StatisticalPackages.R:
                        formatter = new RCommands();
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
                        case Constants.TagType.Verbatim:
                            return formatter.VerbatimResultCommands();
                    }
                }
            }

            return null;
        }

        public static System.Drawing.Font ToggleBoldFont(Control control, bool bold)
        {
            return new System.Drawing.Font(control.Font, bold ? FontStyle.Bold : FontStyle.Regular);
        }

        /// <summary>
        /// Utility function to invoke the Close method in a given dialog/form.
        /// </summary>
        /// <remarks>Because this
        /// uses messaging, it isn't an immediate close of the dialog, but will result in it
        /// being closed when all threads and message queues are processed.</remarks>
        /// <param name="dialog">The dialog/form to be closed</param>
        public static void ManageCloseDialog(Form dialog)
        {
            try
            {
                if (dialog != null && !dialog.IsDisposed)
                {
                    dialog.Invoke((MethodInvoker)(dialog.Close));
                }
            }
            catch (Exception exc)
            {
                // For now we are eating the exception.
            }
        }
    }
}
