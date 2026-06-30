#region Namespaces

using System.Linq;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewSpotElevation sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return a view with the given name in the document.
        /// </summary>
        internal static View FindView(Document doc, string name)
        {
            return GetFirstElementOfTypeNamed(
                doc, typeof(View), name) as View;
        }

        /// <summary>
        ///     Return a reference to the topmost face of the given element.
        /// </summary>
        internal static Reference FindTopMostReference(Element e)
        {
            Reference ret = null;
            var doc = e.Document;

            var opt = doc.Application.Create
                .NewGeometryOptions();

            opt.ComputeReferences = true;

            var geo = e.get_Geometry(opt);

            foreach (var obj in geo)
            {
                var inst = obj as GeometryInstance;

                if (null != inst)
                {
                    geo = inst.GetSymbolGeometry();
                    break;
                }
            }

            var solid = geo.OfType<Solid>()
                .First(sol => null != sol);

            var z = double.MinValue;

            foreach (Face f in solid.Faces)
            {
                var b = f.GetBoundingBox();
                var p = b.Min;
                var q = b.Max;
                var midparam = p + 0.5 * (q - p);
                var midpoint = f.Evaluate(midparam);
                var normal = f.ComputeNormal(midparam);

                if (PointsUpwards(normal))
                    if (midpoint.Z > z)
                    {
                        z = midpoint.Z;
                        ret = f.Reference;
                    }
            }

            return ret;
        }
    }
}
