// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from DirectShapeFromFace by Frode Tørresdal and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/DirectShapeFromFace

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.DirectShapeFromFace.CS
{
    /// <summary>
    ///     Creates a DirectShape from a picked face using sketch-plane reuse,
    ///     model-line triangle visualization, and geometry-instance transform
    ///     stack resolution for nested family instances.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateDirectShapeCommand : IExternalCommand
    {
        const string SketchPlaneNameMarker = "<not associated>";
        const double Epsilon = 1.0e-9;

        static int _sketchPlaneCreationCounter;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var applicationId = commandData.Application.ActiveAddInId
                .GetGUID().ToString();

            Reference faceReference;
            try
            {
                faceReference = uidoc.Selection.PickObject(ObjectType.Face);
            }
            catch (OperationCanceledException)
            {
                message = "Face selection was cancelled.";
                return Result.Cancelled;
            }

            var stableRepresentation = faceReference
                .ConvertToStableRepresentation(doc);
            var element = doc.GetElement(faceReference.ElementId);
            var face = element.GetGeometryObjectFromReference(faceReference) as Face;
            if (face == null)
            {
                message = "The selected reference is not a face.";
                return Result.Failed;
            }

            using var transaction = new Transaction(doc, "Create DirectShape from Face");
            transaction.Start();

            var builder = new TessellatedShapeBuilder();
            builder.OpenConnectedFaceSet(false);

            Transform transform = null;
            if (element is FamilyInstance)
            {
                var options = new Options { ComputeReferences = true };
                var geometry = element.get_Geometry(options);
                var transformStack = new Stack<Transform>();

                if (GetTransformStackForObject(
                        transformStack, geometry, doc, stableRepresentation)
                    && transformStack.Count > 0)
                {
                    transform = Transform.Identity;
                    while (transformStack.Count > 0)
                    {
                        transform = transform.Multiply(transformStack.Pop());
                    }
                }
            }

            var mesh = face.Triangulate();
            if (mesh == null || mesh.NumTriangles == 0)
            {
                message = "The selected face could not be triangulated.";
                transaction.RollBack();
                return Result.Failed;
            }

            if (transform != null)
            {
                mesh = mesh.get_Transformed(transform);
            }

            var triangleCorners = new XYZ[3];
            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                triangleCorners[0] = triangle.get_Vertex(0);
                triangleCorners[1] = triangle.get_Vertex(1);
                triangleCorners[2] = triangle.get_Vertex(2);

                var normal = GetNormal(triangleCorners);
                var sketchPlane = GetSketchPlane(
                    doc, triangleCorners[0], normal);

                DrawModelLineLoop(sketchPlane, triangleCorners);

                var tessellatedFace = new TessellatedFace(
                    triangleCorners, ElementId.InvalidElementId);

                if (builder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                {
                    builder.AddFace(tessellatedFace);
                }
            }

            var directShape = CreateDirectShapeInitialCommand.BuildDirectShape(
                doc, builder, applicationId, element.UniqueId);

            transaction.Commit();

            TaskDialog.Show(
                "DirectShape from Face",
                $"Created DirectShape element {directShape.Id.Value} "
                + $"from face on {element.Name}.\n"
                + $"Also drew model lines for each triangle loop.");

            return Result.Succeeded;
        }

        static bool IsAlmostZero(double value, double tolerance)
        {
            return tolerance > Math.Abs(value);
        }

        static bool IsAlmostZero(double value)
        {
            return IsAlmostZero(value, Epsilon);
        }

        static bool IsAlmostEqual(double a, double b)
        {
            return IsAlmostZero(b - a);
        }

        static XYZ GetNormal(XYZ v1, XYZ v2)
        {
            return v1.CrossProduct(v2).Normalize();
        }

        static XYZ GetNormal(XYZ[] triangleCorners)
        {
            return GetNormal(
                triangleCorners[1] - triangleCorners[0],
                triangleCorners[2] - triangleCorners[0]);
        }

        static double SignedDistanceTo(Plane plane, XYZ point)
        {
            Debug.Assert(
                IsAlmostEqual(plane.Normal.GetLength(), 1),
                "expected normalised plane normal");

            return plane.Normal.DotProduct(point - plane.Origin);
        }

        static bool SketchPlaneMatches(
            SketchPlane sketchPlane,
            XYZ origin,
            XYZ normal)
        {
            if (!sketchPlane.Name.Equals(SketchPlaneNameMarker))
            {
                return false;
            }

            var plane = sketchPlane.GetPlane();
            return plane.Normal.IsAlmostEqualTo(normal)
                && IsAlmostZero(SignedDistanceTo(plane, origin));
        }

        static SketchPlane GetSketchPlane(
            Document doc,
            XYZ origin,
            XYZ normal)
        {
            var sketchPlane = new FilteredElementCollector(doc)
                .OfClass(typeof(SketchPlane))
                .Cast<SketchPlane>()
                .FirstOrDefault(x => SketchPlaneMatches(x, origin, normal));

            if (sketchPlane == null)
            {
                var plane = Plane.CreateByNormalAndOrigin(normal, origin);
                sketchPlane = SketchPlane.Create(doc, plane);
                ++_sketchPlaneCreationCounter;
            }

            return sketchPlane;
        }

        static void DrawModelLineLoop(SketchPlane sketchPlane, XYZ[] corners)
        {
            var factory = sketchPlane.Document.Create;
            var count = corners.Length;

            for (var i = 0; i < count; i++)
            {
                var previous = i == 0 ? count - 1 : i - 1;
                factory.NewModelCurve(
                    Line.CreateBound(corners[previous], corners[i]),
                    sketchPlane);
            }
        }

        static bool GetTransformStackForObject(
            Stack<Transform> transformStack,
            GeometryElement geometry,
            Document doc,
            string stableRepresentation)
        {
            foreach (var geometryObject in geometry)
            {
                if (geometryObject is GeometryInstance geometryInstance)
                {
                    transformStack.Push(geometryInstance.Transform);

                    if (GetTransformStackForObject(
                            transformStack,
                            geometryInstance.GetSymbolGeometry(),
                            doc,
                            stableRepresentation))
                    {
                        return true;
                    }

                    transformStack.Pop();
                    continue;
                }

                if (geometryObject is not Solid solid)
                {
                    continue;
                }

                var isFace = stableRepresentation.EndsWith("SURFACE");
                var isEdge = stableRepresentation.EndsWith("LINEAR");

                if (isFace && solid.Faces.Size > 0)
                {
                    foreach (Face solidFace in solid.Faces)
                    {
                        var representation = solidFace.Reference
                            .ConvertToStableRepresentation(doc);

                        if (representation.Equals(stableRepresentation))
                        {
                            return true;
                        }
                    }
                }

                if (isEdge && solid.Edges.Size > 0)
                {
                    foreach (Edge edge in solid.Edges)
                    {
                        var representation = edge.Reference
                            .ConvertToStableRepresentation(doc);

                        if (representation.Equals(stableRepresentation))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
