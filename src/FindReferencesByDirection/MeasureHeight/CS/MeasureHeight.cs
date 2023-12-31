// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FindReferencesByDirection.MeasureHeight.CS
{
    /// <summary>
    ///     Calculate the height above the ground floor of a selected skylight.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     Revit application
        /// </summary>
        private Application m_app;

        /// <summary>
        ///     Revit active document
        /// </summary>
        private Document m_doc;

        /// <summary>
        ///     Floor element
        /// </summary>
        private Floor m_floor;

        /// <summary>
        ///     skylight family instance
        /// </summary>
        private FamilyInstance m_skylight;

        /// <summary>
        ///     a 3d view
        /// </summary>
        private View3D m_view3D;

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            m_app = revit.Application.Application;
            m_doc = revit.Application.ActiveUIDocument.Document;

            var trans = new Transaction(m_doc, "Ara3D.RevitSampleBrowser.MeasureHeight");
            trans.Start();
            // Find a 3D view to use for the ray tracing operation
            var collector = new FilteredElementCollector(m_doc);
            Func<View3D, bool> isNotTemplate = v3 => !v3.IsTemplate;
            m_view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First(isNotTemplate);

            var selection = revit.Application.ActiveUIDocument.Selection;

            // If skylight is selected, process it.
            m_skylight = null;
            if (selection.GetElementIds().Count == 1)
                foreach (var eId in selection.GetElementIds())
                {
                    var e = revit.Application.ActiveUIDocument.Document.GetElement(eId);
                    if (e is FamilyInstance instance)
                    {
                        var isWindow = instance.Category.BuiltInCategory == BuiltInCategory.OST_Windows;
                        var isHostedByRoof = instance.Host.Category.BuiltInCategory == BuiltInCategory.OST_Roofs;

                        if (isWindow && isHostedByRoof)
                            m_skylight = instance;
                    }
                }

            if (m_skylight == null)
            {
                message = "This tool requires exactly one skylight to be selected.";
                trans.RollBack();
                return Result.Cancelled;
            }

            // Find the floor to use for the measurement (hardcoded)
            var id = new ElementId(150314L);
            m_floor = m_doc.GetElement(id) as Floor;

            // Calculate the height
            var line = CalculateLineAboveFloor();

            // Create a model curve to show the distance
            var plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), line.GetEndPoint(0));
            var sketchPlane = SketchPlane.Create(m_doc, plane);

            m_doc.Create.NewModelCurve(line, sketchPlane);

            // Show a message with the length value
            TaskDialog.Show("Distance", $"Distance to floor: {line.Length:f2}");

            trans.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        ///     Determines the line segment that connects the skylight to the already obtained floor.
        /// </summary>
        /// <returns>The line segment.</returns>
        private Line CalculateLineAboveFloor()
        {
            // Use the center of the skylight bounding box as the start point.
            var box = m_skylight.get_BoundingBox(m_view3D);
            var center = box.Min.Add(box.Max).Multiply(0.5);

            // Project in the negative Z direction down to the floor.
            var rayDirection = new XYZ(0, 0, -1);

            // Look for references to faces where the element is the floor element id.
            var referenceIntersector = new ReferenceIntersector(m_floor.Id, FindReferenceTarget.Face, m_view3D);
            var references = referenceIntersector.Find(center, rayDirection);

            var distance = double.PositiveInfinity;
            XYZ intersection = null;
            foreach (var referenceWithContext in references)
            {
                var reference = referenceWithContext.GetReference();
                // Keep the closest matching reference (using the proximity parameter to determine closeness).
                var proximity = referenceWithContext.Proximity;
                if (proximity < distance)
                {
                    distance = proximity;
                    intersection = reference.GlobalPoint;
                }
            }

            // Create line segment from the start point and intersection point.
            var result = Line.CreateBound(center, intersection);
            return result;
        }
    }
}
