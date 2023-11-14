// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.FoundationSlab.CS
{
    /// <summary>
    ///     A class to show properties and profiles of the foundation slabs.
    /// </summary>
    public partial class FoundationSlabForm : Form
    {
        private readonly DataGridViewTextBoxColumn levelNameColumn = new DataGridViewTextBoxColumn();

        // Revit datas for UI to display and operate.
        private readonly SlabData m_datas;
        private readonly DataGridViewTextBoxColumn markColumn = new DataGridViewTextBoxColumn();

        // The columns of DataGridView.
        private readonly DataGridViewCheckBoxColumn selectedColumn = new DataGridViewCheckBoxColumn();
        private readonly DataGridViewTextBoxColumn slabTypeNameColumn = new DataGridViewTextBoxColumn();

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="datas">The object contains floors' datas.</param>
        public FoundationSlabForm(SlabData datas)
        {
            m_datas = datas;

            InitializeComponent();
            InitializeDataGridView(); // DataGridView initialization.
        }

        private void InitializeDataGridView()
        {
            // Edit the columns of the dataGridView.
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(selectedColumn, markColumn, levelNameColumn, slabTypeNameColumn);
            dataGridView.DataSource = m_datas.BaseSlabList;

            // Select
            selectedColumn.DataPropertyName = "Selected";
            selectedColumn.HeaderText = "Select";
            selectedColumn.Name = "SelectedColumn";
            selectedColumn.ReadOnly = false;
            selectedColumn.Width = dataGridView.Width / 8;

            // Mark
            markColumn.DataPropertyName = "Mark";
            markColumn.HeaderText = "Mark";
            markColumn.Name = "MarkColumn";
            markColumn.ReadOnly = true;
            markColumn.Width = dataGridView.Width / 9;

            var remainWidth = dataGridView.Width - dataGridView.RowHeadersWidth - selectedColumn.Width -
                              markColumn.Width;

            // Level
            levelNameColumn.DataPropertyName = "LevelName";
            levelNameColumn.HeaderText = "Level";
            levelNameColumn.Name = "levelNameColumn";
            levelNameColumn.ReadOnly = true;
            levelNameColumn.Width = remainWidth / 2 - 2;

            // Slab Type
            slabTypeNameColumn.DataPropertyName = "SlabTypeName";
            slabTypeNameColumn.HeaderText = "Slab Type";
            slabTypeNameColumn.Name = "SlabTypeNameColumn";
            slabTypeNameColumn.ReadOnly = true;
            slabTypeNameColumn.Width = remainWidth / 2;
        }

        /// <summary>
        ///     Form load.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void FoundationSlabForm_Load(object sender, EventArgs e)
        {
            okButton.Enabled = m_datas.CheckHaveSelected();
            typeComboBox.DataSource = m_datas.FoundationSlabTypeList;
            typeComboBox.DisplayMember = "Name";

            if (0 == m_datas.BaseSlabList.Count)
            {
                selectAllButton.Enabled = false;
                clearAllButton.Enabled = false;
            }
        }

        /// <summary>
        ///     Cell value changed.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            okButton.Enabled = m_datas.CheckHaveSelected();
            pictureBox.Refresh();
        }

        /// <summary>
        ///     Cell click.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Click on CheckBoxes.
            if (e.ColumnIndex >= 0 && "CheckBoxes" == dataGridView.Columns[e.ColumnIndex].Name)
            {
                var newE = new EventArgs();
                dataGridView_CurrentCellDirtyStateChanged(this, newE);
            }
        }

        /// <summary>
        ///     Current Cell Dirty State Changed.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit); // Commit dataGridView.
        }

        /// <summary>
        ///     Paint.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Sketch.DrawProfile(e.Graphics, pictureBox.DisplayRectangle, m_datas.BaseSlabList); // Draw profiles.
        }

        /// <summary>
        ///     Click ok button.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            var IsSuccess = m_datas.CreateFoundationSlabs();
            if (IsSuccess)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     Click cancel button.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Select type.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_datas.FoundationSlabType = typeComboBox.SelectedItem;
        }

        /// <summary>
        ///     Click selectAllButton.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void selectAllButton_Click(object sender, EventArgs e)
        {
            m_datas.ChangeAllSelected(true);
            dataGridView.Refresh();
            okButton.Enabled = true;
            pictureBox.Refresh();
        }

        /// <summary>
        ///     Click clearAllButton.
        /// </summary>
        /// <param name="sender">This object.</param>
        /// <param name="e">A object contains the event data.</param>
        private void clearAllButton_Click(object sender, EventArgs e)
        {
            m_datas.ChangeAllSelected(false);
            dataGridView.Refresh();
            okButton.Enabled = false;
            pictureBox.Refresh();
        }
    }
}
