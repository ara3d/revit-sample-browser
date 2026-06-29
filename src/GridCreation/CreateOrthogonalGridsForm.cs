// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.GridCreation.CS.Properties;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Units;
namespace Ara3D.RevitSampleBrowser.GridCreation.CS
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
            textBoxXSpacing.Text = UnitConversion.CovertFromApi(m_data.Unit, 10).ToString();
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
            m_data.XOrigin = UnitConversion.CovertToApi(Convert.ToDouble(textBoxXCoord.Text), m_data.Unit);
            m_data.YOrigin = UnitConversion.CovertToApi(Convert.ToDouble(textBoxYCoord.Text), m_data.Unit);
            m_data.XNumber = Convert.ToUInt32(textBoxXNumber.Text);
            m_data.YNumber = Convert.ToUInt32(textBoxYNumber.Text);

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0)
            {
                m_data.XSpacing = UnitConversion.CovertToApi(Convert.ToDouble(textBoxXSpacing.Text), m_data.Unit);
                m_data.XBubbleLoc = (BubbleLocation)comboBoxXBubbleLocation.SelectedIndex;
                m_data.XFirstLabel = textBoxXFirstLabel.Text;
            }

            if (Convert.ToUInt32(textBoxYNumber.Text) != 0)
            {
                m_data.YSpacing = UnitConversion.CovertToApi(Convert.ToDouble(textBoxYSpacing.Text), m_data.Unit);
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
            if (!SampleBrowserUtils.ValidateNumbers(textBoxXNumber, textBoxYNumber)) return false;

            if (!SampleBrowserUtils.ValidateCoord(textBoxXCoord) || !SampleBrowserUtils.ValidateCoord(textBoxYCoord)) return false;

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0)
                if (!SampleBrowserUtils.ValidateLength(textBoxXSpacing, "Spacing", false) ||
                    !SampleBrowserUtils.ValidateLabel(textBoxXFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxYNumber.Text) != 0)
                if (!SampleBrowserUtils.ValidateLength(textBoxYSpacing, "Spacing", false) ||
                    !SampleBrowserUtils.ValidateLabel(textBoxYFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxXNumber.Text) != 0 && Convert.ToUInt32(textBoxYNumber.Text) != 0)
                if (!SampleBrowserUtils.ValidateLabels(textBoxXFirstLabel, textBoxYFirstLabel))
                    return false;

            return true;
        }
    }
}
