// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CropViewToRoom by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CropViewToRoom

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CropViewToRoom.CS
{
    /// <summary>
    ///     For each level, duplicate the associated plan view per room and crop
    ///     the duplicate to the room boundary offset by wall thickness.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        static readonly XYZ CropOffsetNormal = new(0, 0, 1);

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var boundaryOptions = new SpatialElementBoundaryOptions();
            var dateIso = DateTime.Now.ToString("yyyy-MM-dd");

            var created = 0;
            var skipped = 0;
            var processed = 0;

            var levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            using (var tx = new Transaction(doc))
            {
                tx.Start("Create cropped views for each room");

                foreach (var level in levels)
                {
                    var planViewId = level.FindAssociatedPlanViewId();
                    if (planViewId == ElementId.InvalidElementId)
                    {
                        continue;
                    }

                    if (doc.GetElement(planViewId) is not ViewPlan planView)
                    {
                        continue;
                    }

                    var rooms = new FilteredElementCollector(doc, planViewId)
                        .OfClass(typeof(SpatialElement))
                        .WhereElementIsNotElementType()
                        .Cast<SpatialElement>()
                        .OfType<Room>()
                        .ToList();

                    foreach (var room in rooms)
                    {
                        processed++;

                        var boundaryLoops = room.GetBoundarySegments(boundaryOptions);
                        if (boundaryLoops == null)
                        {
                            skipped++;
                            continue;
                        }

                        var croppedViewId = planView.Duplicate(
                            ViewDuplicateOption.AsDependent);

                        if (doc.GetElement(croppedViewId) is not View croppedView)
                        {
                            skipped++;
                            continue;
                        }

                        croppedView.Name = string.Format(
                            "{0}_cropped_to_room_{1}_date_{2}",
                            planView.Name,
                            room.Name,
                            dateIso);

                        if (!RoomCropGeometry.TryCreateOffsetCropLoop(
                                doc,
                                boundaryLoops,
                                CropOffsetNormal,
                                out var cropLoop)
                            || !RoomCropGeometry.TryApplyCropShape(
                                croppedView,
                                cropLoop))
                        {
                            skipped++;
                            continue;
                        }

                        created++;
                    }
                }

                tx.Commit();
            }

            TaskDialog.Show(
                "Crop View To Room",
                $"Processed {processed} room(s).\n"
                + $"Created {created} cropped plan view(s).\n"
                + $"Skipped {skipped} room(s).");

            return Result.Succeeded;
        }
    }
}
