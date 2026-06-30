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
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        const string Caption = "Get Centroid";

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
            var computed = 0;
            var skipped = 0;

            foreach (var id in ids)
            {
                var element = doc.GetElement(id);

                if (!SolidCentroidCalculator.TryGetCentroidFromElement(
                        element, options, out var centroidVolume))
                {
                    skipped++;
                    report.AppendLine(
                        $"{Util.ElementDescription(element)}: no tessellatable solids");
                    continue;
                }

                computed++;
                report.AppendLine(
                    $"{Util.ElementDescription(element)}: {centroidVolume}");
            }

            TaskDialog.Show(
                Caption,
                $"Processed {ids.Count} element(s).\n"
                + $"Computed {computed} centroid(s), skipped {skipped}.\n\n"
                + report.ToString().TrimEnd());

            return Result.Succeeded;
        }
    }
}
