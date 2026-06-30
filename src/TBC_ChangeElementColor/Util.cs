using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static void ChangeElementColor(Document doc, ElementId id)
        {
            Color color = new(
                200, 100, 100);

            OverrideGraphicSettings ogs = new();
            ogs.SetProjectionLineColor(color);

            using Transaction tx = new(doc);
            tx.Start("Change Element Color");
            doc.ActiveView.SetElementOverrides(id, ogs);
            tx.Commit();
        }

        internal static void ChangeElementMaterial(Document doc, ElementId id)
        {
            var e = doc.GetElement(id);

            if (null != e.Category)
            {
                var im = e.Category.Material.Id.Value;

                List<Material> materials = [.. new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .OfClass(typeof(Material))
                        .ToElements()
                        .Where(m
                            => m.Id.Value != im)
                        .Cast<Material>()];

                Random r = new();
                var i = r.Next(materials.Count);

                using Transaction tx = new(doc);
                tx.Start("Change Element Material");
                e.Category.Material = materials[i];
                tx.Commit();
            }
        }

        internal static void PaintStairs(UIDocument uidoc, Material mat)
        {
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            var pickedRef = sel.PickObject(
                ObjectType.PointOnElement,
                "Please select a Face");

            var elem = doc.GetElement(pickedRef);

            var geoObject = elem
                .GetGeometryObjectFromReference(pickedRef);

            var fc = geoObject as Face;

            if (elem.Category.Id.Value == -2000120)
            {
                var flag = false;
                var str = elem as Stairs;
                var landings = str.GetStairsLandings();
                var runs = str.GetStairsLandings();
                using Transaction transaction = new(doc);
                transaction.Start("Paint Material");
                foreach (var id in landings)
                {
                    doc.Paint(id, fc, mat.Id);
                    flag = true;
                    break;
                }

                if (!flag)
                    foreach (var id in runs)
                    {
                        doc.Paint(id, fc, mat.Id);
                        break;
                    }

                transaction.Commit();
            }
        }

        internal static void PaintSelectedFace(UIDocument uidoc, Material mat)
        {
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var pickedRef = sel.PickObject(
                ObjectType.PointOnElement,
                "Please select a face to paint");

            var elem = doc.GetElement(pickedRef);

            var geoObject = elem
                .GetGeometryObjectFromReference(pickedRef);

            var selected_face = geoObject as Face;

            using Transaction transaction = new(doc);
            transaction.Start("Paint Selected Face");

            if (elem.Category.Id.Value.Equals(
                (int)BuiltInCategory.OST_Stairs))
            {
                var str = elem as Stairs;
                var isLand = false;

                var landings = str.GetStairsLandings();
                var runs = str.GetStairsRuns();

                foreach (var id in landings)
                {
                    var land = doc.GetElement(id);
                    var solids = Util.GetElemSolids(
                        land.get_Geometry(new Options()));

                    isLand = SolidsContainFace(solids, selected_face);

                    if (isLand) break;
                }

                if (isLand)
                    foreach (var id in landings)
                    {
                        doc.Paint(id, selected_face, mat.Id);
                        break;
                    }
                else
                    foreach (var id in runs)
                    {
                        doc.Paint(id, selected_face, mat.Id);
                        break;
                    }
            }
            else
            {
                try
                {
                    doc.Paint(elem.Id, selected_face, mat.Id);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error painting selected face",
                        ex.Message);
                }
            }

            transaction.Commit();
        }

        internal static bool SolidsContainFace(List<Solid> solids, Face face)
        {
            foreach (var s in solids)
                if (null != s
                    && 0 < s.Volume)
                    foreach (Face f in s.Faces)
                        if (f == face)
                            return true;
                        else if (f.HasRegions)
                            foreach (var f2 in f.GetRegions())
                                if (f2 == face)
                                    return true;
            return false;
        }

    }
}
