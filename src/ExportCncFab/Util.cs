// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ExportCncFab by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/ExportCncFab

using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ExportCncFab.CS
{
    internal static class Util
    {
        public static bool BrowseDirectory(ref string path, bool allowCreate)
        {
            FolderBrowserDialog browseDlg = new()
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
