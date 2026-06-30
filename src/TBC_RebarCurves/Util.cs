#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static IList<Curve> GetRebarCurves(Document doc)
        {
            IList<Curve> curves = new List<Curve>();

            var rebars
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Rebar));

            int n, nElements = 0, nCurves = 0;

            foreach (Rebar rebar in rebars)
            {
                ++nElements;

                n = rebar.NumberOfBarPositions;

                nCurves += n;

                for (var i = 0; i < n; ++i)
                {
                    var centerlineCurves
                        = rebar.GetCenterlineCurves(
                            true, false, false,
                            MultiplanarOption.IncludeAllMultiplanarCurves,
                            i);

                    if (rebar.IsRebarShapeDriven())
                    {
                        var accessor
                            = rebar.GetShapeDrivenAccessor();

                        var trf = accessor
                            .GetBarPositionTransform(i);

                        foreach (var c in centerlineCurves) curves.Add(c.CreateTransformed(trf));
                    }
                    else
                    {
                        foreach (var c in centerlineCurves)
                            curves.Add(c);
                    }
                }
            }

            n = curves.Count;

            Debug.Print("Processed {0} rebar element{1} "
                        + "with {2} bar position{3}, extracted {4} "
                        + "curve{5}",
                nElements, PluralSuffix(nElements),
                nCurves, PluralSuffix(nCurves),
                n, PluralSuffix(n));

            return curves;
        }
    }
}
