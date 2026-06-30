#region Namespaces

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static Dictionary<ElementId, List<ElementId>>
            GetHostedElementIds(
                Document doc,
                FilteredElementCollector elements)
        {
            Dictionary<ElementId, List<ElementId>> dict =
                [];

            var fmt = "{0} is hosted by {1}";

            foreach (FamilyInstance fi in elements)
            {
                var id = fi.Id;
                var idHost = fi.Host.Id;

                Debug.Print(fmt,
                    ElementDescription(fi),
                    ElementDescription(doc.GetElement(idHost)));

                if (!dict.ContainsKey(idHost)) dict.Add(idHost, []);
                dict[idHost].Add(id);
            }

            return dict;
        }

        public static void DumpHostedElements(
            Document doc,
            Dictionary<ElementId, List<ElementId>> ids)
        {
            foreach (var idHost in ids.Keys)
            {
                var s = string.Empty;

                foreach (var id in ids[idHost])
                {
                    if (0 < s.Length) s += ", ";
                    s += ElementDescription(doc.GetElement(id));
                }

                var n = ids[idHost].Count;

                Debug.Print(
                    "{0} hosts {1} opening{2}: {3}",
                    ElementDescription(doc.GetElement(idHost)),
                    n, PluralSuffix(n), s);
            }
        }
    }
}
