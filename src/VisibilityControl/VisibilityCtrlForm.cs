// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.VisibilityControl.CS
{
    public partial class VisibilityCtrlForm : Form
    {
        // an object control visibility by category
        private readonly VisibilityCtrl m_visibilityCtrl;

        public VisibilityCtrlForm(VisibilityCtrl visibilityCtrl)
        {
            m_visibilityCtrl = visibilityCtrl ?? throw new ArgumentNullException(nameof(visibilityCtrl));

            InitializeComponent();
        }

        private void VisibilityCtrlForm_Load(object sender, EventArgs e)
        {
            // initialize the  checked list box
            allCategoriesListView.Columns.Add("name");
            foreach (string name in m_visibilityCtrl.AllCategories.Keys)
            {
                var check = m_visibilityCtrl.AllCategories[name].ToString().Equals("True");
                ListViewItem item = new(name)
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
            var visible = e.NewValue == CheckState.Checked;
            var name = allCategoriesListView.Items[e.Index].Text;

            // change the visibility of the category
            if (!m_visibilityCtrl.SetVisibility(visible, name))
                TaskDialog.Show("Revit", "This category can not change visible in the active view.");
        }

        private void isolateButton_Click(object sender, EventArgs e)
        {
            // set the IsolateMode
            m_visibilityCtrl.IsolateMode = pickOneRadioButton.Checked
                ? IsolateMode.PickOne
                : windowSelectRadioButton.Checked ? IsolateMode.WindowSelect : IsolateMode.None;

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
