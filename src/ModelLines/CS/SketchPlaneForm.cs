// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ModelLines.CS
{
    /// <summary>
    ///     This UserControl is used to collect the information for sketch plane creation
    /// </summary>
    public partial class SketchPlaneForm : Form
    {
        // Private members
        private readonly ModelLines m_dataBuffer; // A reference of ModelLines.

        /// <summary>
        ///     Constructor of SketchPlaneForm
        /// </summary>
        /// <param name="dataBuffer">a reference of ModelLines class</param>
        public SketchPlaneForm(ModelLines dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of ModelLines
            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Check the data which the user input are integrated or not
        /// </summary>
        /// <returns>If the data are integrated return true, otherwise false</returns>
        private bool AssertDataIntegrity()
        {
            return normalUserControl.AssertPointIntegrity()
                   && originUserControl.AssertPointIntegrity();
        }

        /// <summary>
        ///     The event method for okButton click
        /// </summary>
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

        /// <summary>
        ///     The event method for cancelButton click
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
