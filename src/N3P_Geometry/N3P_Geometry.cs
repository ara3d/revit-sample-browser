using System.Linq;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Geometry.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Geometry : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var wall = N3PSelection.GetSelectedOrAll(uidoc, typeof(Wall)).Cast<Wall>().FirstOrDefault();
            if (wall == null)
            {
                message = "No walls found for geometry demo.";
                return Result.Failed;
            }

            N3POutput.Header("Nice3point Geometry extensions");

            var bbox = wall.get_BoundingBox(null);
            if (bbox != null)
            {
                N3POutput.Line("BBox ComputeVolume()", bbox.ComputeVolume().ToMillimeters().Round(0) + " mm³ approx");
                N3POutput.Line("BBox ComputeSurfaceArea()", bbox.ComputeSurfaceArea().ToMillimeters().Round(0));
                N3POutput.Line("BBox ComputeCentroid()", bbox.ComputeCentroid());
            }

            var solid = GetFirstSolid(wall);

            if (solid != null)
            {
                N3POutput.Line("Solid IsValidForTessellation", solid.IsValidForTessellation);
                N3POutput.Line("Solid Volume (internal)", solid.Volume.Round(4));
            }

            return Result.Succeeded;
        }

        private static Solid GetFirstSolid(Element element)
        {
            var options = new Options { DetailLevel = ViewDetailLevel.Fine };
            var geo = element.get_Geometry(options);
            if (geo == null) return null;

            foreach (var obj in geo)
            {
                if (obj is Solid s && s.Volume > 0)
                    return s;
                if (obj is GeometryInstance gi)
                {
                    foreach (var instObj in gi.GetInstanceGeometry())
                    {
                        if (instObj is Solid s2 && s2.Volume > 0)
                            return s2;
                    }
                }
            }

            return null;
        }
    }
}
