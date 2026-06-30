#region Namespaces

using Autodesk.Revit.DB;
using System;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewDimensionLabel sample.</summary>
    internal static partial class Util
    {
        internal static SketchPlane FindSketchPlane(
            Document doc,
            XYZ normal)
        {
            FilteredElementCollector collector
                = new(doc);

            collector.OfClass(typeof(SketchPlane));

            Func<SketchPlane, bool> normalEquals = e => e.GetPlane().Normal.IsAlmostEqualTo(normal);

            return collector.Cast<SketchPlane>().First(normalEquals);
        }
    }
}
