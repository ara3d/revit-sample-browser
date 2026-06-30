// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    public partial class NewFilterForm : Form
    {
        private readonly ICollection<string> m_inUseFilterNames;

        public NewFilterForm(ICollection<string> inUseNames)
        {
            InitializeComponent();
            m_inUseFilterNames = inUseNames;
        }

        public string FilterName { get; private set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            var newName = newFilterNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                ViewFiltersForm.MyMessageBox("Filter name is empty!");
                newFilterNameTextBox.Focus();
                return;
            }

            // Revit filter names reject these characters; not the same set as Path.GetInvalidFileNameChars().
            char[] invalidFileChars = { '\\', ':', '{', '}', '[', ']', '|', ';', '<', '>', '?', '\'', '~' };
            foreach (var invalidChr in invalidFileChars)
            {
                if (newName.Contains(invalidChr))
                {
                    ViewFiltersForm.MyMessageBox($"Filter name contains invalid character: {invalidChr}");
                    return;
                }
            }

            var inUsed = m_inUseFilterNames.Contains(newName, StringComparer.OrdinalIgnoreCase);
            if (inUsed)
            {
                ViewFiltersForm.MyMessageBox("The name you supplied is already in use. Enter a unique name please.");
                newFilterNameTextBox.Focus();
                return;
            }

            FilterName = newName;
            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
