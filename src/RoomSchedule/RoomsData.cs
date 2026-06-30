// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
namespace Ara3D.RevitSampleBrowser.RoomSchedule.CS
{
    /// <summary>
    ///     Iterates through the rooms in the project and get the information of all the rooms
    /// </summary>
    public class RoomsData
    {
        /// <summary>
        ///     Constant name for RoomID, this column must exist in first row.
        /// </summary>
        public const string RoomId = "ID";

        /// <summary>
        ///     Constant name for room area, this column must exist in first row
        /// </summary>
        public const string RoomArea = "Room Area";

        /// <summary>
        ///     Constant named for room name, this column must exist in first row
        /// </summary>
        public const string RoomName = "Room Name";

        /// <summary>
        ///     Constant name for room number, this column must exist in first row
        /// </summary>
        public const string RoomNumber = "Room Number";

        /// <summary>
        ///     Constant name for room number, this column must exist in first row
        /// </summary>
        public const string RoomComments = "Room Comments";

        /// <summary>
        ///     Constant name for shared parameter,
        ///     the mapped room id of spread sheet will saved in this parameter.
        /// </summary>
        public const string SharedParam = "External Room ID";

        private readonly Document m_activeDocument;

        private readonly List<string> m_columnNames = [];

        private readonly List<BuiltInParameter> m_parameters = [];

        private List<Room> m_rooms = [];

        public RoomsData(Document activeDocument)
        {
            m_activeDocument = activeDocument;

            InitializeParameters();

            GetAllRooms(activeDocument);
        }

        public ReadOnlyCollection<Room> Rooms => new(m_rooms);

        /// <summary>
        ///     Update rooms data after room creation happens in Revit
        /// </summary>
        public void UpdateRoomsData()
        {
            // clear all rooms and re-retrieve data from Revit
            m_rooms.Clear();
            GetAllRooms(m_activeDocument);
        }

        public void UpdateParameters(ReadOnlyCollection<BuiltInParameter> specifiedParams)
        {
            // if there is no instance, parameter setting is not allowed
            if (m_rooms.Count <= 0) throw new Exception("No element instance to set parameters");

            // clear old parameters data
            m_parameters.Clear();
            m_columnNames.Clear();

            var firstRoom = m_rooms[0];
            foreach (var param in specifiedParams)
            {
                m_parameters.Add(param);

                // store all specified parameter names of room.
                var toomPara = firstRoom.get_Parameter(param);
                m_columnNames.Add(toomPara.Definition.Name);
            }
        }

        /// <summary>
        ///     Generate all rooms which are located in specified level.
        ///     A DataTable data object will be generated after this method call.
        /// </summary>
        /// <param name="level">the specified level to retrieve rooms</param>
        /// <returns>DataTable generated from rooms</returns>
        public DataTable GenRoomsDataTable(Level level)
        {
            if (m_rooms.Count == 0) return null;

            // generate columns by all parameters            
            DataTable newTable = new();
            foreach (var col in m_columnNames)
            {
                DataColumn column = new()
                {
                    ColumnName = col,
                    ReadOnly = true,
                    DataType = Type.GetType("System.String")
                };
                newTable.Columns.Add(column);
            }

            DataColumn constantCol = new()
            {
                ColumnName = SharedParam,
                ReadOnly = true,
                DataType = Type.GetType("System.String")
            };
            newTable.Columns.Add(constantCol);

            // filter rooms by level
            foreach (var room in m_rooms)
            {
                if (null == level || (m_activeDocument.GetElement(room.LevelId) != null && room.LevelId == level.Id))
                {
                    var dataRow = newTable.NewRow();
                    for (var i = 0; i < m_parameters.Count; i++)
                        dataRow[i] = SampleBrowserUtils.GetProperty(m_activeDocument, room, m_parameters[i], true);

                    Parameter param = null;
                    var bExist = ParameterAccess.ShareParameterExists(room, SharedParam, ref param);
                    dataRow[m_parameters.Count] = bExist && null != param && false == string.IsNullOrEmpty(param.AsString()) ? param.AsString() : "<null>";

                    newTable.Rows.Add(dataRow);
                }
            }

            return newTable;
        }

        private void GetAllRooms(Document activeDoc)
        {
            // try to find all rooms in the project and add to the list
            RoomFilter filter = new();
            FilteredElementCollector collector = new(activeDoc);
            m_rooms = collector.WherePasses(filter).ToElements().Cast<Room>().ToList();
            // sort rooms by number
            m_rooms.Sort(CompRoomByNumber);
        }

        private static int CompRoomByNumber(Room room1, Room room2)
        {
            return null == room1 || null == room2 ? -1 : room1.Number.CompareTo(room2.Number);
        }

        private void InitializeParameters()
        {
            // Room name
            m_parameters.Add(BuiltInParameter.ROOM_NAME);
            m_columnNames.Add("Name");

            // Room Number
            m_parameters.Add(BuiltInParameter.ROOM_NUMBER);
            m_columnNames.Add("Number");

            // Room Area
            m_parameters.Add(BuiltInParameter.ROOM_AREA);
            m_columnNames.Add("Area");

            // Room Comments 
            m_parameters.Add(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
            m_columnNames.Add("Comments");

            // Level 
            m_parameters.Add(BuiltInParameter.LEVEL_NAME);
            m_columnNames.Add("Level");

            // Phase 
            m_parameters.Add(BuiltInParameter.ROOM_PHASE);
            m_columnNames.Add("Phase");
        }
    }
}
