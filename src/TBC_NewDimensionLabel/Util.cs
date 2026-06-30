#region Namespaces

using System;
using System.Linq;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewDimensionLabel sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return a sketch plane from the given document with
        ///     the specified normal vector, if one exists, else null.
        /// </summary>
        internal static SketchPlane FindSketchPlane(
            Document doc,
            XYZ normal)
        {
            var collector
                = new FilteredElementCollector(doc);

            collector.OfClass(typeof(SketchPlane));

            Func<SketchPlane, bool> normalEquals = e => e.GetPlane().Normal.IsAlmostEqualTo(normal);

            return collector.Cast<SketchPlane>().First(normalEquals);
        }
    }
}
