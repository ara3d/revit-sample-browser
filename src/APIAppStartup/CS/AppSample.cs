// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace APIAppStartup
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class AppSample : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            TaskDialog.Show("Revit", "Quit External Application!");
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var version = application.ControlledApplication.VersionName;

            //display splash window for 10 seconds
            SplashWindow.StartSplash();
            SplashWindow.ShowVersion(version);
            Thread.Sleep(10000);
            SplashWindow.StopSplash();

            return Result.Succeeded;
        }
    }
}
