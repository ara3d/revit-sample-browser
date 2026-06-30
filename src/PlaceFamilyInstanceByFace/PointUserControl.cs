// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS
{
    public partial class PointUserControl : UserControl
    {
        public PointUserControl()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // initialize the TextBox data
            xCoordinateTextBox.Text = "0";
            yCoordinateTextBox.Text = "0";
            zCoordinateTextBox.Text = "0";
        }

        public XYZ GetPointData()
        {
            var x = Convert.ToDouble(xCoordinateTextBox.Text); // Get x coordinate
            // Store the temporary x coordinate
            var y = Convert.ToDouble(yCoordinateTextBox.Text); // Get x coordinate
            // Store the temporary y coordinate
            var z = Convert.ToDouble(zCoordinateTextBox.Text); // Get x coordinate
            // Store the temporary z coordinate 
            return new XYZ(x, y, z);
        }

        public void SetPointData(XYZ data)
        {
            xCoordinateTextBox.Text = data.X.ToString("F2");
            yCoordinateTextBox.Text = data.Y.ToString("F2");
            zCoordinateTextBox.Text = data.Z.ToString("F2");
        }

        public bool AssertPointIntegrity()
        {
            return !string.IsNullOrEmpty(xCoordinateTextBox.Text) && ! // x coordinate empty
                string.IsNullOrEmpty(yCoordinateTextBox.Text) && ! // y coordinate empty
                string.IsNullOrEmpty(zCoordinateTextBox.Text); // z coordinate empty
            // If all coordinates are not empty, return true
        }

        private void CoordinateTextBox_Validating(object sender, CancelEventArgs e)
        {
            // Check whether the sender is a TextBox reference
            if (sender is not TextBox numberTextBox)
                // If it is not a TextBox, just return
                return;

            // Invoke IsNumber() method to judge whether the input data are right
            if (!IsNumber(numberTextBox.Text))
                // If not, give error information, and set the text to be empty
                TaskDialog.Show("Revit", "Please input a double data.");
        }

        public static bool IsNumber(string number)
        {
            // First check whether the string is empty
            if (string.IsNullOrEmpty(number))
                // If the string is empty, return true
                return true;

            // Use Convert.ToDouble() method to changed string to double,
            // If an exception is thrown out, that means the string can't change 
            try
            {
                // Invoke Convert.ToDouble() method
                Convert.ToDouble(number);
            }
            catch (Exception)
            {
                return false;
            }

            // If everything goes well, return true
            return true;
        }
    }
}
