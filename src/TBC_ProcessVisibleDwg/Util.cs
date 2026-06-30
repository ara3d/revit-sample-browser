#region Namespaces

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ProcessVisibleDwg sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Pick a DWG import instance, extract polylines
        ///     from it visible in the current view and create
        ///     filled regions from them.
        /// </summary>
        public static void ProcessVisible(UIDocument uidoc)
        {
            var doc = uidoc.Document;
            var active_view = doc.ActiveView;

            var visible_dwg_geo
                = new List<GeometryObject>();

            var r = uidoc.Selection.PickObject(
                ObjectType.Element,
                new JtElementsOfClassSelectionFilter<ImportInstance>());

            var import = doc.GetElement(r) as ImportInstance;

            var ge = import.get_Geometry(new Options());

            foreach (var go in ge)
                if (go is GeometryInstance gi)
                {
                    var ge2 = gi.GetInstanceGeometry();

                    if (ge2 != null)
                        foreach (var obj in ge2)
                            if (obj is PolyLine)
                            {
                                var gStyle = doc.GetElement(
                                    obj.GraphicsStyleId) as GraphicsStyle;

                                if (!active_view.GetCategoryHidden(
                                    gStyle.GraphicsStyleCategory.Id))
                                    visible_dwg_geo.Add(obj);
                            }
                }

            if (visible_dwg_geo.Count > 0)
            {
                var filledType = new FilteredElementCollector(doc)
                    .WhereElementIsElementType()
                    .OfClass(typeof(FilledRegionType))
                    .OfType<FilledRegionType>()
                    .First();

                using var t = new Transaction(doc);
                t.Start("ProcessDWG");

                foreach (var obj in visible_dwg_geo)
                {
                    var poly = obj as PolyLine;

                    if (null != poly)
                    {
                        var curveLoop = new CurveLoop();

                        var points = poly.GetCoordinates();

                        for (var i = 0; i < points.Count - 1; ++i)
                            curveLoop.Append(Line.CreateBound(
                                points[i], points[i + 1]));

                        FilledRegion.Create(doc,
                            filledType.Id, active_view.Id,
                            new List<CurveLoop> {curveLoop});
                    }
                }

                t.Commit();
            }
        }
    }
}
