// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DocumentChanged.CS
{
    public partial class ChangesInformationForm : Form
    {
        public ChangesInformationForm()
        {
            InitializeComponent();
        }

        public ChangesInformationForm(DataTable dataBuffer)
            : this()
        {
            changesdataGridView.DataSource = dataBuffer;
            changesdataGridView.AutoGenerateColumns = false;
        }

        private void ChangesInfoForm_Shown(object sender, EventArgs e)
        {
            // set window's display location
            var left = Screen.PrimaryScreen.WorkingArea.Right - Width - 5;
            var top = Screen.PrimaryScreen.WorkingArea.Bottom - Height;
            var windowLocation = new Point(left, top);
            Location = windowLocation;
        }

        private void changesdataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            changesdataGridView.CurrentCell = changesdataGridView.Rows[changesdataGridView.Rows.Count - 1].Cells[0];
        }
    }
}
