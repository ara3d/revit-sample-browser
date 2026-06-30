#region Header

//
// CmdNestedInstanceGeo.cs - analyse
// nested instance geometry and structure
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdNestedInstanceGeo : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var a = new List<Element>();

            if (!Util.GetSelectedElementsOrAll(a, uidoc,
                typeof(FamilyInstance)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some family instances."
                    : "No family instances found.";
                return Result.Failed;
            }

            var inst = a[0] as FamilyInstance;

            // GeometryInstance.SymbolGeometry has correct placement but not nested structure; EditFamily components have structure but symbol-local positions.

            var opt = app.Application.Create.NewGeometryOptions();
            var geoElement = inst.get_Geometry(opt);


            var n = geoElement.Count();

            Debug.Print(
                "Family instance geometry has {0} geometry object{1}{2}",
                n, Util.PluralSuffix(n), Util.DotOrColon(n));

            var i = 0;

            foreach (var o1 in geoElement)
            {
                var geoInstance = o1 as GeometryInstance;
                if (null != geoInstance)
                {
                    var symbolGeo = geoInstance.SymbolGeometry;

                    foreach (var o2 in symbolGeo)
                    {
                        var s = o2 as Solid;
                        if (null != s && 0 < s.Edges.Size)
                        {
                            var vertices = new List<XYZ>();
                            Util.GetVertices(vertices, s);
                            n = vertices.Count;

                            Debug.Print("Solid {0} has {1} vertices{2} {3}",
                                i++, n, Util.DotOrColon(n),
                                Util.PointArrayString(vertices));
                        }
                    }
                }
            }

            var fdoc = doc.EditFamily(inst.Symbol.Family);

            var collector
                = new FilteredElementCollector(fdoc);

            collector.OfClass(typeof(FamilyInstance));
            var components = collector.ToElements();

            n = components.Count;

            Debug.Print(
                "Family instance symbol family has {0} component{1}{2}",
                n, Util.PluralSuffix(n), Util.DotOrColon(n));

            foreach (var e in components)
            {
                var lp = e.Location as LocationPoint;
                Debug.Print("{0} at {1}",
                    Util.ElementDescription(e),
                    Util.PointString(lp.Point));
            }

            return Result.Failed;
        }
    }
}