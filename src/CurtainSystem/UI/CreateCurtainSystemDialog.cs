// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.UI
{
    /// <summary>
    ///     the winForm for user to create a new curtain system
    /// </summary>
    public partial class CreateCurtainSystemDialog : Form
    {
        // the flag for curtain system creation, if it's true, the curtain system
        // will be created by face array (can't add/remove curtain grids on this kind of curtain system)
        // otherwise be created by reference array
        private bool m_byFaceArray;
        private readonly MyDocument m_mydocument;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="mydoc">
        ///     the document of the sample
        /// </param>
        public CreateCurtainSystemDialog(MyDocument mydoc)
        {
            m_mydocument = mydoc;

            InitializeComponent();

            //
            // initialize data
            //
            // by default, create the curtain system by reference array
            m_byFaceArray = false;
        }

        /// <summary>
        ///     check/uncheck the "by face array" flag
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void byFaceArrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_byFaceArray = byFaceArrayCheckBox.Checked;
        }

        /// <summary>
        ///     select all the faces
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < facesCheckedListBox.Items.Count; i++) facesCheckedListBox.SetItemChecked(i, true);
        }

        /// <summary>
        ///     reverse the selection
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void reverseSelButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < facesCheckedListBox.Items.Count; i++)
            {
                var itemChecked = facesCheckedListBox.GetItemChecked(i);
                // toggle the checked status
                facesCheckedListBox.SetItemChecked(i, !itemChecked);
            }
        }

        /// <summary>
        ///     clear the selected faces
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < facesCheckedListBox.Items.Count; i++) facesCheckedListBox.SetItemChecked(i, false);
        }

        /// <summary>
        ///     create a new curtain system by the selected faces
        /// </summary>
        /// <param name="sender">
        ///     object who sent this event
        /// </param>
        /// <param name="e">
        ///     event args
        /// </param>
        private void createCSButton_Click(object sender, EventArgs e)
        {
            // step 1: get the faces for curtain system creation
            var checkedIndices = new List<int>();
            for (var i = 0; i < facesCheckedListBox.Items.Count; i++)
            {
                var itemChecked = facesCheckedListBox.GetItemChecked(i);

                if (itemChecked) checkedIndices.Add(i);
            }

            // step 2: create the new curtain system
            m_mydocument.SystemData.CreateCurtainSystem(checkedIndices, m_byFaceArray);
            Close();
        }

        private void CreateCurtainSystemDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // just refresh the main UI
            m_mydocument.SystemData.CreateCurtainSystem(null, m_byFaceArray);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void reverseSelection_Click(object sender, EventArgs e)
        {
        }
    } // end of class
}
