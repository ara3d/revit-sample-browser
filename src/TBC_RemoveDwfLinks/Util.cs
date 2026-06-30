#region Namespaces

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static int Unpin(List<ElementId> ids, Document doc)
        {
            var count = 0;

            foreach (var id in ids)
            {
                var e = doc.GetElement(id);
                if (e.Pinned)
                {
                    e.Pinned = false;
                    ++count;
                }
            }

            return count;
        }

        public static bool ElementCategoryContainsDwf(Element e)
        {
            return null != e.Category
                   && e.Category.Name.ToLower()
                       .Contains(".dwf");
        }

        // Non-functional; use RemoveDwfLinkUsingExternalFileUtils instead.
        public static int RemoveDwfLinkUsingDelete(Document doc)
        {
            var nDeleted = 0;

            var col
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType();

            var ids = new List<ElementId>();

            var pinned = 0;

            foreach (var e in col)
                if (ElementCategoryContainsDwf(e))
                {
                    Debug.Print(ElementDescription(e));
                    pinned += e.Pinned ? 1 : 0;
                    ids.Add(e.Id);
                }

            ICollection<ElementId> idsDeleted = null;
            Transaction t;

            var n = ids.Count;
            var unpinned = 0;

            if (0 < n)
            {
                if (0 < pinned)
                    using (t = new Transaction(doc))
                    {
                        t.Start(
                            "Unpin non-ElementType '.dwf' elements");

                        unpinned = Unpin(ids, doc);

                        t.Commit();
                    }

                using (t = new Transaction(doc))
                {
                    t.Start(
                        "Delete non-ElementType '.dwf' elements");

                    idsDeleted = doc.Delete(ids);

                    t.Commit();
                }
            }

            var m = null == idsDeleted
                ? 0
                : idsDeleted.Count;

            Debug.Print("Selected {0} non-ElementType element{1}, "
                        + "{2} pinned, {3} unpinned, "
                        + "{4} successfully deleted.", n, PluralSuffix(n), pinned, unpinned, m);

            nDeleted += m;

            col = new FilteredElementCollector(doc)
                .WhereElementIsElementType();

            ids.Clear();
            pinned = 0;

            foreach (var e in col)
                if (ElementCategoryContainsDwf(e))
                {
                    Debug.Print(ElementDescription(e));
                    pinned += e.Pinned ? 1 : 0;
                    ids.Add(e.Id);
                }

            n = ids.Count;

            if (0 < n)
            {
                if (0 < pinned)
                    using (t = new Transaction(doc))
                    {
                        t.Start(
                            "Unpin element type '.dwf' elements");

                        unpinned = Unpin(ids, doc);

                        t.Commit();
                    }

                using (t = new Transaction(doc))
                {
                    t.Start("Delete element type '.dwf' elements");

                    idsDeleted = doc.Delete(ids);

                    t.Commit();
                }
            }

            m = null == idsDeleted ? 0 : idsDeleted.Count;

            Debug.Print("Selected {0} element type{1}, "
                        + "{2} pinned, {3} unpinned, "
                        + "{4} successfully deleted.", n, PluralSuffix(n), pinned, unpinned, m);

            nDeleted += m;

            return nDeleted;
        }

        public static int RemoveDwfLinkUsingExternalFileUtils(
            Document doc)
        {
            var idsToDelete
                = new List<ElementId>();

            var ids = ExternalFileUtils
                .GetAllExternalFileReferences(doc);

            foreach (var id in ids)
            {
                var e = doc.GetElement(id);

                Debug.Print(ElementDescription(e));

                var xr = ExternalFileUtils
                    .GetExternalFileReference(doc, id);

                var xrType
                    = xr.ExternalFileReferenceType;

                if (xrType == ExternalFileReferenceType.DWFMarkup)
                {
                    var xrPath = xr.GetPath();

                    var path = ModelPathUtils
                        .ConvertModelPathToUserVisiblePath(xrPath);

                    if (path.EndsWith(".dwf")
                        || path.EndsWith(".dwfx"))
                        idsToDelete.Add(id);
                }
            }

            var n = idsToDelete.Count;

            ICollection<ElementId> idsDeleted = null;

            if (0 < n)
            {
                using var t = new Transaction(doc);
                t.Start("Delete DWFx Links");

                idsDeleted = doc.Delete(idsToDelete);

                t.Commit();
            }

            var m = null == idsDeleted
                ? 0
                : idsDeleted.Count;

            Debug.Print("Selected {0} DWF external file reference{1}, "
                        + "{2} element{3} successfully deleted.", n, PluralSuffix(n), m, PluralSuffix(m));

            return m;
        }
    }
}
