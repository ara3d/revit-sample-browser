// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitMultiSample.Reinforcement.CS
{
    /// <summary>
    ///     The form is used for collecting information of beam reinforcement creation
    /// </summary>
    public partial class BeamFramReinMakerForm : Form
    {
        // Private members
        private readonly BeamFramReinMaker m_dataBuffer;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer">the BeamFramReinMaker reference</param>
        public BeamFramReinMakerForm(BeamFramReinMaker dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // Store the reference of BeamFramReinMaker
            m_dataBuffer = dataBuffer;

            // Bing the data source for all combo boxes
            BingingDataSource();

            // set the initialization data of the spacing
            transverseCenterSpacingTextBox.Text = 0.1.ToString("0.0");
            transverseEndSpacingTextBox.Text = 0.1.ToString("0.0");
        }

        /// <summary>
        ///     Bing the data source for all combo boxes
        /// </summary>
        private void BingingDataSource()
        {
            // bind the topEndRebarTypeComboBox
            topEndRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            topEndRebarTypeComboBox.DisplayMember = "Name";

            // bind the topCenterRebarTypeComboBox
            topCenterRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            topCenterRebarTypeComboBox.DisplayMember = "Name";

            // bind the bottomRebarTypeComboBox
            bottomRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            bottomRebarTypeComboBox.DisplayMember = "Name";

            // bind the transverseRebarTypeComboBox
            transverseRebarTypeComboBox.DataSource = m_dataBuffer.RebarTypes;
            transverseRebarTypeComboBox.DisplayMember = "Name";

            // bind the topBarHookComboBox
            topBarHookComboBox.DataSource = m_dataBuffer.HookTypes;
            topBarHookComboBox.DisplayMember = "Name";

            // bind the transverseBarHookComboBox
            transverseBarHookComboBox.DataSource = m_dataBuffer.HookTypes;
            transverseBarHookComboBox.DisplayMember = "Name";
        }

        /// <summary>
        ///     When the user click ok, refresh the data of BeamFramReinMaker and close form
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            // set TopEndRebarType data
            var type = topEndRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.TopEndRebarType = type;

            // set TopCenterRebarType data
            type = topCenterRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.TopCenterRebarType = type;

            // set BottomRebarType data
            type = bottomRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.BottomRebarType = type;

            // set TransverseRebarType data
            type = transverseRebarTypeComboBox.SelectedItem as RebarBarType;
            m_dataBuffer.TransverseRebarType = type;

            // set TopHookType data
            var hookType = topBarHookComboBox.SelectedItem as RebarHookType;
            m_dataBuffer.TopHookType = hookType;

            // set TransverseHookType data
            hookType = transverseBarHookComboBox.SelectedItem as RebarHookType;
            m_dataBuffer.TransverseHookType = hookType;

            try
            {
                // set TransverseEndSpacing data
                var spacing = Convert.ToDouble(transverseEndSpacingTextBox.Text);
                m_dataBuffer.TransverseEndSpacing = spacing;

                // set TransverseCenterSpacing data
                spacing = Convert.ToDouble(transverseCenterSpacingTextBox.Text);
                m_dataBuffer.TransverseCenterSpacing = spacing;
            }
            catch (FormatException)
            {
                // spacing text boxes should only input number information
                TaskDialog.Show("Revit", "Please input double number in spacing TextBox.");
                return;
            }
            catch (Exception ex)
            {
                // other unexpected error, just show the information
                TaskDialog.Show("Revit", ex.Message);
                return;
            }

            DialogResult = DialogResult.OK; // set dialog result
            Close(); // close the form
        }

        /// <summary>
        ///     When the user click the cancel, just close the form
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel; // set dialog result
            Close(); // close the form
        }
    }
}
