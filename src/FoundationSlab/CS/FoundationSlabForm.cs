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
        private readonly DataGridViewTextBoxColumn m_levelNameColumn = new DataGridViewTextBoxColumn();

        // Revit datas for UI to display and operate.
        private readonly SlabData m_datas;
        private readonly DataGridViewTextBoxColumn m_markColumn = new DataGridViewTextBoxColumn();

        // The columns of DataGridView.
        private readonly DataGridViewCheckBoxColumn m_selectedColumn = new DataGridViewCheckBoxColumn();
        private readonly DataGridViewTextBoxColumn m_slabTypeNameColumn = new DataGridViewTextBoxColumn();

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
            dataGridView.Columns.AddRange(m_selectedColumn, m_markColumn, m_levelNameColumn, m_slabTypeNameColumn);
            dataGridView.DataSource = m_datas.BaseSlabList;

            // Select
            m_selectedColumn.DataPropertyName = "Selected";
            m_selectedColumn.HeaderText = "Select";
            m_selectedColumn.Name = "SelectedColumn";
            m_selectedColumn.ReadOnly = false;
            m_selectedColumn.Width = dataGridView.Width / 8;

            // Mark
            m_markColumn.DataPropertyName = "Mark";
            m_markColumn.HeaderText = "Mark";
            m_markColumn.Name = "MarkColumn";
            m_markColumn.ReadOnly = true;
            m_markColumn.Width = dataGridView.Width / 9;

            var remainWidth = dataGridView.Width - dataGridView.RowHeadersWidth - m_selectedColumn.Width -
                              m_markColumn.Width;

            // Level
            m_levelNameColumn.DataPropertyName = "LevelName";
            m_levelNameColumn.HeaderText = "Level";
            m_levelNameColumn.Name = "levelNameColumn";
            m_levelNameColumn.ReadOnly = true;
            m_levelNameColumn.Width = remainWidth / 2 - 2;

            // Slab Type
            m_slabTypeNameColumn.DataPropertyName = "SlabTypeName";
            m_slabTypeNameColumn.HeaderText = "Slab Type";
            m_slabTypeNameColumn.Name = "SlabTypeNameColumn";
            m_slabTypeNameColumn.ReadOnly = true;
            m_slabTypeNameColumn.Width = remainWidth / 2;
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
            var isSuccess = m_datas.CreateFoundationSlabs();
            if (isSuccess)
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
