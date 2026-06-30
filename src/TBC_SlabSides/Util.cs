#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SlabSides sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Determine the vertical boundary faces
        ///     of a given "horizontal" solid object
        ///     such as a floor slab. Currently only
        ///     supports planar and cylindrical faces.
        /// </summary>
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
