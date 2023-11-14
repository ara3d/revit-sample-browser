// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.Journaling.CS
{
    /// <summary>
    ///     The form used to collect the support data for wall creation and store in journal
    /// </summary>
    public partial class JournalingForm : Form
    {
        // Private members
        private const double Precision = 0.00001; //precision when judge whether two doubles are equal
        private readonly Journaling m_dataBuffer; // A reference of Journaling.


        // Methods
        /// <summary>
        ///     Constructor of JournalingForm
        /// </summary>
        /// <param name="dataBuffer">A reference of Journaling class</param>
        public JournalingForm(Journaling dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of ModelLines
            m_dataBuffer = dataBuffer;

            // Bind the data source of the typeComboBox and levelComboBox
            typeComboBox.DataSource = m_dataBuffer.WallTypes;
            typeComboBox.DisplayMember = "Name";
            levelComboBox.DataSource = m_dataBuffer.Levels;
            levelComboBox.DisplayMember = "Name";
        }


        /// <summary>
        ///     The okButton click event method,
        ///     this method collect the data, and pass them to the journaling class
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            // Get the support data from the UI controls
            var startPoint = startPointUserControl.GetPointData(); // start point 
            var endPoint = endPointUserControl.GetPointData(); // end point
            if (startPoint.Equals(endPoint)) // Don't allow start point equals end point
            {
                TaskDialog.Show("Revit", "Start point should not equal end point.");
                return;
            }

            var diff = Math.Abs(startPoint.Z - endPoint.Z);
            if (diff > Precision)
            {
                TaskDialog.Show("Revit", "Z coordinate of start and end points should be equal.");
                return;
            }

            if (!(levelComboBox.SelectedItem is Level level)) // assert it isn't null
            {
                TaskDialog.Show("Revit", "The selected level is null or incorrect.");
                return;
            }

            if (!(typeComboBox.SelectedItem is WallType type)) // assert it isn't null
            {
                TaskDialog.Show("Revit", "The selected wall type is null or incorrect.");
                return;
            }

            // Invoke SetNecessaryData method to set the collected support data 
            m_dataBuffer.SetNecessaryData(startPoint, endPoint, level, type);

            // Set result information and close the form
            DialogResult = DialogResult.OK;
            Close();
        }


        /// <summary>
        ///     The cancelButton click event method
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Only set result to be cancel and close the form
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
