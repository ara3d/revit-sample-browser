// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AutoStamp.CS
{
    /// <summary>
    ///     This class implements the methods of interface IExternalApplication and register View Print related events.
    ///     OnStartUp method will register ViewPrinting and ViewPrinted events and unregister them in OnShutDown method.
    ///     The registered events will help implement the sample functionalities.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     Events reactor for ViewPrint related events
        /// </summary>
        private EventsReactor m_eventsReactor;

        /// <summary>
        ///     Implements the OnStartup method to register events when Revit starts.
        /// </summary>
        /// <param name="application">Controlled application of to be loaded to Revit process.</param>
        /// <returns>Return the status of the external application.</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // Register related events
            m_eventsReactor = new EventsReactor();
            application.ControlledApplication.ViewPrinting += m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted += m_eventsReactor.AppViewPrinted;
            return Result.Succeeded;
        }

        /// <summary>
        ///     Implements this method to unregister the subscribed events when Revit exits.
        /// </summary>
        /// <param name="application">Controlled application to be shutdown.</param>
        /// <returns>Return the status of the external application.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            // just close log file
            m_eventsReactor.CloseLogFiles();
            //
            // unregister events
            application.ControlledApplication.ViewPrinting -= m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted -= m_eventsReactor.AppViewPrinted;
            return Result.Succeeded;
        }
    }
}
