// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;
namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public partial class CreateFrameForm : Form
    {
        private readonly FrameData m_frameData;

        private CreateFrameForm()
        {
        }

        public CreateFrameForm(FrameData data)
        {
            InitializeComponent();
            m_frameData = data;
        }

        private void CreateFramingForm_Load(object sender, EventArgs e)
        {
            distanceTextBox.Text = m_frameData.Distance.ToString();
            xNumberTextBox.Text = m_frameData.XNumber.ToString();
            yNumberTextBox.Text = m_frameData.YNumber.ToString();
            floorNumberTextBox.Text = m_frameData.FloorNumber.ToString();
            levelHeightTextBox.Text = m_frameData.LevelHeight.ToString();
            originXtextBox.Text = m_frameData.FrameOrigin.U.ToString();
            originYtextBox.Text = m_frameData.FrameOrigin.V.ToString();
            originAngletextBox.Text = m_frameData.FrameOriginAngle.ToString();

            SampleBrowserUtils.RefreshListControl(columnTypeComboBox, m_frameData.ColumnSymbolsMgr);
            SampleBrowserUtils.RefreshListControl(beamTypeComboBox, m_frameData.BeamSymbolsMgr);
            SampleBrowserUtils.RefreshListControl(braceTypeComboBox, m_frameData.BraceSymbolsMgr);
        }

        private void columnDuplicateButton_Click(object sender, EventArgs e)
        {
            if (SampleBrowserUtils.DuplicateSymbol(m_frameData.ColumnSymbolsMgr, columnTypeComboBox.SelectedValue))
                SampleBrowserUtils.RefreshListControl(columnTypeComboBox, m_frameData.ColumnSymbolsMgr);
        }

        private void beamDuplicateButton_Click(object sender, EventArgs e)
        {
            if (SampleBrowserUtils.DuplicateSymbol(m_frameData.BeamSymbolsMgr, beamTypeComboBox.SelectedValue))
            {
                SampleBrowserUtils.RefreshListControl(beamTypeComboBox, m_frameData.BeamSymbolsMgr);
                SampleBrowserUtils.RefreshListControl(braceTypeComboBox, m_frameData.BraceSymbolsMgr);
            }
        }

        private void braceDuplicateButton_Click(object sender, EventArgs e)
        {
            if (SampleBrowserUtils.DuplicateSymbol(m_frameData.BraceSymbolsMgr, braceTypeComboBox.SelectedValue))
            {
                SampleBrowserUtils.RefreshListControl(beamTypeComboBox, m_frameData.BeamSymbolsMgr);
                SampleBrowserUtils.RefreshListControl(braceTypeComboBox, m_frameData.BraceSymbolsMgr);
            }
        }

        private void distanceTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = double.Parse(distanceTextBox.Text);
                m_frameData.Distance = value;
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a integer.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                distanceTextBox.Text = m_frameData.Distance.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
        }

        private void xNumberTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = int.Parse(xNumberTextBox.Text);
                m_frameData.XNumber = value;
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a integer.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                xNumberTextBox.Text = m_frameData.XNumber.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                TaskDialog.Show("Revit", "Please input a valid integer.");
                e.Cancel = true;
            }
        }

        private void yNumberTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = int.Parse(yNumberTextBox.Text);
                m_frameData.YNumber = value;
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a integer.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                yNumberTextBox.Text = m_frameData.YNumber.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                TaskDialog.Show("Revit", "Please input a valid integer.");
                e.Cancel = true;
            }
        }

        private void floorNumberTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = int.Parse(floorNumberTextBox.Text);
                m_frameData.FloorNumber = value;
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a integer.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                floorNumberTextBox.Text = m_frameData.FloorNumber.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                TaskDialog.Show("Revit", "Please input a valid integer.");
                e.Cancel = true;
            }

            levelHeightTextBox.Enabled = m_frameData.FloorNumber + 1 > m_frameData.Levels.Count;
        }

        private void levelHeightTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = double.Parse(levelHeightTextBox.Text);
                m_frameData.LevelHeight = value;
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a integer.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                levelHeightTextBox.Text = m_frameData.LevelHeight.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
        }

        private void originXtextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = double.Parse(originXtextBox.Text);
                var oldValue = m_frameData.FrameOrigin;
                m_frameData.FrameOrigin = new UV(value, oldValue.V);
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a number.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                originXtextBox.Text = m_frameData.FrameOrigin.U.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
        }

        private void originYtextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var value = double.Parse(originYtextBox.Text);
                var oldValue = m_frameData.FrameOrigin;
                m_frameData.FrameOrigin = new UV(oldValue.U, value);
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a number.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                originYtextBox.Text = m_frameData.FrameOrigin.V.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
        }

        private void originAngletextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                m_frameData.FrameOriginAngle = double.Parse(originAngletextBox.Text);
            }
            catch (FormatException formatEx)
            {
                Debug.WriteLine(formatEx.Message);
                TaskDialog.Show("Revit", "Please input a number.");
                e.Cancel = true;
            }
            catch (ErrorMessageException msgEx)
            {
                originAngletextBox.Text = m_frameData.FrameOriginAngle.ToString();
                TaskDialog.Show("Revit", msgEx.Message);
                e.Cancel = true;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void columnTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_frameData.SetColumnSymbol(columnTypeComboBox.SelectedItem);
        }

        private void beamTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_frameData.SetBeamSymbol(beamTypeComboBox.SelectedItem);
        }

        private void braceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_frameData.SetBraceSymbol(braceTypeComboBox.SelectedItem);
        }
    }
}
