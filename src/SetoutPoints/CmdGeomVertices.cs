// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SetoutPoints by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/SetoutPoints

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ara3D.RevitSampleBrowser.SetoutPoints.CS
{
    /// <summary>
    /// Places setout point family instances on every corner of structural concrete elements.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdGeomVertices : IExternalCommand
    {
        private enum HostType
        {
            Floors,
            Ramps,
            StructuralColumns,
            StructuralFraming,
            StructuralFoundations,
            Walls
        }

        public const string FamilyName = "SetoutPoint";
        public const string SymbolName = "SetoutPoint";

        public static Guid ParameterKey { get; } = new("f48cd131-b9b6-432a-8b5c-a7534183a880");
        public static Guid ParameterPointNr { get; } = new("febfe8b9-6938-4099-8cf6-d62f58a9c933");

        private static readonly Guid ParameterHostType = new("27188736-2491-4ac8-b634-8f4c9399afef");
        private static readonly Guid ParameterHostId = new("64221c53-558b-4f29-a469-039a2001a037");
        private static readonly Guid ParameterX = new("7a5d1056-a1df-4389-b026-9f32fc3ac5fb");
        private static readonly Guid ParameterY = new("84f9a2be-85d5-44da-94b9-fc5b7808026b");
        private static readonly Guid ParameterZ = new("04c33d6a-f7f1-450c-8b15-9ac9aba24606");

        private static int _pointNumber;

        private static string GetFamilyPath([CallerFilePath] string callerFilePath = null)
        {
            var sampleFolder = Path.GetDirectoryName(callerFilePath);
            var fromSource = Path.Combine(sampleFolder, "test", $"{FamilyName}.rfa");
            return File.Exists(fromSource)
                ? fromSource
                : Path.Combine(
                AssemblyPathHelper.GetAssemblyPath(),
                "SetoutPoints",
                "test",
                $"{FamilyName}.rfa");
        }

        private static HostType GetHostType(Element e)
        {
            if (e is WallFoundation)
                return HostType.StructuralFoundations;
            if (e is Floor)
                return HostType.Floors;
            if (e is Wall)
                return HostType.Walls;

            if (e.Category != null)
            {
                switch ((BuiltInCategory)e.Category.Id.Value)
                {
                    case BuiltInCategory.OST_StructuralColumns: return HostType.StructuralColumns;
                    case BuiltInCategory.OST_StructuralFraming: return HostType.StructuralFraming;
                    case BuiltInCategory.OST_StructuralFoundation: return HostType.StructuralFoundations;
                    case BuiltInCategory.OST_Floors: return HostType.Floors;
                    case BuiltInCategory.OST_Ramps: return HostType.Ramps;
                }
            }

            Debug.Assert(false, "Unexpected host type for setout point placement.");
            return HostType.Floors;
        }

        public static string ElementDescription(Element e)
        {
            if (e == null)
                return "<null>";

            var fi = e as FamilyInstance;
            var typeName = e.GetType().Name;
            var categoryName = e.Category == null ? string.Empty : e.Category.Name + " ";
            var familyName = fi == null ? string.Empty : fi.Symbol.Family.Name + " ";
            var symbolName = fi == null || e.Name.Equals(fi.Symbol.Name)
                ? string.Empty
                : fi.Symbol.Name + " ";

            return $"{typeName} {categoryName}{familyName}{symbolName}<{e.Id.Value} {e.Name}>";
        }

        private static FilteredElementCollector GetStructuralElements(Document doc)
        {
            var bics = new[]
            {
                BuiltInCategory.OST_StructuralColumns,
                BuiltInCategory.OST_StructuralFraming,
                BuiltInCategory.OST_StructuralFoundation,
                BuiltInCategory.OST_Floors,
                BuiltInCategory.OST_Ramps
            };

            List<ElementFilter> categoryFilters = new(bics.Length);
            foreach (var bic in bics)
                categoryFilters.Add(new ElementCategoryFilter(bic));

            LogicalOrFilter categoryFilter = new(categoryFilters);

            LogicalOrFilter structuralMaterialFilter = new(
            [
                new StructuralMaterialTypeFilter(StructuralMaterialType.Concrete),
                new StructuralMaterialTypeFilter(StructuralMaterialType.PrecastConcrete)
            ]);

            LogicalAndFilter familyInstanceFilter = new(
            [
                new ElementClassFilter(typeof(FamilyInstance)),
                structuralMaterialFilter,
                categoryFilter
            ]);

            LogicalOrFilter classFilter = new(
            [
                new ElementClassFilter(typeof(Wall)),
                new ElementClassFilter(typeof(Floor)),
                familyInstanceFilter
            ]);

            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(classFilter);
        }

        private static Transform GetProjectLocationTransform(Document doc)
        {
            var projectPosition = doc.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            var translationTransform = Transform.CreateTranslation(new XYZ(
                projectPosition.EastWest,
                projectPosition.NorthSouth,
                projectPosition.Elevation));
            var rotationTransform = Transform.CreateRotation(XYZ.BasisZ, projectPosition.Angle);
            return translationTransform.Multiply(rotationTransform);
        }

        public static FamilySymbol[] GetFamilySymbols(Document doc, bool loadIt)
        {
            FamilySymbol[] symbols = null;
            var familyPath = GetFamilyPath();

            var family = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .FirstOrDefault(e => e.Name.Equals(FamilyName)) as Family;

            if (family == null && loadIt)
            {
                using Transaction tx = new(doc);
                tx.Start("Load Setout Point Family");

                if (doc.LoadFamily(familyPath, out family))
                {
                    foreach (var id in family.GetFamilySymbolIds())
                        (doc.GetElement(id) as FamilySymbol)?.Activate();

                    tx.Commit();
                }
                else
                {
                    tx.RollBack();
                }
            }

            if (family != null)
            {
                symbols = new FamilySymbol[2];
                var i = 0;
                foreach (var id in family.GetFamilySymbolIds())
                    symbols[i++] = doc.GetElement(id) as FamilySymbol;

                Debug.Assert(
                    symbols[0].Name.EndsWith("Major"),
                    "expected major (key) setout point first");
                Debug.Assert(
                    symbols[1].Name.EndsWith("Minor"),
                    "expected minor setout point second");
            }

            return symbols;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var app = commandData.Application.Application;
            var doc = uidoc.Document;
            var projectLocationTransform = GetProjectLocationTransform(doc);
            var symbols = GetFamilySymbols(doc, true);

            if (symbols == null)
            {
                message = $"Unable to load setout point family from '{GetFamilyPath()}'.";
                return Result.Failed;
            }

            var col = GetStructuralElements(doc);
            var opt = app.Create.NewGeometryOptions();
            var first = true;

            using Transaction tx = new(doc);
            tx.Start("Place Setout Points");

            foreach (var e in col)
            {
                var solids = GeomVertices.GetSolids(e, opt, out var t);
                var desc = ElementDescription(e);

                if (solids == null || solids.Count == 0)
                {
                    Debug.Print("Unable to access element solid for element {0}.", desc);
                    continue;
                }

                var corners = GeomVertices.GetCorners(solids);
                Debug.Print("{0}: {1} corners found:", desc, corners.Count);

                foreach (var p in corners.Keys)
                {
                    ++_pointNumber;
                    Debug.Print("  {0}: {1}", _pointNumber, GeomVertices.PointString(p));

                    var insertionPoint = t.OfPoint(p);
                    var fi = doc.Create.NewFamilyInstance(
                        insertionPoint,
                        symbols[1],
                        StructuralType.NonStructural);

                    if (first)
                    {
                        if (fi.get_Parameter(ParameterX) == null)
                        {
                            message =
                                "The required shared parameters "
                                + "X, Y, Z, Host_Id, Host_Type and "
                                + "Point_Number are missing.";

                            tx.RollBack();
                            return Result.Failed;
                        }

                        first = false;
                    }

                    var surveyPoint = projectLocationTransform.OfPoint(p);

                    fi.get_Parameter(ParameterHostType).Set(GetHostType(e).ToString());
                    fi.get_Parameter(ParameterHostId).Set((int)e.Id.Value);
                    fi.get_Parameter(ParameterPointNr).Set(_pointNumber.ToString());
                    fi.get_Parameter(ParameterX).Set(surveyPoint.X);
                    fi.get_Parameter(ParameterY).Set(surveyPoint.Y);
                    fi.get_Parameter(ParameterZ).Set(surveyPoint.Z);
                }
            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}
