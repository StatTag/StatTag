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
using StatTag.Core.Models;
using StatTag.Models;

namespace StatTag
{
    public sealed partial class DocumentProperties : Form
    {
        private const string BlankValueLabelTemplate =
            "Note: The current document will use a different placeholder for missing values than your user settings, which are set to [BLANK_VALUE].";

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

            missingValueSettings1.ValueChanged += MissingValueSettings_OnValueChanged;
        }

        private void MissingValueSettings_OnValueChanged(object sender, EventArgs eventArgs)
        {
            UpdateMissingValueControls();
        }

        private void UpdateMissingValueControls()
        {
            var settings = Manager.SettingsManager.Settings;
            var customMissingValue = missingValueSettings1.GetCustomMissingValueString();
            var representMissingValues = missingValueSettings1.GetMissingValuesSelection();
            lblMissingValueInformation.Visible = false;
            if (settings != null && (!representMissingValues.Equals(settings.RepresentMissingValues)
                                     ||
                                     (representMissingValues.Equals(Constants.MissingValueOption.CustomValue) &&
                                      !customMissingValue.Equals(settings.CustomMissingValue))))
            {
                lblMissingValueInformation.Text = BlankValueLabelTemplate.Replace("[BLANK_VALUE]",
                    DocumentMetadata.GetMissingValueReplacementAsString(settings.RepresentMissingValues, settings.CustomMissingValue));
                lblMissingValueInformation.Visible = true;
            }
        }

        private void InitializeProperites()
        {
            var metadata = Manager.LoadMetadataFromDocument(ActiveDocument, false);

            // If there is no metadata, inform the user that we are using the default settings
            if (metadata == null)
            {
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
            }
        }

        private void DocumentProperties_Shown(object sender, EventArgs e)
        {
            InitializeProperites();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            var metadata = Manager.LoadMetadataFromDocument(ActiveDocument, true);
            metadata.RepresentMissingValues = missingValueSettings1.GetMissingValuesSelection();
            metadata.CustomMissingValue = missingValueSettings1.GetCustomMissingValueString();
            Manager.SaveMetadataToDocument(ActiveDocument, metadata);
        }
    }
}
