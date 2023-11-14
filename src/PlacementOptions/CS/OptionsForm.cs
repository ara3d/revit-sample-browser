// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.PlacementOptions.CS
{
    /// <summary>
    ///     The dialog for choosing the face based family instance or sketch based family instance.
    /// </summary>
    public partial class OptionsForm : Form
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public OptionsForm()
        {
            InitializeComponent();

            radioButton1.Checked = true;
            OptionType = PlacementOptionsEnum.FaceBased;
        }

        /// <summary>
        ///     The option for choosing the face based family instance or sketch based family instance.
        /// </summary>
        public PlacementOptionsEnum OptionType { get; private set; }

        /// <summary>
        ///     Use the PlacementOptionsEnum.FaceBased option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            OptionType = PlacementOptionsEnum.FaceBased;
        }

        /// <summary>
        ///     Use the PlacementOptionsEnum.SketchBased option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            OptionType = PlacementOptionsEnum.SketchBased;
        }
    }
}
