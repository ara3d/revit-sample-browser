//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.MultiplanarRebar.CS
{
    /// <summary>
    ///     This class is to parse the geometry information of given Corbel FamilyInstance,
    ///     and finally construct a CorbelFrame according to the parsed geometry information.
    /// </summary>
    internal class GeometryUtil
    {
        /// <summary>
        ///     This method parses geometry information of given Corbel to construct the CorbelFrame.
        /// </summary>
        /// <param name="corbel">Given corbel family instance to parse</param>
        /// <returns>CorbelFrame object</returns>
        public static CorbelFrame ParseCorbelGeometry(FamilyInstance corbel)
        {
            // Get Corbel Host information.
            var corbelHost = corbel.Host;
            var corbelHostFace = corbel.HostFace;

            var hostPlane = corbelHost.GetGeometryObjectFromReference(corbelHostFace) as PlanarFace;
            var hostNormal = GetNormalOutside(hostPlane);

            // Extract the faces in Corbel parallel with Corbel host face.
            var corbelSolid = GetElementSolid(corbel);
            PlanarFace corbelBottomFace = null;
            foreach (Face face in corbelSolid.Faces)
            {
                var planarFace = face as PlanarFace;
                var normal = GetNormalOutside(planarFace);
                if (normal.IsAlmostEqualTo(hostNormal))
                {
                }
                else if (normal.IsAlmostEqualTo(-hostNormal))
                {
                    corbelBottomFace = planarFace;
                }
            }

            // Extract the faces in Corbel Host parallel with Corbel host face.
            var hostSolid = GetElementSolid(corbelHost);
            PlanarFace hostTopFace = null;
            var hostBottomFace = hostPlane;
            foreach (Face face in hostSolid.Faces)
            {
                var planarFace = face as PlanarFace;
                var normal = GetNormalOutside(planarFace);
                if (normal.IsAlmostEqualTo(-hostNormal)) hostTopFace = planarFace;
            }

            // Parse the side faces to find out the Trapezoid face.
            Edge topEdge = null;
            Edge leftEdge = null;
            Edge bottomEdge = null;
            Edge rightEdge = null;
            PlanarFace trapezoidFace = null;
            var foundEdgeIndex = -1;
            var foundTrapezoid = false;
            var bottomEdges = corbelBottomFace.EdgeLoops.get_Item(0);
            foreach (Edge edge in bottomEdges)
            {
                bottomEdge = edge;
                foundEdgeIndex++;
                foundTrapezoid = IsTrapezoid(hostNormal, corbelBottomFace, bottomEdge,
                    out trapezoidFace, out topEdge, out leftEdge, out rightEdge);
                if (foundTrapezoid) break;
            }

            // Check to see if the Trapezoid faces was found.
            if (!foundTrapezoid)
                // Throw if no any trapezoid face in corbel.
                throw new Exception("Didn't find the trapezoid face in corbel [Id:" + corbel.Id + "].");

            var depthEdge = bottomEdges.get_Item((foundEdgeIndex + 1) % bottomEdges.Size);

            var hostDepth = GetDistance(hostTopFace, hostBottomFace);

            // Compute the host face cover distance.
            var corbelHostData = RebarHostData.GetRebarHostData(corbelHost);
            // Get CoverType of the given host face
            var coverType = corbelHostData.GetCoverType(hostTopFace.Reference);
            // if the host face don't have a CoverType, then try to get the common CoverType.
            if (coverType == null)
                coverType = corbelHostData.GetCommonCoverType();
            // Get the Cover Distance
            var coverDistance = coverType.CoverDistance;

            // Construct the CorbelFrame from the given parsed trapezoid information.
            return ConstructCorbelFrame(
                corbel, depthEdge,
                leftEdge, bottomEdge, rightEdge, topEdge,
                corbel.Document, trapezoidFace,
                hostDepth, coverDistance);
        }

        /// <summary>
        ///     Check if the given bottom edge was shared by a trapezoid face with left edge vertical.
        /// </summary>
        /// <param name="hostNormal">Corbel Host face Normal</param>
        /// <param name="corbelBottomFace">Bottom Face of Corbel</param>
        /// <param name="bottomEdge">Given bottom edge to test</param>
        /// <param name="trapezoidFace">Output the trapezoid Face</param>
        /// <param name="topEdge">Output trapezoid top edge</param>
        /// <param name="leftEdge">Output trapezoid left edge</param>
        /// <param name="rightEdge">Output trapezoid right edge</param>
        /// <returns>True if there is a trapezoid face share the given bottom edge, otherwise false.</returns>
        private static bool IsTrapezoid(
            XYZ hostNormal, PlanarFace corbelBottomFace, Edge bottomEdge,
            out PlanarFace trapezoidFace, out Edge topEdge,
            out Edge leftEdge, out Edge rightEdge)
        {
            var face1 = bottomEdge.GetFace(0) as PlanarFace;
            var face2 = bottomEdge.GetFace(1) as PlanarFace;

            trapezoidFace = face1 == corbelBottomFace ? face2 : face1;

            var trapezoidFaceEdges = trapezoidFace.EdgeLoops.get_Item(0);
            var bottomEdgeDir = (bottomEdge.Evaluate(1.0) - bottomEdge.Evaluate(0.0)).Normalize();
            var bottomEdgeIndex = -1;
            topEdge = null;
            for (var i = 0; i < trapezoidFaceEdges.Size; i++)
            {
                var edge = trapezoidFaceEdges.get_Item(i);
                var edgeDir = (edge.Evaluate(1.0) - edge.Evaluate(0.0)).Normalize();
                if (edgeDir.IsAlmostEqualTo(bottomEdgeDir) ||
                    edgeDir.IsAlmostEqualTo(-bottomEdgeDir))
                {
                    if (edge.Evaluate(0.0).IsAlmostEqualTo(bottomEdge.Evaluate(0.0)))
                    {
                        bottomEdge = edge;
                        bottomEdgeIndex = i;
                    }
                    else
                    {
                        topEdge = edge;
                    }
                }
            }

            leftEdge = trapezoidFaceEdges.get_Item((trapezoidFaceEdges.Size + bottomEdgeIndex - 1) %
                                                   trapezoidFaceEdges.Size);
            rightEdge = trapezoidFaceEdges.get_Item((bottomEdgeIndex + 1) % trapezoidFaceEdges.Size);

            var leftEdgeDir = (leftEdge.Evaluate(1.0) - leftEdge.Evaluate(0.0)).Normalize();
            var isLeftEdgeVertical = false;
            if (leftEdgeDir.IsAlmostEqualTo(hostNormal) ||
                leftEdgeDir.IsAlmostEqualTo(-hostNormal))
                isLeftEdgeVertical = true;

            var rightEdgeDir = (rightEdge.Evaluate(1.0) - rightEdge.Evaluate(0.0)).Normalize();
            var rightEdgeIsVertical = false;
            if (rightEdgeDir.IsAlmostEqualTo(hostNormal) ||
                rightEdgeDir.IsAlmostEqualTo(-hostNormal))
                rightEdgeIsVertical = true;

            return isLeftEdgeVertical && !rightEdgeIsVertical;
        }

        /// <summary>
        ///     Create the CorbelFrame object with the given trapezoid face, corbel and its host information.
        /// </summary>
        /// <param name="corbel">Corbel instance</param>
        /// <param name="depthEdge">Depth Edge which is vertical with trapezoid face</param>
        /// <param name="leftEdge">Left edge of trapezoid</param>
        /// <param name="bottomEdge">Bottom edge of trapezoid</param>
        /// <param name="rightEdge">Right edge of trapezoid</param>
        /// <param name="topEdge">Top edge of trapezoid</param>
        /// <param name="revitDoc">Revit Document</param>
        /// <param name="trapezoidFace">Trapezoid Face</param>
        /// <param name="hostDepth">Corbel Host depth</param>
        /// <param name="hostTopCoverDistance">Corbel Host Top face cover distance</param>
        /// <returns>CorbelFrame object</returns>
        private static CorbelFrame ConstructCorbelFrame(
            FamilyInstance corbel,
            Edge depthEdge, Edge leftEdge, Edge bottomEdge, Edge rightEdge, Edge topEdge,
            Document revitDoc, PlanarFace trapezoidFace,
            double hostDepth, double hostTopCoverDistance)
        {
            var leftEdgeDir = (leftEdge.Evaluate(1.0) - leftEdge.Evaluate(0.0)).Normalize();
            var leftEdgeV0 = leftEdge.Evaluate(0.0);
            var leftEdgeLine = Line.CreateUnbound(leftEdgeV0, leftEdgeDir);

            var rightEdgeDir = (rightEdge.Evaluate(1.0) - rightEdge.Evaluate(0.0)).Normalize();
            var rightEdgeV0 = rightEdge.Evaluate(0.0);
            var rightEdgeLine = Line.CreateUnbound(rightEdgeV0, rightEdgeDir);

            var topEdgeDir = (topEdge.Evaluate(1.0) - topEdge.Evaluate(0.0)).Normalize();
            var topEdgeV0 = topEdge.Evaluate(0.0);
            var topEdgeLine = Line.CreateUnbound(topEdgeV0, topEdgeDir);

            IntersectionResultArray intersections;
            topEdgeLine.Intersect(leftEdgeLine, out intersections);
            var prevX = intersections.get_Item(0).XYZPoint;

            topEdgeLine.Intersect(rightEdgeLine, out intersections);
            var nextX = intersections.get_Item(0).XYZPoint;

            var edgeV0 = GetCommonVertex(bottomEdge, leftEdge);
            var edgeV1 = GetCommonVertex(bottomEdge, rightEdge);

            var topBoundLine = Line.CreateBound(nextX, prevX);
            var leftBoundLine = Line.CreateBound(prevX, edgeV0);
            var bottomBoundLine = Line.CreateBound(edgeV0, edgeV1);
            var rightBoundLine = Line.CreateBound(edgeV1, nextX);

            var profile = new Trapezoid(topBoundLine, leftBoundLine, bottomBoundLine, rightBoundLine);

            var depthEdgeV0 = depthEdge.Evaluate(0.0);
            var depthEdgeV1 = depthEdge.Evaluate(1.0);
            Line depthLine = null;
            if (depthEdgeV0.IsAlmostEqualTo(edgeV0))
                depthLine = Line.CreateBound(depthEdgeV0, depthEdgeV1);
            else if (depthEdgeV1.IsAlmostEqualTo(edgeV0)) depthLine = Line.CreateBound(depthEdgeV1, depthEdgeV0);

            var frame = new CorbelFrame(corbel, profile, depthLine, hostDepth, hostTopCoverDistance);

            return frame;
        }

        /// <summary>
        ///     Get the common vertex XYZ of two edges.
        /// </summary>
        /// <param name="edge1">Edge 1</param>
        /// <param name="edge2">Edge 2</param>
        /// <returns>Common vertex XYZ</returns>
        private static XYZ GetCommonVertex(Edge edge1, Edge edge2)
        {
            var edge1V0 = edge1.Evaluate(0.0);
            var edge1V1 = edge1.Evaluate(1.0);

            var edge2V0 = edge2.Evaluate(0.0);
            var edge2V1 = edge2.Evaluate(1.0);

            if (edge1V0.IsAlmostEqualTo(edge2V0) ||
                edge1V0.IsAlmostEqualTo(edge2V1))
                return edge1V0;
            if (edge1V1.IsAlmostEqualTo(edge2V0) ||
                edge1V1.IsAlmostEqualTo(edge2V1))
                return edge1V1;
            return null;
        }

        /// <summary>
        ///     Extract the Solid of given element.
        /// </summary>
        /// <param name="element">Given Element to get its Solid</param>
        /// <returns>Solid of given element</returns>
        private static Solid GetElementSolid(Element element)
        {
            var goption = new Options();
            goption.ComputeReferences = true;
            var gelem = element.get_Geometry(goption);
            Solid resultSolid = null;
            //foreach (GeometryObject gobj in gelem.Objects)
            var Objects = gelem.GetEnumerator();
            while (Objects.MoveNext())
            {
                var gobj = Objects.Current;

                var gIns = gobj as GeometryInstance;
                if (gIns != null)
                {
                    var finalGeom = gIns.GetInstanceGeometry();
                    //foreach (GeometryObject gobj2 in finalGeom.Objects)
                    var Objects1 = finalGeom.GetEnumerator();
                    while (Objects1.MoveNext())
                    {
                        var gobj2 = Objects1.Current;

                        var tSolid = gobj2 as Solid;
                        if (tSolid != null && tSolid.Faces.Size > 0 && tSolid.Volume > 0)
                        {
                            resultSolid = tSolid;
                            break;
                        }
                    }
                }

                if (resultSolid == null)
                {
                    var tSolid2 = gobj as Solid;
                    if (tSolid2 != null && tSolid2.Faces.Size > 0 && tSolid2.Volume > 0)
                    {
                        resultSolid = tSolid2;
                        break;
                    }
                }
            }

            return resultSolid;
        }

        /// <summary>
        ///     Compute the outside normal of given face.
        /// </summary>
        /// <param name="face">Given face to get its outside normal</param>
        /// <returns>Outside normal of given face</returns>
        private static XYZ GetNormalOutside(Face face)
        {
            var edge = face.EdgeLoops.get_Item(0).get_Item(0);
            var pt = edge.EvaluateOnFace(0.5, face);
            var faceNormal = face.ComputeNormal(pt);
            return faceNormal;
        }

        /// <summary>
        ///     Compute the distance between two planar faces.
        /// </summary>
        /// <param name="face1">Face 1</param>
        /// <param name="face2">Face 2</param>
        /// <returns>Distance of the two planar faces</returns>
        private static double GetDistance(PlanarFace face1, PlanarFace face2)
        {
            var boxUV = face2.GetBoundingBox();
            var center = (boxUV.Max + boxUV.Min) * 0.5;
            var centerPt = face2.Evaluate(center);
            face1.Project(centerPt);
            return face1.Project(centerPt).Distance;
        }
    }
}