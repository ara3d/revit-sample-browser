using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static Dictionary<string, Family> GetFamilies(Document doc)
        {
            var families = new Dictionary<string, Family>();

            var instances = new FilteredElementCollector(doc);
            instances.OfClass(typeof(FamilyInstance));

            foreach (FamilyInstance i in instances)
            {
                var family = i.Symbol.Family;
                if (!families.ContainsKey(family.Name))
                    families[family.Name] = family;
            }

            var annotations = new FilteredElementCollector(doc);
            annotations.OfClass(typeof(AnnotationSymbol));

            foreach (AnnotationSymbol a in annotations)
            {
                var family = a.Symbol.Family;

                if (!families.ContainsKey(family.Name))
                    families[family.Name] = family;
            }

            return families;
        }

        internal static void ListImportsAndSearchForMore(
            int recursionLevel,
            Document doc,
            Dictionary<string, Family> families)
        {
            var indent = new string(' ', 2 * recursionLevel);

            var keys = new List<string>(families.Keys);
            keys.Sort();

            foreach (var key in keys)
            {
                var family = families[key];

                if (family.IsInPlace)
                {
                    Debug.Print(indent + "Family '{0}' is in-place.", key);
                }
                else
                {
                    var fdoc = doc.EditFamily(family);

                    var c = new FilteredElementCollector(doc);
                    c.OfClass(typeof(ImportInstance));

                    var imports = c.ToElements();
                    var n = imports.Count;

                    Debug.Print(indent
                                + "Family '{0}' contains {1} import instance{2}{3}",
                        key, n, PluralSuffix(n), DotOrColon(n));

                    if (0 < n)
                        foreach (ImportInstance i in imports)
                        {
                            var s = i.Pinned ? "" : "not ";
                            var name = doc.GetElement(i.GetTypeId()).Name;

                            Debug.Print(indent
                                        + "  '{0}' {1}pinned",
                                name, s);

                            i.Pinned = !i.Pinned;
                        }

                    var nestedFamilies = GetFamilies(fdoc);

                    ListImportsAndSearchForMore(
                        recursionLevel + 1, fdoc, nestedFamilies);
                }
            }
        }
    }
}
