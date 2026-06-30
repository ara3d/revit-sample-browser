// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetCentroid by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/GetCentroid

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BuildingCoder;
using System.Collections.Generic;
using System.Text;

namespace Ara3D.RevitSampleBrowser.GetCentroid.CS
{
    /// <summary>
    ///     Compare tessellation-based centroid and volume with Revit's
    ///     native <see cref="Solid.ComputeCentroid" /> and <see cref="Solid.Volume" />.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CompareCentroidCommand : IExternalCommand
    {
        const string Caption = "Compare Centroid";

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var ids = GetCentroidSelection.GetElementIds(uidoc, ref message);
            if (ids == null)
            {
                return string.IsNullOrEmpty(message)
                    ? Result.Cancelled
                    : Result.Failed;
            }

            if (ids.Count == 0)
            {
                message = "Select at least one element.";
                return Result.Failed;
            }

            var options = commandData.Application.Application
                .Create.NewGeometryOptions();

            StringBuilder report = new();
            var compared = 0;
            var skipped = 0;

            foreach (var id in ids)
            {
                var element = doc.GetElement(id);
                var elementHeader = Util.ElementDescription(element);
                var wroteElementHeader = false;

                foreach (var solid in SolidCentroidCalculator.GetSolidsFromElement(
                             element, options))
                {
                    if (!SolidCentroidCalculator.TryCompareSolid(
                            solid, out var comparison))
                    {
                        skipped++;
                        continue;
                    }

                    if (!wroteElementHeader)
                    {
                        report.AppendLine(elementHeader + ":");
                        wroteElementHeader = true;
                    }

                    compared++;
                    report.AppendLine(
                        $"  tessellated {comparison.Tessellated}");
                    report.AppendLine(
                        $"  native      {Util.RealString(comparison.NativeVolume)}@"
                        + $"{Util.PointString(comparison.NativeCentroid)}");
                    report.AppendLine(
                        $"  delta       distance {Util.RealString(comparison.CentroidDistance)}"
                        + $", volume {Util.RealString(comparison.VolumeDifference)}"
                        + $" ({Util.RealString(comparison.RelativeVolumeDifference * 100)}%)");
                }

                if (!wroteElementHeader)
                {
                    skipped++;
                    report.AppendLine(
                        $"{elementHeader}: no tessellatable solids");
                }
            }

            TaskDialog.Show(
                Caption,
                $"Compared {compared} solid(s), skipped {skipped}.\n\n"
                + report.ToString().TrimEnd());

            return Result.Succeeded;
        }
    }
}
