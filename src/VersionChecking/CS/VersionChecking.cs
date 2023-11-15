// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.VersionChecking.CS
{
    /// <summary>
    ///     Get the product name, version and build number about Revit main program.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // properties
        /// <summary>
        ///     get product name of Revit main program
        /// </summary>
        public string ProductName { get; private set; } = "";

        /// <summary>
        ///     get version number of current Revit main program
        /// </summary>
        public string ProductVersion { get; private set; } = "";

        /// <summary>
        ///     get build number of current Revit main program
        /// </summary>
        public string BuildNumner { get; private set; } = "";

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            // get currently executable application
            var revitApplication = revit.Application.Application;

            // get product name, version number and build number information
            // via corresponding Properties of Autodesk.Revit.ApplicationServices.Application class
            ProductName = revitApplication.VersionName;
            ProductVersion = revitApplication.VersionNumber;
            BuildNumner = revitApplication.VersionBuild;

            //Show forms dialog which is a UI
            using (var displayForm = new VersionCheckingForm(this))
            {
                displayForm.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
