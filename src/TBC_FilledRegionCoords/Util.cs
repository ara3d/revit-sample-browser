using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static List<XYZ> GetBoundaryCorners(FilledRegion region)
        {
            var result = new List<XYZ>();

            var id = new ElementId(region.Id.Value - 1);

            if (region.Document.GetElement(id) is Sketch sketch)
            {
                var curves = sketch.Profile.get_Item(0);

                if (null != curves)
                    foreach (Curve cur in curves)
                    {
                        var corner = cur.GetEndPoint(0);
                        result.Add(corner);
                    }
            }

            return result;
        }

        internal static void EditFilledRegion(Document doc)
        {
            var fillRegionIds
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(FilledRegion))
                    .ToElementIds();

            using var tx = new Transaction(doc);
            tx.Start("Move all Filled Regions");

            var v = XYZ.BasisX;
            ElementTransformUtils.MoveElements(doc, fillRegionIds, v);
            ElementTransformUtils.MoveElements(doc, fillRegionIds, -v);

            tx.Commit();
        }
    }
}
