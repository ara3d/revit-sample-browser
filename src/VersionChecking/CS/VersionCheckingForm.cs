// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.VersionChecking.CS
{
    /// <summary>
    ///     UI that display the version information
    /// </summary>
    public partial class versionCheckingForm : Form
    {
        // a instance of Command class
        private readonly Command m_dataBuffer;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer">a instance of Command class</param>
        public versionCheckingForm(Command dataBuffer)
        {
            InitializeComponent();
            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     display the version information in a multiline text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VersionCheckingForm_Load(object sender, EventArgs e)
        {
            versionInformationTextBox.ReadOnly = true;

            var productName = "Product Name:    " + m_dataBuffer.ProductName + "\r\n";
            var productVersion = "Product Version: " + m_dataBuffer.ProductVersion + "\r\n";
            var buildNumber = "Build Number:    " + m_dataBuffer.BuildNumner + "\r\n";

            versionInformationTextBox.AppendText(productName);
            versionInformationTextBox.AppendText(productVersion);
            versionInformationTextBox.AppendText(buildNumber);

            SetDialogLocation();
        }

        /// <summary>
        ///     close UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     set dialog's display location.
        /// </summary>
        private void SetDialogLocation()
        {
            // set dialog's display location
            var left = (Screen.PrimaryScreen.WorkingArea.Right - Width) / 2;
            var top = (Screen.PrimaryScreen.WorkingArea.Bottom - Height) / 2;
            var windowLocation = new Point(left, top);
            Location = windowLocation;
        }
    }
}
