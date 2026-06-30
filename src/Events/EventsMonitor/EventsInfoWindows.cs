// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    public partial class EventsInfoWindows : Form
    {
        private readonly LogManager m_dataBuffer;

        public EventsInfoWindows()
        {
            InitializeComponent();
        }

        public EventsInfoWindows(LogManager dataBuffer)
            : this()
        {
            m_dataBuffer = dataBuffer;
            Initialize();
        }

        private void Initialize()
        {
            appEventsLogDataGridView.AutoGenerateColumns = false;
            appEventsLogDataGridView.DataSource = m_dataBuffer.EventsLog;
            timeColumn.DataPropertyName = "Time";
            eventColumn.DataPropertyName = "Event";
            typeColumn.DataPropertyName = "Type";
        }

        private void InformationWindows_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Cleared so ExternalApplication can recreate the window on next Show().
            ExternalApplication.InfoWindows = null;
        }

        private void applicationEventsInfoWindows_Shown(object sender, EventArgs e)
        {
            var left = Screen.PrimaryScreen.WorkingArea.Right - Width - 5;
            var top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
            var windowLocation = new Point(left, top);
            Location = windowLocation;
        }

        private void appEventsLogDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            appEventsLogDataGridView.CurrentCell =
                appEventsLogDataGridView.Rows[appEventsLogDataGridView.Rows.Count - 1].Cells[0];
        }
    }
}
