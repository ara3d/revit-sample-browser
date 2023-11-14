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

namespace Revit.SDK.Samples.SpotDimension.CS
{
    /// <summary>
    ///     spot dimension form to display information
    /// </summary>
    public partial class SpotDimensionInfoDlg : Form
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public SpotDimensionInfoDlg()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Overload constructor
        /// </summary>
        /// <param name="data"></param>
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

        /// <summary>
        ///     Initialize custom component
        /// </summary>
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

        /// <summary>
        ///     Display all the spot dimensions in the selected view
        /// </summary>
        /// <param name="viewName"></param>
        private void DisplaySpotDimensionInfos(string viewName)
        {
            spotDimensionsListView.Items.Clear();

            //add SpotDimensions to the listview
            foreach (var tmpSpotDimension in m_data.SpotDimensions)
                if (tmpSpotDimension.View.Name == viewName)
                {
                    //create a list view Item
                    var tmpItem = new ListViewItem(tmpSpotDimension.Id.ToString());
                    tmpItem.Tag = tmpSpotDimension;

                    //add the item to the listview
                    spotDimensionsListView.Items.Add(tmpItem);
                }
        }

        /// <summary>
        ///     When selected another spot dimension then update the DataGridView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     When selected view changed, then update the list box to show the spot dimensions in that view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectViewName = viewsComboBox.SelectedItem as string;
            if (selectViewName != null) DisplaySpotDimensionInfos(selectViewName);
        }
    }
}