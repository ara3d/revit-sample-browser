// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public partial class SelectViewsForm : Form
    {
        private readonly SelectViewsData m_selectViewsData;

        public SelectViewsForm(SelectViewsData selectViewsData)
        {
            InitializeComponent();
            m_selectViewsData = selectViewsData;
            InitializeControls();
        }

        private void InitializeControls()
        {
            UpdateViews();
        }

        private void buttonCheckAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxViews.Items.Count; ++i) checkedListBoxViews.SetItemChecked(i, true);
        }

        private void buttonCheckNone_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBoxViews.Items.Count; ++i) checkedListBoxViews.SetItemChecked(i, false);
        }

        private void checkBoxSheets_CheckedChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void checkBoxViews_CheckedChanged(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void UpdateViews()
        {
            checkedListBoxViews.Items.Clear();
            if (checkBoxViews.Checked)
                foreach (View view in m_selectViewsData.PrintableViews)
                {
                    checkedListBoxViews.Items.Add($"{view.ViewType}: {view.Name}");
                }

            if (checkBoxSheets.Checked)
                foreach (ViewSheet viewSheet in m_selectViewsData.PrintableSheets)
                {
                    checkedListBoxViews.Items.Add($"Drawing Sheet: {viewSheet.SheetNumber} - {viewSheet.Name}");
                }

            checkedListBoxViews.Sorted = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            GetSelectedViews();
            Close();
        }

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
                    {
                        if (viewSheet.SheetNumber == sheetNumber && viewSheet.Name == sheetViewName)
                        {
                            m_selectViewsData.SelectedViews.Insert(viewSheet);
                            break;
                        }
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
