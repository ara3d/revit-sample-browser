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

using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceUIServer.CS
{
    /// <summary>
    /// Registers the UI server at startup. IExternalResourceUIServer must be registered from
    /// IExternalApplication, not IExternalDBApplication.
    /// </summary>
    public class UiServerApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            var externalResourceUiService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.ExternalResourceUIService);
            if (externalResourceUiService == null)
                return Result.Failed;

            IExternalResourceUIServer sampleUiServer = new SampleExternalResourceUiServer();
            externalResourceUiService.AddServer(sampleUiServer);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
