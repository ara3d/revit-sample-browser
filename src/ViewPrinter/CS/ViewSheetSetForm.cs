// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ViewPrinter.CS
{
    public partial class viewSheetSetForm : Form
    {
        private bool m_stopUpdateFlag;

        private readonly ViewSheets m_viewSheets;

        public viewSheetSetForm(ViewSheets viewSheets)
        {
            InitializeComponent();

            m_viewSheets = viewSheets;
        }

        private void ViewSheetSetForm_Load(object sender, EventArgs e)
        {
            viewSheetSetNameComboBox.DataSource = m_viewSheets.ViewSheetSetNames;
            viewSheetSetNameComboBox.SelectedValueChanged += viewSheetSetNameComboBox_SelectedValueChanged;
            viewSheetSetNameComboBox.SelectedItem = m_viewSheets.SettingName;

            showSheetsCheckBox.Checked = true;
            showViewsCheckBox.Checked = true;
            ListViewSheetSet();
            viewSheetSetListView.ItemChecked += viewSheetSetListView_ItemChecked;
        }

        private void ListViewSheetSet()
        {
            VisibleType vt;
            switch (showSheetsCheckBox.Checked)
            {
                case true when showViewsCheckBox.Checked:
                    vt = VisibleType.VT_BothViewAndSheet;
                    break;
                case true when !showViewsCheckBox.Checked:
                    vt = VisibleType.VT_SheetOnly;
                    break;
                case false when showViewsCheckBox.Checked:
                    vt = VisibleType.VT_ViewOnly;
                    break;
                default:
                    vt = VisibleType.VT_None;
                    break;
            }

            var views = m_viewSheets.AvailableViewSheetSet(vt);
            viewSheetSetListView.Items.Clear();
            foreach (var view in views)
            {
                var item = new ListViewItem(view.ViewType + ": " + view.Name);
                item.Checked = m_viewSheets.IsSelected(item.Text);
                viewSheetSetListView.Items.Add(item);
            }
        }

        private void viewSheetSetNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (m_stopUpdateFlag)
                return;

            m_viewSheets.SettingName = viewSheetSetNameComboBox.SelectedItem as string;
            ListViewSheetSet();

            saveButton.Enabled = revertButton.Enabled = false;

            reNameButton.Enabled = deleteButton.Enabled =
                m_viewSheets.SettingName.Equals("<In-Session>") ? false : true;
        }

        private void showSheetsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ListViewSheetSet();
        }

        private void showViewsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ListViewSheetSet();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var names = new List<string>();
            foreach (ListViewItem item in viewSheetSetListView.Items)
                if (item.Checked)
                    names.Add(item.Text);

            m_viewSheets.ChangeCurrentViewSheetSet(names);

            m_viewSheets.Save();
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveAsForm(m_viewSheets))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            viewSheetSetNameComboBox.DataSource = m_viewSheets.ViewSheetSetNames;
            viewSheetSetNameComboBox.Update();
            m_stopUpdateFlag = false;

            viewSheetSetNameComboBox.SelectedItem = m_viewSheets.SettingName;
        }

        private void revertButton_Click(object sender, EventArgs e)
        {
            m_viewSheets.Revert();
            ViewSheetSetForm_Load(null, null);
        }

        private void reNameButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new ReNameForm(m_viewSheets))
            {
                dlg.ShowDialog();
            }

            m_stopUpdateFlag = true;
            viewSheetSetNameComboBox.DataSource = m_viewSheets.ViewSheetSetNames;
            viewSheetSetNameComboBox.Update();
            m_stopUpdateFlag = false;

            viewSheetSetNameComboBox.SelectedItem = m_viewSheets.SettingName;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            m_viewSheets.Delete();

            m_stopUpdateFlag = true;
            viewSheetSetNameComboBox.DataSource = m_viewSheets.ViewSheetSetNames;
            viewSheetSetNameComboBox.Update();
            m_stopUpdateFlag = false;

            viewSheetSetNameComboBox.SelectedItem = m_viewSheets.SettingName;
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in viewSheetSetListView.Items) item.Checked = true;
        }

        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in viewSheetSetListView.Items) 
                item.Checked = false;
        }

        private void viewSheetSetListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!m_viewSheets.SettingName.Equals("<In-Session>")
                && !saveButton.Enabled)
                saveButton.Enabled = revertButton.Enabled
                    = reNameButton.Enabled = true;
        }
    }
}
