// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from DirectShapeFromFace by Frode Tørresdal and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/DirectShapeFromFace

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.DirectShapeFromFace.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateDirectShapeSimpleCommand : IExternalCommand
    {
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

            if (element is FamilyInstance familyInstance)
            {
                mesh = mesh.get_Transformed(familyInstance.GetTotalTransform());
            }

            using var transaction = new Transaction(doc, "Create DirectShape from Face");
            transaction.Start();

            var builder = new TessellatedShapeBuilder();
            builder.OpenConnectedFaceSet(false);

            var triangleCorners = new XYZ[3];
            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                triangleCorners[0] = triangle.get_Vertex(0);
                triangleCorners[1] = triangle.get_Vertex(1);
                triangleCorners[2] = triangle.get_Vertex(2);

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
                + $"from face on {element.Name}.");

            return Result.Succeeded;
        }
    }
}
