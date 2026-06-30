using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_LinkedFiles sample.</summary>
    internal static partial class Util
    {
        public static FilteredElementCollector GetLinkedFiles(
            Document doc)
        {
            return GetElementsOfType(doc,
                typeof(Instance),
                BuiltInCategory.OST_RvtLinks);
        }

        public static Dictionary<string, string> GetFilePaths(
            Application app,
            bool onlyImportedFiles)
        {
            var docs = app.Documents;
            var n = docs.Size;

            var dict
                = new Dictionary<string, string>(n);

            foreach (Document doc in docs)
                if (!onlyImportedFiles
                    || null == doc.ActiveView)
                {
                    var path = doc.PathName;
                    var i = path.LastIndexOf("\\") + 1;
                    var name = path.Substring(i);
                    dict.Add(name, path);
                }

            return dict;
        }
    }
}
