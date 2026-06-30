// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Ara3D.RevitSampleBrowser.Events.SelectionChanged.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SelectionChanged : IExternalApplication
    {
        // UI trigger points require UIControlledApplication; API triggers use Application or Document with the same += / -= syntax.
        private static UIControlledApplication _ctrlApp;

        private static InfoWindow _infoWindow;

        private static readonly string AddInPath = typeof(SelectionChanged).Assembly.Location;

        public static InfoWindow InfoWindow { get; set; }

        public Result OnStartup(UIControlledApplication application)
        {
            _ctrlApp = application;

            var ribbonPanel = _ctrlApp.CreateRibbonPanel("SelectionChanged Event");
            var showInfoWindowButton = new PushButtonData("showInfoWindow", "Show Event Info", AddInPath,
                "Ara3D.RevitSampleBrowser.SelectionChanged.CS.Command")
            {
                ToolTip = "Show Event Monitor window"
            };
            ribbonPanel.AddItem(showInfoWindowButton);

            application.SelectionChanged += SelectionChangedHandler;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.SelectionChanged -= SelectionChangedHandler;

            LogManager.LogFinalize();

            if (_infoWindow != null)
            {
                _infoWindow.Close();
                _infoWindow = null;
            }

            return Result.Succeeded;
        }

        private void SelectionChangedHandler(object sender, SelectionChangedEventArgs args)
        {
            var doc = args.GetDocument();

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

            LogManager.WriteLog(args.GetInfo());

            InfoWindow?.RevitUIApp_SelectionChanged(sender, args);
        }
    }
}
