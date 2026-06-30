using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static void ListWallGeom(Wall w, Application app)
        {
            var s = "";

            var options = app.Create.NewGeometryOptions();

            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;

            var geomElem = w.get_Geometry(options);

            foreach (var obj in geomElem)
            {
                var vis = obj.Visibility;

                var visString = vis.ToString();

                var arc = obj as Arc;
                var line = obj as Line;
                var solid = obj as Solid;

                if (arc != null)
                {
                    var length = arc.ApproximateLength;

                    s += $"Length (arc) ({visString}): {length}\n";
                }

                if (line != null)
                {
                    var length = line.ApproximateLength;

                    s += $"Length (line) ({visString}): {length}\n";
                }

                if (solid != null)
                {
                    var faceCount = solid.Faces.Size;

                    s += $"Faces: {faceCount}\n";

                    foreach (Face face in solid.Faces)
                        s += $"Face area ({visString}): {face.Area}\n";
                }

                if (line == null && solid == null && arc == null) s += "<Other>\n";
            }

            TaskDialog.Show("revit", s);
        }

        internal static List<Solid> GetElementSolids(Element e)
        {
            return GetSolidsFromElement(e).ToList();
        }

        internal static void GetCurtainWallPanelGeometry(
            Document doc,
            ElementId curtainWallId,
            List<Solid> solids)
        {
            var wall = doc.GetElement(curtainWallId) as Wall;
            var grid = wall.CurtainGrid;

            foreach (var id in grid.GetPanelIds())
            {
                var e = doc.GetElement(id);
                solids.AddRange(GetElementSolids(e));
            }

            var cwPanels
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_CurtainWallPanels)
                    .OfClass(typeof(Wall));

            foreach (Wall cwp in cwPanels)
                if (cwp.StackedWallOwnerId == curtainWallId)
                    solids.AddRange(GetElementSolids(cwp));
        }
    }
}
