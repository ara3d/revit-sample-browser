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

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;

namespace Ara3D.RevitSampleBrowser.ExternalResourceServer.ExternalResourceDBServer.CS
{
    /// <summary>
    /// Registers the DB server at startup. IExternalResourceServer has no UI, so use IExternalDBApplication
    /// rather than IExternalApplication.
    /// </summary>
    public class DbApplication : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            var externalResourceService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.ExternalResourceService);

            if (externalResourceService == null)
                return ExternalDBApplicationResult.Failed;

            IExternalResourceServer sampleServer = new SampleExternalResourceDbServer();
            externalResourceService.AddServer(sampleServer);
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }
    }
}
