// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS
{
    /// <summary>
    ///     This form provides an entrance for user to add parameters to RebarShape.
    /// </summary>
    public partial class AddParameter : Form
    {
        private readonly List<RebarShapeParameter> m_parameterList;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public AddParameter()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor, initialize fields.
        /// </summary>
        /// <param name="list"></param>
        public AddParameter(List<RebarShapeParameter> list)
            : this()
        {
            m_parameterList = list;
        }

        /// <summary>
        ///     Is it formula parameter or not?
        /// </summary>
        public bool IsFormula => paramFormulaRadioButton.Checked;

        /// <summary>
        ///     Parameter name from paramNameTextBox.
        /// </summary>
        public string ParamName => paramNameTextBox.Text;

        /// <summary>
        ///     Parameter value from paramValueTextBox.
        /// </summary>
        public string ParamValue => paramValueTextBox.Text;

        /// <summary>
        ///     Cancel Button, Return DialogResult.Cancel and close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     OK button, check the correction of data, then Return DialogResult.OK
        ///     and close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ParamName) || string.IsNullOrEmpty(ParamValue))
            {
                TaskDialog.Show("Revit", "Parameter Name and Value should not be null.");
                return;
            }

            if (!IsFormula)
                try
                {
                    double.Parse(ParamValue);
                }
                catch
                {
                    TaskDialog.Show("Revit", "Input value - " + ParamValue + " - should be double.");
                    return;
                }

            // Make sure Parameter name should be started with letter
            // And just contains letters, numbers and underlines 
            var regex = new Regex("^[a-zA-Z]\\w*$");
            if (!regex.IsMatch(ParamName))
            {
                TaskDialog.Show("Revit",
                    "Parameter name should be started with letter \r\n And just contains letters, numbers and underlines.");
                paramNameTextBox.Focus();
                return;
            }

            // Make sure the name is unique.
            foreach (var param in m_parameterList)
                if (param.Name.Equals(ParamName))
                {
                    TaskDialog.Show("Revit", "The name is already exist, please input again.");
                    paramNameTextBox.Focus();
                    return;
                }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
