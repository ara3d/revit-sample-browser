// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS
{
    /// <summary>
    ///     Provide a dialog which lets users choose views to export.
    /// </summary>
    public partial class SelectViewsForm : Form
    {
        /// <summary>
        ///     Data class
        /// </summary>
        private readonly SelectViewsData m_selectViewsData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="selectViewsData"></param>
        public SelectViewsForm(SelectViewsData selectViewsData)
        {
            InitializeComponent();
            m_selectViewsData = selectViewsData;
            InitializeControls();
        }

        /// <summary>
        ///     Initialize values and status of controls
        /// </summary>
        private void InitializeControls()
        {
            UpdateViews();
        }

        /// <summary>
        ///     Check all items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxViews.Items.Count; ++i) checkedListBoxViews.SetItemChecked(i, true);
        }

        /// <summary>
        ///     Un-check all items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCheckNone_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxViews.Items.Count; ++i) checkedListBoxViews.SetItemChecked(i, false);
        }

        /// <summary>
        ///     Whether to show the sheets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxSheets_CheckedChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        /// <summary>
        ///     Whether to show the views (except sheets)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxViews_CheckedChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        /// <summary>
        ///     Update the views in the checked list box
        /// </summary>
        private void UpdateViews()
        {
            checkedListBoxViews.Items.Clear();
            if (checkBoxViews.Checked)
                foreach (View view in m_selectViewsData.PrintableViews)
                    checkedListBoxViews.Items.Add(view.ViewType + ": " + view.Name);

            if (checkBoxSheets.Checked)
                foreach (ViewSheet viewSheet in m_selectViewsData.PrintableSheets)
                    checkedListBoxViews.Items.Add("Drawing Sheet: " + viewSheet.SheetNumber + " - " +
                                                  viewSheet.Name);
            checkedListBoxViews.Sorted = true;
        }

        /// <summary>
        ///     OK button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            GetSelectedViews();
            Close();
        }

        /// <summary>
        ///     Transfer information back to SelectViewsData class
        /// </summary>
        /// <returns></returns>
        private void GetSelectedViews()
        {
            m_selectViewsData.Contain3DView = false;

            foreach (int index in checkedListBoxViews.CheckedIndices)
            {
                var text = checkedListBoxViews.Items[index].ToString();
                var sheetPrefix = "Drawing Sheet: ";
                if (text.StartsWith(sheetPrefix))
                {
                    text = text.Substring(sheetPrefix.Length);
                    var sheetNumber = text.Substring(0, text.IndexOf(" - "));
                    var sheetViewName = text.Substring(text.IndexOf(" - ") + 3);
                    foreach (ViewSheet viewSheet in m_selectViewsData.PrintableSheets)
                        if (viewSheet.SheetNumber == sheetNumber && viewSheet.Name == sheetViewName)
                        {
                            m_selectViewsData.SelectedViews.Insert(viewSheet);
                            break;
                        }
                }
                else
                {
                    var viewType = text.Substring(0, text.IndexOf(": "));
                    var viewName = text.Substring(text.IndexOf(": ") + 2);
                    foreach (View view in m_selectViewsData.PrintableViews)
                    {
                        var vt = view.ViewType;
                        if (viewType == vt.ToString() && viewName == view.Name)
                        {
                            m_selectViewsData.SelectedViews.Insert(view);
                            if (vt == ViewType.ThreeD) m_selectViewsData.Contain3DView = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
