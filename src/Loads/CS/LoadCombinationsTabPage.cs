// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    ///     mainly deal with the operation on load combination page on the form
    /// </summary>
    public partial class LoadsForm
    {
        private DataGridViewTextBoxColumn combinationFormulaColumn;

        // Private Members
        // Define the columns in LoadCombination DataGridView control
        private DataGridViewTextBoxColumn combinationNameColumn;
        private DataGridViewTextBoxColumn combinationStateColumn;
        private DataGridViewTextBoxColumn combinationTypeColumn;
        private DataGridViewTextBoxColumn combinationUsageColumn;
        private DataGridViewComboBoxColumn formulaCaseColumn;

        // Define the columns in Formula DataGridView control
        private DataGridViewTextBoxColumn formulaFactorColumn;
        private DataGridViewTextBoxColumn usageNameColumn;

        // Define the columns in Usage DataGridView control
        private DataGridViewCheckBoxColumn usageSetColumn;

        // Methods
        /// <summary>
        ///     Initialize the data on this page.
        /// </summary>
        private void InitializeLoadCombinationPage()
        {
            // Add the Items in combinationType and combinationState comboBox
            combinationTypeComboBox.Items.AddRange(new object[] { "Combination", "Envelope" });
            combinationStateComboBox.Items.AddRange(new object[] { "Serviceability", "Ultimate" });

            // Initialize the loadCombination DataGridView control
            InitializeCombinationGrid();

            // Initialize the load combination usage DataGridView control
            InitializeUsageGrid();

            // Initialize the load combination formula DataGridView control
            InitializeFormulaGrid();

            // Other initialization
            combinationNameTextBox.Text = null;
            combinationTypeComboBox.SelectedIndex = 0;
            combinationStateComboBox.SelectedIndex = 0;

            // Set the state of the button on this page.
            CombinationsTabPageButtonEnable();
        }

        /// <summary>
        ///     Initialize the loadCombination DataGridView control
        /// </summary>
        private void InitializeCombinationGrid()
        {
            // Initialize the column data members.
            combinationNameColumn = new DataGridViewTextBoxColumn();
            combinationFormulaColumn = new DataGridViewTextBoxColumn();
            combinationTypeColumn = new DataGridViewTextBoxColumn();
            combinationStateColumn = new DataGridViewTextBoxColumn();
            combinationUsageColumn = new DataGridViewTextBoxColumn();

            // Binging the columns to the DataGridView
            combinationDataGridView.AutoGenerateColumns = false;
            combinationDataGridView.Columns.AddRange(combinationNameColumn, combinationFormulaColumn,
                combinationTypeColumn, combinationStateColumn, combinationUsageColumn);

            // Binging the data source and set this grid to readonly.
            combinationDataGridView.DataSource = m_dataBuffer.LoadCombinationMap;
            combinationDataGridView.ReadOnly = true;

            // Initialize each column
            combinationNameColumn.DataPropertyName = "Name";
            combinationNameColumn.HeaderText = "Name";
            combinationNameColumn.Name = "combinationNameColumn";
            combinationNameColumn.Width = combinationDataGridView.Width / 7;

            combinationFormulaColumn.DataPropertyName = "Formula";
            combinationFormulaColumn.HeaderText = "Formula";
            combinationFormulaColumn.Name = "combinationFormulaColumn";
            combinationFormulaColumn.Width = combinationDataGridView.Width / 4;

            combinationTypeColumn.DataPropertyName = "Type";
            combinationTypeColumn.HeaderText = "Type";
            combinationTypeColumn.Name = "combinationTypeColumn";
            combinationTypeColumn.Width = combinationDataGridView.Width / 7;

            combinationStateColumn.DataPropertyName = "State";
            combinationStateColumn.HeaderText = "State";
            combinationStateColumn.Name = "combinationStateColumn";
            combinationStateColumn.Width = combinationDataGridView.Width / 7;

            combinationUsageColumn.DataPropertyName = "Usage";
            combinationUsageColumn.HeaderText = "Usage";
            combinationUsageColumn.Name = "combinationUsageColumn";
            combinationUsageColumn.Width = combinationDataGridView.Width / 3;
        }

        /// <summary>
        ///     Initialize the load combination usage DataGridView control
        /// </summary>
        private void InitializeUsageGrid()
        {
            // Initialize the column data members.
            usageSetColumn = new DataGridViewCheckBoxColumn();
            usageNameColumn = new DataGridViewTextBoxColumn();

            // Binging the columns to the DataGridView
            usageDataGridView.AutoGenerateColumns = false;
            usageDataGridView.Columns.AddRange(usageSetColumn, usageNameColumn);

            // Binding the data source.
            usageDataGridView.DataSource = m_dataBuffer.UsageMap;

            // Binding event
            usageDataGridView.CellValidating += usageDataGridView_CellValidating;
            // Initialize each column.
            usageSetColumn.HeaderText = "Set";
            usageSetColumn.DataPropertyName = "Set";
            usageSetColumn.Name = "usageSetColumn";
            usageSetColumn.Width = usageDataGridView.Width / 4;

            usageNameColumn.DataPropertyName = "Name";
            usageNameColumn.HeaderText = "Case";
            usageNameColumn.Name = "usageNameColumn";
            usageNameColumn.Width = usageDataGridView.Width / 2;
        }


        /// <summary>
        ///     Initialize the load combination formula DataGridView control
        /// </summary>
        private void InitializeFormulaGrid()
        {
            // Initialize the column data members.
            formulaFactorColumn = new DataGridViewTextBoxColumn();
            formulaCaseColumn = new DataGridViewComboBoxColumn();

            // Binging the columns to the DataGridView
            formulaDataGridView.AutoGenerateColumns = false;
            formulaDataGridView.Columns.AddRange(formulaFactorColumn, formulaCaseColumn);

            // Binging the data source.
            formulaDataGridView.DataSource = m_dataBuffer.FormulaMap;

            // Initialize each column.
            formulaFactorColumn.DataPropertyName = "Factor";
            formulaFactorColumn.HeaderText = "Factor";
            formulaFactorColumn.Name = "formulaFactorColumn";
            formulaFactorColumn.Width = formulaDataGridView.Width / 4;

            formulaCaseColumn.DataPropertyName = "Case";
            formulaCaseColumn.HeaderText = "Case";
            formulaCaseColumn.Name = "formulaCaseColumn";
            formulaCaseColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            formulaCaseColumn.Width = formulaDataGridView.Width / 2;

            formulaCaseColumn.DataSource = m_dataBuffer.LoadCases;
            formulaCaseColumn.DisplayMember = "Name";
            formulaCaseColumn.ValueMember = "Name";
        }

        // The Button Event method.
        /// <summary>
        ///     Check All Button
        /// </summary>
        private void usageCheckAllButton_Click(object sender, EventArgs e)
        {
            foreach (var map in m_dataBuffer.UsageMap) map.Set = true;
            // Disable the CheckAll button, enable CheckNone button and refresh.
            usageCheckAllButton.Enabled = false;
            usageCheckNoneButton.Enabled = true;
            usageDataGridView.Refresh();
        }

        /// <summary>
        ///     Check None Button
        /// </summary>
        private void usageCheckNoneButton_Click(object sender, EventArgs e)
        {
            foreach (var map in m_dataBuffer.UsageMap) map.Set = false;

            // Disable the CheckNone button, enable CheckAll button and refresh.
            usageCheckAllButton.Enabled = true;
            usageCheckNoneButton.Enabled = false;
            usageDataGridView.Refresh();
        }

        /// <summary>
        ///     Usage Add Button
        /// </summary>
        private void usageAddButton_Click(object sender, EventArgs e)
        {
            var usageString = new StringBuilder("Usage");
            var i = 1;
            var needFind = true; // need to find another name which is not used

            // First, need to find a name which is not the same as the other usages.
            while (needFind)
            {
                var isEqual = false;
                usageString.Append(i);
                foreach (var s in m_dataBuffer.LoadUsageNames)
                    if (s == usageString.ToString())
                    {
                        isEqual = true;
                        break;
                    }

                if (isEqual)
                {
                    usageString.Remove(0, usageString.Length);
                    usageString.Append("Usage");
                    i++;
                }
                else
                {
                    needFind = false;
                }
            }

            // Begin to add new Load Combination Usage
            var usageName = usageString.ToString();
            if (!m_dataBuffer.NewLoadUsage(usageName))
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // Refresh the Load Combination Usage DataGridView control
            usageDataGridView.DataSource = null;
            usageDataGridView.DataSource = m_dataBuffer.UsageMap;

            // Change the state of the buttons and refresh.
            CombinationsTabPageButtonEnable();
            usageDataGridView.Refresh();
        }

        /// <summary>
        ///     Usage Delete Button
        /// </summary>
        private void usageDeleteButton_Click(object sender, EventArgs e)
        {
            // // Get selected index in usage DataGridView
            var index = usageDataGridView.CurrentRow.Index;
            if (0 > index)
            {
                TaskDialog.Show("Revit", "The program should go here.");
                return;
            }

            // Begin to delete the Usage
            if (!m_dataBuffer.DeleteUsage(index))
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // After deleting usage, refresh usage DataGridView
            usageDataGridView.DataSource = null;
            usageDataGridView.DataSource = m_dataBuffer.UsageMap;

            // Set the state of the button on this page and refresh.
            CombinationsTabPageButtonEnable();
            Refresh();
        }

        /// <summary>
        ///     When modify Usage name, judge if the inputted Name is unique.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usageDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Modifying the usage name
            if (1 == usageDataGridView.CurrentCell.ColumnIndex)
            {
                var newName = e.FormattedValue as string;
                var oldName = usageDataGridView.CurrentCell.FormattedValue as string;
                if (newName != oldName)
                    foreach (var usageMap in m_dataBuffer.UsageMap)
                        if (usageMap.Name == newName)
                            e.Cancel = true;
            }
        }

        /// <summary>
        ///     New Combination Button
        /// </summary>
        private void newCombinationButton_Click(object sender, EventArgs e)
        {
            // First get the combination name
            if (null == combinationNameTextBox.Text || "" == combinationNameTextBox.Text)
            {
                TaskDialog.Show("Revit", "Combination name should be input.");
                return;
            }

            //  Check if it has been used
            var name = combinationNameTextBox.Text;
            foreach (var s in m_dataBuffer.LoadCombinationNames)
                if (s == name)
                {
                    TaskDialog.Show("Revit", "Combination name has been used by another combination.");
                    return;
                }

            // get the data and begin to create.
            var typeIndex = combinationTypeComboBox.SelectedIndex;
            var stateIndex = combinationStateComboBox.SelectedIndex;
            if (!m_dataBuffer.NewLoadCombination(name, typeIndex, stateIndex))
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // If create combination successfully, reset some controls.
            combinationNameTextBox.Text = null;
            combinationTypeComboBox.SelectedIndex = 0;
            combinationStateComboBox.SelectedIndex = 0;
            combinationDataGridView.DataSource = null;
            combinationDataGridView.DataSource = m_dataBuffer.LoadCombinationMap;
            formulaDataGridView.DataSource = null;
            formulaDataGridView.DataSource = m_dataBuffer.FormulaMap;

            // Set the state of the button on this page and refresh.
            CombinationsTabPageButtonEnable();
            Refresh();
        }

        /// <summary>
        ///     Delete Combination Button
        /// </summary>
        private void deleteCombinationButton_Click(object sender, EventArgs e)
        {
            // Get selected index in Combination DataGridView
            var index = combinationDataGridView.CurrentRow.Index;
            if (0 > index)
            {
                TaskDialog.Show("Revit", "The program should go here.");
                return;
            }

            // Begin to delete the combination
            if (!m_dataBuffer.DeleteCombination(index))
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // After deleting combination, refresh combination DataGridView
            combinationDataGridView.DataSource = null;
            combinationDataGridView.DataSource = m_dataBuffer.LoadCombinationMap;

            // Set the state of the button on this page and refresh.
            CombinationsTabPageButtonEnable();
            Refresh();
        }

        /// <summary>
        ///     Add Formula Button
        /// </summary>
        private void formulaAddButton_Click(object sender, EventArgs e)
        {
            // Add formula.
            if (!m_dataBuffer.AddFormula())
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // After adding formula, refresh formula DataGridView
            formulaDataGridView.DataSource = null;
            formulaDataGridView.DataSource = m_dataBuffer.FormulaMap;

            // Set the state of the button on this page and refresh.
            CombinationsTabPageButtonEnable();
            Refresh();
        }

        /// <summary>
        ///     Delete Formula Button
        /// </summary>
        private void formulaDeleteButton_Click(object sender, EventArgs e)
        {
            // Get selected index in formula DataGridView
            var index = formulaDataGridView.CurrentRow.Index;
            if (0 > index)
            {
                TaskDialog.Show("Revit", "The program should go here.");
                return;
            }

            // Begin to delete the formula
            if (!m_dataBuffer.DeleteFormula(index))
            {
                TaskDialog.Show("Revit", m_dataBuffer.ErrorInformation);
                return;
            }

            // After deleting formula, refresh formula DataGridView
            formulaDataGridView.DataSource = null;
            formulaDataGridView.DataSource = m_dataBuffer.FormulaMap;

            // Set the state of the button on this page and refresh.
            CombinationsTabPageButtonEnable();
            Refresh();
        }

        // The following is the event when the DataGridView got focus
        private void combinationDataGridView_GotFocus(object sender, EventArgs e)
        {
            CombinationsTabPageButtonEnable();
        }

        /// <summary>
        ///     If the formulaDataGridView get focus, enable the buttons in CombinationsTabPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formulaDataGridView_GotFocus(object sender, EventArgs e)
        {
            CombinationsTabPageButtonEnable();
        }

        /// <summary>
        ///     If the usageDataGridView get focus, enable the buttons in CombinationsTabPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usageDataGridView_GotFocus(object sender, EventArgs e)
        {
            CombinationsTabPageButtonEnable();
        }

        /// <summary>
        ///     The following is the event when the usage DataGridView selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void usageDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            CombinationsTabPageButtonEnable();
        }

        /// <summary>
        ///     This method used to check the state of all the buttons on Load Combination page,
        ///     and judge whether the buttons should be enable or disable.
        /// </summary>
        private void CombinationsTabPageButtonEnable()
        {
            // first make each button to unable
            var usageCheckAllButtonEnabled = true;
            var usageCheckNoneButtonEnabled = true;
            var usageAddButtonEnabled = true;
            var usageDeleteButtonEnabled = true;
            var newCombinationButtonEnabled = true;
            var deleteCombinationButtonEnabled = true;
            var formulaAddButtonEnabled = true;
            var formulaDeleteButtonEnabled = true;

            // If there is no LoadCase,
            // All the button control formula should be disable
            if (0 == m_dataBuffer.LoadCases.Count)
            {
                formulaAddButtonEnabled = false;
                formulaDeleteButtonEnabled = false;
            }

            // If the usage DataGridView has no data
            // CheckAll, CheckNone, Delete button should be disable
            if (0 == m_dataBuffer.UsageMap.Count)
            {
                usageCheckAllButtonEnabled = false;
                usageCheckNoneButtonEnabled = false;
                usageDeleteButtonEnabled = false;
            }
            else
            {
                var checkedCount = 0;
                foreach (var map in m_dataBuffer.UsageMap)
                    if (map.Set)
                        checkedCount++;

                if (checkedCount <= 0)
                {
                    usageCheckAllButtonEnabled = true;
                    usageCheckNoneButtonEnabled = false;
                }
                else if (checkedCount > 0 && checkedCount < m_dataBuffer.UsageMap.Count)
                {
                    usageCheckAllButtonEnabled = true;
                    usageCheckNoneButtonEnabled = true;
                }
                else
                {
                    usageCheckAllButtonEnabled = false;
                    usageCheckNoneButtonEnabled = true;
                }
            }

            // If the formula DataGridView has no data or is not focused
            // Delete formula button should be disable
            if (0 == m_dataBuffer.FormulaMap.Count) formulaDeleteButtonEnabled = false;
            // If the combination DataGridView has no data or is not focused
            // Delete combination button should be disable.
            if (0 == m_dataBuffer.LoadCombinationMap.Count) deleteCombinationButtonEnabled = false;

            // At last, set the Buttons state
            usageCheckAllButton.Enabled = usageCheckAllButtonEnabled;
            usageCheckNoneButton.Enabled = usageCheckNoneButtonEnabled;
            usageAddButton.Enabled = usageAddButtonEnabled;
            usageDeleteButton.Enabled = usageDeleteButtonEnabled;
            newCombinationButton.Enabled = newCombinationButtonEnabled;
            deleteCombinationButton.Enabled = deleteCombinationButtonEnabled;
            formulaAddButton.Enabled = formulaAddButtonEnabled;
            formulaDeleteButton.Enabled = formulaDeleteButtonEnabled;
        }

        private void usageDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CombinationsTabPageButtonEnable();
        }
    }
}
