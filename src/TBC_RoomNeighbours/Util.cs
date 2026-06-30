#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_RoomNeighbours sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return the neighbouring room to the given one
        ///     on the other side of the midpoint of the given
        ///     boundary segment.
        /// </summary>
        public static Room GetRoomNeighbourAt(
            BoundarySegment bs,
            Room r)
        {
            var doc = r.Document;

            var w = doc.GetElement(bs.ElementId) as Wall;

            var wallThickness = w.Width;

            var derivatives = bs.GetCurve()
                .ComputeDerivatives(0.5, true);

            var midPoint = derivatives.Origin;

            Debug.Assert(
                midPoint.IsAlmostEqualTo(
                    bs.GetCurve().Evaluate(0.5, true)),
                "expected same result from Evaluate and derivatives");

            var tangent = derivatives.BasisX.Normalize();

            var normal = new XYZ(tangent.Y,
                tangent.X * -1, tangent.Z);

            var p = midPoint + wallThickness * normal;

            var otherRoom = doc.GetRoomAtPoint(p);

            if (null != otherRoom)
                if (otherRoom.Id == r.Id)
                {
                    normal = new XYZ(tangent.Y * -1,
                        tangent.X, tangent.Z);

                    p = midPoint + wallThickness * normal;

                    otherRoom = doc.GetRoomAtPoint(p);

                    Debug.Assert(null == otherRoom
                                 || otherRoom.Id != r.Id,
                        "expected different room on other side");
                }

            return otherRoom;
        }
    }
}
