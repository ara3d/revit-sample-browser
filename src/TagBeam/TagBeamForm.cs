// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.TagBeam.CS
{
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tagComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            tagSymbolComboBox.DataSource = m_dataBuffer[(TagMode)tagComboBox.SelectedItem];
            tagSymbolComboBox.DisplayMember = "Name";
        }
    }
}
