using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static List<PlanarFace> GetWallOpeningPlanarFaces(
            Wall wall,
            ElementId openingId)
        {
            var faceList = new List<PlanarFace>();

            var solidList = new List<Solid>();

            var geomOptions = wall.Document.Application.Create.NewGeometryOptions();

            if (geomOptions != null)
            {
                var geoElem = wall.get_Geometry(geomOptions);

                if (geoElem != null)
                    foreach (var geomObj in geoElem)
                        if (geomObj is Solid obj)
                            solidList.Add(obj);
            }

            foreach (var solid in solidList)
            foreach (Face face in solid.Faces)
                if (face is PlanarFace planarFace)
                    if (wall.GetGeneratingElementIds(face)
                        .Any(x => x == openingId))
                        faceList.Add(planarFace);
            return faceList;
        }
    }
}
