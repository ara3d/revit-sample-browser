// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.AutoTagRooms.CS
{
    /// <summary>
    ///     The graphic user interface of auto tag rooms
    /// </summary>
    public partial class AutoTagRoomsForm : Form
    {
        /// <summary>
        ///     Default constructor of AutoTagRoomsForm
        /// </summary>
        private AutoTagRoomsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor of AutoTagRoomsForm
        /// </summary>
        /// <param name="roomsData">The data source of AutoTagRoomsForm</param>
        public AutoTagRoomsForm(RoomsData roomsData) : this()
        {
            m_roomsData = roomsData;
            InitRoomListView();
        }

        /// <summary>
        ///     When the AutoTagRoomsForm is loading, initialize the levelsComboBox and tagTypesComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoTagRoomsForm_Load(object sender, EventArgs e)
        {
            // levelsComboBox
            levelsComboBox.DataSource = m_roomsData.Levels;
            levelsComboBox.DisplayMember = "Name";
            levelsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            levelsComboBox.Sorted = true;
            levelsComboBox.DropDown += levelsComboBox_DropDown;

            // tagTypesComboBox
            tagTypesComboBox.DataSource = m_roomsData.RoomTagTypes;
            tagTypesComboBox.DisplayMember = "Name";
            tagTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            tagTypesComboBox.DropDown += tagTypesComboBox_DropDown;
        }

        /// <summary>
        ///     When the tagTypesComboBox drop down, adjust its width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tagTypesComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }

        /// <summary>
        ///     When the levelsComboBox drop down, adjust its width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelsComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }

        /// <summary>
        ///     Initialize the roomsListView
        /// </summary>
        private void InitRoomListView()
        {
            roomsListView.Columns.Clear();

            // Create the columns of the roomsListView
            roomsListView.Columns.Add("Room Name");
            foreach (var type in m_roomsData.RoomTagTypes) roomsListView.Columns.Add(type.Name);

            roomsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            roomsListView.FullRowSelect = true;
        }

        /// <summary>
        ///     Update the rooms information in the specified level
        /// </summary>
        private void UpdateRoomsList()
        {
            // when update the RoomsListView, clear all the items first
            roomsListView.Items.Clear();

            foreach (var tmpRoom in m_roomsData.Rooms)
            {
                var level = levelsComboBox.SelectedItem as Level;

                if (tmpRoom.LevelId == level.Id)
                {
                    var item = new ListViewItem(tmpRoom.Name);

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

        /// <summary>
        ///     When clicked the autoTag button, then tag all rooms in the specified level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoTagButton_Click(object sender, EventArgs e)
        {
            if (levelsComboBox.SelectedItem is Level level && tagTypesComboBox.SelectedItem is RoomTagType tagType) m_roomsData.AutoTagRooms(level, tagType);

            UpdateRoomsList();
        }

        /// <summary>
        ///     When selected different level, then update the roomsListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRoomsList();
        }


        /// <summary>
        ///     Adjust combo box drop down list width to longest string width
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
