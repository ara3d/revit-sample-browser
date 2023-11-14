// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RevitMultiSample.EventsMonitor.CS
{
    /// <summary>
    ///     The UI to allow user to choose which event they want to subscribe.
    ///     This class is not the main one, is just a assistant in this sample.
    ///     If you just want to learn how to use Revit events,
    ///     please pay more attention to EventManager class.
    /// </summary>
    public partial class EventsSettingForm : Form
    {
        /// <summary>
        ///     A list to storage the selection user made
        /// </summary>
        private List<string> m_appSelection;

        /// <summary>
        ///     Constructor without any argument
        /// </summary>
        public EventsSettingForm()
        {
            InitializeComponent();
            m_appSelection = new List<string>();
        }

        /// <summary>
        ///     Property to get and set private member variables of SeletionMap
        /// </summary>
        public List<string> AppSelectionList
        {
            get { return m_appSelection ?? (m_appSelection = new List<string>()); }
            set => m_appSelection = value;
        }

        /// <summary>
        ///     Event handler for click OK button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishToggle_Click(object sender, EventArgs e)
        {
            // clear lists.
            m_appSelection.Clear();
            foreach (var item in AppEventsCheckedList.CheckedItems) m_appSelection.Add(item.ToString());
            DialogResult = DialogResult.OK;
            Hide();
        }

        /// <summary>
        ///     Event handler for close windows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Hide();
        }

        /// <summary>
        ///     Event handler for clicking check all button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkAllButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < AppEventsCheckedList.Items.Count; i++) AppEventsCheckedList.SetItemChecked(i, true);
        }

        /// <summary>
        ///     Events handler for clicking check none button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkNoneButton_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < AppEventsCheckedList.Items.Count; i++) AppEventsCheckedList.SetItemChecked(i, false);
        }

        /// <summary>
        ///     Events handler for clicking cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }
    }
}
