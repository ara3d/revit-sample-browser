// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.TagBeam.CS
{
    /// <summary>
    ///     Form to get input from user to create beam tags.
    /// </summary>
    public partial class TagBeamForm : Form
    {
        //Required designer variable.
        private readonly TagBeamData m_dataBuffer;

        /// <summary>
        ///     Initialize GUI with TagBeamData
        /// </summary>
        /// <param name="dataBuffer">relevant data from revit</param>
        public TagBeamForm(TagBeamData dataBuffer)
        {
            InitializeComponent();
            m_dataBuffer = dataBuffer;
            InitializeComboBoxes();
        }

        /// <summary>
        ///     Initialize the combo boxes.
        /// </summary>
        private void InitializeComboBoxes()
        {
            //Initialize the tag mode comboBox.
            tagComboBox.DataSource = Enum.GetValues(typeof(TagMode));
            //Initialize the tag orientation comboBox.
            tagOrientationComboBox.DataSource = Enum.GetValues(typeof(TagOrientation));

            //Set DropDownStyle of combo boxes to "DropDownList"
            tagComboBox.DropDownStyle =
                tagSymbolComboBox.DropDownStyle =
                    tagOrientationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        ///     Create tags on beam's start and end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (tagSymbolComboBox.SelectedItem != null)
                    m_dataBuffer.CreateTag((TagMode)tagComboBox.SelectedItem,
                        tagSymbolComboBox.SelectedItem as FamilySymbolWrapper,
                        leadercheckBox.Checked,
                        (TagOrientation)tagOrientationComboBox.SelectedItem);
                else
                    throw new ApplicationException("No tag type selected.");

                Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }

        /// <summary>
        ///     Close the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Update tag types in tagComboBox according to the selected tag mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tagComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            tagSymbolComboBox.DataSource = m_dataBuffer[(TagMode)tagComboBox.SelectedItem];
            tagSymbolComboBox.DisplayMember = "Name";
        }
    }
}
