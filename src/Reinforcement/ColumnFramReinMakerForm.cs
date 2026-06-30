// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Structure;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Reinforcement.CS
{
    public partial class ColumnFramReinMakerForm : Form
    {
        // Private members
        private readonly ColumnFramReinMaker m_dataBuffer;

        public ColumnFramReinMakerForm(ColumnFramReinMaker dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // Store the reference of ColumnFramReinMaker
            m_dataBuffer = dataBuffer;

            // Bing the data source for all combo boxes
            BingingDataSource();

            centerSpacingTextBox.Text = 0.1.ToString("0.0");
            endSpacingTextBox.Text = 0.1.ToString("0.0");
        }

        private void BingingDataSource()
        {
            // bind the verticalRebarTypeComboBox
            verticalRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            verticalRebarTypeComboBox.DisplayMember = "Name";

            // bind the centerTransverseRebarTypeComboBox
            centerTransverseRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            centerTransverseRebarTypeComboBox.DisplayMember = "Name";

            // bind the endTransverseRebarTypeComboBox
            endTransverseRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            endTransverseRebarTypeComboBox.DisplayMember = "Name";

            // bind the transverseRebarHookComboBox
            transverseRebarHookComboBox.DataSource = m_dataBuffer.HookTypes;
            transverseRebarHookComboBox.DisplayMember = "Name";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var type = centerTransverseRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.TransverseCenterType = type;

            type = endTransverseRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.TransverseEndType = type;

            // set VerticalRebarType data
            type = verticalRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.VerticalRebarType = type;

            // set TransverseHookType data
            var hookType = transverseRebarHookComboBox.SelectedItem as RebarHookType;
            m_dataBuffer.TransverseHookType = hookType;

            // set VerticalRebarNumber data
            var number = (int)rebarQuantityNumericUpDown.Value;
            m_dataBuffer.VerticalRebarNumber = number;

            try
            {
                var spacing = Convert.ToDouble(centerSpacingTextBox.Text);
                m_dataBuffer.TransverseCenterSpacing = spacing;

                spacing = Convert.ToDouble(endSpacingTextBox.Text);
                m_dataBuffer.TransverseEndSpacing = spacing;
            }
            catch (FormatException)
            {
                // spacing text boxes should only input number information
                TaskDialog.Show("Revit", "Please input double number in spacing TextBox.");
                return;
            }
            catch (Exception ex)
            {
                // if other unexpected error, just show the information
                TaskDialog.Show("Revit", ex.Message);
            }

            DialogResult = DialogResult.OK;
            Close(); // close the form
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close(); // close the form
        }
    }
}
