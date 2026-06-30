// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Forms
{
    /// <summary>
    ///     This form is used to collect parameters for LWinder creation. It also validates the input
    ///     parameters and will warn if there is any invalid parameters when trying to submit the form.
    /// </summary>
    public partial class LWinderOptions : Form
    {
        public LWinderOptions()
        {
            InitializeComponent();
        }

        public uint NumStepsAtStart
        {
            get => uint.Parse(numAtStartTextBox.Text);
            set => numAtStartTextBox.Text = value.ToString();
        }

        public uint NumStepsInCorner
        {
            get => uint.Parse(numInCornerTextBox.Text);
            set => numInCornerTextBox.Text = value.ToString();
        }

        public uint NumStepsAtEnd
        {
            get => uint.Parse(numAtEndTextBox.Text);
            set => numAtEndTextBox.Text = value.ToString();
        }

        public double RunWidth
        {
            get => double.Parse(runWidthTextBox.Text);
            set => runWidthTextBox.Text = value.ToString();
        }

        public double CenterOffsetE
        {
            get => double.Parse(centerOffsetETextBox.Text);
            set => centerOffsetETextBox.Text = value.ToString();
        }

        public double CenterOffsetF
        {
            get => double.Parse(centerOffsetFTextBox.Text);
            set => centerOffsetFTextBox.Text = value.ToString();
        }

        public bool Dmu => dmuCheckBox.Checked;

        public bool Sketch => sketchCheckBox.Checked;

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

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                Close();
                DialogResult = DialogResult.OK;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
