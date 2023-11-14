//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.BoundaryConditions.CS
{
    /// <summary>
    ///     user enter a positive number as the SpringModulus
    /// </summary>
    public partial class SpringModulusForm : Form
    {
        // member
        private ConversionValue m_conversion; //conversion rule of current SpringModulus

        /// <summary>
        ///     constructor
        /// </summary>
        public SpringModulusForm()
        {
            InitializeComponent();
        }

        // property
        /// <summary>
        ///     set conversion rule between diaplay value and inside value of current SpringModulus
        /// </summary>
        public ConversionValue Conversion
        {
            get => m_conversion;
            set => m_conversion = value;
        }

        /// <summary>
        ///     get the new value after interact with user
        /// </summary>
        public double StringModulus { get; private set; }

        /// <summary>
        ///     set the old value before interact with user
        /// </summary>
        public double OldStringModulus { get; set; }

        /// <summary>
        ///     display the old value in the text box when show the interaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpringModulusForm_Load(object sender, EventArgs e)
        {
            // get the old value which is the inside value
            StringModulus = OldStringModulus;

            // conversion the inside value into display value
            var displaySpringModulus = OldStringModulus / m_conversion.Ratio;

            // deal with the diaplay value with the special percision
            displaySpringModulus = Math.Round(displaySpringModulus, m_conversion.Precision);

            // display the value and show user the unit of the value
            springModulusTextBox.Text = displaySpringModulus + m_conversion.UnitName;
            springModulusTextBox.Focus();
            springModulusTextBox.SelectAll();
        }

        /// <summary>
        ///     user affirm the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!springModulusTextBox.Focused) DialogResult = DialogResult.OK;
        }

        /// <summary>
        ///     user cancel the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            StringModulus = OldStringModulus;
        }

        /// <summary>
        ///     user can click Enter key to end of the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void springModulusTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (13 == e.KeyValue) okButton.Focus();
        }

        /// <summary>
        ///     check if the input is valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void springModulusTextBox_Leave(object sender, EventArgs e)
        {
            // store all the text of the text box
            var allText = springModulusTextBox.Text;
            // store the text of the text box after removing the unit and white chatracters
            var springModulusText = allText;

            // parse the text to get the valid springModulus
            if (allText.Contains(m_conversion.UnitName))
            {
                // removes the unit
                var index = allText.IndexOf(m_conversion.UnitName);
                springModulusText = allText.Substring(0, index);

                // removes all white chatracters from the beginning and end of this string 
                springModulusText = springModulusText.Trim();
            }

            try
            {
                // the spring modulus should be a double number
                var temp = double.Parse(springModulusText);

                // deal with the input with the given precision
                temp = Math.Round(temp, m_conversion.Precision);

                // add corresponding unit
                springModulusTextBox.Text = temp + m_conversion.UnitName;

                // convert the display value into the inside value
                StringModulus = temp * m_conversion.Ratio;

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