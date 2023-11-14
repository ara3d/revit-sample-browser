//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Windows.Forms;

using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Units.CS
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UnitsForm : Form
    {
        //Required designer variable.
       private Autodesk.Revit.DB.Units m_units;

        /// <summary>
        /// Initialize GUI with ProjectUnitData 
        /// </summary>
        /// <param name="units">units in current document</param>
        public UnitsForm(Autodesk.Revit.DB.Units units)
        {
            InitializeComponent();
            m_units = units;
        }

        private void UnitsForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize the combo box and list view. 
                disciplineCombox.BeginUpdate();
                foreach (var disciplineTypeId in Autodesk.Revit.DB.UnitUtils.GetAllDisciplines())
                {
                    disciplineCombox.Items.AddRange(new object[] { Autodesk.Revit.DB.LabelUtils.GetLabelForDiscipline(disciplineTypeId) });
                }
                disciplineCombox.SelectedItem = disciplineCombox.Items[0];
                disciplineCombox.EndUpdate();

                DecimalSymbolComboBox.BeginUpdate();
                foreach (Autodesk.Revit.DB.DecimalSymbol ds in Enum.GetValues(typeof(
                         Autodesk.Revit.DB.DecimalSymbol)))
                {
                    DecimalSymbolComboBox.Items.AddRange(new object[] { ds });
                }
                DecimalSymbolComboBox.EndUpdate();
                DecimalSymbolComboBox.SelectedItem = m_units.DecimalSymbol;

                DigitGroupingAmountComboBox.BeginUpdate();
                foreach (Autodesk.Revit.DB.DigitGroupingAmount dga in Enum.GetValues(typeof(
                         Autodesk.Revit.DB.DigitGroupingAmount)))
                {
                    DigitGroupingAmountComboBox.Items.AddRange(new object[] { dga });
                }
                DigitGroupingAmountComboBox.EndUpdate();
                DigitGroupingAmountComboBox.SelectedItem = m_units.DigitGroupingAmount;

                DigitGroupingSymbolComboBox.BeginUpdate();
                foreach (var enumName in Enum.GetNames(typeof(
                         Autodesk.Revit.DB.DigitGroupingSymbol)))
                {
                    DigitGroupingSymbolComboBox.Items.AddRange(new object[] { enumName });
                }
                DigitGroupingSymbolComboBox.EndUpdate();
                DigitGroupingSymbolComboBox.SelectedItem = Enum.GetName(typeof(Autodesk.Revit.DB.DigitGroupingSymbol), m_units.DigitGroupingSymbol);

                DecimalSymbolAndGroupingtextBox.Text = getDecimalSymbolAndGroupingstring();

                DigitGroupingSymbolComboBox.SelectedIndexChanged += new EventHandler(DigitGroupingSymbolComboBox_SelectedIndexChanged);
                DigitGroupingAmountComboBox.SelectedIndexChanged += new EventHandler(DigitGroupingAmountComboBox_SelectedIndexChanged);
                DecimalSymbolComboBox.SelectedIndexChanged += new EventHandler(DecimalSymbolComboBox_SelectedIndexChanged);
            }
            catch
            {
                throw;
            }
        }

        private void disciplineCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            FillGrid();
        }

        /// <summary>
        /// Fill the grid with selected discipline 
        /// </summary>
        private void FillGrid()
        {
            try
            {
                // using the static example value 1234.56789
                var count = 0;
                foreach (var specTypeId in Autodesk.Revit.DB.UnitUtils.GetAllMeasurableSpecs())
                {
                    if (Autodesk.Revit.DB.LabelUtils.GetLabelForDiscipline(Autodesk.Revit.DB.UnitUtils.GetDiscipline(specTypeId)) == disciplineCombox.SelectedItem.ToString())
                    {
                        dataGridView.Rows.Add();
                        dataGridView["UnitType", count].Value = specTypeId;
                        dataGridView["Label_UnitType", count].Value = Autodesk.Revit.DB.LabelUtils.GetLabelForSpec(specTypeId);
                        dataGridView["FormatOptions", count].Value =
                           Autodesk.Revit.DB.UnitFormatUtils.Format(m_units, specTypeId, 1234.56789, false);
                        count++;
                    }
                }
            }
            catch
            {
                throw;
            }

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                // show UI
                var specTypeId = (Autodesk.Revit.DB.ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value;
                using (var displayForm = new FormatForm(specTypeId, m_units.GetFormatOptions(specTypeId)))
                {
                    DialogResult result;
                    while (DialogResult.Cancel != (result = displayForm.ShowDialog()))
                    {
                        if (DialogResult.OK == result)
                        {
                            try
                            {
                                m_units.SetFormatOptions((Autodesk.Revit.DB.ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value, displayForm.FormatOptions);
                                dataGridView["FormatOptions", e.RowIndex].Value =
                                   Autodesk.Revit.DB.UnitFormatUtils.Format(m_units, (Autodesk.Revit.DB.ForgeTypeId)dataGridView["UnitType", e.RowIndex].Value, 1234.56789, false);
                                break;
                            }
                            catch (Exception ex)
                            {
                                TaskDialog.Show(ex.GetType().ToString(), "Set FormatOptions error : \n" + ex.Message, TaskDialogCommonButtons.Ok);
                            }
                        }
                    }
                }
            }
        }

        private string getDecimalSymbolAndGroupingstring()
        {
            var formatvalueoptions = new Autodesk.Revit.DB.FormatValueOptions();
            formatvalueoptions.AppendUnitSymbol = false;

            var formatoptions = new Autodesk.Revit.DB.FormatOptions(Autodesk.Revit.DB.UnitTypeId.Currency, new Autodesk.Revit.DB.ForgeTypeId());
            formatoptions.UseDefault = false;
            formatoptions.SetUnitTypeId(Autodesk.Revit.DB.UnitTypeId.Currency);
            formatoptions.SetSymbolTypeId(new Autodesk.Revit.DB.ForgeTypeId());
            formatoptions.Accuracy = 0.01;
            //formatoptions.SuppressLeadingZeros = true;
            formatoptions.SuppressSpaces = false;
            formatoptions.SuppressTrailingZeros = false;
            formatoptions.UseDigitGrouping = true;
            formatoptions.UsePlusPrefix = false;

            formatvalueoptions.SetFormatOptions(formatoptions);

            return Autodesk.Revit.DB.UnitFormatUtils.Format(m_units, Autodesk.Revit.DB.SpecTypeId.Number, 123456789.0, false, formatvalueoptions);
            
        }

        private void DecimalSymbolComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_units.DecimalSymbol = (Autodesk.Revit.DB.DecimalSymbol)DecimalSymbolComboBox.SelectedItem;
                DecimalSymbolAndGroupingtextBox.Text = getDecimalSymbolAndGroupingstring();
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
                m_units.DigitGroupingAmount = (Autodesk.Revit.DB.DigitGroupingAmount)DigitGroupingAmountComboBox.SelectedItem;
                DecimalSymbolAndGroupingtextBox.Text = getDecimalSymbolAndGroupingstring();
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
                m_units.DigitGroupingSymbol = (Autodesk.Revit.DB.DigitGroupingSymbol)Enum.Parse(typeof(Autodesk.Revit.DB.DigitGroupingSymbol), (string)DigitGroupingSymbolComboBox.SelectedItem);
                DecimalSymbolAndGroupingtextBox.Text = getDecimalSymbolAndGroupingstring();
            }
            catch
            {
                DigitGroupingSymbolComboBox.SelectedItem = Enum.GetName(typeof(Autodesk.Revit.DB.DigitGroupingSymbol), m_units.DigitGroupingSymbol);
            }
        }
    }
}
