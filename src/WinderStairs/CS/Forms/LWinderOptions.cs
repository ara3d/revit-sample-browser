// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.WinderStairs.CS
{
    /// <summary>
    ///     This form is used to collect parameters for LWinder creation. It also validates the input
    ///     parameters and will warn if there is any invalid parameters when trying to submit the form.
    /// </summary>
    internal partial class LWinderOptions : Form
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public LWinderOptions()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Number of straight steps at start.
        /// </summary>
        public uint NumStepsAtStart
        {
            get => uint.Parse(numAtStartTextBox.Text);
            set => numAtStartTextBox.Text = value.ToString();
        }

        /// <summary>
        ///     Number of straight steps in winder corner.
        /// </summary>
        public uint NumStepsInCorner
        {
            get => uint.Parse(numInCornerTextBox.Text);
            set => numInCornerTextBox.Text = value.ToString();
        }

        /// <summary>
        ///     Number of straight steps at end.
        /// </summary>
        public uint NumStepsAtEnd
        {
            get => uint.Parse(numAtEndTextBox.Text);
            set => numAtEndTextBox.Text = value.ToString();
        }

        /// <summary>
        ///     Winder stairs run width.
        /// </summary>
        public double RunWidth
        {
            get => double.Parse(runWidthTextBox.Text);
            set => runWidthTextBox.Text = value.ToString();
        }

        /// <summary>
        ///     Center point offset distance from internal boundary.
        /// </summary>
        public double CenterOffsetE
        {
            get => double.Parse(centerOffsetETextBox.Text);
            set => centerOffsetETextBox.Text = value.ToString();
        }

        /// <summary>
        ///     Center point offset distance from internal boundary.
        /// </summary>
        public double CenterOffsetF
        {
            get => double.Parse(centerOffsetFTextBox.Text);
            set => centerOffsetFTextBox.Text = value.ToString();
        }

        /// <summary>
        ///     A switch indicates whether to support DMU(dynamic model update).
        /// </summary>
        public bool Dmu => dmuCheckBox.Checked;

        /// <summary>
        ///     A switch to control the sketch drawing of winder stairs.
        /// </summary>
        public bool Sketch => sketchCheckBox.Checked;

        /// <summary>
        ///     Validate the UI input and it will warn if there are invalid user inputs.
        /// </summary>
        /// <returns>true if all input is fine.</returns>
        private bool ValidateInput()
        {
            double runWidth;
            if (!double.TryParse(runWidthTextBox.Text, out runWidth) || runWidth < 1.0e-6)
            {
                TaskDialog.Show("L-Winder Warning", "Run Width should be positive double", TaskDialogCommonButtons.Ok);
                runWidthTextBox.Focus();
                runWidthTextBox.SelectAll();
                return false;
            }

            if (!uint.TryParse(numAtStartTextBox.Text, out _))
            {
                TaskDialog.Show("L-Winder Warning", "Start steps should be unsigned integer",
                    TaskDialogCommonButtons.Ok);
                numAtStartTextBox.Focus();
                numAtStartTextBox.SelectAll();
                return false;
            }

            if (!uint.TryParse(numAtEndTextBox.Text, out _))
            {
                TaskDialog.Show("L-Winder Warning", "End steps should be unsigned integer", TaskDialogCommonButtons.Ok);
                numAtEndTextBox.Focus();
                numAtEndTextBox.SelectAll();
                return false;
            }

            uint numInCorner;
            if (!uint.TryParse(numInCornerTextBox.Text, out numInCorner) || numInCorner < 1)
            {
                TaskDialog.Show("L-Winder Warning", "Corner steps should be unsigned integer and >= 1",
                    TaskDialogCommonButtons.Ok);
                numInCornerTextBox.Focus();
                numInCornerTextBox.SelectAll();
                return false;
            }

            double offsetE;
            if (!double.TryParse(centerOffsetETextBox.Text, out offsetE) || offsetE < 0.0)
            {
                TaskDialog.Show("L-Winder Warning", "Center offset (E) should be non-negative double",
                    TaskDialogCommonButtons.Ok);
                centerOffsetETextBox.Focus();
                centerOffsetETextBox.SelectAll();
                return false;
            }

            double offsetF;
            if (!double.TryParse(centerOffsetFTextBox.Text, out offsetF) || offsetF < 0.0)
            {
                TaskDialog.Show("L-Winder Warning", "Center offset (F) should be non-negative double",
                    TaskDialogCommonButtons.Ok);
                centerOffsetFTextBox.Focus();
                centerOffsetFTextBox.SelectAll();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Submit the UI input.
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                Close();
                DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        ///     Cancel the UI input.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
