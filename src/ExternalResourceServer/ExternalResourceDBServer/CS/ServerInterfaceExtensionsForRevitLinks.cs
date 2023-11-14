// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.IO;
using Autodesk.Revit.DB;

namespace RevitMultiSample.ExternalResourceDBServer.CS
{
    /// <summary>
    ///     Extension operator to support "Open (and Unload)" command.
    ///     Revit will call this method to determine the location of the locally-cached copy
    ///     of the linked model, and will open that copy directly.
    /// </summary>
    //=====================================================================================
    public class GetLinkPathForOpen : IGetLocalPathForOpenCallback
    {
        /// <summary>
        ///     This implementation simply retrieves the same local file name that the server
        ///     uses when first copying the link document to the user's machine.
        /// </summary>
        public string GetLocalPathForOpen(ExternalResourceReference desiredReference)
        {
            return SampleExternalResourceDbServer.GetFullLinkCachedFilePath(desiredReference);
        }
    }

    /// <summary>
    ///     Extension operator to support handle updates when the user saves new shared coordinates in the
    ///     local copy of a link.
    /// </summary>
    //=====================================================================================
    public class LocalLinkSharedCoordinatesSaved : IOnLocalLinkSharedCoordinatesSavedCallback
    {
        /// <summary>
        ///     When the user updates the shared coordinates in the link model, Revit calls this
        ///     method.  In this implementation, the updated local version of the link on the user's
        ///     machine is uploaded (copied) back to the server location.
        /// </summary>
        public void OnLocalLinkSharedCoordinatesSaved(ExternalResourceReference changedReference)
        {
            var localLinkPath = SampleExternalResourceDbServer.GetFullLinkCachedFilePath(changedReference);
            var fullServerPath = SampleExternalResourceDbServer.GetFullServerLinkFilePath(changedReference);
            var serverDirectoryName = Path.GetDirectoryName(fullServerPath);
            if (!Directory.Exists(serverDirectoryName)) Directory.CreateDirectory(serverDirectoryName);
            File.Copy(localLinkPath, fullServerPath, true); // Overwrite
        }
    }
}
