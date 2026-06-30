// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from PathOfTravelDoors by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PathOfTravelDoors

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTravelElement = Autodesk.Revit.DB.Analysis.PathOfTravel;

namespace Ara3D.RevitSampleBrowser.PathOfTravelDoors.CS
{
    /// <summary>
    ///     Finds doors crossed by a path of travel using ReferenceIntersector
    ///     with a geometric fallback for families that lack ray-hit geometry.
    /// </summary>
    internal static class PathOfTravelDoorFinder
    {
        const double ProximityTolerance = 1e-6;

        public static IList<FamilyInstance> FindDoorsInTraversalOrder(
            PathOfTravelElement pathOfTravel,
            ViewPlan viewPlan,
            View3D view3D)
        {
            var doc = pathOfTravel.Document;
            var curves = pathOfTravel.GetCurves();
            if (curves == null || curves.Count == 0)
            {
                return Array.Empty<FamilyInstance>();
            }

            var elevation = GetPathElevation(curves);
            var orderedHits = new List<DoorHit>();

            foreach (var curve in curves)
            {
                if (curve is not Line segment || segment.Length < ProximityTolerance)
                {
                    continue;
                }

                var segmentHits = FindDoorsOnSegment(segment, viewPlan, view3D, doc, elevation);
                orderedHits.AddRange(segmentHits);
            }

            return DeduplicatePreservingOrder(orderedHits)
                .Select(hit => hit.Door)
                .ToList();
        }

        static double GetPathElevation(IList<Curve> curves)
        {
            foreach (var curve in curves)
            {
                if (curve == null)
                {
                    continue;
                }

                return curve.GetEndPoint(0).Z;
            }

            return 0;
        }

        static IEnumerable<DoorHit> FindDoorsOnSegment(
            Line segment,
            ViewPlan viewPlan,
            View3D view3D,
            Document doc,
            double elevation)
        {
            var hits = FindDoorsByReferenceIntersector(segment, view3D);
            if (hits.Count == 0)
            {
                hits = FindDoorsByGeometry(segment, viewPlan, doc, elevation);
            }

            var start = segment.GetEndPoint(0);
            return hits
                .Select(door => new DoorHit(door, start.DistanceTo(GetDoorPlanPoint(door, elevation))));
        }

        static List<FamilyInstance> FindDoorsByReferenceIntersector(
            Line segment,
            View3D view3D)
        {
            if (view3D == null)
            {
                return [];
            }

            var doc = view3D.Document;
            var direction = segment.Direction;
            var length = segment.Length;
            var start = segment.GetEndPoint(0);
            var end = segment.GetEndPoint(1);

            var doorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var intersector = new ReferenceIntersector(
                doorFilter,
                FindReferenceTarget.Element,
                view3D);

            var forwardDoor = FindDoorOnRay(intersector, doc, start, direction, length);
            var reverseDoor = FindDoorOnRay(intersector, doc, end, direction.Negate(), length);

            return forwardDoor != null
                && reverseDoor != null
                && forwardDoor.Id == reverseDoor.Id
                ? [forwardDoor]
                : [];
        }

        static FamilyInstance FindDoorOnRay(
            ReferenceIntersector intersector,
            Document doc,
            XYZ origin,
            XYZ direction,
            double maxDistance)
        {
            var hit = intersector.FindNearest(origin, direction);
            return hit == null || hit.Proximity > maxDistance + ProximityTolerance ? null : doc.GetElement(hit.GetReference()) as FamilyInstance;
        }

        static List<FamilyInstance> FindDoorsByGeometry(
            Line segment,
            ViewPlan viewPlan,
            Document doc,
            double elevation)
        {
            var planSegment = ToPlanLine(segment, elevation);
            if (planSegment == null)
            {
                return [];
            }

            var doors = new FilteredElementCollector(doc, viewPlan.Id)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>();

            var hits = new List<FamilyInstance>();
            foreach (var door in doors)
            {
                var opening = GetDoorOpeningLine(door, elevation);
                if (opening == null)
                {
                    continue;
                }

                if (PlanSegmentsIntersect(planSegment, opening))
                {
                    hits.Add(door);
                }
            }

            return hits;
        }

        static Line ToPlanLine(Line segment, double elevation)
        {
            var start = segment.GetEndPoint(0);
            var end = segment.GetEndPoint(1);
            return Line.CreateBound(
                new XYZ(start.X, start.Y, elevation),
                new XYZ(end.X, end.Y, elevation));
        }

        static bool PlanSegmentsIntersect(Line pathSegment, Line doorOpening)
        {
            var result = pathSegment.Intersect(doorOpening, out var intersections);
            if (result != SetComparisonResult.Overlap || intersections == null || intersections.Size == 0)
            {
                return false;
            }

            for (var i = 0; i < intersections.Size; i++)
            {
                var intersection = intersections.get_Item(i);
                if (intersection.XYZPoint != null)
                {
                    return true;
                }
            }

            return false;
        }

        static Line GetDoorOpeningLine(FamilyInstance door, double elevation)
        {
            if (door.Host is Wall wall)
            {
                try
                {
                    var loop = ExporterIFCUtils.GetInstanceCutoutFromWall(
                        door.Document,
                        wall,
                        door,
                        out _);

                    return GetLongestPlanLineFromLoop(loop, elevation);
                }
                catch
                {
                    // Fall through to location-based opening line.
                }
            }

            if (door.Location is not LocationPoint locationPoint)
            {
                return null;
            }

            var width = door.Symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH)?.AsDouble();
            if (!width.HasValue || width.Value <= ProximityTolerance)
            {
                width = 3.0;
            }

            var center = new XYZ(
                locationPoint.Point.X,
                locationPoint.Point.Y,
                elevation);

            var hand = door.HandOrientation;
            if (hand.GetLength() < ProximityTolerance)
            {
                hand = door.FacingOrientation;
            }

            hand = hand.Normalize();
            var halfWidth = hand.Multiply(width.Value / 2.0);
            return Line.CreateBound(center - halfWidth, center + halfWidth);
        }

        static Line GetLongestPlanLineFromLoop(CurveLoop loop, double elevation)
        {
            if (loop == null)
            {
                return null;
            }

            Line longest = null;
            var maxLength = 0.0;

            foreach (var curve in loop)
            {
                if (curve is not Line line)
                {
                    continue;
                }

                var planLine = Line.CreateBound(
                    new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, elevation),
                    new XYZ(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, elevation));

                var length = planLine.Length;
                if (length > maxLength)
                {
                    maxLength = length;
                    longest = planLine;
                }
            }

            return longest;
        }

        static XYZ GetDoorPlanPoint(FamilyInstance door, double elevation)
        {
            if (door.Location is LocationPoint locationPoint)
            {
                return new XYZ(
                    locationPoint.Point.X,
                    locationPoint.Point.Y,
                    elevation);
            }

            var bbox = door.get_BoundingBox(null);
            return bbox == null
                ? XYZ.Zero
                : new XYZ(
                (bbox.Min.X + bbox.Max.X) / 2.0,
                (bbox.Min.Y + bbox.Max.Y) / 2.0,
                elevation);
        }

        static IList<DoorHit> DeduplicatePreservingOrder(IList<DoorHit> hits)
        {
            var seen = new HashSet<ElementId>();
            var result = new List<DoorHit>();

            foreach (var hit in hits.OrderBy(item => item.DistanceAlongPath))
            {
                if (seen.Add(hit.Door.Id))
                {
                    result.Add(hit);
                }
            }

            return result;
        }

        readonly struct DoorHit
        {
            public DoorHit(FamilyInstance door, double distanceAlongPath)
            {
                Door = door;
                DistanceAlongPath = distanceAlongPath;
            }

            public FamilyInstance Door { get; }

            public double DistanceAlongPath { get; }
        }
    }
}
