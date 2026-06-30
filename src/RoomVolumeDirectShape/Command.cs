// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RoomVolumeDirectShape by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RoomVolumeDirectShape

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.RoomVolumeDirectShape.CS
{
    /// <summary>
    ///     Create Generic Model DirectShape elements representing room volumes
    ///     from ClosedShell geometry, with room parameters stored as JSON.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        static readonly ElementId CategoryForDirectShape
            = new(BuiltInCategory.OST_GenericModel);

        static readonly BuiltInParameter PropertiesParameter
            = BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var idAddin = commandData.Application.ActiveAddInId.GetGUID()
                .ToString();

            var rooms = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(SpatialElement))
                .Cast<SpatialElement>()
                .OfType<Room>()
                .ToList();

            var created = 0;
            var skipped = 0;

            using (var tx = new Transaction(doc))
            {
                tx.Start(
                    "Generate Direct Shape Elements Representing Room Volumes");

                foreach (var room in rooms)
                {
                    if (!TryCreateRoomVolumeDirectShape(
                            doc, room, idAddin, out _))
                    {
                        skipped++;
                        continue;
                    }

                    created++;
                }

                tx.Commit();
            }

            TaskDialog.Show(
                "Room Volume DirectShape",
                $"Processed {rooms.Count} room(s).\n"
                + $"Created {created} DirectShape element(s).\n"
                + $"Skipped {skipped} room(s).");

            return Result.Succeeded;
        }

        static bool TryCreateRoomVolumeDirectShape(
            Document doc,
            Room room,
            string applicationId,
            out DirectShape directShape)
        {
            directShape = null;

            if (room.Location == null)
            {
                return false;
            }

            if (room.Area.Equals(0))
            {
                return false;
            }

            var geo = room.ClosedShell;
            if (geo == null)
            {
                return false;
            }

            var shape = RoomVolumeGeometry.CopyGeometry(
                geo, ElementId.InvalidElementId);
            if (shape == null || shape.Count == 0)
            {
                return false;
            }

            var json = RoomPropertyHelper.GetRoomPropertiesJson(room);

            directShape = DirectShape.CreateElement(doc, CategoryForDirectShape);
            directShape.ApplicationId = applicationId;
            directShape.ApplicationDataId = room.UniqueId;
            directShape.SetShape(shape);
            directShape.get_Parameter(PropertiesParameter).Set(json);
            directShape.Name = "Room volume for " + room.Name;

            return true;
        }
    }
}
