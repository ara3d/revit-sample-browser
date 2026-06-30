using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_EditFloor sample.</summary>
    internal static partial class Util
    {
        internal static Floor CreateFloorAtElevation(
            Document document,
            double elevation)
        {
            var floorTypeId = Floor.GetDefaultFloorType(
                document, false);

            double offset;
            var levelId = Level.GetNearestLevelId(
                document, elevation, out offset);

            var first = new XYZ(0, 0, 0);
            var second = new XYZ(20, 0, 0);
            var third = new XYZ(20, 15, 0);
            var fourth = new XYZ(0, 15, 0);
            var profile = new CurveLoop();
            profile.Append(Line.CreateBound(first, second));
            profile.Append(Line.CreateBound(second, third));
            profile.Append(Line.CreateBound(third, fourth));
            profile.Append(Line.CreateBound(fourth, first));

            var floor = Floor.Create(document, new List<CurveLoop>
            {
                profile
            }, floorTypeId, levelId);

            var param = floor.get_Parameter(
                BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);

            param.Set(offset);

            return floor;
        }

        /// <summary>
        ///     Return the uppermost horizontal face
        ///     of a given solid object such as a floor slab.
        /// </summary>
        internal static PlanarFace GetTopFace(Solid solid)
        {
            PlanarFace topFace = null;
            var faces = solid.Faces;
            foreach (Face f in faces)
            {
                var pf = f as PlanarFace;
                if (null != pf
                    && IsHorizontal(pf))
                    if (null == topFace
                        || topFace.Origin.Z < pf.Origin.Z)
                        topFace = pf;
            }

            return topFace;
        }

        internal static void SetFloorLevelAndOffset(Document doc)
        {
            var floor
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor))
                    .FirstElement() as Floor;

            var levelIdInt = floor.LevelId.Value;

            var level
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Levels)
                    .OfClass(typeof(Level))
                    .FirstOrDefault(e
                        => e.Id.Value.Equals(
                            levelIdInt));

            if (null != level)
            {
                var p = floor.get_Parameter(
                    BuiltInParameter.LEVEL_PARAM);

                var p1 = floor.get_Parameter(
                    BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);

                using var tx = new Transaction(doc);
                tx.Start("Set Floor Level");
                p.Set(level.Id);
                p1.Set(2);
                tx.Commit();
            }
        }
    }

    internal class SketchEditScopeSample
    {
        public void CreateFloor(Document doc)
        {
            Curve left = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 100, 0));
            Curve upper = Line.CreateBound(new XYZ(0, 100, 0), new XYZ(100, 100, 0));
            Curve right = Line.CreateBound(new XYZ(100, 100, 0), new XYZ(100, 0, 0));
            Curve lower = Line.CreateBound(new XYZ(100, 0, 0), new XYZ(0, 0, 0));

            var floorProfile = new CurveLoop();
            floorProfile.Append(left);
            floorProfile.Append(upper);
            floorProfile.Append(right);
            floorProfile.Append(lower);

            var levelId = Level.GetNearestLevelId(doc, 0.0);

            using var transaction = new Transaction(doc);
            transaction.Start("Create floor");
            var floorTypeId = Floor.GetDefaultFloorType(doc, false);
            Floor.Create(doc,
                new List<CurveLoop> {floorProfile},
                floorTypeId, levelId);
            transaction.Commit();
        }

        public void ReplaceBoundaryLine(Document doc)
        {
            var floorCollector
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor));

            if (floorCollector.FirstOrDefault() is not Floor floor)
            {
                TaskDialog.Show("Error", "doc does not contain a floor.");
                return;
            }

            var sketch = doc.GetElement(floor.SketchId) as Sketch;
            Line line = null;
            foreach (CurveArray curveArray in sketch.Profile)
            {
                foreach (Curve curve in curveArray)
                {
                    line = curve as Line;
                    if (line != null) break;
                }

                if (line != null) break;
            }

            if (line == null)
            {
                TaskDialog.Show("Error",
                    "Sketch does not contain a straight line.");
                return;
            }

            var sketchEditScope = new SketchEditScope(doc,
                "Replace line with an arc");

            sketchEditScope.Start(sketch.Id);

            using (var transaction = new Transaction(doc,
                "Modify sketch"))
            {
                transaction.Start();

                var normal = line.Direction.CrossProduct(XYZ.BasisZ).Normalize().Negate();
                var middle = line.GetEndPoint(0).Add(line.Direction.Multiply(line.Length / 2));
                Curve arc = Arc.Create(line.GetEndPoint(0), line.GetEndPoint(1),
                    middle.Add(normal.Multiply(20)));

                doc.Delete(line.Reference.ElementId);

                doc.Create.NewModelCurve(arc, sketch.SketchPlane);

                transaction.Commit();
            }

            sketchEditScope.Commit(new FailuresPreprocessor());
        }

        public void MakeHole(Document doc)
        {
            var floorCollector
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Floors)
                    .OfClass(typeof(Floor));

            if (floorCollector.FirstOrDefault() is not Floor floor)
            {
                TaskDialog.Show("Error", "Document does not contain a floor.");
                return;
            }

            var sketch = doc.GetElement(floor.SketchId) as Sketch;
            var sketchEditScope = new SketchEditScope(doc,
                "Add profile to the sketch");
            sketchEditScope.Start(sketch.Id);

            using (var transaction = new Transaction(doc,
                "Make a hole"))
            {
                transaction.Start();
                var circle = Ellipse.CreateCurve(new XYZ(50, 50, 0),
                    10, 10, XYZ.BasisX, XYZ.BasisY, 0, 2 * Math.PI);

                doc.Create.NewModelCurve(circle, sketch.SketchPlane);
                transaction.Commit();
            }

            sketchEditScope.Commit(new FailuresPreprocessor());
        }
    }

    internal class FailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(
            FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }
}
