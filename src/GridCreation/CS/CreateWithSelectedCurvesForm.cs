// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    ///     The dialog which provides the options of creating grids with selected lines/arcs
    /// </summary>
    public partial class CreateWithSelectedCurvesForm : Form
    {
        // data class object
        private readonly CreateWithSelectedCurvesData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public CreateWithSelectedCurvesForm(CreateWithSelectedCurvesData data)
        {
            m_data = data;

            InitializeComponent();
            // Set state of controls
            InitializeControls();
        }

        /// <summary>
        ///     Set state of controls
        /// </summary>
        private void InitializeControls()
        {
            comboBoxBubbleLocation.SelectedIndex = 1;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Check if input are validated
            if (ValidateValues())
                // Transfer data back into data class
                SetData();
            else
                DialogResult = DialogResult.None;
        }

        /// <summary>
        ///     Check if input are validated
        /// </summary>
        /// <returns>Whether input is validated</returns>
        private bool ValidateValues()
        {
            return Validation.ValidateLabel(textBoxFirstLabel, m_data.LabelsList);
        }

        /// <summary>
        ///     Transfer data back into data class
        /// </summary>
        private void SetData()
        {
            m_data.BubbleLocation = (BubbleLocation)comboBoxBubbleLocation.SelectedIndex;
            m_data.FirstLabel = textBoxFirstLabel.Text;
            m_data.DeleteSelectedElements = checkBoxDeleteElements.Checked;
        }
    }
}
