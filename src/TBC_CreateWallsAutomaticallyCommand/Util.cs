using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_CreateWallsAutomaticallyCommand sample.</summary>
    internal static partial class Util
    {
        internal static Wall CreateWallForCube(
            FamilyInstance cube,
            Curve curve,
            double height)
        {
            var doc = cube.Document;

            var wallTypeId = doc.GetDefaultElementTypeId(
                ElementTypeGroup.WallType);

            return Wall.Create(doc, curve.CreateReversed(),
                wallTypeId, cube.LevelId, height, 0, false,
                false);
        }

        internal static void CreateDoorForWall(Wall wall)
        {
            var locationCurve = (LocationCurve) wall.Location;

            var position = locationCurve.Curve.Evaluate(
                0.5, true);

            var document = wall.Document;

            var level = (Level) document.GetElement(
                wall.LevelId);

            var symbolId = document.GetDefaultFamilyTypeId(
                new ElementId(BuiltInCategory.OST_Doors));

            var symbol = (FamilySymbol) document.GetElement(
                symbolId);

            if (!symbol.IsActive)
                symbol.Activate();

            document.Create.NewFamilyInstance(position, symbol,
                wall, level, StructuralType.NonStructural);
        }

        internal static IEnumerable<FamilyInstance> FindCubes(
            Document doc)
        {
            var collector = new FilteredElementCollector(doc);

            return collector
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .OfClass(typeof(FamilyInstance))
                .OfType<FamilyInstance>()
                .Where(x => x.Symbol.FamilyName == "cube");
        }

        internal static IEnumerable<CurveLoop> FindCountors(
            FamilyInstance familyInstance)
        {
            return GetSolidsFromElement(familyInstance)
                .SelectMany(x => GetCountoursFromSolid(x,
                    familyInstance));
        }

        internal static IEnumerable<CurveLoop> GetCountoursFromSolid(
            Solid solid,
            Element element)
        {
            try
            {
                var plane = Plane.CreateByNormalAndOrigin(
                    XYZ.BasisZ, element.get_BoundingBox(null).Min);

                var analyzer = ExtrusionAnalyzer.Create(
                    solid, plane, XYZ.BasisZ);

                var face = analyzer.GetExtrusionBase();

                return face.GetEdgesAsCurveLoops();
            }
            catch (InvalidOperationException)
            {
                return Enumerable.Empty<CurveLoop>();
            }
        }
    }
}
