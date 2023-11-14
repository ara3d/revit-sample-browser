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

using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PerformanceAdviserControl.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication,
    /// </summary>
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     The custom API rule we are registering with PerformanceAdviser
        /// </summary>
        private FlippedDoorCheck m_FlippedDoorApiRule;

        /// <summary>
        ///     Basic construction
        /// </summary>
        public Application()
        {
        }

        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit starts before a file or default template is actually loaded.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully started.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If Failed is returned then Revit should inform the user that the external application
        ///     failed to load and the release the internal reference.
        ///     This method also adds a ribbon panel and button to launch an IExternalCommand
        ///     defined in UICommand.cs.  It also registers a new IPerformanceAdviserRule-implementing class
        ///     (m_FlippedDoorApiRule) with PerformanceAdviser.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("PerformanceAdviserControl");
            var currentAssembly = Assembly.GetAssembly(GetType()).Location;
            var pb = rp.AddItem(new PushButtonData("Performance Adviser", "Performance Adviser", currentAssembly,
                "Revit.SDK.Samples.PerformanceAdviserControl.CS.UICommand")) as PushButton;
            var uriImage = new Uri(Path.GetDirectoryName(currentAssembly) + "\\Button32.png");
            var largeImage = new BitmapImage(uriImage);
            pb.LargeImage = largeImage;

            m_FlippedDoorApiRule = new FlippedDoorCheck();
            PerformanceAdviser.GetPerformanceAdviser().AddRule(m_FlippedDoorApiRule.getRuleId(), m_FlippedDoorApiRule);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Implement this method to implement the external application which should be called when
        ///     Revit is about to exit, Any documents must have been closed before this method is called.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        /// <returns>
        ///     Return the status of the external application.
        ///     A result of Succeeded means that the external application successfully shutdown.
        ///     Cancelled can be used to signify that the user cancelled the external operation at
        ///     some point.
        ///     If Failed is returned then the Revit user should be warned of the failure of the external
        ///     application to shut down correctly.
        ///     This method also unregisters a the IPerformanceAdviserRule-implementing class
        ///     (m_FlippedDoorApiRule) with PerformanceAdviser.
        /// </returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            PerformanceAdviser.GetPerformanceAdviser().DeleteRule(m_FlippedDoorApiRule.getRuleId());
            m_FlippedDoorApiRule = null;

            return Result.Succeeded;
        }
    }
}