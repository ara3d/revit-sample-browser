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

        public int ControlIndex { get; }

        public ElementId TrackedElementId { get; }

        public static IssueMarker Create(Document document, ElementId elementId)
        {
            var resourceProvider = ResourceProvider.GetInstance();

            // InCanvasControlData needs position and image path; all markers share one image in this sample.
            var elementTracked = document.GetElement(elementId);

            XYZ elementLocation = new();
            switch (elementTracked.Location)
            {
                case LocationPoint pointLoc:
                    elementLocation = pointLoc.Point;
                    break;
                case LocationCurve curveLoc:
                    elementLocation = curveLoc.Curve.GetEndPoint(0);
                    break;
            }

            InCanvasControlData inCanvasControlData = new(resourceProvider.IssueImage, elementLocation);

            var manager = TemporaryGraphicsManager.GetTemporaryGraphicsManager(document);
            var controlIndex = manager.AddControl(inCanvasControlData, ElementId.InvalidElementId);

            return new IssueMarker(elementId, controlIndex, inCanvasControlData);
        }
    }
}
