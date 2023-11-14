// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ChangesMonitor.CS
{
    /// <summary>
    ///     The UI to show the change history logs. This class is not the main one just a assistant
    ///     in this sample. If you just want to learn how to use DocumentChanges event,
    ///     please pay more attention to ExternalApplication class.
    /// </summary>
    public partial class ChangesInformationForm : Form
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public ChangesInformationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor with one argument
        /// </summary>
        /// <param name="dataBuffer">prepare the informations which is shown in this UI</param>
        public ChangesInformationForm(DataTable dataBuffer)
            : this()
        {
            changesdataGridView.DataSource = dataBuffer;
            changesdataGridView.AutoGenerateColumns = false;
        }


        /// <summary>
        ///     windows shown event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangesInfoForm_Shown(object sender, EventArgs e)
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
        private void changesdataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            changesdataGridView.CurrentCell = changesdataGridView.Rows[changesdataGridView.Rows.Count - 1].Cells[0];
        }
    }
}
