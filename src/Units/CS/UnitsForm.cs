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
    public partial class UnitsForm : Form
    {
        //Required designer variable.
        private readonly Autodesk.Revit.DB.Units m_units;

        /// <summary>
        ///     Initialize GUI with ProjectUnitData
        /// </summary>
        /// <param name="units">units in current document</param>
        public UnitsForm(Autodesk.Revit.DB.Units units)
        {
            InitializeComponent();
            m_units = units;
        }

        private void UnitsForm_Load(object sender, EventArgs e)
        {
            // Initialize the combo box and list view. 
            disciplineCombox.BeginUpdate();
            foreach (var disciplineTypeId in UnitUtils.GetAllDisciplines())
                disciplineCombox.Items.AddRange(new object[] { LabelUtils.GetLabelForDiscipline(disciplineTypeId) });
            disciplineCombox.SelectedItem = disciplineCombox.Items[0];
            disciplineCombox.EndUpdate();

            DecimalSymbolComboBox.BeginUpdate();
            foreach (DecimalSymbol ds in Enum.GetValues(typeof(
                         DecimalSymbol)))
                DecimalSymbolComboBox.Items.AddRange(new object[] { ds });
            DecimalSymbolComboBox.EndUpdate();
            DecimalSymbolComboBox.SelectedItem = m_units.DecimalSymbol;

            DigitGroupingAmountComboBox.BeginUpdate();
            foreach (DigitGroupingAmount dga in Enum.GetValues(typeof(
                         DigitGroupingAmount)))
                DigitGroupingAmountComboBox.Items.AddRange(new object[] { dga });
            DigitGroupingAmountComboBox.EndUpdate();
            DigitGroupingAmountComboBox.SelectedItem = m_units.DigitGroupingAmount;

            DigitGroupingSymbolComboBox.BeginUpdate();
            foreach (var enumName in Enum.GetNames(typeof(
                         DigitGroupingSymbol)))
                DigitGroupingSymbolComboBox.Items.AddRange(new object[] { enumName });
            DigitGroupingSymbolComboBox.EndUpdate();
            DigitGroupingSymbolComboBox.SelectedItem =
                Enum.GetName(typeof(DigitGroupingSymbol), m_units.DigitGroupingSymbol);

            DecimalSymbolAndGroupingtextBox.Text = GetDecimalSymbolAndGroupingstring();

            DigitGroupingSymbolComboBox.SelectedIndexChanged += DigitGroupingSymbolComboBox_SelectedIndexChanged;
            DigitGroupingAmountComboBox.SelectedIndexChanged += DigitGroupingAmountComboBox_SelectedIndexChanged;
            DecimalSymbolComboBox.SelectedIndexChanged += DecimalSymbolComboBox_SelectedIndexChanged;
        }

        private void disciplineCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            FillGrid();
        }

        /// <summary>
        ///     Fill the grid with selected discipline
        /// </summary>
        private void FillGrid()
        {
            // using the static example value 1234.56789
            var count = 0;
            foreach (var specTypeId in UnitUtils.GetAllMeasurableSpecs())
                if (LabelUtils.GetLabelForDiscipline(UnitUtils.GetDiscipline(specTypeId)) ==
                    disciplineCombox.SelectedItem.ToString())
                {
                    dataGridView.Rows.Add();
                    dataGridView["UnitType", count].Value = specTypeId;
                    dataGridView["Label_UnitType", count].Value = LabelUtils.GetLabelForSpec(specTypeId);
                    dataGridView["FormatOptions", count].Value =
                        UnitFormatUtils.Format(m_units, specTypeId, 1234.56789, false);
                    count++;
                }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                // show UI
                var specTypeId = (ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value;
                using (var displayForm = new FormatForm(specTypeId, m_units.GetFormatOptions(specTypeId)))
                {
                    DialogResult result;
                    while (DialogResult.Cancel != (result = displayForm.ShowDialog()))
                        if (DialogResult.OK == result)
                            try
                            {
                                m_units.SetFormatOptions((ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value,
                                    displayForm.FormatOptions);
                                dataGridView["FormatOptions", e.RowIndex].Value =
                                    UnitFormatUtils.Format(m_units,
                                        (ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value, 1234.56789, false);
                                break;
                            }
                            catch (Exception ex)
                            {
                                TaskDialog.Show(ex.GetType().ToString(), "Set FormatOptions error : \n" + ex.Message,
                                    TaskDialogCommonButtons.Ok);
                            }
                }
            }
        }

        private string GetDecimalSymbolAndGroupingstring()
        {
            var formatvalueoptions = new FormatValueOptions();
            formatvalueoptions.AppendUnitSymbol = false;

            var formatoptions = new FormatOptions(UnitTypeId.Currency, new ForgeTypeId());
            formatoptions.UseDefault = false;
            formatoptions.SetUnitTypeId(UnitTypeId.Currency);
            formatoptions.SetSymbolTypeId(new ForgeTypeId());
            formatoptions.Accuracy = 0.01;
            //formatoptions.SuppressLeadingZeros = true;
            formatoptions.SuppressSpaces = false;
            formatoptions.SuppressTrailingZeros = false;
            formatoptions.UseDigitGrouping = true;
            formatoptions.UsePlusPrefix = false;

            formatvalueoptions.SetFormatOptions(formatoptions);

            return UnitFormatUtils.Format(m_units, SpecTypeId.Number, 123456789.0, false, formatvalueoptions);
        }

        private void DecimalSymbolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_units.DecimalSymbol = (DecimalSymbol)DecimalSymbolComboBox.SelectedItem;
                DecimalSymbolAndGroupingtextBox.Text = GetDecimalSymbolAndGroupingstring();
            }
            catch
            {
                DecimalSymbolComboBox.SelectedItem = m_units.DecimalSymbol;
            }
        }

        private void DigitGroupingAmountComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_units.DigitGroupingAmount = (DigitGroupingAmount)DigitGroupingAmountComboBox.SelectedItem;
                DecimalSymbolAndGroupingtextBox.Text = GetDecimalSymbolAndGroupingstring();
            }
            catch
            {
                DigitGroupingAmountComboBox.SelectedItem = m_units.DigitGroupingAmount;
            }
        }

        private void DigitGroupingSymbolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_units.DigitGroupingSymbol = (DigitGroupingSymbol)Enum.Parse(typeof(DigitGroupingSymbol),
                    (string)DigitGroupingSymbolComboBox.SelectedItem);
                DecimalSymbolAndGroupingtextBox.Text = GetDecimalSymbolAndGroupingstring();
            }
            catch
            {
                DigitGroupingSymbolComboBox.SelectedItem =
                    Enum.GetName(typeof(DigitGroupingSymbol), m_units.DigitGroupingSymbol);
            }
        }
    }
}
