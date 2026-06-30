// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceDBServer.CS
{
    /// <summary>
    /// Returns the locally cached link path for Revit's "Open (and Unload)" command.
    /// </summary>
    public class GetLinkPathForOpen : IGetLocalPathForOpenCallback
    {
        public string GetLocalPathForOpen(ExternalResourceReference desiredReference)
        {
            return SampleExternalResourceDbServer.GetFullLinkCachedFilePath(desiredReference);
        }
    }

    /// <summary>
    /// Uploads shared-coordinate changes from the local link copy back to the server.
    /// </summary>
    public class LocalLinkSharedCoordinatesSaved : IOnLocalLinkSharedCoordinatesSavedCallback
    {
        public void OnLocalLinkSharedCoordinatesSaved(ExternalResourceReference changedReference)
        {
            var localLinkPath = SampleExternalResourceDbServer.GetFullLinkCachedFilePath(changedReference);
            var fullServerPath = SampleExternalResourceDbServer.GetFullServerLinkFilePath(changedReference);
            var serverDirectoryName = Path.GetDirectoryName(fullServerPath);
            if (!Directory.Exists(serverDirectoryName)) Directory.CreateDirectory(serverDirectoryName);
            File.Copy(localLinkPath, fullServerPath, true);
        }
    }
}
