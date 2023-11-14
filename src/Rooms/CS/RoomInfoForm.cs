// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.Rooms.CS
{
    /// <summary>
    ///     UI to display the rooms information
    /// </summary>
    public partial class RoomsInformationForm : Form
    {
        private readonly RoomsData m_data; // Room's data for current active document

        /// <summary>
        ///     constructor
        /// </summary>
        public RoomsInformationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Overload the constructor
        /// </summary>
        /// <param name="data">an instance of Data class</param>
        public RoomsInformationForm(RoomsData data)
        {
            m_data = data;
            InitializeComponent();
        }

        /// <summary>
        ///     add rooms of list roomsWithTag to the listview
        /// </summary>
        private void DisplayRooms(ReadOnlyCollection<Room> roomList, bool isHaveTag)
        {
            // add rooms to the listview
            foreach (var tmpRoom in roomList)
            {
                // make sure the room has Level, that's it locates at level.
                if (tmpRoom.Document.GetElement(tmpRoom.LevelId) == null) continue;

                var roomId = tmpRoom.Id.ToString();

                // create a list view Item
                var tmpItem = new ListViewItem(roomId);
                tmpItem.SubItems.Add(tmpRoom.Name); //display room name.
                tmpItem.SubItems.Add(tmpRoom.Number); //display room number.
                tmpItem.SubItems.Add((tmpRoom.Document.GetElement(tmpRoom.LevelId) as Level).Name); //display the level

                // get department name from Department property 
                var departmentName = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_DEPARTMENT); //department name 
                tmpItem.SubItems.Add(departmentName);

                // get property value 
                var propertyValue = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_AREA); //value of department

                // get the area value
                var areaValue = double.Parse(propertyValue); //room area
                tmpItem.SubItems.Add(propertyValue + " SF");

                // display whether the room with tag or not
                if (isHaveTag)
                    tmpItem.SubItems.Add("Yes");
                else
                    tmpItem.SubItems.Add("No");

                // add the item to the listview
                roomsListView.Items.Add(tmpItem);

                // add the area to the department
                m_data.CalculateDepartmentArea(departmentName, areaValue);
            }
        }

        /// <summary>
        ///     when the form was loaded, display the information of rooms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomInfoForm_Load(object sender, EventArgs e)
        {
            roomsListView.Items.Clear();

            // add rooms in the list roomsWithoutTag to the listview
            DisplayRooms(m_data.RoomsWithoutTag, false);

            // add rooms in the list roomsWithTag to the listview
            DisplayRooms(m_data.RoomsWithTag, true);

            // display the total area of each department
            DisplayDartmentsInfo();

            // if all the rooms have tags ,the button will be set to disable
            if (0 == m_data.RoomsWithoutTag.Count) addTagsButton.Enabled = false;
        }

        /// <summary>
        ///     create room tags for the rooms which are lack of tags
        /// </summary>
        private void addTagButton_Click(object sender, EventArgs e)
        {
            m_data.CreateTags();

            roomsListView.Items.Clear();
            DisplayRooms(m_data.RoomsWithTag, true);
            DisplayRooms(m_data.RoomsWithoutTag, false);

            // if all the rooms have tags ,the button will be set to disable
            if (0 == m_data.RoomsWithoutTag.Count) addTagsButton.Enabled = false;
        }

        /// <summary>
        ///     reorder rooms' number
        /// </summary>
        private void reorderButton_Click(object sender, EventArgs e)
        {
            m_data.ReorderRooms();

            // refresh the listview
            roomsListView.Items.Clear();
            DisplayRooms(m_data.RoomsWithTag, true);
            DisplayRooms(m_data.RoomsWithoutTag, false);
        }

        /// <summary>
        ///     display total rooms' information for each department
        /// </summary>
        private void DisplayDartmentsInfo()
        {
            foreach (var departmentInfo in m_data.DepartmentInfos)
            {
                // create a listview item
                var tmpItem = new ListViewItem(departmentInfo.DepartmentName);
                tmpItem.SubItems.Add(departmentInfo.RoomsAmount.ToString());
                tmpItem.SubItems.Add(departmentInfo.DepartmentAreaValue +
                                     " SF");
                departmentsListView.Items.Add(tmpItem);
            }
        }

        /// <summary>
        ///     Save the information into an Excel file
        /// </summary>
        private void exportButton_Click(object sender, EventArgs e)
        {
            // create a save file dialog
            using (var sfdlg = new SaveFileDialog())
            {
                sfdlg.Title = "Export area of department to Excel file";
                sfdlg.Filter = "CSV(command delimited)(*.csv)|*.csv";
                sfdlg.RestoreDirectory = true;

                if (DialogResult.OK == sfdlg.ShowDialog()) m_data.ExportFile(sfdlg.FileName);
            }
        }
    }
}
