// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.Units.CS
{
    /// <summary>
    /// </summary>
    public partial class FormatForm : Form
    {
        //Required designer variable.
        private readonly ForgeTypeId m_specTypeId;

        /// <summary>
        ///     Initialize GUI with FormatData
        /// </summary>
        ///// <param name="dataBuffer">relevant data from revit</param>
        public FormatForm(ForgeTypeId specTypeId, FormatOptions formatoptions)
        {
            InitializeComponent();
            m_specTypeId = specTypeId;
            FormatOptions = new FormatOptions(formatoptions.GetUnitTypeId(), formatoptions.GetSymbolTypeId());
            FormatOptions.UseDefault = formatoptions.UseDefault;
            FormatOptions.Accuracy = formatoptions.Accuracy;
            FormatOptions.SuppressTrailingZeros = formatoptions.SuppressTrailingZeros;
            FormatOptions.SuppressLeadingZeros = formatoptions.SuppressLeadingZeros;
            FormatOptions.UsePlusPrefix = formatoptions.UsePlusPrefix;
            FormatOptions.UseDigitGrouping = formatoptions.UseDigitGrouping;
            FormatOptions.SuppressSpaces = formatoptions.SuppressSpaces;
        }

        /// <summary>
        ///     format options.
        /// </summary>
        public FormatOptions FormatOptions { get; }

        private void FormatForm_Load(object sender, EventArgs e)
        {
            var isUseDefault = FormatOptions.UseDefault;
            if (isUseDefault) FormatOptions.UseDefault = false;

            FormatForm_Clear();

            // Initialize the combo box and check box. 
            DisplayUnitTypecomboBox.BeginUpdate();
            DisplayUnitcomboBox.BeginUpdate();
            foreach (var unitTypeId in UnitUtils.GetValidUnits(m_specTypeId))
            {
                DisplayUnitTypecomboBox.Items.AddRange(new object[] { unitTypeId });
                DisplayUnitcomboBox.Items.Add(LabelUtils.GetLabelForUnit(unitTypeId));
            }

            DisplayUnitTypecomboBox.SelectedItem = FormatOptions.GetUnitTypeId();
            DisplayUnitcomboBox.SelectedIndex = DisplayUnitTypecomboBox.SelectedIndex;
            DisplayUnitTypecomboBox.EndUpdate();
            DisplayUnitcomboBox.EndUpdate();

            AccuracytextBox.Text = FormatOptions.Accuracy.ToString("###########0.############");

            SuppressTrailingZeroscheckBox.Checked = FormatOptions.SuppressTrailingZeros;
            SuppressLeadingZeroscheckBox.Checked = FormatOptions.SuppressLeadingZeros;
            UsePlusPrefixcheckBox.Checked = FormatOptions.UsePlusPrefix;
            UseDigitGroupingcheckBox.Checked = FormatOptions.UseDigitGrouping;
            SuppressSpacescheckBox.Checked = FormatOptions.SuppressSpaces;

            FormatOptions.UseDefault = isUseDefault;
            UseDefaultcheckBox.Checked = FormatOptions.UseDefault;

            if (!Autodesk.Revit.DB.Units.IsModifiableSpec(m_specTypeId))
            {
                Text = "This unit type is unmodifiable";
                UseDefaultcheckBox.Checked = true;
                UseDefaultcheckBox.Enabled = false;
                buttonOK.Enabled = false;
            }
        }

        private void FormatForm_Clear()
        {
            // Clear the combo box and check box. 
            UseDefaultcheckBox.Checked = false;

            DisplayUnitTypecomboBox.Items.Clear();
            DisplayUnitcomboBox.Items.Clear();

            AccuracytextBox.Text = "";

            UnitSymbolTypecomboBox.Items.Clear();
            UnitSymbolcomboBox.Items.Clear();

            SuppressTrailingZeroscheckBox.Checked = false;
            SuppressLeadingZeroscheckBox.Checked = false;
            UsePlusPrefixcheckBox.Checked = false;
            UseDigitGroupingcheckBox.Checked = false;
            SuppressSpacescheckBox.Checked = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                // Save the properties of FormatOptions. 
                FormatOptions.UseDefault = UseDefaultcheckBox.Checked;

                if (!FormatOptions.UseDefault)
                {
                    DisplayUnitTypecomboBox.SelectedIndex = DisplayUnitcomboBox.SelectedIndex;
                    FormatOptions.SetUnitTypeId((ForgeTypeId)DisplayUnitTypecomboBox.SelectedItem);

                    FormatOptions.Accuracy = double.Parse(AccuracytextBox.Text);

                    UnitSymbolTypecomboBox.SelectedIndex = UnitSymbolcomboBox.SelectedIndex;
                    FormatOptions.SetSymbolTypeId((ForgeTypeId)UnitSymbolTypecomboBox.SelectedItem);

                    FormatOptions.SuppressTrailingZeros = SuppressTrailingZeroscheckBox.Checked;
                    FormatOptions.SuppressLeadingZeros = SuppressLeadingZeroscheckBox.Checked;
                    FormatOptions.UsePlusPrefix = UsePlusPrefixcheckBox.Checked;
                    FormatOptions.UseDigitGrouping = UseDigitGroupingcheckBox.Checked;
                    FormatOptions.SuppressSpaces = SuppressSpacescheckBox.Checked;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Invalid Input", ex.Message, TaskDialogCommonButtons.Ok);
            }
        }

        private void DisplayUnitcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayUnitTypecomboBox.SelectedIndex = DisplayUnitcomboBox.SelectedIndex;

            UnitSymbolTypecomboBox.Items.Clear();
            UnitSymbolcomboBox.Items.Clear();

            UnitSymbolTypecomboBox.BeginUpdate();
            UnitSymbolcomboBox.BeginUpdate();
            foreach (var symbolTypeId in FormatOptions.GetValidSymbols(
                         (ForgeTypeId)DisplayUnitTypecomboBox.SelectedItem))
            {
                UnitSymbolTypecomboBox.Items.AddRange(new object[] { symbolTypeId });
                if (symbolTypeId.Empty())
                    UnitSymbolcomboBox.Items.Add("");
                else
                    UnitSymbolcomboBox.Items.Add(LabelUtils.GetLabelForSymbol(symbolTypeId));
            }

            UnitSymbolTypecomboBox.SelectedItem = FormatOptions.GetSymbolTypeId();
            if (UnitSymbolTypecomboBox.SelectedIndex < 0)
            {
                UnitSymbolTypecomboBox.SelectedIndex = 0;
                FormatOptions.SetSymbolTypeId((ForgeTypeId)UnitSymbolTypecomboBox.SelectedItem);
            }

            UnitSymbolcomboBox.SelectedIndex = UnitSymbolTypecomboBox.SelectedIndex;
            UnitSymbolTypecomboBox.EndUpdate();
            UnitSymbolcomboBox.EndUpdate();
        }

        private void UseDefaultcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FormatOptions.UseDefault = UseDefaultcheckBox.Checked;
            if (UseDefaultcheckBox.Checked)
            {
                DisplayUnitcomboBox.Enabled = false;
                AccuracytextBox.Enabled = false;
                UnitSymbolcomboBox.Enabled = false;
                SuppressTrailingZeroscheckBox.Enabled = false;
                SuppressLeadingZeroscheckBox.Enabled = false;
                UsePlusPrefixcheckBox.Enabled = false;
                UseDigitGroupingcheckBox.Enabled = false;
                SuppressSpacescheckBox.Enabled = false;
            }
            else
            {
                DisplayUnitcomboBox.Enabled = true;
                AccuracytextBox.Enabled = true;
                UnitSymbolcomboBox.Enabled = true;
                SuppressTrailingZeroscheckBox.Enabled = true;
                SuppressLeadingZeroscheckBox.Enabled = true;
                UsePlusPrefixcheckBox.Enabled = true;
                UseDigitGroupingcheckBox.Enabled = true;
                SuppressSpacescheckBox.Enabled = true;
            }
        }
    }
}
