// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.FindReferencesByDirection.MeasureHeight.CS
{
    /// <summary>
    /// Measures vertical distance from a selected roof-hosted window (skylight) down to a floor.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private Application m_app;
        private Document m_doc;
        private Floor m_floor;
        private FamilyInstance m_skylight;
        private View3D m_view3D;

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            m_app = revit.Application.Application;
            m_doc = revit.Application.ActiveUIDocument.Document;

            Transaction trans = new(m_doc, "Ara3D.RevitSampleBrowser.MeasureHeight");
            trans.Start();
            FilteredElementCollector collector = new(m_doc);
            Func<View3D, bool> isNotTemplate = v3 => !v3.IsTemplate;
            m_view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First(isNotTemplate);

            var selection = revit.Application.ActiveUIDocument.Selection;

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

            // Sample-specific floor element id; replace for other models.
            ElementId id = new(150314L);
            m_floor = m_doc.GetElement(id) as Floor;

            var line = CalculateLineAboveFloor();

            var plane = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), line.GetEndPoint(0));
            var sketchPlane = SketchPlane.Create(m_doc, plane);

            m_doc.Create.NewModelCurve(line, sketchPlane);

            TaskDialog.Show("Distance", $"Distance to floor: {line.Length:f2}");

            trans.Commit();
            return Result.Succeeded;
        }

        private Line CalculateLineAboveFloor()
        {
            var box = m_skylight.get_BoundingBox(m_view3D);
            var center = box.Min.Add(box.Max).Multiply(0.5);

            XYZ rayDirection = new(0, 0, -1);

            ReferenceIntersector referenceIntersector = new(m_floor.Id, FindReferenceTarget.Face, m_view3D);
            var references = referenceIntersector.Find(center, rayDirection);

            var distance = double.PositiveInfinity;
            XYZ intersection = null;
            foreach (var referenceWithContext in references)
            {
                var reference = referenceWithContext.GetReference();
                var proximity = referenceWithContext.Proximity;
                if (proximity < distance)
                {
                    distance = proximity;
                    intersection = reference.GlobalPoint;
                }
            }

            var result = Line.CreateBound(center, intersection);
            return result;
        }
    }
}
