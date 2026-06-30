// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.IFC;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal class OpeningHandler
    {
        public double GetOpeningArea(
            Wall elemHost,
            Element elemInsert,
            Room room,
            Solid roomSolid)
        {
            var doc = room.Document;
            var openingArea = 0.0;

            if (elemInsert is FamilyInstance fi)
            {
                if (IsInRoom(room, fi))
                {
                    if (elemHost is Wall wall)
                    {
                        openingArea = GetWallCutArea(fi, wall);
                    }
                }
            }

            if (elemInsert is Wall)
            {
                var solidHandler = new SolidHandler();
                openingArea = solidHandler.GetWallAsOpeningArea(
                    elemInsert, roomSolid);
            }

            return openingArea;
        }

        public double GetWallCutArea(
            FamilyInstance fi,
            Wall wall)
        {
            var doc = fi.Document;

            var curveLoop = ExporterIFCUtils.GetInstanceCutoutFromWall(
                fi.Document, wall, fi, out var cutDir);

            IList<CurveLoop> loops = new List<CurveLoop>(1);
            loops.Add(curveLoop);

            if (!wall.IsStackedWallMember)
            {
                return ExporterIFCUtils.ComputeAreaOfCurveLoops(loops);
            }

            var solHandler = new SolidHandler();
            var optCompRef = doc.Application.Create.NewGeometryOptions();

            if (null != optCompRef)
            {
                optCompRef.ComputeReferences = true;
                optCompRef.DetailLevel = ViewDetailLevel.Medium;
            }

            var geomElemHost = wall.get_Geometry(optCompRef) as GeometryElement;

            var solidOpening = GeometryCreationUtilities
                .CreateExtrusionGeometry(loops,
                    cutDir.Negate(), .1);

            var solidHost = solHandler.CreateSolidFromBoundingBox(
                null, geomElemHost.GetBoundingBox(), null);

            if (solidHost == null)
            {
                return 0;
            }

            var intersectSolid = BooleanOperationsUtils
                .ExecuteBooleanOperation(solidOpening,
                    solidHost, BooleanOperationsType.Intersect);

            if (intersectSolid.Faces.Size.Equals(0))
            {
                solidOpening = GeometryCreationUtilities
                    .CreateExtrusionGeometry(loops, cutDir, .1);

                intersectSolid = BooleanOperationsUtils
                    .ExecuteBooleanOperation(solidOpening,
                        solidHost, BooleanOperationsType.Intersect);
            }

            if (DebugHandler.EnableSolidUtilityVolumes)
            {
                using (var t = new Transaction(doc))
                {
                    t.Start("Stacked1");
                    ShapeCreator.CreateDirectShape(doc,
                        intersectSolid, "stackedOpening");
                    t.Commit();
                }
            }

            return solHandler.GetLargestFaceArea(intersectSolid);
        }

        static bool IsInRoom(Room room, FamilyInstance f)
        {
            var id = room.Id;
            return (f.Room != null && f.Room.Id == id)
                || (f.ToRoom != null && f.ToRoom.Id == id)
                || (f.FromRoom != null && f.FromRoom.Id == id);
        }
    }
}
