// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AutoTagRooms.CS
{
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

        public RoomsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            GetRooms();
            GetRoomTagTypes();
            GetRoomWithTags();
        }

        public ReadOnlyCollection<Room> Rooms => new ReadOnlyCollection<Room>(m_rooms);

        public ReadOnlyCollection<Level> Levels => new ReadOnlyCollection<Level>(m_levels);

        public ReadOnlyCollection<RoomTagType> RoomTagTypes => new ReadOnlyCollection<RoomTagType>(m_roomTagTypes);

        private void GetRooms()
        {
            var document = m_revit.ActiveUIDocument.Document;
            foreach (PlanTopology planTopology in document.PlanTopologies)
            {
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
        }

        private void GetRoomTagTypes()
        {
            m_roomTagTypes = m_revit.ActiveUIDocument.Document.GetElements<RoomTagType>().ToList();
        }

        private void GetRoomWithTags()
        {
            var roomTags = m_revit.ActiveUIDocument.Document.GetElements<RoomTag>();
            foreach (var roomTag in roomTags)
            {
                if (m_roomWithTags.ContainsKey(roomTag.Room.Id))
                {
                    m_roomWithTags[roomTag.Room.Id].Add(roomTag);
                }
            }
        }

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

        public int GetTagNumber(Room room, RoomTagType tagType)
        {
            var count = 0;
            var tagListInTheRoom = m_roomWithTags[room.Id];
            foreach (var roomTag in tagListInTheRoom)
            {
                if (roomTag.RoomTagType.Id == tagType.Id)
                    count++;
            }

            return count;
        }
    }
}
