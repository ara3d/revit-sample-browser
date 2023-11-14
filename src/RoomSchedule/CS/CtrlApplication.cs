// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RoomSchedule
{
    /// <summary>
    ///     This class implements the IExternalApplication interface,
    ///     OnStartup will subscribe Save/SaveAs and DocumentClose events when Revit starts and OnShutdown will unregister
    ///     these events when Revit exists.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CrtlApplication : IExternalApplication
    {
        /// <summary>
        ///     The events reactor for this application.
        /// </summary>
        private static EventsReactor _eventReactor;

        /// <summary>
        ///     Access the event reactor instance
        /// </summary>
        public static EventsReactor EventReactor
        {
            get
            {
                if (null == _eventReactor)
                    throw new ArgumentException(
                        "External application was not loaded yet, please make sure you register external application by correct full path of dll.",
                        "EventReactor");
                return _eventReactor;
            }
        }

        /// <summary>
        ///     Implement OnStartup method to subscribe related events.
        /// </summary>
        /// <param name="application">Current loaded application.</param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            // specify the log
            var assemblyName = GetType().Assembly.Location;
            _eventReactor = new EventsReactor(assemblyName.Replace(".dll", ".log"));
            //
            // subscribe events
            application.ControlledApplication.DocumentSaving += EventReactor.DocumentSaving;
            application.ControlledApplication.DocumentSavingAs += EventReactor.DocumentSavingAs;
            application.ControlledApplication.DocumentClosed += EventReactor.DocumentClosed;
            return Result.Succeeded;
        }

        /// <summary>
        ///     Unregister subscribed events when Revit exists
        /// </summary>
        /// <param name="application">Current loaded application.</param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            _eventReactor.Dispose();
            application.ControlledApplication.DocumentSaving -= EventReactor.DocumentSaving;
            application.ControlledApplication.DocumentSavingAs -= EventReactor.DocumentSavingAs;
            application.ControlledApplication.DocumentClosed -= EventReactor.DocumentClosed;
            return Result.Succeeded;
        }
    }
}
