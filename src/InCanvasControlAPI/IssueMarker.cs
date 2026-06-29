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

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.InCanvasControlAPI.CS
{
    /// <summary>
    ///     A simple object to keep the connection between marker control and the given element.
    /// </summary>
    public class IssueMarker
    {
        private IssueMarker(ElementId elementId, int controlIndex, InCanvasControlData inCanvasControlData)
        {
            TrackedElementId = elementId;
            ControlIndex = controlIndex;
            InCanvasControlData = inCanvasControlData;
        }

        /// <summary>
        ///     Data with which an In-Canvas control was created. We need to keep this to make small changes later on.
        /// </summary>
        public InCanvasControlData InCanvasControlData { get; set; }

        /// <summary>
        ///     Index of the control, returned by TemporaryGraphicsManager
        /// </summary>
        public int ControlIndex { get; }

        /// <summary>
        ///     Id of the element that the marker tracks
        /// </summary>
        public ElementId TrackedElementId { get; }

        /// <summary>
        ///     Creates an issue marker. It also creates an In-Canvas control on given element's position.
        /// </summary>
        /// <param name="document">Document in which the tracked element is.</param>
        /// <param name="elementId">Tracked element id.</param>
        /// <returns>IssueMarker created from data</returns>
        public static IssueMarker Create(Document document, ElementId elementId)
        {
            var resourceProvider = ResourceProvider.GetInstance();

            // Prepare InCanvasControlData. It needs position and image path. 
            // In this example, all controls will share the same image - though it is possible to create controls with different images, or even change it via an update (see IssueMarkerSelector::SelectMarker).
            var elementTracked = document.GetElement(elementId);

            var elementLocation = new XYZ();
            switch (elementTracked.Location)
            {
                case LocationPoint pointLoc:
                    elementLocation = pointLoc.Point;
                    break;
                case LocationCurve curveLoc:
                    elementLocation = curveLoc.Curve.GetEndPoint(0);
                    break;
            }

            var inCanvasControlData = new InCanvasControlData(resourceProvider.IssueImage, elementLocation);

            // Create In-Canvas control
            var manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
            var controlIndex = manager.AddControl(inCanvasControlData, ElementId.InvalidElementId);

            return new IssueMarker(elementId, controlIndex, inCanvasControlData);
        }
    }
}
