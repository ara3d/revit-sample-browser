#region Header

//
// CmdFilledRegionCoords.cs - retrieve FilledRegion coordinates
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdFilledRegionCoords : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            List<Element> filledRegions
                = new();

            if (Util.GetSelectedElementsOrAll(
                filledRegions, uidoc, typeof(FilledRegion)))
            {
                var n = filledRegions.Count;

                var results = new string[n];

                var i = 0;

                foreach (var region in
                    filledRegions.Cast<FilledRegion>())
                {
                    var desc = Util.ElementDescription(region);

                    var corners = Util.GetBoundaryCorners(
                        region);

                    var result = null == corners
                        ? "failed"
                        : string.Join(", ",
                            corners.ConvertAll(
                                    p => Util.PointString(p))
                                .ToArray());

                    results[i++] = $"{desc}: {result}";
                }

                var s = $"Retrieving corners for {n} filled region{Util.PluralSuffix(n)}{Util.DotOrColon(n)}";

                var t = string.Join("\r\n", results);

                Util.InfoMsg2(s, t);
            }

            return Result.Succeeded;
        }
    }
}