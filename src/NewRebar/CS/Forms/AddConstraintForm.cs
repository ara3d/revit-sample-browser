// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS
{
    /// <summary>
    ///     This form provides an entrance for user to add constraints to RebarShape.
    /// </summary>
    public partial class AddConstraint : Form
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public AddConstraint()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Constructor, Initialize fields.
        /// </summary>
        /// <param name="constraintTypes"></param>
        public AddConstraint(List<Type> constraintTypes)
            : this()
        {
            constraintTypesComboBox.DataSource = constraintTypes;
            constraintTypesComboBox.DisplayMember = "Name";
        }

        /// <summary>
        ///     Return the type from constraintTypesComboBox selection.
        /// </summary>
        public Type ConstraintType => constraintTypesComboBox.SelectedItem as Type;

        /// <summary>
        ///     OK Button, Return DialogResult.OK and close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

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
    }
}
