// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.InCanvasControlAPI.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var choices = commandData.Application.ActiveUIDocument.Selection;
                var tracking = IssueMarkerTrackingManager.GetInstance().GetTracking(document);

                // Pick one object from Revit.
                var hasPickOne = choices.PickObject(ObjectType.Element, "Select an element to create a control on.");

                if (hasPickOne != null && tracking.GetMarkerByElementId(hasPickOne.ElementId) == null)
                {
                    var marker = IssueMarker.Create(document, hasPickOne.ElementId);

                    // Register the marker in tracking.
                    tracking.SubscribeMarker(marker);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
