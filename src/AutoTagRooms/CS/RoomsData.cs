// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AutoTagRooms.CS
{
    /// <summary>
    ///     This class can get all the rooms, rooms tags, room tag types and levels
    /// </summary>
    public class RoomsData
    {
        // Store all levels which have rooms in the current document
        private readonly List<Level> m_levels = new List<Level>();

        // Store the reference of the application in revit
        private readonly UIApplication m_revit;

        // Store all the rooms
        private readonly List<Room> m_rooms = new List<Room>();

        // Store all the RoomTagTypes
        private List<RoomTagType> m_roomTagTypes = new List<RoomTagType>();

        // Store the room ID and all the tags which tagged to that room
        private readonly Dictionary<ElementId, List<RoomTag>> m_roomWithTags =
            new Dictionary<ElementId, List<RoomTag>>();

        /// <summary>
        ///     Constructor of RoomsData
        /// </summary>
        /// <param name="commandData">The data source of RoomData class</param>
        public RoomsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            GetRooms();
            GetRoomTagTypes();
            GetRoomWithTags();
        }

        /// <summary>
        ///     Get all the rooms in the current document
        /// </summary>
        public ReadOnlyCollection<Room> Rooms => new ReadOnlyCollection<Room>(m_rooms);

        /// <summary>
        ///     Get all the levels which have rooms in the current document
        /// </summary>
        public ReadOnlyCollection<Level> Levels => new ReadOnlyCollection<Level>(m_levels);

        /// <summary>
        ///     Get all the RoomTagTypes in the current document
        /// </summary>
        public ReadOnlyCollection<RoomTagType> RoomTagTypes => new ReadOnlyCollection<RoomTagType>(m_roomTagTypes);

        /// <summary>
        ///     Find all the rooms in the current document
        /// </summary>
        private void GetRooms()
        {
            var document = m_revit.ActiveUIDocument.Document;
            foreach (PlanTopology planTopology in document.PlanTopologies)
                if (planTopology.GetRoomIds().Count != 0 && planTopology.Level != null)
                {
                    m_levels.Add(planTopology.Level);
                    foreach (var eid in planTopology.GetRoomIds())
                    {
                        var tmpRoom = document.GetElement(eid) as Room;

                        if (document.GetElement(tmpRoom.LevelId) != null &&
                            m_roomWithTags.ContainsKey(tmpRoom.Id) == false)
                        {
                            m_rooms.Add(tmpRoom);
                            m_roomWithTags.Add(tmpRoom.Id, new List<RoomTag>());
                        }
                    }
                }
        }

        /// <summary>
        ///     Get all the RoomTagTypes in the current document
        /// </summary>
        private void GetRoomTagTypes()
        {
            var filteredElementCollector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
            filteredElementCollector.OfClass(typeof(FamilySymbol));
            filteredElementCollector.OfCategory(BuiltInCategory.OST_RoomTags);
            m_roomTagTypes = filteredElementCollector.Cast<RoomTagType>().ToList();
        }

        /// <summary>
        ///     Get all the room tags which tagged rooms
        /// </summary>
        private void GetRoomWithTags()
        {
            var document = m_revit.ActiveUIDocument.Document;
            var roomTags =
                from elem in new FilteredElementCollector(document).WherePasses(new RoomTagFilter()).ToElements()
                let roomTag = elem as RoomTag
                where roomTag != null && roomTag.Room != null
                select roomTag;

            foreach (var roomTag in roomTags)
                if (m_roomWithTags.ContainsKey(roomTag.Room.Id))
                {
                    var tmpList = m_roomWithTags[roomTag.Room.Id];
                    tmpList.Add(roomTag);
                }
        }

        /// <summary>
        ///     Auto tag rooms with specified RoomTagType in a level
        /// </summary>
        /// <param name="level">The level where rooms will be auto tagged</param>
        /// <param name="tagType">The room tag type</param>
        public void AutoTagRooms(Level level, RoomTagType tagType)
        {
            var planTopology = m_revit.ActiveUIDocument.Document.get_PlanTopology(level);

            var subTransaction = new SubTransaction(m_revit.ActiveUIDocument.Document);
            subTransaction.Start();
            foreach (var eid in planTopology.GetRoomIds())
            {
                var tmpRoom = m_revit.ActiveUIDocument.Document.GetElement(eid) as Room;

                if (m_revit.ActiveUIDocument.Document.GetElement(tmpRoom.LevelId) != null && tmpRoom.Location != null)
                {
                    // Create a specified type RoomTag to tag a room
                    var locationPoint = tmpRoom.Location as LocationPoint;
                    var point = new UV(locationPoint.Point.X, locationPoint.Point.Y);
                    var newTag =
                        m_revit.ActiveUIDocument.Document.Create.NewRoomTag(new LinkElementId(tmpRoom.Id), point, null);
                    newTag.RoomTagType = tagType;

                    var tagListInTheRoom = m_roomWithTags[newTag.Room.Id];
                    tagListInTheRoom.Add(newTag);
                }
            }

            subTransaction.Commit();
        }

        /// <summary>
        ///     Get the amount of room tags in a room with the specified RoomTagType
        /// </summary>
        /// <param name="room">A specified room</param>
        /// <param name="tagType">A specified tag type</param>
        /// <returns></returns>
        public int GetTagNumber(Room room, RoomTagType tagType)
        {
            var count = 0;
            var tagListInTheRoom = m_roomWithTags[room.Id];
            foreach (var roomTag in tagListInTheRoom)
                if (roomTag.RoomTagType.Id == tagType.Id)
                    count++;
            return count;
        }
    }
}
