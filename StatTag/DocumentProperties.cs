using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using StatTag.Models;

namespace StatTag
{
    public sealed partial class DocumentProperties : Form
    {
        private Document ActiveDocument { get; set; }
        private DocumentManager Manager { get; set; }

        public DocumentProperties(Document document, DocumentManager manager)
        {
            InitializeComponent();

            // Reset the missing value information label - we have it visible with placeholder text in the designer,
            // just so we can easily manage it, but we never want that at runtime.
            lblMissingValueInformation.Text = string.Empty;
            lblMissingValueInformation.Visible = false;

            UIUtility.ScaleFont(this);
            MinimumSize = Size;
            UIUtility.SetDialogTitle(this);

            ActiveDocument = document;
            Manager = manager;
        }

        private void InitializeProperites()
        {
            var metadata = Manager.LoadMetadataFromDocument(ActiveDocument, false);

            // If there is no metadata, inform the user that we are using the default settings
            if (metadata == null)
            {
                lblMissingValueInformation.Text = "The document has been set to your default user settings.";
                lblMissingValueInformation.Visible = true;

                var settings = Manager.SettingsManager.Settings;
                missingValueSettings1.SetMissingValuesSelection(settings.RepresentMissingValues);
                missingValueSettings1.SetCustomMissingValueString(settings.CustomMissingValue);
                missingValueSettings1.UpdateDisplay();
            }
            else
            {
                var settings = Manager.SettingsManager.Settings;
                missingValueSettings1.SetMissingValuesSelection(metadata.RepresentMissingValues);
                missingValueSettings1.SetCustomMissingValueString(metadata.CustomMissingValue);

                if (!settings.RepresentMissingValues.Equals(metadata.RepresentMissingValues))
                {
                    lblMissingValueInformation.Text =
                        "Note: The document properties are different than your default user settings.";
                    lblMissingValueInformation.Visible = true;
                }
                else
                {
                    lblMissingValueInformation.Text = string.Empty;
                    lblMissingValueInformation.Visible = false;
                }
            }

            ResizeDialogForInformationalMessages();
        }

        private void ResizeDialogForInformationalMessages()
        {
            if (lblMissingValueInformation.Visible)
            {
                // Only adjust things if the default layout is the current state
                if (missingValueSettings1.Top == lblMissingValueInformation.Top)
                {
                    missingValueSettings1.Top = lblMissingValueInformation.Bottom + lblMissingValueInformation.Margin.Bottom;
                    this.Height = this.Height + lblMissingValueInformation.Height +
                                  lblMissingValueInformation.Margin.Bottom;
                    this.MinimumSize = this.Size;
                }
            }
            else
            {
                // Only adjust things if the default layout is not the current state
                if (missingValueSettings1.Top != lblMissingValueInformation.Top)
                {
                    missingValueSettings1.Top = lblMissingValueInformation.Top;
                }
            }
        }

        private void DocumentProperties_Shown(object sender, EventArgs e)
        {
            InitializeProperites();
        }
    }
}
