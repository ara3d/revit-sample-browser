// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.PlaceFamilyInstanceByFace.CS
{
    /// <summary>
    ///     This form class is for user choose a based-type of creating family instance
    /// </summary>
    public partial class BasedTypeForm : Form
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public BasedTypeForm()
        {
            InitializeComponent();
        }
        // based-type


        /// <summary>
        ///     based-type
        /// </summary>
        public BasedType BaseType { get; private set; } = BasedType.Point;

        /// <summary>
        ///     Process the click event of "Next" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (radioButtonPoint.Checked)
                BaseType = BasedType.Point;
            else if (radioButtonLine.Checked)
                BaseType = BasedType.Line;
            else
                throw new Exception("An error occured in selecting based type.");
            Close();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        ///     Process the click event of "Cancel" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }
    }

    /// <summary>
    ///     Based-type
    /// </summary>
    public enum BasedType
    {
        /// <summary>
        ///     Point-based
        /// </summary>
        Point = 0,

        /// <summary>
        ///     Line-based
        /// </summary>
        Line
    }
}
