// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace RevitMultiSample.Loads.CS
{
    /// <summary>
    ///     mainly deal with the operation on load case page on the form
    /// </summary>
    public partial class LoadsForm
    {
        private DataGridViewComboBoxColumn m_loadCasesCategory;
        private DataGridViewTextBoxColumn m_loadCasesName;
        private DataGridViewComboBoxColumn m_loadCasesNature;
        private DataGridViewTextBoxColumn m_loadCasesNumber;
        private DataGridViewTextBoxColumn m_loadNatureName;
        private int m_loadCaseDataGridViewSelectedIndex;
        private int m_loadNatureDataGridViewSelectedIndex;

        // Methods
        /// <summary>
        ///     Initialize the data on this page.
        /// </summary>
        private void InitializeLoadCasePage()
        {
            InitializeLoadCasesDataGridView();
            InitializeLoadNaturesDataGridView();

            if (0 == m_dataBuffer.LoadCases.Count) duplicateLoadCasesButton.Enabled = false;
            if (0 == m_dataBuffer.LoadNatures.Count) addLoadNaturesButton.Enabled = false;
            addLoadNaturesButton.Enabled = false;
        }

        /// <summary>
        ///     Initialize the loadCasesDataGridView
        /// </summary>
        private void InitializeLoadCasesDataGridView()
        {
            m_loadCasesName = new DataGridViewTextBoxColumn();
            m_loadCasesNumber = new DataGridViewTextBoxColumn();
            m_loadCasesNature = new DataGridViewComboBoxColumn();
            m_loadCasesCategory = new DataGridViewComboBoxColumn();
            loadCasesDataGridView.AutoGenerateColumns = false;
            loadCasesDataGridView.Columns.AddRange(m_loadCasesName, m_loadCasesNumber, m_loadCasesNature, m_loadCasesCategory);
            loadCasesDataGridView.DataSource = m_dataBuffer.LoadCasesMap;

            m_loadCasesName.DataPropertyName = "LoadCasesName";
            m_loadCasesName.HeaderText = "Name";
            m_loadCasesName.Name = "LoadCasesName";
            m_loadCasesName.ReadOnly = false;
            m_loadCasesName.Width = loadCasesDataGridView.Width / 6;

            m_loadCasesNumber.DataPropertyName = "LoadCasesNumber";
            m_loadCasesNumber.HeaderText = "Case Number";
            m_loadCasesNumber.Name = "LoadCasesNumber";
            m_loadCasesNumber.ReadOnly = true;
            m_loadCasesNumber.Width = loadCasesDataGridView.Width / 4;

            m_loadCasesNature.DataPropertyName = "LoadCasesNatureId";
            m_loadCasesNature.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            m_loadCasesNature.HeaderText = "Nature";
            m_loadCasesNature.Name = "LoadCasesNature";
            m_loadCasesNature.Resizable = DataGridViewTriState.True;
            m_loadCasesNature.SortMode = DataGridViewColumnSortMode.Automatic;
            m_loadCasesNature.Width = loadCasesDataGridView.Width / 4;

            m_loadCasesNature.DataSource = m_dataBuffer.LoadNatures;
            m_loadCasesNature.DisplayMember = "Name";
            m_loadCasesNature.ValueMember = "Id";

            m_loadCasesCategory.DataPropertyName = "LoadCasesCategoryId";
            m_loadCasesCategory.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            m_loadCasesCategory.HeaderText = "Category";
            m_loadCasesCategory.Name = "LoadCasesCategory";
            m_loadCasesCategory.Resizable = DataGridViewTriState.True;
            m_loadCasesCategory.SortMode = DataGridViewColumnSortMode.Automatic;
            m_loadCasesCategory.Width = loadCasesDataGridView.Width / 4;

            m_loadCasesCategory.DataSource = m_dataBuffer.LoadCaseCategories;
            m_loadCasesCategory.DisplayMember = "Name";
            m_loadCasesCategory.ValueMember = "Id";
            loadCasesDataGridView.MultiSelect = false;
            loadCasesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        /// <summary>
        ///     Initialize the loadNaturesDataGridView
        /// </summary>
        private void InitializeLoadNaturesDataGridView()
        {
            m_loadNatureName = new DataGridViewTextBoxColumn();
            loadNaturesDataGridView.AutoGenerateColumns = false;
            loadNaturesDataGridView.Columns.AddRange(m_loadNatureName);
            loadNaturesDataGridView.DataSource = m_dataBuffer.LoadNaturesMap;
            m_loadNatureName.DataPropertyName = "LoadNaturesName";
            m_loadNatureName.HeaderText = "Name";
            m_loadNatureName.Name = "LoadNaturesName";
            m_loadNatureName.ReadOnly = false;
            m_loadNatureName.Width = loadCasesDataGridView.Width - 100;
            loadNaturesDataGridView.MultiSelect = false;
            loadNaturesDataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        /// <summary>
        ///     Respond the loadCasesDataGridView_CellClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Initilize();
            m_loadCaseDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        ///     Respond the loadNaturesDataGridView_CellClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Initilize();
            m_loadNatureDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        ///     Respond the loadCasesDataGridView_ColumnHeaderMouseClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_loadCaseDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        ///     Respond the loadNaturesDataGridView_RowHeaderMouseClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_loadNatureDataGridViewSelectedIndex = e.RowIndex;
        }

        /// <summary>
        ///     Respond the DataGridView cell validating event,
        ///     check the user's input whether it is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadNaturesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var objectTemp = loadNaturesDataGridView.CurrentCell.Value;
            var nameTemp = objectTemp as string;

            var changeValue = e.FormattedValue;
            var changeValueTemp = changeValue as string;

            if (nameTemp == changeValueTemp) return;

            switch (changeValueTemp)
            {
                case null:
                    TaskDialog.Show("Revit", "Name can not be null");
                    e.Cancel = true;
                    return;
                case "":
                    TaskDialog.Show("Revit", "Name can not be null");
                    e.Cancel = true;
                    return;
            }

            if (!m_dataBuffer.LoadCasesDeal.IsNatureNameUnique(changeValueTemp))
            {
                TaskDialog.Show("Revit", "Name can not be same");
                e.Cancel = true;
            }
        }

        /// <summary>
        ///     Respond the DataGridView cell validating event,
        ///     check the user's input whether it is correct.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadCasesDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            var cellTemp = loadCasesDataGridView.CurrentCell;
            if (null == cellTemp) return;
            if (!(cellTemp.Value is string nameTemp))
            {
                e.Cancel = false;
                return;
            }

            var changeValue = e.FormattedValue;
            var changeValueTemp = changeValue as string;

            if (nameTemp == changeValueTemp) return;

            switch (changeValueTemp)
            {
                case null:
                    TaskDialog.Show("Revit", "Name can not be null");
                    e.Cancel = true;
                    return;
                case "":
                    TaskDialog.Show("Revit", "Name can not be null");
                    e.Cancel = true;
                    return;
            }

            if (!m_dataBuffer.LoadCasesDeal.IsCaseNameUnique(changeValueTemp))
            {
                TaskDialog.Show("Revit", "Name can not be same");
                e.Cancel = true;
            }
        }

        /// <summary>
        ///     When duplicateLoadCasesButton clicked, duplicate a load case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateLoadCasesButton_Click(object sender, EventArgs e)
        {
            m_loadCaseDataGridViewSelectedIndex = loadCasesDataGridView.CurrentCell.RowIndex;
            if (!m_dataBuffer.LoadCasesDeal.DuplicateLoadCase(m_loadCaseDataGridViewSelectedIndex))
            {
                TaskDialog.Show("Revit", "Duplicate failed");
                return;
            }

            ReLoad();
        }

        /// <summary>
        ///     When addLoadNaturesButton clicked, add a new load nature.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addLoadNaturesButton_Click(object sender, EventArgs e)
        {
            if (!m_dataBuffer.LoadCasesDeal.AddLoadNature(m_loadNatureDataGridViewSelectedIndex))
            {
                TaskDialog.Show("Revit", "Add Nature Failed");
                return;
            }

            ReLoad();
        }

        /// <summary>
        ///     Reload the data of the cases and natures
        /// </summary>
        private void ReLoad()
        {
            loadNaturesDataGridView.DataSource = null;
            loadCasesDataGridView.DataSource = null;
            m_loadCasesNature.SortMode = DataGridViewColumnSortMode.Automatic;
            loadNaturesDataGridView.DataSource = m_dataBuffer.LoadNaturesMap;
            loadCasesDataGridView.DataSource = m_dataBuffer.LoadCasesMap;
            Refresh();
        }

        /// <summary>
        ///     enable button
        /// </summary>
        private void Initilize()
        {
            if (loadCasesDataGridView.Focused)
            {
                addLoadNaturesButton.Enabled = false;
                duplicateLoadCasesButton.Enabled = true;
            }
            else if (loadNaturesDataGridView.Focused)
            {
                addLoadNaturesButton.Enabled = true;
                duplicateLoadCasesButton.Enabled = false;
            }

            Refresh();
        }
    }
}
