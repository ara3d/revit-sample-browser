// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Revit.SDK.Samples.SelectionChanged.CS
{
    /// <summary>
    ///     This class is an external application which monitors when the selection is changed.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SelectionChanged : IExternalApplication
    {
        /// <summary>
        ///     A controlled application used to register the events. Because all trigger points
        ///     in this sample come from UI, all events in application level must be registered
        ///     to ControlledApplication. If the trigger point is from API, user can register it
        ///     to Application or Document according to what level it is in. But then,
        ///     the syntax is the same in these three cases.
        /// </summary>
        private static UIControlledApplication _ctrlApp;

        /// <summary>
        ///     The window is used to show SelectionChanged event information.
        /// </summary>
        private static InfoWindow _infoWindow;

        private static readonly string AddInPath = typeof(SelectionChanged).Assembly.Location;

        /// <summary>
        ///     Property to get and set private member variable of InfoWindows
        /// </summary>
        public static InfoWindow InfoWindow { get; set; }

        /// <summary>
        ///     Implement OnStartup method of IExternalApplication interface.
        ///     This method subscribes to SelectionChanged event.
        /// </summary>
        /// <param name="application">Controlled application to be loaded to Revit process.</param>
        /// <returns>The status of the external application</returns>
        public Result OnStartup(UIControlledApplication application)
        {
            _ctrlApp = application;

            var ribbonPanel = _ctrlApp.CreateRibbonPanel("SelectionChanged Event");
            var showInfoWindowButton = new PushButtonData("showInfoWindow", "Show Event Info", AddInPath,
                "Revit.SDK.Samples.SelectionChanged.CS.Command")
            {
                ToolTip = "Show Event Monitor window"
            };
            ribbonPanel.AddItem(showInfoWindowButton);

            // subscribe to SelectionChanged event
            application.SelectionChanged += SelectionChangedHandler;

            return Result.Succeeded;
        }

        /// <summary>
        ///     Implement OnShutdown method of IExternalApplication interface.
        ///     This method unsubscribes from SelectionChanged event
        /// </summary>
        /// <param name="application">Controlled application to be shutdown.</param>
        /// <returns>The status of the external application.</returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            // unsubscribe to SelectionChanged event
            application.SelectionChanged -= SelectionChangedHandler;

            // finalize the log file.
            LogManager.LogFinalize();

            if (_infoWindow != null)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Event handler method for SelectionChanged event.
        ///     This method will check that the selection reported by event is the same with the actual Revit selection
        ///     and print the current selection in the log file.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event arguments that contains the event data.</param>
        private void SelectionChangedHandler(object sender, SelectionChangedEventArgs args)
        {
            // The document associated with the event. 
            var doc = args.GetDocument();

            //Check if Revit selection matches the selection reported by the event
            var uidoc = new UIDocument(doc);
            var currentSelection = uidoc.Selection.GetReferences();
            var reportedSelection = args.GetReferences();
            if (!currentSelection.All(i => null != reportedSelection.FirstOrDefault(r => r.EqualTo(i))))
                LogManager.WriteLog(
                    "Error: Current selection is different from the selection reported by the selectionchanged event");

            var currentSelectedElementIds = uidoc.Selection.GetElementIds();
            var reportedSelectedElementIds = args.GetSelectedElements();
            if (!currentSelectedElementIds.All(i => null != reportedSelectedElementIds.FirstOrDefault(r => r == i)))
                LogManager.WriteLog(
                    "Error: Current selected ElementIds is different from the one reported by the selectionchanged event");

            // write to log file. 
            LogManager.WriteLog(args.GetInfo());

            InfoWindow?.RevitUIApp_SelectionChanged(sender, args);
        }
    }
}
