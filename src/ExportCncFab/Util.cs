// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ExportCncFab by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/ExportCncFab

using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ExportCncFab.CS
{
    internal static class Util
    {
        /// <summary>
        /// Prompt user to interactively select a directory.
        /// </summary>
        /// <param name="path">Input initial path and return selected value</param>
        /// <param name="allowCreate">Enable creation of new folder</param>
        /// <returns>True on successful selection</returns>
        public static bool BrowseDirectory(ref string path, bool allowCreate)
        {
            var browseDlg = new FolderBrowserDialog
            {
                SelectedPath = path,
                ShowNewFolderButton = allowCreate
            };

            var rc = DialogResult.OK == browseDlg.ShowDialog();

            if (rc)
                path = browseDlg.SelectedPath;

            return rc;
        }
    }
}
