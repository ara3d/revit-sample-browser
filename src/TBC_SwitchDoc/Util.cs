using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SwitchDoc sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Zoom to the given elements, switching view if needed.
        /// </summary>
        internal static Result ZoomToElements(
            UIDocument uidoc,
            ICollection<ElementId> ids,
            ref string message,
            ElementSet elements)
        {
            var n = ids.Count;

            if (0 == n)
            {
                message = "Please select at least one element to zoom to.";
                return Result.Cancelled;
            }

            try
            {
                uidoc.ShowElements(ids);
            }
            catch
            {
                var doc = uidoc.Document;

                foreach (var id in ids)
                {
                    var e = doc.GetElement(id);
                    elements.Insert(e);
                }

                message = $"Cannot zoom to element{(1 == n ? "" : "s")}.";

                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Toggle back and forth between two different documents.
        /// </summary>
        internal static void ToggleViews(
            View view1,
            string filepath2)
        {
            var doc = view1.Document;
            var uidoc = new UIDocument(doc);
            var app = doc.Application;
            var uiapp = new UIApplication(app);

            var idsView1
                = new FilteredElementCollector(doc, view1.Id)
                    .WhereElementIsNotElementType()
                    .ToElementIds();

            var uidoc2 = uiapp
                .OpenAndActivateDocument(filepath2);

            var doc2 = uidoc2.Document;

            using (var tx = new Transaction(doc2))
            {
                tx.Start("Change Scale");
                doc2.ActiveView.get_Parameter(
                        BuiltInParameter.VIEW_SCALE_PULLDOWN_METRIC)
                    .Set(20);
                tx.Commit();
            }

            var opt = new SaveAsOptions
            {
                OverwriteExistingFile = true
            };

            doc2.SaveAs(filepath2, opt);

            if (!string.IsNullOrEmpty(doc.PathName))
            {
                uiapp.OpenAndActivateDocument(
                    doc.PathName);

                doc2.Close(false);
            }
            else
            {
                uidoc.ShowElements(idsView1);
                uidoc.RefreshActiveView();
            }
        }

        /// <summary>
        ///     Zoom active view to element in linked document.
        /// </summary>
        internal static void ZoomToLinkedElement(
            UIDocument uidoc,
            RevitLinkInstance link,
            ElementId id)
        {
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            var uiView = uidoc
                .GetOpenUIViews()
                .FirstOrDefault(uv
                    => uv.ViewId.Equals(view.Id));

            var e = doc.GetElement(id);
            var lp = e.Location as LocationPoint;
            var transform1 = link.GetTransform();
            var newLocation2 = transform1.OfPoint(lp.Point);

            uiView.ZoomAndCenterRectangle(
                new XYZ(newLocation2.X - 4, newLocation2.Y - 4, newLocation2.Z - 4),
                new XYZ(newLocation2.X + 4, newLocation2.Y + 4, newLocation2.Z + 4));
        }
    }
}
