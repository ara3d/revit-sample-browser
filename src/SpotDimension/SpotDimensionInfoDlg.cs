// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SpotDimension.CS
{
    /// <summary>
    ///     spot dimension form to display information
    /// </summary>
    public partial class SpotDimensionInfoDlg : Form
    {
        public SpotDimensionInfoDlg()
        {
            InitializeComponent();
        }

        public SpotDimensionInfoDlg(ExternalCommandData data)
        {
            m_data = new SpotDimensionsData(data);
            m_typeParamsData = new SpotDimensionParams(data.Application.ActiveUIDocument.Document);
            InitializeComponent();
            InitializeCustomComponent();
        }

        /// <summary>
        ///     Get the last selected spot dimension
        /// </summary>
        public Autodesk.Revit.DB.SpotDimension SelectedSpotDimension { get; private set; }

        private void InitializeCustomComponent()
        {
            viewsComboBox.DataSource = m_data.Views;
            if (viewsComboBox.Items.Count > 0) viewsComboBox.SelectedIndex = 0;

            typeParamsDataGridView.ScrollBars = ScrollBars.Both;
            typeParamsDataGridView.AllowUserToResizeColumns = false;
            typeParamsDataGridView.ColumnHeadersVisible = true;
            typeParamsDataGridView.RowHeadersVisible = false;
            typeParamsDataGridView.AllowUserToResizeRows = false;
            typeParamsDataGridView.AllowUserToOrderColumns = false;
            typeParamsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void DisplaySpotDimensionInfos(string viewName)
        {
            spotDimensionsListView.Items.Clear();

            foreach (var tmpSpotDimension in m_data.SpotDimensions)
            {
                if (tmpSpotDimension.View.Name == viewName)
                {
                    var tmpItem = new ListViewItem(tmpSpotDimension.Id.ToString())
                    {
                        Tag = tmpSpotDimension
                    };

                    spotDimensionsListView.Items.Add(tmpItem);
                }
            }
        }

        private void spotDimensionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (spotDimensionsListView.FocusedItem != null)
            {
                SelectedSpotDimension = spotDimensionsListView.FocusedItem.Tag as Autodesk.Revit.DB.SpotDimension;
                typeParamsDataGridView.DataSource
                    = m_typeParamsData.GetParameterTable(SelectedSpotDimension);

                typeParamsDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                if (typeParamsDataGridView.Columns[0].Width + typeParamsDataGridView.Columns[1].Width <
                    typeParamsDataGridView.Width)
                {
                    typeParamsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    typeParamsDataGridView.AutoResizeColumns();
                }
            }
        }

        private void viewsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (viewsComboBox.SelectedItem is string selectViewName) DisplaySpotDimensionInfos(selectViewName);
        }
    }
}
