// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from PipeSystemExporter by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PipeSystemExporter

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.PipeSystemExporter.CS
{
    /// <summary>
    ///     Enumerates all pipes and pipe fittings in the document and logs
    ///     connector endpoints, diameters, and fitting types to the debug output.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            if (null == uidoc)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;

            var col = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .OfClass(typeof(Pipe));

            var n = col.GetElementCount();
            Debug.Print($"{n} pipe{Util.PluralSuffix(n)}{Util.DotOrColon(n)}");

            foreach (Pipe pipe in col)
            {
                var diameter = pipe.Diameter;
                const double inch = 25.4;
                const double foot = 12 * inch;
                var diamMm = Convert.ToInt32(diameter * foot);
                var pts = Util.GetConnectorPoints(pipe);
                Debug.Assert(2 == pts.Count, "expected two endpoints on pipe");
                Debug.Print(
                    $"  pipe '{pipe.Name}' {diamMm}mm {Util.RealString(diameter)} {Util.PointString(pts[0])} {Util.PointString(pts[1])}");
            }

            col = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.OST_PipeFitting)
                .OfClass(typeof(FamilyInstance));

            n = col.GetElementCount();
            Debug.Print($"{n} fitting{Util.PluralSuffix(n)}{Util.DotOrColon(n)}");

            foreach (FamilyInstance fitting in col)
            {
                var pts = Util.GetConnectorPoints(fitting);
                n = pts.Count;
                Debug.Assert(1 == n || 2 == n || 3 == n, "expected one, two or three endpoints on fitting");
                var s = 1 == n ? "plug" : 2 == n ? "elbow" : "tee";
                var t = string.Join(" ", pts.Select(p => Util.PointString(p)));
                Debug.Print($"  {s} '{fitting.Symbol.FamilyName}' '{fitting.Name}' {t}");
            }

            return Result.Succeeded;
        }
    }
}
