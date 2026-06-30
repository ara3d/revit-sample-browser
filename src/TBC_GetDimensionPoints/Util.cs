using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_GetDimensionPoints sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return dimension origin, i.e., the midpoint
        ///     of the dimension or of its first segment.
        /// </summary>
        internal static XYZ GetDimensionStartPoint(Dimension dim)
        {
            XYZ p = null;

            try
            {
                p = dim.Origin;
            }
            catch (ApplicationException ex)
            {
                Debug.Assert(ex.Message.Equals(
                    "Cannot access this method if this dimension has more than one segment."));

                foreach (DimensionSegment seg in dim.Segments)
                {
                    p = seg.Origin;
                    break;
                }
            }

            return p;
        }

        /// <summary>
        ///     Retrieve start and end points of each dimension segment.
        /// </summary>
        internal static List<XYZ> GetDimensionPoints(
            Dimension dim,
            XYZ pStart)
        {
            var dimLine = dim.Curve as Line;
            if (dimLine == null) return null;
            var pts = new List<XYZ>();

            dimLine.MakeBound(0, 1);
            var pt1 = dimLine.GetEndPoint(0);
            var pt2 = dimLine.GetEndPoint(1);
            var direction = pt2.Subtract(pt1).Normalize();

            if (0 == dim.Segments.Size)
            {
                var v = 0.5 * (double) dim.Value * direction;
                pts.Add(pStart - v);
                pts.Add(pStart + v);
            }
            else
            {
                var p = pStart;
                foreach (DimensionSegment seg in dim.Segments)
                {
                    var v = (double) seg.Value * direction;
                    if (0 == pts.Count) pts.Add(p = pStart - 0.5 * v);
                    pts.Add(p = p.Add(v));
                }
            }

            return pts;
        }

        /// <summary>
        ///     Draw an X marker at the given position using model lines.
        /// </summary>
        internal static void DrawDimensionMarker(
            XYZ p,
            double size,
            SketchPlane sketchPlane)
        {
            size *= 0.5;
            var v = new XYZ(size, size, 0);
            var doc = sketchPlane.Document;
            doc.Create.NewModelCurve(Line.CreateBound(
                p - v, p + v), sketchPlane);
            v = new XYZ(size, -size, 0);
            doc.Create.NewModelCurve(Line.CreateBound(
                p - v, p + v), sketchPlane);
        }
    }
}
