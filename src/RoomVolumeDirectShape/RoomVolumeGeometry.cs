// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RoomVolumeDirectShape by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RoomVolumeDirectShape

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.RoomVolumeDirectShape.CS
{
    internal static class RoomVolumeGeometry
    {
        public static IList<GeometryObject> CopyGeometry(
            GeometryElement geo,
            ElementId materialId)
        {
            foreach (var obj in geo)
            {
                if (obj is not Solid solid || solid.Volume <= 0)
                {
                    continue;
                }

                if (!SolidUtils.IsValidForTessellation(solid))
                {
                    continue;
                }

                TessellatedShapeBuilder builder = new();
                List<XYZ> vertices = new(3);

                builder.OpenConnectedFaceSet(false);

                SolidOrShellTessellationControls controls = new()
                {
                    Accuracy = 0.03,
                    LevelOfDetail = 0.1,
                    MinAngleInTriangle = 3 * Math.PI / 180.0,
                    MinExternalAngleBetweenTriangles = 0.2 * Math.PI
                };

                var shell = SolidUtils.TessellateSolidOrShell(solid, controls);
                if (shell.ShellComponentCount != 1)
                {
                    continue;
                }

                var component = shell.GetShellComponent(0);

                for (var i = 0; i < component.TriangleCount; ++i)
                {
                    var triangle = component.GetTriangle(i);

                    vertices.Clear();
                    vertices.Add(component.GetVertex(triangle.VertexIndex0));
                    vertices.Add(component.GetVertex(triangle.VertexIndex1));
                    vertices.Add(component.GetVertex(triangle.VertexIndex2));

                    TessellatedFace face = new(vertices, materialId);
                    if (builder.DoesFaceHaveEnoughLoopsAndVertices(face))
                    {
                        builder.AddFace(face);
                    }
                }

                builder.CloseConnectedFaceSet();
                builder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
                builder.Fallback = TessellatedShapeBuilderFallback.Mesh;
                builder.Build();

                var result = builder.GetBuildResult();
                var objects = result.GetGeometricalObjects();
                if (objects != null && objects.Count > 0)
                {
                    return objects;
                }
            }

            return null;
        }
    }
}
