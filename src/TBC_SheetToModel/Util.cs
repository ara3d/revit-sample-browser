#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SheetToModel sample.</summary>
    internal static partial class Util
    {
        public static void QTO_2_PlaceHoldersFromDWFMarkups(
            Document doc,
            string activityId)
        {
            var activeView = doc.ActiveView;

            if (!(activeView is ViewSheet vs))
            {
                TaskDialog.Show("QTO",
                    "The current view must be a Sheet View with DWF markups");
                return;
            }

            var vp = doc.GetElement(
                vs.GetAllViewports().First()) as Viewport;

            var plan = doc.GetElement(vp.ViewId) as View;

            var scale = vp.Parameters.Cast<Parameter>()
                .First(x => x.Id.Value.Equals(
                    (int) BuiltInParameter.VIEW_SCALE))
                .AsInteger();

            var dwfMarkups
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ImportInstance))
                    .WhereElementIsNotElementType()
                    .Where(x => x.Name.StartsWith("Markup")
                                && x.OwnerViewId.Value.Equals(
                                    activeView.Id.Value));

            using var tg = new TransactionGroup(doc);
            tg.Start("DWF markups placeholders");

            using (var t = new Transaction(doc))
            {
                t.Start("DWF Transfer");

                plan.Parameters.Cast<Parameter>()
                    .First(x => x.Id.Value.Equals(
                        (int) BuiltInParameter.VIEWER_CROP_REGION))
                    .Set(1);

                var vc = (plan.CropBox.Min + plan.CropBox.Max) / 2;

                var bc = vp.GetBoxCenter();

                t.RollBack();

                foreach (var e in dwfMarkups)
                {
                    var geoElem = e.get_Geometry(new Options());

                    var gi = geoElem.Cast<GeometryInstance>().First();

                    var gei = gi.GetSymbolGeometry();

                    if (gei.Count(x => x is Arc) > 0) continue;

                    foreach (var go in gei)
                    {
                        var med = new XYZ();

                        if (go is PolyLine pl)
                        {
                            var min = new XYZ(pl.GetCoordinates().Min(p => p.X),
                                pl.GetCoordinates().Min(p => p.Y),
                                pl.GetCoordinates().Min(p => p.Z));

                            var max = new XYZ(pl.GetCoordinates().Max(p => p.X),
                                pl.GetCoordinates().Max(p => p.Y),
                                pl.GetCoordinates().Max(p => p.Z));

                            med = (min + max) / 2;
                        }

                        med = med - bc;

                        var a = vc + new XYZ(med.X * scale, med.Y * scale, 0);
                    }
                }

                t.Start("DWF Transfer");

                foreach (var e in dwfMarkups)
                {
                    var geoElem = e.get_Geometry(new Options());

                    var gi = geoElem.Cast<GeometryInstance>().First();

                    var gei = gi.GetSymbolGeometry();

                    if (gei.Count(x => x is Arc) == 0) continue;

                    foreach (var go in gei)
                        if (go is Arc)
                        {
                            var c = go as Curve;

                            var med = c.Evaluate(0.5, true);

                            med = med - bc;

                            var a = vc + new XYZ(med.X * scale, med.Y * scale, 0);

                            var textTypeId = new FilteredElementCollector(doc)
                                .OfClass(typeof(TextNoteType))
                                .FirstElementId();

                            TextNote.Create(doc, plan.Id, a, activityId, textTypeId);
                        }

                    t.Commit();
                }
            }

            tg.Assimilate();
        }
    }
}
