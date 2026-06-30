#region Namespaces

using Autodesk.Revit.DB;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        // Only planar and cylindrical faces are supported.
        public static void GetSideFaces(
            List<Face> verticalFaces,
            Solid solid)
        {
            var faces = solid.Faces;
            foreach (Face f in faces)
                switch (f)
                {
                    case PlanarFace face:
                        {
                            if (IsVertical(face))
                                verticalFaces.Add(face);
                            break;
                        }
                    case CylindricalFace cylindricalFace:
                        {
                            if (IsVertical(cylindricalFace))
                                verticalFaces.Add(cylindricalFace);
                            break;
                        }
                }
        }
    }
}
