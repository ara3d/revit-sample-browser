// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ModelLines.CS
{
    /// <summary>
    ///     This UserControl is used to collect the information for sketch plane creation
    /// </summary>
    public partial class SketchPlaneForm : Form
    {
        // Private members
        private readonly ModelLines m_dataBuffer; // A reference of ModelLines.

        public SketchPlaneForm(ModelLines dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of ModelLines
            m_dataBuffer = dataBuffer;
        }

        private bool AssertDataIntegrity()
        {
            return normalUserControl.AssertPointIntegrity()
                   && originUserControl.AssertPointIntegrity();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // First, check data integrity 
            if (!AssertDataIntegrity())
            {
                TaskDialog.Show("Revit", "Please make the data integrated first.");
                return;
            }

            try
            {
                // Get the necessary information and invoke the method to create sketch plane
                var normal = normalUserControl.GetPointData();
                var origin = originUserControl.GetPointData();
                m_dataBuffer.CreateSketchPlane(normal, origin);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
                return;
            }

            // If the creation is successful, close this form
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
