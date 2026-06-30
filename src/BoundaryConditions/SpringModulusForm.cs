// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.BoundaryConditions.CS
{
    public partial class SpringModulusForm : Form
    {
        // member

        public SpringModulusForm()
        {
            InitializeComponent();
        }

        // property
        public ConversionValue Conversion { get; set; }

        public double StringModulus { get; private set; }

        public double OldStringModulus { get; set; }

        private void SpringModulusForm_Load(object sender, EventArgs e)
        {
            // get the old value which is the inside value
            StringModulus = OldStringModulus;

            // conversion the inside value into display value
            var displaySpringModulus = OldStringModulus / Conversion.Ratio;

            // deal with the diaplay value with the special percision
            displaySpringModulus = Math.Round(displaySpringModulus, Conversion.Precision);

            // display the value and show user the unit of the value
            springModulusTextBox.Text = displaySpringModulus + Conversion.UnitName;
            springModulusTextBox.Focus();
            springModulusTextBox.SelectAll();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!springModulusTextBox.Focused) DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            StringModulus = OldStringModulus;
        }

        private void springModulusTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (13 == e.KeyValue) okButton.Focus();
        }

        private void springModulusTextBox_Leave(object sender, EventArgs e)
        {
            // store all the text of the text box
            var allText = springModulusTextBox.Text;
            // store the text of the text box after removing the unit and white chatracters
            var springModulusText = allText;

            // parse the text to get the valid springModulus
            if (allText.Contains(Conversion.UnitName))
            {
                // removes the unit
                var index = allText.IndexOf(Conversion.UnitName);
                springModulusText = allText.Substring(0, index);

                // removes all white chatracters from the beginning and end of this string 
                springModulusText = springModulusText.Trim();
            }

            try
            {
                // the spring modulus should be a double number
                var temp = double.Parse(springModulusText);

                // deal with the input with the given precision
                temp = Math.Round(temp, Conversion.Precision);

                // add corresponding unit
                springModulusTextBox.Text = temp + Conversion.UnitName;

                // convert the display value into the inside value
                StringModulus = temp * Conversion.Ratio;

                // the value must be a positive number
                if (0 >= StringModulus)
                {
                    springModulusTextBox.Focus();
                    springModulusTextBox.SelectAll();
                }
            }
            catch (Exception)
            {
                springModulusTextBox.Focus();
                springModulusTextBox.SelectAll();
            }
        }
    }
}
