using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Diagnostics;
using System.Linq;

namespace BuildingCoder
{
    internal static partial class Util
    {
        private const string FaceWallConceptualMassTemplatePath
            = "C:/ProgramData/Autodesk/RVT 2015"
              + "/Family Templates/English/Conceptual Mass"
              + "/Metric Mass.rft";

        private const string FaceWallFamilyName = "TestFamily";

        private const string FaceWallFamilyPath = "C:/" + FaceWallFamilyName + ".rfa";

        internal static ModelCurve MakeLine(
            Document doc,
            XYZ p,
            XYZ q)
        {
            var line = Line.CreateBound(p, q);
            var norm = p.CrossProduct(q);
            if (norm.GetLength() == 0) norm = XYZ.BasisZ;

            var plane = Plane.CreateByNormalAndOrigin(norm, q);

            var skplane = SketchPlane.Create(doc, plane);

            return doc.FamilyCreate.NewModelCurve(line, skplane);
        }

        internal static void CreateMassExtrusion(Document doc)
        {
            using Transaction tx = new(doc);
            tx.Start("Create Mass");

            ReferenceArray refar = new();

            var pts = new[]
            {
                new XYZ(-10, -10, 0),
                new XYZ(+10, -10, 0),
                new XYZ(+10, +10, 0),
                new XYZ(-10, +10, 0)
            };

            int j, n = pts.Length;

            for (var i = 0; i < n; ++i)
            {
                j = i + 1;

                if (j >= n) j = 0;

                var c = MakeLine(doc, pts[i], pts[j]);

                refar.Append(c.GeometryCurve.Reference);
            }

            XYZ direction = new(0, 0, 20);

            doc.FamilyCreate.NewExtrusionForm(true, refar, direction);

            tx.Commit();
        }

        internal static void CreateFaceWalls(Document doc)
        {
            var app = doc.Application;

            var massDoc = app.NewFamilyDocument(
                FaceWallConceptualMassTemplatePath);

            CreateMassExtrusion(massDoc);

            SaveAsOptions opt = new()
            {
                OverwriteExistingFile = true
            };

            massDoc.SaveAs(FaceWallFamilyPath, opt);

            using Transaction tx = new(doc);
            tx.Start("Create FaceWall");

            if (!doc.LoadFamily(FaceWallFamilyPath))
                throw new Exception("DID NOT LOAD FAMILY");

            var family = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Where(x => x.Name.Equals(FaceWallFamilyName))
                .Cast<Family>()
                .FirstOrDefault();

            var fs = doc.GetElement(
                    family.GetFamilySymbolIds().First())
                as FamilySymbol;

            var level = doc.ActiveView.GenLevel;

            var fi = doc.Create.NewFamilyInstance(
                XYZ.Zero, fs, level, StructuralType.NonStructural);

            doc.Regenerate();

            var wallType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .Cast<WallType>()
                .Where(x => FaceWall.IsWallTypeValidForFaceWall(doc, x.Id))
                .FirstOrDefault();

            var options = app.Create.NewGeometryOptions();
            options.ComputeReferences = true;

            var geo = fi.get_Geometry(options);

            foreach (var obj in geo)
            {
                var solid = obj as Solid;

                if (null != solid)
                    foreach (Face f in solid.Faces)
                    {
                        Debug.Assert(null != f.Reference,
                            "we asked for references, didn't we?");

                        if (f is PlanarFace pf)
                        {
                            var v = pf.FaceNormal;

                            if (!IsVertical(v))
                                FaceWall.Create(
                                    doc, wallType.Id,
                                    WallLocationLine.CoreCenterline,
                                    f.Reference);
                        }
                    }
            }

            tx.Commit();
        }

        internal static void CreateFaceWallsAndMassFloors(UIDocument uidoc)
        {
            var doc = uidoc.Document;

            var fi = doc.GetElement(
                    uidoc.Selection.PickObject(ObjectType.Element))
                as FamilyInstance;

            var wType = new FilteredElementCollector(doc)
                .OfClass(typeof(WallType))
                .Cast<WallType>().FirstOrDefault(q
                    => q.Name == "Generic - 6\" Masonry");

            Options opt = new()
            {
                ComputeReferences = true
            };

            using Transaction t = new(doc);
            t.Start("Create Face Walls & Mass Floors");

            foreach (var solid in fi.get_Geometry(opt)
                .Where(q => q is Solid).Cast<Solid>())
                foreach (Face f in solid.Faces)
                    if (FaceWall.IsValidFaceReferenceForFaceWall(
                        doc, f.Reference))
                        FaceWall.Create(doc, wType.Id,
                            WallLocationLine.CoreExterior,
                            f.Reference);

            var levels
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Level));

            foreach (Level level in levels)
                MassInstanceUtils.AddMassLevelDataToMassInstance(
                    doc, fi.Id, level.Id);

            t.Commit();
        }
    }
}
