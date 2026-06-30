// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal static class ShapeCreator
    {
        public static DirectShape CreateDirectShape(
            Document doc,
            Solid transientSolid,
            string dsName)
        {
            var catId = new ElementId(BuiltInCategory.OST_GenericModel);
            var addInId = doc.Application.ActiveAddInId;

            var ds = DirectShape.CreateElement(doc, catId);
            ds.ApplicationId = addInId.GetGUID().ToString();
            ds.ApplicationDataId = string.Empty;

            if (ds.IsValidGeometry(transientSolid))
            {
                ds.SetShape(new GeometryObject[] { transientSolid });
            }
            else
            {
                var result = GetTessellatedSolid(doc, transientSolid);
                ds.SetShape(result.GetGeometricalObjects());
            }

            ds.Name = dsName;
            return ds;
        }

        static TessellatedShapeBuilderResult GetTessellatedSolid(
            Document doc,
            Solid transientSolid)
        {
            var builder = new TessellatedShapeBuilder();

            var idMaterial = new FilteredElementCollector(doc)
                .OfClass(typeof(Material))
                .FirstElementId();

            var idGraphicsStyle = new FilteredElementCollector(doc)
                .OfClass(typeof(GraphicsStyle))
                .FirstOrDefault(gs => gs.Name.Equals("Walls"))
                .Id;

            builder.OpenConnectedFaceSet(true);

            var faceArray = transientSolid.Faces;

            foreach (Face face in faceArray)
            {
                var triFace = new List<XYZ>(3);
                var mesh = face.Triangulate();
                var triCount = mesh.NumTriangles;

                for (var i = 0; i < triCount; i++)
                {
                    triFace.Clear();

                    for (var n = 0; n < 3; n++)
                    {
                        triFace.Add(mesh.get_Triangle(i).get_Vertex(n));
                    }

                    builder.AddFace(new TessellatedFace(triFace, idMaterial));
                }
            }

            builder.CloseConnectedFaceSet();
            builder.Fallback = TessellatedShapeBuilderFallback.Abort;
            builder.Target = TessellatedShapeBuilderTarget.Solid;
            builder.GraphicsStyleId = idGraphicsStyle;
            builder.Build();

            return builder.GetBuildResult();
        }
    }
}
