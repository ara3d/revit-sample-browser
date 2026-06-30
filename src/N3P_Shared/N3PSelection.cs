using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Shared.CS
{
    internal static class N3PSelection
    {
        public static IList<Element> GetSelectedOrAll(UIDocument uidoc, System.Type elementType, int maxCount = 5)
        {
            var doc = uidoc.Document;
            var ids = uidoc.Selection.GetElementIds();

            return ids.Count > 0
                ? (IList<Element>)ids
                    .Select(id => doc.GetElement(id))
                    .Where(e => e != null && elementType.IsInstanceOfType(e))
                    .Take(maxCount)
                    .ToList()
                : doc.CollectElements()
                .OfClass(elementType)
                .Take(maxCount)
                .ToList();
        }
    }
}
