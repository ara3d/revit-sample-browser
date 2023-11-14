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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PrintLog.CS
{
    /// <summary>
    ///     This class implements the methods of interface IExternalApplication and register Print related events.
    ///     OnStartUp method will register ViewPrint and DocumentPrint events and unregister them in OnShutDown method.
    ///     The registered events will help implements the sample functionalities.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     Events reactor for ViewPrint and DocumentPrint related
        /// </summary>
        private EventsReactor m_eventsReactor;


        /// <summary>
        ///     Implement the OnStartup method to register events when Revit starts.
        /// </summary>
        /// <param name="application">Controlled application of to be loaded to Revit process.</param>
        /// <returns>Return the status of the external application.</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // Register related events
            m_eventsReactor = new EventsReactor();
            application.ControlledApplication.ViewPrinting += m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted += m_eventsReactor.AppViewPrinted;
            application.ControlledApplication.DocumentPrinting += m_eventsReactor.AppDocumentPrinting;
            application.ControlledApplication.DocumentPrinted += m_eventsReactor.AppDocumentPrinted;
            return Result.Succeeded;
        }

        /// <summary>
        ///     Implement this method to unregister the subscribed events when Revit exits.
        /// </summary>
        /// <param name="application">Controlled application to be shutdown.</param>
        /// <returns>Return the status of the external application.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            // just close log file and return success
            m_eventsReactor.CloseLogFiles();
            //
            // unregister events
            application.ControlledApplication.ViewPrinting -= m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted -= m_eventsReactor.AppViewPrinted;
            application.ControlledApplication.DocumentPrinting -= m_eventsReactor.AppDocumentPrinting;
            application.ControlledApplication.DocumentPrinted -= m_eventsReactor.AppDocumentPrinted;
            return Result.Succeeded;
        }
    }
}