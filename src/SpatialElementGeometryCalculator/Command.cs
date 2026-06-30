// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using BuildingCoder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    /// <summary>
    ///     Determine net and gross wall area per room using
    ///     SpatialElementGeometryCalculator and opening deductions.
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
            var doc = commandData.Application.ActiveUIDocument.Document;
            Result rc;

            try
            {
                SpatialElementBoundaryOptions sebOptions = new()
                {
                    SpatialElementBoundaryLocation =
                        SpatialElementBoundaryLocation.Finish
                };

                var rooms = new FilteredElementCollector(doc)
                    .OfClass(typeof(SpatialElement))
                    .Cast<SpatialElement>()
                    .OfType<Room>();

                List<string> compareWallAndRoom = new();
                OpeningHandler openingHandler = new();
                List<SpatialBoundaryCache> lstSpatialBoundaryCache = new();

                foreach (var room in rooms)
                {
                    if (room == null) continue;
                    if (room.Location == null) continue;
                    if (room.Area.Equals(0)) continue;

                    Autodesk.Revit.DB.SpatialElementGeometryCalculator calc = new(
                        doc, sebOptions);

                    var results = calc.CalculateSpatialElementGeometry(room);
                    var roomSolid = results.GetGeometry();

                    foreach (Face face in results.GetGeometry().Faces)
                    {
                        var boundaryFaceInfo = results.GetBoundaryFaceInfo(face);

                        foreach (var spatialSubFace in boundaryFaceInfo)
                        {
                            if (spatialSubFace.SubfaceType != SubfaceType.Side)
                            {
                                continue;
                            }

                            SpatialBoundaryCache spatialData = new();


                            if (doc.GetElement(spatialSubFace
                                .SpatialBoundaryElement.HostElementId) is not Wall wall)
                            {
                                continue;
                            }

                            var wallType = doc.GetElement(
                                wall.GetTypeId()) as WallType;

                            if (wallType.Kind == WallKind.Curtain)
                            {
                                LogCreator.LogEntry("WallType is CurtainWall");
                                continue;
                            }

                            HostObject hostObject = wall;

                            var insertsThisHost = hostObject.FindInserts(
                                true, false, true, true);

                            var openingArea = 0.0;

                            foreach (var idInsert in insertsThisHost)
                            {
                                var countOnce = room.Id.ToString()
                                    + wall.Id.ToString() + idInsert.ToString();

                                if (!compareWallAndRoom.Contains(countOnce))
                                {
                                    var elemOpening = doc.GetElement(
                                        idInsert);

                                    openingArea += openingHandler.GetOpeningArea(
                                        wall, elemOpening, room, roomSolid);

                                    compareWallAndRoom.Add(countOnce);
                                }
                            }

                            spatialData.roomName = room.Name;
                            spatialData.idElement = wall.Id;
                            spatialData.idMaterial = spatialSubFace
                                .GetBoundingElementFace().MaterialElementId;
                            spatialData.dblNetArea = SqFootToSquareM(
                                spatialSubFace.GetSubface().Area - openingArea);
                            spatialData.dblOpeningArea = SqFootToSquareM(
                                openingArea);

                            lstSpatialBoundaryCache.Add(spatialData);
                        }
                    }
                }

                List<string> t = new();

                var groupedData = SortByRoom(lstSpatialBoundaryCache);

                foreach (var sbc in groupedData)
                {
                    t.Add(sbc.roomName
                        + "; all wall types and materials: "
                        + sbc.AreaReport);
                }

                Util.InfoMsg2("Total Net Area in m2 by Room",
                    string.Join(Environment.NewLine, t));

                t.Clear();

                groupedData = SortByRoomAndWallType(lstSpatialBoundaryCache);

                foreach (var sbc in groupedData)
                {
                    var elemWall = doc.GetElement(sbc.idElement);

                    t.Add(sbc.roomName + "; " + elemWall.Name
                        + "(" + sbc.idElement.ToString() + "): "
                        + sbc.AreaReport);
                }

                Util.InfoMsg2("Net Area in m2 by Wall Type",
                    string.Join(Environment.NewLine, t));

                t.Clear();

                groupedData = SortByRoomAndMaterial(lstSpatialBoundaryCache);

                foreach (var sbc in groupedData)
                {
                    var materialName = (sbc.idMaterial == ElementId.InvalidElementId)
                        ? string.Empty
                        : doc.GetElement(sbc.idMaterial).Name;

                    t.Add(sbc.roomName + "; " + materialName + ": "
                        + sbc.AreaReport);
                }

                Util.InfoMsg2(
                    "Net Area in m2 by Outer Layer Material",
                    string.Join(Environment.NewLine, t));

                rc = Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Room Boundaries",
                    ex.Message + "\r\n" + ex.StackTrace);

                rc = Result.Failed;
            }

            return rc;
        }

        static double SqFootToSquareM(double sqFoot)
        {
            return Math.Round(sqFoot * 0.092903, 2);
        }

        static List<SpatialBoundaryCache> SortByRoom(
            List<SpatialBoundaryCache> lstRawData)
        {
            var sortedCache =
                from rawData in lstRawData
                group rawData by new { room = rawData.roomName }
                into sortedData
                select new SpatialBoundaryCache
                {
                    roomName = sortedData.Key.room,
                    idElement = ElementId.InvalidElementId,
                    dblNetArea = sortedData.Sum(x => x.dblNetArea),
                    dblOpeningArea = sortedData.Sum(y => y.dblOpeningArea),
                };

            return sortedCache.ToList();
        }

        static List<SpatialBoundaryCache> SortByRoomAndWallType(
            List<SpatialBoundaryCache> lstRawData)
        {
            var sortedCache =
                from rawData in lstRawData
                group rawData by new
                {
                    room = rawData.roomName,
                    wallid = rawData.idElement
                }
                into sortedData
                select new SpatialBoundaryCache
                {
                    roomName = sortedData.Key.room,
                    idElement = sortedData.Key.wallid,
                    dblNetArea = sortedData.Sum(x => x.dblNetArea),
                    dblOpeningArea = sortedData.Sum(y => y.dblOpeningArea),
                };

            return sortedCache.ToList();
        }

        static List<SpatialBoundaryCache> SortByRoomAndMaterial(
            List<SpatialBoundaryCache> lstRawData)
        {
            var sortedCache =
                from rawData in lstRawData
                group rawData by new
                {
                    room = rawData.roomName,
                    mid = rawData.idMaterial
                }
                into sortedData
                select new SpatialBoundaryCache
                {
                    roomName = sortedData.Key.room,
                    idMaterial = sortedData.Key.mid,
                    dblNetArea = sortedData.Sum(x => x.dblNetArea),
                    dblOpeningArea = sortedData.Sum(y => y.dblOpeningArea),
                };

            return sortedCache.ToList();
        }
    }
}
