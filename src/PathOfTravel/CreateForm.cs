// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PathOfTravel.CS
{
    /// <summary>
    ///     Form presented to the user to fill in the options to control the path of travel creation.
    /// </summary>
    public partial class CreateForm : Form
    {
        public CreateForm()
        {
            InitializeComponent();

            radioButton1.Checked = true;
            PathCreateOption = PathCreateOptions.SingleRoomCornersToSingleDoor;
        }

        /// <summary>
        ///     The option for creating Path of Travel.
        /// </summary>
        public PathCreateOptions PathCreateOption { get; private set; }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.SingleRoomCornersToSingleDoor;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.AllRoomCenterToSingleDoor;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.AllRoomCornersToAllDoors;
        }
    }
}
