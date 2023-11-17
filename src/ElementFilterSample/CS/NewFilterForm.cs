// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    /// <summary>
    ///     Form used to create new filter
    /// </summary>
    public partial class NewFilterForm : Form
    {
        /// <summary>
        ///     New filter name
        /// </summary>
        private string m_filterName;

        /// <summary>
        ///     In-use filter names
        /// </summary>
        private readonly ICollection<string> m_inUseFilterNames;

        /// <summary>
        ///     Show form for new filter name
        /// </summary>
        /// <param name="inUseNames">Filter names should be excluded.</param>
        public NewFilterForm(ICollection<string> inUseNames)
        {
            InitializeComponent();
            m_inUseFilterNames = inUseNames;
        }

        /// <summary>
        ///     Get new filter name
        /// </summary>
        public string FilterName => m_filterName;

        /// <summary>
        ///     Check if input name is valid for new filter, the name should be unique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            // Check name is not empty
            var newName = newFilterNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                ViewFiltersForm.MyMessageBox("Filter name is empty!");
                newFilterNameTextBox.Focus();
                return;
            }

            //
            // Check if filter name contains invalid characters
            // These character are different from Path.GetInvalidFileNameChars()
            char[] invalidFileChars = { '\\', ':', '{', '}', '[', ']', '|', ';', '<', '>', '?', '\'', '~' };
            foreach (var invalidChr in invalidFileChars)
            {
                if (newName.Contains(invalidChr))
                {
                    ViewFiltersForm.MyMessageBox("Filter name contains invalid character: " + invalidChr);
                    return;
                }
            }

            // 
            // Check if name is used
            // check if name is already used by other filters
            var inUsed = m_inUseFilterNames.Contains(newName, StringComparer.OrdinalIgnoreCase);
            if (inUsed)
            {
                ViewFiltersForm.MyMessageBox("The name you supplied is already in use. Enter a unique name please.");
                newFilterNameTextBox.Focus();
                return;
            }

            m_filterName = newName;
            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
