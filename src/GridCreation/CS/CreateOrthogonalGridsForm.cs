// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Revit.SDK.Samples.GridCreation.CS.Properties;

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    ///     The dialog which provides the options of creating orthogonal grids
    /// </summary>
    public partial class CreateOrthogonalGridsForm : Form
    {
        // data class object
        private readonly CreateOrthogonalGridsData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public CreateOrthogonalGridsForm(CreateOrthogonalGridsData data)
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
            // Set length unit related labels
            var unit = Resources.ResourceManager.GetString(m_data.Unit.TypeId);
            labelUnitX.Text = unit;
            labelUnitY.Text = unit;
            labelXCoordUnit.Text = unit;
            labelYCoordUnit.Text = unit;

            // Set spacing values
            textBoxXSpacing.Text = Unit.CovertFromAPI(m_data.Unit, 10).ToString();
            textBoxYSpacing.Text = textBoxXSpacing.Text;

            // Set bubble locations to end point
            comboBoxXBubbleLocation.SelectedIndex = 1;
            comboBoxYBubbleLocation.SelectedIndex = 1;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            // Check if input are validated
            if (ValidateValues())
                // Transfer data back into data class
                SetData();
            else
                DialogResult = DialogResult.None;
        }

        /// <summary>
        ///     Transfer data back into data class
        /// </summary>
        private void SetData()
        {
            m_data.XOrigin = Unit.CovertToAPI(Convert.ToDouble(textBoxXCoord.Text), m_data.Unit);
            m_data.YOrigin = Unit.CovertToAPI(Convert.ToDouble(textBoxYCoord.Text), m_data.Unit);
            m_data.XNumber = Convert.ToUInt32(textBoxXNumber.Text);
            m_data.YNumber = Convert.ToUInt32(textBoxYNumber.Text);

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0)
            {
                m_data.XSpacing = Unit.CovertToAPI(Convert.ToDouble(textBoxXSpacing.Text), m_data.Unit);
                m_data.XBubbleLoc = (BubbleLocation)comboBoxXBubbleLocation.SelectedIndex;
                m_data.XFirstLabel = textBoxXFirstLabel.Text;
            }

            if (Convert.ToUInt32(textBoxYNumber.Text) != 0)
            {
                m_data.YSpacing = Unit.CovertToAPI(Convert.ToDouble(textBoxYSpacing.Text), m_data.Unit);
                m_data.YBubbleLoc = (BubbleLocation)comboBoxYBubbleLocation.SelectedIndex;
                m_data.YFirstLabel = textBoxYFirstLabel.Text;
            }
        }

        /// <summary>
        ///     Check if input are validated
        /// </summary>
        /// <returns>Whether input is validated</returns>
        private bool ValidateValues()
        {
            if (!Validation.ValidateNumbers(textBoxXNumber, textBoxYNumber)) return false;

            if (!Validation.ValidateCoord(textBoxXCoord) || !Validation.ValidateCoord(textBoxYCoord)) return false;

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0)
                if (!Validation.ValidateLength(textBoxXSpacing, "Spacing", false) ||
                    !Validation.ValidateLabel(textBoxXFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxYNumber.Text) != 0)
                if (!Validation.ValidateLength(textBoxYSpacing, "Spacing", false) ||
                    !Validation.ValidateLabel(textBoxYFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0 && Convert.ToUInt32(textBoxYNumber.Text) != 0)
                if (!Validation.ValidateLabels(textBoxXFirstLabel, textBoxYFirstLabel))
                    return false;

            return true;
        }
    }
}
