// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Rooms.CS
{
    /// <summary>
    ///     Iterates through the rooms in the project and get the information of all the rooms
    /// </summary>
    public class RoomsData
    {
        // a list to store the information of departments
        private readonly List<DepartmentInfo> m_departmentInfos = new List<DepartmentInfo>();
        private readonly UIApplication m_revit; // Store the reference of the application in revit

        private readonly List<Room> m_rooms = new List<Room>(); // a list to store all rooms in the project
        private readonly List<Room> m_roomsWithoutTag = new List<Room>(); // a list to store all rooms without tag
        private readonly List<Room> m_roomsWithTag = new List<Room>(); // a list to store all rooms with tag
        private readonly List<RoomTag> m_roomTags = new List<RoomTag>(); // a list to store all room tags


        /// <summary>
        ///     constructor
        /// </summary>
        public RoomsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;

            // get all the rooms and room tags in the project
            GetAllRoomsAndTags();

            // find out the rooms that without room tag
            ClassifyRooms();
        }


        /// <summary>
        ///     a list of all department
        /// </summary>
        public ReadOnlyCollection<DepartmentInfo> DepartmentInfos =>
            new ReadOnlyCollection<DepartmentInfo>(m_departmentInfos);


        /// <summary>
        ///     a list of all the rooms in the project
        /// </summary>
        public ReadOnlyCollection<Room> Rooms => new ReadOnlyCollection<Room>(m_rooms);


        /// <summary>
        ///     a list of all the room tags in the project
        /// </summary>
        public ReadOnlyCollection<RoomTag> RoomTags => new ReadOnlyCollection<RoomTag>(m_roomTags);


        /// <summary>
        ///     a list of the rooms that had tag
        /// </summary>
        public ReadOnlyCollection<Room> RoomsWithTag => new ReadOnlyCollection<Room>(m_roomsWithTag);


        /// <summary>
        ///     a list of the rooms which lack room tag
        /// </summary>
        public ReadOnlyCollection<Room> RoomsWithoutTag => new ReadOnlyCollection<Room>(m_roomsWithoutTag);


        /// <summary>
        ///     create the room tags for the rooms which lack room tag
        /// </summary>
        public void CreateTags()
        {
            try
            {
                foreach (var tmpRoom in m_roomsWithoutTag)
                {
                    // get the location point of the room
                    var locPoint = tmpRoom.Location as LocationPoint;
                    if (null == locPoint)
                    {
                        var roomId = "Room Id:  " + tmpRoom.Id;
                        var errMsg = roomId + "\r\nFault to create room tag," +
                                     "can't get the location point!";
                        throw new Exception(errMsg);
                    }

                    // create a instance of Autodesk.Revit.DB.UV class
                    var point = new UV(locPoint.Point.X, locPoint.Point.Y);

                    //create room tag
                    var tmpTag =
                        m_revit.ActiveUIDocument.Document.Create.NewRoomTag(new LinkElementId(tmpRoom.Id), point, null);
                    if (null != tmpTag) m_roomTags.Add(tmpTag);
                }

                // classify rooms
                ClassifyRooms();

                // display a message box
                TaskDialog.Show("Revit", "Add room tags complete!");
            }
            catch (Exception exception)
            {
                TaskDialog.Show("Revit", exception.Message);
            }
        }


        /// <summary>
        ///     reorder all the rooms' number
        /// </summary>
        public void ReorderRooms()
        {
            var result =
                // sort all the rooms by ascending order according their coordinate
                SortRooms();

            // fault to reorder rooms' number
            if (!result)
            {
                TaskDialog.Show("Revit", "Fault to reorder rooms' number,can't get location point!");
                return;
            }

            // to avoid revit display the warning message,
            // change the rooms' name to a temp name 
            foreach (var tmpRoom in m_rooms) tmpRoom.Number += "XXX";

            // set the tag number of rooms in order
            for (var i = 1; i <= m_rooms.Count; i++) m_rooms[i - 1].Number = i.ToString();

            // display a message box
            TaskDialog.Show("Revit", "Reorder room's number complete!");
        }


        /// <summary>
        ///     get the room property and Department property according the property name
        /// </summary>
        /// <param name="room">a instance of room class</param>
        /// <param name="paraEnum">the property name</param>
        public string GetProperty(Room room, BuiltInParameter paraEnum)
        {
            string propertyValue = null; //the value of parameter 

            // get the parameter via the parameterId
            var param = room.get_Parameter(paraEnum);
            if (null == param) return "";
            // get the parameter's storage type
            var storageType = param.StorageType;
            switch (storageType)
            {
                case StorageType.Integer:
                    var iVal = param.AsInteger();
                    propertyValue = iVal.ToString();
                    break;
                case StorageType.String:
                    var stringVal = param.AsString();
                    propertyValue = stringVal;
                    break;
                case StorageType.Double:
                    var dVal = param.AsDouble();
                    dVal = Math.Round(dVal, 2);
                    propertyValue = dVal.ToString();
                    break;
            }

            return propertyValue;
        }


        /// <summary>
        ///     calculate the area of rooms for each department
        /// </summary>
        /// <param name="departName">the department name</param>
        /// <param name="areaValue">the value of room area</param>
        public void CalculateDepartmentArea(string departName, double areaValue)
        {
            //if the list is empty, add a new  DepartmentArea instance
            if (0 == m_departmentInfos.Count)
            {
                // create a new instance of DepartmentArea struct and insert it to the list
                var tmpDep = new DepartmentInfo(departName, 1, areaValue);
                m_departmentInfos.Add(tmpDep);
            }
            else
            {
                var flag = false;
                // find whether the department exist in the project
                for (var i = 0; i < m_departmentInfos.Count; i++)
                    if (departName == m_departmentInfos[i].DepartmentName)
                    {
                        var newAmount = m_departmentInfos[i].RoomsAmount + 1;
                        var tempValue = m_departmentInfos[i].DepartmentAreaValue + areaValue;
                        var tempInstance = new DepartmentInfo(departName, newAmount, tempValue);
                        m_departmentInfos[i] = tempInstance;
                        flag = true;
                    }

                // if a new department is found,
                // create a new instance of DepartmentArea struct and insert it to the list
                if (!flag)
                {
                    var tmpDep = new DepartmentInfo(departName, 1, areaValue);
                    m_departmentInfos.Add(tmpDep);
                }
            }
        }


        /// <summary>
        ///     export data into an Excel file
        /// </summary>
        /// <param name="fileName"></param>
        public void ExportFile(string fileName)
        {
            // store all the information that to be exported
            var allData = "";

            // get the project title
            var projectTitle = m_revit.ActiveUIDocument.Document.Title; //the name of the project
            allData += "Total Rooms area of " + projectTitle + "\r\n";
            allData += "Department" + "," + "Rooms Amount" + "," + "Total Area" + "\r\n";

            foreach (var tmp in m_departmentInfos)
                allData += tmp.DepartmentName + "," + tmp.RoomsAmount +
                           "," + tmp.DepartmentAreaValue + " SF\r\n";

            // save the information into a Excel file
            if (0 < allData.Length)
            {
                var exportinfo = new StreamWriter(fileName);
                exportinfo.WriteLine(allData);
                exportinfo.Close();
            }
        }


        /// <summary>
        ///     get all the rooms and room tags in the project
        /// </summary>
        private void GetAllRoomsAndTags()
        {
            // get the active document 
            var document = m_revit.ActiveUIDocument.Document;
            var roomFilter = new RoomFilter();
            var roomTagFilter = new RoomTagFilter();
            var orFilter = new LogicalOrFilter(roomFilter, roomTagFilter);

            var elementIterator =
                new FilteredElementCollector(document).WherePasses(orFilter).GetElementIterator();
            elementIterator.Reset();

            // try to find all the rooms and room tags in the project and add to the list
            while (elementIterator.MoveNext())
            {
                object obj = elementIterator.Current;

                // find the rooms, skip those rooms which don't locate at Level yet.
                var tmpRoom = obj as Room;
                if (null != tmpRoom && null != document.GetElement(tmpRoom.LevelId))
                {
                    m_rooms.Add(tmpRoom);
                    continue;
                }

                // find the room tags
                var tmpTag = obj as RoomTag;
                if (null != tmpTag)
                {
                    m_roomTags.Add(tmpTag);
                }
            }
        }


        /// <summary>
        ///     find out the rooms that without room tag
        /// </summary>
        private void ClassifyRooms()
        {
            m_roomsWithoutTag.Clear();
            m_roomsWithTag.Clear();

            // copy the all the elements in list Rooms to list RoomsWithoutTag
            m_roomsWithoutTag.AddRange(m_rooms);

            // get the room id from room tag via room property
            // if find the room id in list RoomWithoutTag,
            // add it to the list RoomWithTag and delete it from list RoomWithoutTag
            foreach (var tmpTag in m_roomTags)
            {
                var idValue = tmpTag.Room.Id;
                m_roomsWithTag.Add(tmpTag.Room);

                // search the id for list RoomWithoutTag
                foreach (var tmpRoom in m_rooms)
                    if (idValue == tmpRoom.Id)
                        m_roomsWithoutTag.Remove(tmpRoom);
            }
        }


        /// <summary>
        ///     sort all the rooms by ascending order according their coordinate
        /// </summary>
        private bool SortRooms()
        {
            var result = 0; //a temp variable
            var amount = m_rooms.Count; //the number of rooms
            var flag = false;

            // sort the rooms according their location point 
            for (var i = 0; i < amount - 1; i++)
            {
                var tmpRoom = m_rooms[i];
                for (var j = i + 1; j < amount; j++)
                {
                    var tmpPoint = tmpRoom.Location as LocationPoint;
                    var listRoom = m_rooms[j];
                    var roomPoint = listRoom.Location as LocationPoint;

                    // if can't get location point, return false;
                    if (null == tmpPoint || null == roomPoint) return false;

                    // rooms in different level
                    if (tmpPoint.Point.Z > roomPoint.Point.Z)
                    {
                        tmpRoom = listRoom;
                        result = j;

                        // if tmpRoom was changed, set flag to 1
                        flag = true;
                    }
                    // the two rooms in the same level
                    else if (tmpPoint.Point.Z == roomPoint.Point.Z)
                    {
                        if (tmpPoint.Point.X > roomPoint.Point.X)
                        {
                            tmpRoom = listRoom;
                            result = j;
                            flag = true;
                        }
                        else if (tmpPoint.Point.X == roomPoint.Point.X &&
                                 tmpPoint.Point.Y > roomPoint.Point.Y)
                        {
                            tmpRoom = listRoom;
                            result = j;
                            flag = true;
                        }
                    }
                }

                // if flag equals 1 ,move the room to the front of list
                if (flag)
                {
                    var tempRoom = m_rooms[i];
                    m_rooms[i] = m_rooms[result];
                    m_rooms[result] = tempRoom;
                    flag = false;
                }
            }

            return true;
        }


        /// <summary>
        ///     a struct to store the value of property Area and Department name
        /// </summary>
        public struct DepartmentInfo
        {
            /// <summary>
            ///     the name of department
            /// </summary>
            public string DepartmentName { get; }


            /// <summary>
            ///     get the amount of rooms in the department
            /// </summary>
            public int RoomsAmount { get; }

            /// <summary>
            ///     the total area of the rooms in department
            /// </summary>
            public double DepartmentAreaValue { get; }

            /// <summary>
            ///     constructor
            /// </summary>
            public DepartmentInfo(string departmentName, int roomAmount, double areaValue)
            {
                DepartmentName = departmentName;
                RoomsAmount = roomAmount;
                DepartmentAreaValue = areaValue;
            }
        }
    }
}
