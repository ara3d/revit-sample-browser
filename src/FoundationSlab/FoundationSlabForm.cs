// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using System;
using System.Windows.Forms;
namespace Ara3D.RevitSampleBrowser.FoundationSlab.CS
{
    public partial class FoundationSlabForm : Form
    {
        private readonly DataGridViewTextBoxColumn m_levelNameColumn = new();
        private readonly SlabData m_datas;
        private readonly DataGridViewTextBoxColumn m_markColumn = new();
        private readonly DataGridViewCheckBoxColumn m_selectedColumn = new();
        private readonly DataGridViewTextBoxColumn m_slabTypeNameColumn = new();

        public FoundationSlabForm(SlabData datas)
        {
            m_datas = datas;

            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(m_selectedColumn, m_markColumn, m_levelNameColumn, m_slabTypeNameColumn);
            dataGridView.DataSource = m_datas.BaseSlabList;

            m_selectedColumn.DataPropertyName = "Selected";
            m_selectedColumn.HeaderText = "Select";
            m_selectedColumn.Name = "SelectedColumn";
            m_selectedColumn.ReadOnly = false;
            m_selectedColumn.Width = dataGridView.Width / 8;

            m_markColumn.DataPropertyName = "Mark";
            m_markColumn.HeaderText = "Mark";
            m_markColumn.Name = "MarkColumn";
            m_markColumn.ReadOnly = true;
            m_markColumn.Width = dataGridView.Width / 9;

            var remainWidth = dataGridView.Width - dataGridView.RowHeadersWidth - m_selectedColumn.Width -
                              m_markColumn.Width;

            m_levelNameColumn.DataPropertyName = "LevelName";
            m_levelNameColumn.HeaderText = "Level";
            m_levelNameColumn.Name = "levelNameColumn";
            m_levelNameColumn.ReadOnly = true;
            m_levelNameColumn.Width = (remainWidth / 2) - 2;

            m_slabTypeNameColumn.DataPropertyName = "SlabTypeName";
            m_slabTypeNameColumn.HeaderText = "Slab Type";
            m_slabTypeNameColumn.Name = "SlabTypeNameColumn";
            m_slabTypeNameColumn.ReadOnly = true;
            m_slabTypeNameColumn.Width = remainWidth / 2;
        }

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

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            okButton.Enabled = m_datas.CheckHaveSelected();
            pictureBox.Refresh();
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && "CheckBoxes" == dataGridView.Columns[e.ColumnIndex].Name)
            {
                EventArgs newE = new();
                dataGridView_CurrentCellDirtyStateChanged(this, newE);
            }
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            SampleBrowserUtils.DrawProfile(e.Graphics, pictureBox.DisplayRectangle, m_datas.BaseSlabList);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var isSuccess = m_datas.CreateFoundationSlabs();
            DialogResult = isSuccess ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_datas.FoundationSlabType = typeComboBox.SelectedItem;
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            m_datas.ChangeAllSelected(true);
            dataGridView.Refresh();
            okButton.Enabled = true;
            pictureBox.Refresh();
        }

        private void clearAllButton_Click(object sender, EventArgs e)
        {
            m_datas.ChangeAllSelected(false);
            dataGridView.Refresh();
            okButton.Enabled = false;
            pictureBox.Refresh();
        }
    }
}
