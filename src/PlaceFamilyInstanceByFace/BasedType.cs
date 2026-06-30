// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PlaceFamilyInstanceByFace.CS
{
    /// <summary>
    ///     This form class is for user choose a based-type of creating family instance
    /// </summary>
    public partial class BasedTypeForm : Form
    {
        public BasedTypeForm()
        {
            InitializeComponent();
        }
        // based-type

        /// <summary>
        ///     based-type
        /// </summary>
        public BasedType BaseType { get; private set; } = BasedType.Point;

        private void buttonNext_Click(object sender, EventArgs e)
        {
            BaseType = radioButtonPoint.Checked
                ? BasedType.Point
                : radioButtonLine.Checked ? BasedType.Line : throw new Exception("An error occured in selecting based type.");
            Close();
            DialogResult = DialogResult.OK;
        }

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
