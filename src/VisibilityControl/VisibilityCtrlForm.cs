// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.VisibilityControl.CS
{
    public partial class VisibilityCtrlForm : Form
    {
        // an object control visibility by category
        private readonly VisibilityCtrl m_visibilityCtrl;

        public VisibilityCtrlForm(VisibilityCtrl visibilityCtrl)
        {
            if (null == visibilityCtrl)
                throw new ArgumentNullException(nameof(visibilityCtrl));
            m_visibilityCtrl = visibilityCtrl;

            InitializeComponent();
        }

        private void VisibilityCtrlForm_Load(object sender, EventArgs e)
        {
            // initialize the  checked list box
            allCategoriesListView.Columns.Add("name");
            foreach (string name in m_visibilityCtrl.AllCategories.Keys)
            {
                var check = m_visibilityCtrl.AllCategories[name].ToString().Equals("True") ? true : false;
                var item = new ListViewItem(name)
                {
                    Checked = check
                };
                allCategoriesListView.Items.Add(item);
            }

            allCategoriesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            // append the ItemCheck event handler method
            allCategoriesListView.ItemCheck += allCategoriesListView_ItemCheck;
        }

        private void allCategoriesListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var visible = e.NewValue == CheckState.Checked ? true : false;
            var name = allCategoriesListView.Items[e.Index].Text;

            // change the visibility of the category
            if (!m_visibilityCtrl.SetVisibility(visible, name))
                TaskDialog.Show("Revit", "This category can not change visible in the active view.");
        }

        private void isolateButton_Click(object sender, EventArgs e)
        {
            // set the IsolateMode
            if (pickOneRadioButton.Checked)
                m_visibilityCtrl.IsolateMode = IsolateMode.PickOne;
            else if (windowSelectRadioButton.Checked)
                m_visibilityCtrl.IsolateMode = IsolateMode.WindowSelect;
            else
                m_visibilityCtrl.IsolateMode = IsolateMode.None;

            // close the form
            Close();
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in allCategoriesListView.Items)
            {
                item.Checked = true;
            }
        }

        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in allCategoriesListView.Items)
            {
                item.Checked = false;
            }
        }
    }
}
