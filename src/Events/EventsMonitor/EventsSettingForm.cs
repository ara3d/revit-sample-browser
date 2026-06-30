// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    public partial class EventsSettingForm : Form
    {
        private List<string> m_appSelection;

        public EventsSettingForm()
        {
            InitializeComponent();
            m_appSelection = new List<string>();
        }

        public List<string> AppSelectionList
        {
            get { return m_appSelection ?? (m_appSelection = new List<string>()); }
            set => m_appSelection = value;
        }

        private void FinishToggle_Click(object sender, EventArgs e)
        {
            m_appSelection.Clear();
            foreach (var item in AppEventsCheckedList.CheckedItems)
            {
                m_appSelection.Add(item.ToString());
            }

            DialogResult = DialogResult.OK;
            Hide();
        }

        private void ToggleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Hide();
        }

        private void checkAllButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < AppEventsCheckedList.Items.Count; i++) AppEventsCheckedList.SetItemChecked(i, true);
        }

        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < AppEventsCheckedList.Items.Count; i++) AppEventsCheckedList.SetItemChecked(i, false);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }
    }
}
