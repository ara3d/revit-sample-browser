// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.Rooms.CS
{
    /// <summary>
    ///     UI to display the rooms information
    /// </summary>
    public partial class RoomsInformationForm : Form
    {
        private readonly RoomsData m_data; // Room's data for current active document

        public RoomsInformationForm()
        {
            InitializeComponent();
        }

        public RoomsInformationForm(RoomsData data)
        {
            m_data = data;
            InitializeComponent();
        }

        private void DisplayRooms(ReadOnlyCollection<Room> roomList, bool isHaveTag)
        {
            foreach (var tmpRoom in roomList)
            {
                // make sure the room has Level, that's it locates at level.
                if (tmpRoom.Document.GetElement(tmpRoom.LevelId) == null) continue;

                var roomId = tmpRoom.Id.ToString();

                var tmpItem = new ListViewItem(roomId);
                tmpItem.SubItems.Add(tmpRoom.Name); //display room name.
                tmpItem.SubItems.Add(tmpRoom.Number); //display room number.
                tmpItem.SubItems.Add((tmpRoom.Document.GetElement(tmpRoom.LevelId) as Level).Name); //display the level

                var departmentName = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_DEPARTMENT); //department name 
                tmpItem.SubItems.Add(departmentName);

                var propertyValue = m_data.GetProperty(tmpRoom, BuiltInParameter.ROOM_AREA); //value of department

                var areaValue = double.Parse(propertyValue); //room area
                tmpItem.SubItems.Add($"{propertyValue} SF");

                // display whether the room with tag or not
                if (isHaveTag)
                    tmpItem.SubItems.Add("Yes");
                else
                    tmpItem.SubItems.Add("No");

                roomsListView.Items.Add(tmpItem);

                m_data.CalculateDepartmentArea(departmentName, areaValue);
            }
        }

        private void RoomInfoForm_Load(object sender, EventArgs e)
        {
            roomsListView.Items.Clear();

            DisplayRooms(m_data.RoomsWithoutTag, false);

            DisplayRooms(m_data.RoomsWithTag, true);

            // display the total area of each department
            DisplayDartmentsInfo();

            // if all the rooms have tags ,the button will be set to disable
            if (0 == m_data.RoomsWithoutTag.Count) addTagsButton.Enabled = false;
        }

        private void addTagButton_Click(object sender, EventArgs e)
        {
            m_data.CreateTags();

            roomsListView.Items.Clear();
            DisplayRooms(m_data.RoomsWithTag, true);
            DisplayRooms(m_data.RoomsWithoutTag, false);

            // if all the rooms have tags ,the button will be set to disable
            if (0 == m_data.RoomsWithoutTag.Count) addTagsButton.Enabled = false;
        }

        private void reorderButton_Click(object sender, EventArgs e)
        {
            m_data.ReorderRooms();

            // refresh the listview
            roomsListView.Items.Clear();
            DisplayRooms(m_data.RoomsWithTag, true);
            DisplayRooms(m_data.RoomsWithoutTag, false);
        }

        private void DisplayDartmentsInfo()
        {
            foreach (var departmentInfo in m_data.DepartmentInfos)
            {
                var tmpItem = new ListViewItem(departmentInfo.DepartmentName);
                tmpItem.SubItems.Add(departmentInfo.RoomsAmount.ToString());
                tmpItem.SubItems.Add($"{departmentInfo.DepartmentAreaValue} SF");
                departmentsListView.Items.Add(tmpItem);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using var sfdlg = new SaveFileDialog();
            sfdlg.Title = "Export area of department to Excel file";
            sfdlg.Filter = "CSV(command delimited)(*.csv)|*.csv";
            sfdlg.RestoreDirectory = true;

            if (DialogResult.OK == sfdlg.ShowDialog()) m_data.ExportFile(sfdlg.FileName);
        }
    }
}
