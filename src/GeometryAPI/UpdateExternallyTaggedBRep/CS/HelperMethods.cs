// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.UpdateExternallyTaggedBRep.CS
{
    internal static class HelperMethods
    {
        /// <summary>
        ///     Makes the main part of the CreateBRep external command actions.
        ///     See CreateBRep.Execute method summary for the details.
        /// </summary>
        /// <param name="document">A Document that will be used for Transaction and DirectShape creation.</param>
        public static Result ExecuteCreateBRepCommand(Document document)
        {
            // Create the ExternallyTaggedBRep named "Podium".
            var taggedBRep = CreateExternallyTaggedPodium(40.0, 12.0, 30.0);
            if (null == taggedBRep)
                return Result.Failed;

            using (var transaction = new Transaction(document, "CreateExternallyTaggedBRep"))
            {
                transaction.Start();

                // Create the new DirectShape for this open Document and add the created ExternallyTaggedBRep to this DirectShape.
                CreateBRep.CreatedDirectShape = CreateDirectShapeWithExternallyTaggedBRep(document, taggedBRep);
                if (null == CreateBRep.CreatedDirectShape)
                    return Result.Failed;

                // Retrieve the ExternallyTaggedBRep by its ExternalId from the DirectShape and check that the returned BRep is valid.
                if (!(CreateBRep.CreatedDirectShape.GetExternallyTaggedGeometry(taggedBRep.ExternalId) is ExternallyTaggedBRep retrievedBRep))
                    return Result.Failed;

                // Retrieve the Face by its ExternalGeometryId from the ExternallyTaggedBRep and check that the returned face is valid.
                // "faceRiser1" is a hardcoded ExternalGeometryId of the one Face in the "Podium" BRep, see Podium.cs file.
                var retrievedFace = retrievedBRep.GetTaggedGeometry(new ExternalGeometryId("faceRiser1")) as Face;
                if (null == retrievedFace)
                    return Result.Failed;

                // Retrieve the Edge by its ExternalGeometryId from the ExternallyTaggedBRep and check that the returned edge is valid.
                // "edgeLeftRiser1" is a hardcoded ExternalGeometryId of the one Edge in the "Podium" BRep, see Podium.cs file.
                var retrievedEdge = retrievedBRep.GetTaggedGeometry(new ExternalGeometryId("edgeLeftRiser1")) as Edge;
                if (null == retrievedEdge)
                    return Result.Failed;

                transaction.Commit();

                return Result.Succeeded;
            }
        }

        /// <summary>
        ///     Creates stairs BRep as ExternallyTaggedBRep.
        /// </summary>
        /// <param name="width">The width of the stairs BRep.</param>
        /// <param name="height">The height of the stairs BRep.</param>
        /// <param name="depth">The depth of the stairs BRep.</param>
        public static ExternallyTaggedBRep CreateExternallyTaggedPodium(double width, double height, double depth)
        {
            var podium = new Podium(width, height, depth);
            return podium.CreateStairs();
        }

        /// <summary>
        ///     Creates the new DirectShape and adds the ExternallyTaggedBRep to it.
        /// </summary>
        /// <param name="document">A Document that will be used for the DirectShape creation.</param>
        /// <param name="taggedBRep">An ExternallyTaggedBRep that will be added to the created DirectShape.</param>
        public static DirectShape CreateDirectShapeWithExternallyTaggedBRep(Document document,
            ExternallyTaggedBRep taggedBRep)
        {
            var directShape = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_Stairs));
            if (null == directShape)
                return null;
            directShape.ApplicationId = "TestCreateExternallyTaggedBRep";
            directShape.ApplicationDataId = "ExternallyTaggedBRep";

            directShape.AddExternallyTaggedGeometry(taggedBRep);
            return directShape;
        }
    }
}
