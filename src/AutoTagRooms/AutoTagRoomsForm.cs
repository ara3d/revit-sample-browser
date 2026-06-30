// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Drawing;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.AutoTagRooms.CS
{
    public partial class AutoTagRoomsForm : Form
    {
        private AutoTagRoomsForm()
        {
            InitializeComponent();
        }

        public AutoTagRoomsForm(RoomsData roomsData) : this()
        {
            m_roomsData = roomsData;
            InitRoomListView();
        }

        private void AutoTagRoomsForm_Load(object sender, EventArgs e)
        {
            levelsComboBox.DataSource = m_roomsData.Levels;
            levelsComboBox.DisplayMember = "Name";
            levelsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            levelsComboBox.Sorted = true;
            levelsComboBox.DropDown += levelsComboBox_DropDown;

            tagTypesComboBox.DataSource = m_roomsData.RoomTagTypes;
            tagTypesComboBox.DisplayMember = "Name";
            tagTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tagTypesComboBox.DropDown += tagTypesComboBox_DropDown;
        }

        private void tagTypesComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }

        private void levelsComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }

        private void InitRoomListView()
        {
            roomsListView.Columns.Clear();

            roomsListView.Columns.Add("Room Name");
            foreach (var type in m_roomsData.RoomTagTypes)
            {
                roomsListView.Columns.Add(type.Name);
            }

            roomsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            roomsListView.FullRowSelect = true;
        }

        private void UpdateRoomsList()
        {
            // when update the RoomsListView, clear all the items first
            roomsListView.Items.Clear();

            foreach (var tmpRoom in m_roomsData.Rooms)
            {
                var level = levelsComboBox.SelectedItem as Level;

                if (tmpRoom.LevelId == level.Id)
                {
                    ListViewItem item = new(tmpRoom.Name);

                    // Shows the number of each type of RoomTags that the room has
                    foreach (var type in m_roomsData.RoomTagTypes)
                    {
                        var count = m_roomsData.GetTagNumber(tmpRoom, type);
                        var str = count.ToString();
                        item.SubItems.Add(str);
                    }

                    roomsListView.Items.Add(item);
                }
            }
        }

        private void autoTagButton_Click(object sender, EventArgs e)
        {
            if (levelsComboBox.SelectedItem is Level level && tagTypesComboBox.SelectedItem is RoomTagType tagType) m_roomsData.AutoTagRooms(level, tagType);

            UpdateRoomsList();
        }

        private void levelsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRoomsList();
        }

        private void AdjustWidthComboBox_DropDown(object sender, EventArgs e)
        {
            var senderComboBox = (ComboBox)sender;
            var width = senderComboBox.DropDownWidth;
            var g = senderComboBox.CreateGraphics();
            var font = senderComboBox.Font;
            var vertScrollBarWidth =
                senderComboBox.Items.Count > senderComboBox.MaxDropDownItems
                    ? SystemInformation.VerticalScrollBarWidth
                    : 0;

            foreach (Element element in ((ComboBox)sender).Items)
            {
                var s = element.Name;
                var newWidth = (int)g.MeasureString(s, font).Width
                               + vertScrollBarWidth;
                if (width < newWidth) width = newWidth;
            }

            senderComboBox.DropDownWidth = width;
        }
    }
}
