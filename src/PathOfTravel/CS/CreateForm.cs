using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.PathOfTravelCreation.CS
{
    /// <summary>
    ///     Form presented to the user to fill in the options to control the path of travel creation.
    /// </summary>
    public partial class CreateForm : Form
    {
        /// <summary>
        ///     Constructor
        /// </summary>
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

        /// <summary>
        ///     Set the CreateOptions.SingleRoomCornersToSingleDoor option.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.SingleRoomCornersToSingleDoor;
        }

        /// <summary>
        ///     Set the CreateOptions.AllRoomCenterToSingleDoor option.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.AllRoomCenterToSingleDoor;
        }

        /// <summary>
        ///     Set the CreateOptions.AllRoomCornersToAllDoors option.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            PathCreateOption = PathCreateOptions.AllRoomCornersToAllDoors;
        }
    }
}