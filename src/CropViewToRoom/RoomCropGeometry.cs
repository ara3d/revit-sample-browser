// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CropViewToRoom by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CropViewToRoom

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.CropViewToRoom.CS
{
    internal static class RoomCropGeometry
    {
        const double WallPaddingFeet = 0.1;

        public static bool TryCreateOffsetCropLoop(
            Document doc,
            IList<IList<BoundarySegment>> boundaryLoops,
            XYZ offsetNormal,
            out CurveLoop cropLoop)
        {
            cropLoop = null;

            if (boundaryLoops == null || boundaryLoops.Count == 0)
            {
                return false;
            }

            foreach (var boundaryLoop in boundaryLoops)
            {
                var innerLoop = new CurveLoop();
                var wallThicknesses = new List<double>();

                if (!TryBuildInnerLoopAndThicknesses(
                        doc, boundaryLoop, innerLoop, wallThicknesses))
                {
                    return false;
                }

                var offsetLoop = CurveLoop.CreateViaOffset(
                    innerLoop, wallThicknesses, offsetNormal);

                cropLoop = TessellateToLineLoop(offsetLoop);
                return cropLoop != null;

                // Use only the first boundary loop; ignore holes and disjoint parts.
            }

            return false;
        }

        public static bool TryApplyCropShape(View view, CurveLoop loop)
        {
            if (view == null || loop == null)
            {
                return false;
            }

            var cropManager = view.GetCropRegionShapeManager();
            if (!cropManager.IsCropRegionShapeValid(loop))
            {
                return false;
            }

            view.CropBoxVisible = true;
            view.CropBoxActive = true;
            cropManager.SetCropShape(loop);
            return true;
        }

        static bool TryBuildInnerLoopAndThicknesses(
            Document doc,
            IList<BoundarySegment> boundaryLoop,
            CurveLoop innerLoop,
            IList<double> wallThicknesses)
        {
            var firstPass = true;
            string previousOrientation = null;
            var previousWidth = 0.0;

            foreach (var segment in boundaryLoop)
            {
                innerLoop.Append(segment.GetCurve());

                var elem = doc.GetElement(segment.ElementId);
                if (elem != null)
                {
                    AddWallThicknessForElement(
                        doc,
                        elem,
                        wallThicknesses,
                        ref firstPass,
                        ref previousOrientation,
                        ref previousWidth);
                }
                else
                {
                    wallThicknesses.Add(-WallPaddingFeet);
                }
            }

            return innerLoop.Count() > 0
                && wallThicknesses.Count == innerLoop.Count();
        }

        static void AddWallThicknessForElement(
            Document doc,
            Element elem,
            IList<double> wallThicknesses,
            ref bool firstPass,
            ref string previousOrientation,
            ref double previousWidth)
        {
            var category = elem.Category?.BuiltInCategory;
            if (category == BuiltInCategory.OST_Walls && elem is Wall wall)
            {
                AddWallThickness(
                    wall,
                    wallThicknesses,
                    ref firstPass,
                    ref previousOrientation,
                    ref previousWidth);
                return;
            }

            if (category == BuiltInCategory.OST_RoomSeparationLines)
            {
                AddRoomSeparatorThickness(
                    elem,
                    wallThicknesses,
                    ref firstPass,
                    ref previousOrientation,
                    ref previousWidth);
            }
        }

        static void AddWallThickness(
            Wall wall,
            IList<double> wallThicknesses,
            ref bool firstPass,
            ref string previousOrientation,
            ref double previousWidth)
        {
            var orientation = GetWallOrientationAxis(wall);

            if (firstPass)
            {
                firstPass = false;
                previousOrientation = orientation;
                previousWidth = wall.Width + WallPaddingFeet;
                wallThicknesses.Add(previousWidth);
                return;
            }

            if (orientation == previousOrientation)
            {
                if (wall.Width > previousWidth)
                {
                    wallThicknesses[wallThicknesses.Count - 1]
                        = wall.Width + WallPaddingFeet;
                }

                wallThicknesses.Add(wall.Width + WallPaddingFeet);
                previousWidth = wall.Width + WallPaddingFeet;
                previousOrientation = orientation;
                return;
            }

            previousOrientation = orientation;
            previousWidth = wall.Width + WallPaddingFeet;
            wallThicknesses.Add(previousWidth);
        }

        static void AddRoomSeparatorThickness(
            Element elem,
            IList<double> wallThicknesses,
            ref bool firstPass,
            ref string previousOrientation,
            ref double previousWidth)
        {
            var opt = new Options();
            var geometry = elem.get_Geometry(opt);
            if (geometry == null)
            {
                return;
            }

            foreach (var geomObj in geometry)
            {
                if (geomObj is not Line line)
                {
                    continue;
                }

                previousOrientation = GetLineOrientationAxis(line);

                if (firstPass)
                {
                    firstPass = false;
                    previousWidth = WallPaddingFeet;
                    wallThicknesses.Add(previousWidth);
                }
                else
                {
                    wallThicknesses.Add(previousWidth);
                    previousWidth = WallPaddingFeet;
                }
            }
        }

        static string GetWallOrientationAxis(Wall wall)
        {
            return Math.Abs(Math.Round(wall.Orientation.Y, 0)) == 1
                ? "Y"
                : "X";
        }

        static string GetLineOrientationAxis(Line line)
        {
            var deltaX = line.GetEndPoint(1).X - line.GetEndPoint(0).X;
            var deltaY = line.GetEndPoint(1).Y - line.GetEndPoint(0).Y;
            return Math.Abs(deltaX) < Math.Abs(deltaY) ? "Y" : "X";
        }

        static CurveLoop TessellateToLineLoop(CurveLoop offsetLoop)
        {
            var lineLoop = new CurveLoop();

            foreach (var curve in offsetLoop)
            {
                var points = curve.Tessellate();
                for (var i = 0; i < points.Count - 1; i++)
                {
                    lineLoop.Append(Line.CreateBound(points[i], points[i + 1]));
                }
            }

            return lineLoop.Count() > 0 ? lineLoop : null;
        }
    }
}
