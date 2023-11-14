// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace Revit.SDK.Samples.InCanvasControlAPI.CS
{
    /// <summary>
    ///     This class demonstrates updating in-canvas controls in a DocumentUpdated event handler.
    /// </summary>
    public class IssueMarkerUpdater
    {
        /// <summary>
        ///     Perform updates on in-canvas controls.
        ///     In this example, the In-Canvas controls will be deleted, or have their positions changed, depending on the changes
        ///     to related elements.
        /// </summary>
        /// <param name="data">Data about changes in the document.</param>
        public static void Execute(DocumentChangedEventArgs data)
        {
            var doc = data.GetDocument();
            var temporaryGraphicsManager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(doc);
            var tracking = IssueMarkerTrackingManager.GetInstance().GetTracking(doc);

            foreach (var deleted in data.GetDeletedElementIds())
                if (tracking.GetMarkerByElementId(deleted) is IssueMarker marker)
                {
                    // This is how to delete control
                    temporaryGraphicsManager.RemoveControl(marker.ControlIndex);

                    // Don't forget to clean up your own data
                    tracking.RemoveMarkerByElement(deleted);
                }

            foreach (var updated in data.GetModifiedElementIds())
                if (tracking.GetMarkerByElementId(updated) is IssueMarker marker)
                {
                    var element = doc.GetElement(updated);

                    // Since we keep a copy of InCanvasControlData, we can avoid creating a new one. It already has image and position set - and we can just change the position
                    var controlData = marker.InCanvasControlData;
                    switch (element.Location)
                    {
                        case LocationPoint pointLoc:
                            controlData.Position = pointLoc.Point;
                            break;
                        case LocationCurve curveLoc:
                            controlData.Position = curveLoc.Curve.GetEndPoint(0);
                            break;
                    }

                    marker.InCanvasControlData = controlData;

                    // This is how to set updated data to a control
                    temporaryGraphicsManager.UpdateControl(marker.ControlIndex, marker.InCanvasControlData);
                }
        }
    }
}
