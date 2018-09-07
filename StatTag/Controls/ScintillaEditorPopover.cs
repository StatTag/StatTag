using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using StatTag.Core.Models;
using StatTag.Models;

namespace StatTag.Controls
{
    /// <summary>
    /// The general concept for this control is based on the idea presented at:
    /// https://www.codeproject.com/Articles/17502/Simple-Popup-Control
    /// but does not reflect code copied from or code derived from this project.
    /// </summary>
    public sealed class ScintillaEditorPopover : ToolStripDropDown
    {
        private Scintilla Editor { get; set; }

        public ScintillaEditorPopover()
        {
            Editor = new Scintilla {Width = 600, Height = 150, ReadOnly = true};

            var host = new ToolStripControlHost(Editor)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            Items.Add(host);
        }

        public void ShowCodeFileLines(CodeFile codeFile, int startLine, int endLine)
        {
            Editor.Text = string.Empty;
            if (codeFile != null)
            {
                Editor.ReadOnly = false;
                Editor.Text = string.Join("\r\n", codeFile.Content.Skip(startLine).Take(endLine - startLine + 1));
                Editor.ReadOnly = true;
                ScintillaManager.ConfigureEditor(Editor, codeFile);
            }
            Editor.EmptyUndoBuffer();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Editor != null)
                {
                    Editor.Dispose();
                    Editor = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
