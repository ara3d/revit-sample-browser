// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.EventsMonitor.CS
{
    /// <summary>
    ///     The UI to show the events history logs. This class is not the main one just a assistant
    ///     in this sample. If you just want to learn how to use Revit events,
    ///     please pay more attention to EventManager class.
    /// </summary>
    public partial class EventsInfoWindows : Form
    {
        /// <summary>
        ///     An instance of RevitApplicationEvents class
        ///     Which prepares the informations which is shown in this UI
        /// </summary>
        private readonly LogManager m_dataBuffer;

        /// <summary>
        ///     Constructor without any argument
        /// </summary>
        public EventsInfoWindows()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor with one argument
        /// </summary>
        /// <param name="dataBuffer">prepare the informations which is shown in this UI</param>
        public EventsInfoWindows(LogManager dataBuffer)
            : this()
        {
            m_dataBuffer = dataBuffer;
            Initialize();
        }

        /// <summary>
        ///     Initialize the DataGridView property
        /// </summary>
        private void Initialize()
        {
            // set dataSource
            appEventsLogDataGridView.AutoGenerateColumns = false;
            appEventsLogDataGridView.DataSource = m_dataBuffer.EventsLog;
            timeColumn.DataPropertyName = "Time";
            eventColumn.DataPropertyName = "Event";
            typeColumn.DataPropertyName = "Type";
        }

        /// <summary>
        ///     form closed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InformationWindows_FormClosed(object sender, FormClosedEventArgs e)
        {
            // when form closed the relevant resource will be set free. 
            // Then the instance InfoWindows become invalid, so we set InfoWindows with null.
            ExternalApplication.InfoWindows = null;
        }

        /// <summary>
        ///     windows shown event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationEventsInfoWindows_Shown(object sender, EventArgs e)
        {
            // set window's display location
            var left = Screen.PrimaryScreen.WorkingArea.Right - Width - 5;
            var top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
            var windowLocation = new Point(left, top);
            Location = windowLocation;
        }

        /// <summary>
        ///     Scroll to last line when add new log lines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appEventsLogDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            appEventsLogDataGridView.CurrentCell =
                appEventsLogDataGridView.Rows[appEventsLogDataGridView.Rows.Count - 1].Cells[0];
        }
    }
}
