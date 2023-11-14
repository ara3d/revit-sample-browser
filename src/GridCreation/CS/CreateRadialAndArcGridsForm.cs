// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using RevitMultiSample.GridCreation.CS.Properties;

namespace RevitMultiSample.GridCreation.CS
{
    public partial class CreateRadialAndArcGridsForm : Form
    {
        // data class object
        private readonly CreateRadialAndArcGridsData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public CreateRadialAndArcGridsForm(CreateRadialAndArcGridsData data)
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
            labelUnitFirstRadius.Text = unit;
            labelXCoordUnit.Text = unit;
            labelYCoordUnit.Text = unit;

            // Set length values
            textBoxArcSpacing.Text = Unit.CovertFromApi(m_data.Unit, 10).ToString();
            textBoxArcFirstRadius.Text = textBoxArcSpacing.Text;
            textBoxLineFirstDistance.Text = Unit.CovertFromApi(m_data.Unit, 8).ToString();

            radioButton360.Checked = true;
            radioButtonCustomize.Checked = false;
            labelStartDegree.Enabled = false;
            textBoxStartDegree.Enabled = false;
            labelEndDegree.Enabled = false;
            textBoxEndDegree.Enabled = false;
            comboBoxArcBubbleLocation.SelectedIndex = 1;
            comboBoxLineBubbleLocation.SelectedIndex = 1;
        }

        private void radioButtonCustomize_CheckedChanged(object sender, EventArgs e)
        {
            var isCustomize = radioButtonCustomize.Checked;
            labelStartDegree.Enabled = isCustomize;
            textBoxStartDegree.Enabled = isCustomize;
            labelEndDegree.Enabled = isCustomize;
            textBoxEndDegree.Enabled = isCustomize;
        }

        private void radioButton360_MouseClick(object sender, MouseEventArgs e)
        {
            radioButtonCustomize.Checked = !radioButton360.Checked;
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
            m_data.XOrigin = Unit.CovertToApi(Convert.ToDouble(textBoxXCoord.Text), m_data.Unit);
            m_data.YOrigin = Unit.CovertToApi(Convert.ToDouble(textBoxYCoord.Text), m_data.Unit);

            if (radioButton360.Checked)
            {
                m_data.StartDegree = 0;
                m_data.EndDegree = 2 * Values.Pi;
            }
            else
            {
                m_data.StartDegree = Convert.ToDouble(textBoxStartDegree.Text) * Values.Degtorad;
                m_data.EndDegree = Convert.ToDouble(textBoxEndDegree.Text) * Values.Degtorad;
            }

            m_data.ArcNumber = Convert.ToUInt32(textBoxArcNumber.Text);
            m_data.LineNumber = Convert.ToUInt32(textBoxLineNumber.Text);

            if (Convert.ToUInt32(textBoxArcNumber.Text) != 0)
            {
                m_data.ArcSpacing = Unit.CovertToApi(Convert.ToDouble(textBoxArcSpacing.Text), m_data.Unit);
                m_data.ArcFirstRadius = Unit.CovertToApi(Convert.ToDouble(textBoxArcFirstRadius.Text), m_data.Unit);
                m_data.ArcFirstBubbleLoc = (BubbleLocation)comboBoxArcBubbleLocation.SelectedIndex;
                m_data.ArcFirstLabel = textBoxArcFirstLabel.Text;
            }

            if (Convert.ToUInt32(textBoxLineNumber.Text) != 0)
            {
                m_data.LineFirstDistance =
                    Unit.CovertToApi(Convert.ToDouble(textBoxLineFirstDistance.Text), m_data.Unit);
                m_data.LineFirstBubbleLoc = (BubbleLocation)comboBoxLineBubbleLocation.SelectedIndex;
                m_data.LineFirstLabel = textBoxLineFirstLabel.Text;
            }
        }

        /// <summary>
        ///     Check if input are validated
        /// </summary>
        /// <returns>Whether input is validated</returns>
        private bool ValidateValues()
        {
            if (!Validation.ValidateNumbers(textBoxArcNumber, textBoxLineNumber)) return false;

            if (!Validation.ValidateCoord(textBoxXCoord) || !Validation.ValidateCoord(textBoxYCoord)) return false;

            if (Convert.ToUInt32(textBoxArcNumber.Text) != 0)
                if (!Validation.ValidateLength(textBoxArcSpacing, "Spacing", false) ||
                    !Validation.ValidateLength(textBoxArcFirstRadius, "Radius", false) ||
                    !Validation.ValidateLabel(textBoxArcFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxLineNumber.Text) != 0)
                if (!Validation.ValidateLength(textBoxLineFirstDistance, "Distance", true) ||
                    !Validation.ValidateLabel(textBoxLineFirstLabel, m_data.LabelsList))
                    return false;

            if (Convert.ToUInt32(textBoxArcNumber.Text) != 0 && Convert.ToUInt32(textBoxLineNumber.Text) != 0)
                if (!Validation.ValidateLabels(textBoxArcFirstLabel, textBoxLineFirstLabel))
                    return false;

            if (radioButtonCustomize.Checked)
                if (!Validation.ValidateDegrees(textBoxStartDegree, textBoxEndDegree))
                    return false;

            return true;
        }
    }
}
