// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.VersionChecking.CS
{
    public partial class VersionCheckingForm : Form
    {
        // a instance of Command class
        private readonly Command m_dataBuffer;

        public VersionCheckingForm(Command dataBuffer)
        {
            InitializeComponent();
            m_dataBuffer = dataBuffer;
        }

        private void VersionCheckingForm_Load(object sender, EventArgs e)
        {
            versionInformationTextBox.ReadOnly = true;

            var productName = $"Product Name:    {m_dataBuffer.ProductName}\r\n";
            var productVersion = $"Product Version: {m_dataBuffer.ProductVersion}\r\n";
            var buildNumber = $"Build Number:    {m_dataBuffer.BuildNumner}\r\n";

            versionInformationTextBox.AppendText(productName);
            versionInformationTextBox.AppendText(productVersion);
            versionInformationTextBox.AppendText(buildNumber);

            DialogHelper.CenterOnScreen(this);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
