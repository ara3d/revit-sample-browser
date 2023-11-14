//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.VersionChecking.CS
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
            using (var displayForm = new versionCheckingForm(this))
            {
                displayForm.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}