// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class ViewTemplateHelper
    {
        public const string SampleName = "RevitView Template Creation sample";

        public static void ShowWarningMessageBox(string message)
        {
            MessageBox.Show(message, SampleName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInformationMessageBox(string message)
        {
            MessageBox.Show(message, SampleName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}