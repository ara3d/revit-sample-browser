// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from DirectShapeFromFace by Frode Tørresdal and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/DirectShapeFromFace

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.DirectShapeFromFace.CS
{
    /// <summary>
    ///     Creates a DirectShape from a picked face using the initial
    ///     tessellated shape builder approach with a LocationPoint offset.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateDirectShapeInitialCommand : IExternalCommand
    {
        static readonly ElementId CategoryForDirectShape
            = new(BuiltInCategory.OST_GenericModel);

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var applicationId = commandData.Application.ActiveAddInId
                .GetGUID().ToString();

            Reference reference;
            try
            {
                reference = uidoc.Selection.PickObject(ObjectType.Face);
            }
            catch (OperationCanceledException)
            {
                message = "Face selection was cancelled.";
                return Result.Cancelled;
            }

            var element = doc.GetElement(reference.ElementId);
            var face = element.GetGeometryObjectFromReference(reference) as Face;
            if (face == null)
            {
                message = "The selected reference is not a face.";
                return Result.Failed;
            }

            var mesh = face.Triangulate();
            if (mesh == null || mesh.NumTriangles == 0)
            {
                message = "The selected face could not be triangulated.";
                return Result.Failed;
            }

            var offset = XYZ.Zero;
            if (element.Location is LocationPoint locationPoint)
            {
                offset = locationPoint.Point;
            }

            using var transaction = new Transaction(doc, "Create DirectShape from Face");
            transaction.Start();

            var builder = new TessellatedShapeBuilder();
            builder.OpenConnectedFaceSet(false);

            var triangleCorners = new List<XYZ>(3);
            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                triangleCorners.Clear();
                triangleCorners.Add(triangle.get_Vertex(0).Add(offset));
                triangleCorners.Add(triangle.get_Vertex(1).Add(offset));
                triangleCorners.Add(triangle.get_Vertex(2).Add(offset));

                var tessellatedFace = new TessellatedFace(
                    triangleCorners, ElementId.InvalidElementId);

                if (builder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace))
                {
                    builder.AddFace(tessellatedFace);
                }
            }

            var directShape = BuildDirectShape(
                doc, builder, applicationId, element.UniqueId);

            transaction.Commit();

            TaskDialog.Show(
                "DirectShape from Face",
                $"Created DirectShape element {directShape.Id.Value} "
                + $"from face on {element.Name}.");

            return Result.Succeeded;
        }

        internal static DirectShape BuildDirectShape(
            Document doc,
            TessellatedShapeBuilder builder,
            string applicationId,
            string applicationDataId)
        {
            builder.CloseConnectedFaceSet();
            builder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
            builder.Fallback = TessellatedShapeBuilderFallback.Mesh;
            builder.Build();

            var result = builder.GetBuildResult();
            var directShape = DirectShape.CreateElement(
                doc, CategoryForDirectShape);
            directShape.ApplicationId = applicationId;
            directShape.ApplicationDataId = applicationDataId;
            directShape.SetShape(result.GetGeometricalObjects());
            directShape.Name = "DirectShape from face";
            return directShape;
        }
    }
}
