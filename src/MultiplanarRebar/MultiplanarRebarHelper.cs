// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;

namespace Ara3D.RevitSampleBrowser.MultiplanarRebar.CS
{
    public static class MultiplanarRebarHelper
    {
        public static ElementId GetOrCreateDef(string name, Document revitDoc)
        {
            var ed = GetOrCreateDef(name, revitDoc.Application);
            return RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(revitDoc, ed);
        }

        public static ExternalDefinition GetOrCreateDef(string name, Application revitApp) =>
            GetOrCreateDef(name, "MultiplanarRebarParameters", revitApp);

        public static ExternalDefinition GetOrCreateDef(string name, string groupName, Application revitApp)
        {
            var defFile = revitApp.OpenSharedParameterFile();
            if (defFile == null)
                throw new InvalidOperationException("No shared parameter file is open.");

            var defGroups = defFile.Groups;
            var defGroup = defGroups.get_Item(groupName) ?? defGroups.Create(groupName);
            if (!(defGroup.Definitions.get_Item(name) is ExternalDefinition externalDefinition))
            {
                var options = new ExternalDefinitionCreationOptions(name, SpecTypeId.ReinforcementLength);
                externalDefinition = defGroup.Definitions.Create(options) as ExternalDefinition;
            }

            return externalDefinition;
        }

        public static CorbelFrame ParseCorbelGeometry(FamilyInstance corbel)
        {
            var corbelHost = corbel.Host;
            var corbelHostFace = corbel.HostFace;
            var hostPlane = corbelHost.GetGeometryObjectFromReference(corbelHostFace) as PlanarFace;
            var hostNormal = GetNormalOutside(hostPlane);

            var corbelSolid = ElementQuery.GetElementSolid(corbel);
            PlanarFace corbelBottomFace = null;
            foreach (Face face in corbelSolid.Faces)
            {
                if (face is PlanarFace planarFace)
                {
                    var normal = GetNormalOutside(planarFace);
                    if (normal.IsAlmostEqualTo(-hostNormal))
                        corbelBottomFace = planarFace;
                }
            }

            var hostSolid = ElementQuery.GetElementSolid(corbelHost);
            PlanarFace hostTopFace = null;
            var hostBottomFace = hostPlane;
            foreach (Face face in hostSolid.Faces)
            {
                if (face is PlanarFace planarFace && GetNormalOutside(planarFace).IsAlmostEqualTo(-hostNormal))
                    hostTopFace = planarFace;
            }

            Edge topEdge = null, leftEdge = null, bottomEdge = null, rightEdge = null;
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

            if (!foundTrapezoid)
                throw new Exception($"Didn't find the trapezoid face in corbel [Id:{corbel.Id}].");

            var depthEdge = bottomEdges.get_Item((foundEdgeIndex + 1) % bottomEdges.Size);
            var hostDepth = FaceAndSolidGeometry.GetDistance(hostTopFace, hostBottomFace);
            var corbelHostData = RebarHostData.GetRebarHostData(corbelHost);
            var coverType = corbelHostData.GetCoverType(hostTopFace.Reference) ?? corbelHostData.GetCommonCoverType();
            return ConstructCorbelFrame(corbel, depthEdge, leftEdge, bottomEdge, rightEdge, topEdge,
                corbel.Document, trapezoidFace, hostDepth, coverType.CoverDistance);
        }

        private static bool IsTrapezoid(XYZ hostNormal, PlanarFace corbelBottomFace, Edge bottomEdge,
            out PlanarFace trapezoidFace, out Edge topEdge, out Edge leftEdge, out Edge rightEdge)
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
                if (edgeDir.IsAlmostEqualTo(bottomEdgeDir) || edgeDir.IsAlmostEqualTo(-bottomEdgeDir))
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
            var isLeftEdgeVertical = leftEdgeDir.IsAlmostEqualTo(hostNormal) || leftEdgeDir.IsAlmostEqualTo(-hostNormal);
            var rightEdgeDir = (rightEdge.Evaluate(1.0) - rightEdge.Evaluate(0.0)).Normalize();
            var rightEdgeIsVertical = rightEdgeDir.IsAlmostEqualTo(hostNormal) || rightEdgeDir.IsAlmostEqualTo(-hostNormal);
            return isLeftEdgeVertical && !rightEdgeIsVertical;
        }

        private static CorbelFrame ConstructCorbelFrame(FamilyInstance corbel, Edge depthEdge,
            Edge leftEdge, Edge bottomEdge, Edge rightEdge, Edge topEdge, Document revitDoc,
            PlanarFace trapezoidFace, double hostDepth, double hostTopCoverDistance)
        {
            var leftEdgeDir = (leftEdge.Evaluate(1.0) - leftEdge.Evaluate(0.0)).Normalize();
            var leftEdgeLine = Line.CreateUnbound(leftEdge.Evaluate(0.0), leftEdgeDir);
            var rightEdgeDir = (rightEdge.Evaluate(1.0) - rightEdge.Evaluate(0.0)).Normalize();
            var rightEdgeLine = Line.CreateUnbound(rightEdge.Evaluate(0.0), rightEdgeDir);
            var topEdgeDir = (topEdge.Evaluate(1.0) - topEdge.Evaluate(0.0)).Normalize();
            var topEdgeLine = Line.CreateUnbound(topEdge.Evaluate(0.0), topEdgeDir);

            topEdgeLine.Intersect(leftEdgeLine, out var intersections);
            var prevX = intersections.get_Item(0).XYZPoint;
            topEdgeLine.Intersect(rightEdgeLine, out intersections);
            var nextX = intersections.get_Item(0).XYZPoint;

            var edgeV0 = GetCommonVertex(bottomEdge, leftEdge);
            var edgeV1 = GetCommonVertex(bottomEdge, rightEdge);
            var profile = new Trapezoid(
                Line.CreateBound(nextX, prevX),
                Line.CreateBound(prevX, edgeV0),
                Line.CreateBound(edgeV0, edgeV1),
                Line.CreateBound(edgeV1, nextX));

            var depthEdgeV0 = depthEdge.Evaluate(0.0);
            var depthEdgeV1 = depthEdge.Evaluate(1.0);
            Line depthLine = null;
            if (depthEdgeV0.IsAlmostEqualTo(edgeV0))
                depthLine = Line.CreateBound(depthEdgeV0, depthEdgeV1);
            else if (depthEdgeV1.IsAlmostEqualTo(edgeV0))
                depthLine = Line.CreateBound(depthEdgeV1, depthEdgeV0);

            return new CorbelFrame(corbel, profile, depthLine, hostDepth, hostTopCoverDistance);
        }

        private static XYZ GetCommonVertex(Edge edge1, Edge edge2)
        {
            var edge1V0 = edge1.Evaluate(0.0);
            var edge1V1 = edge1.Evaluate(1.0);
            var edge2V0 = edge2.Evaluate(0.0);
            var edge2V1 = edge2.Evaluate(1.0);
            if (edge1V0.IsAlmostEqualTo(edge2V0) || edge1V0.IsAlmostEqualTo(edge2V1)) return edge1V0;
            if (edge1V1.IsAlmostEqualTo(edge2V0) || edge1V1.IsAlmostEqualTo(edge2V1)) return edge1V1;
            return null;
        }

        private static XYZ GetNormalOutside(Face face)
        {
            var edge = face.EdgeLoops.get_Item(0).get_Item(0);
            return face.ComputeNormal(edge.EvaluateOnFace(0.5, face));
        }
    }
}
