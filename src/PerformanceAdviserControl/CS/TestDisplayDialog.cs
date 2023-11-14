// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.PerformanceAdviserControl.CS
{
    public partial class TestDisplayDialog : Form
    {
        private readonly Document m_document;
        private readonly PerformanceAdviser m_performanceAdviser;

        /// <summary>
        ///     Basic setup -- stores references to the active document and PerformanceAdviser for later use
        /// </summary>
        /// <param name="performanceAdviser">The revit PerformanceAdviser class</param>
        /// <param name="document">The active document</param>
        public TestDisplayDialog(PerformanceAdviser performanceAdviser, Document document)
        {
            m_performanceAdviser = performanceAdviser;
            m_document = document;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Fixed3D;
        }

        /// <summary>
        ///     Called when the user clicks the "Run Selected Tests" button
        /// </summary>
        private void btn_RunTests_Click(object sender, EventArgs e)
        {
            Close();

            //Iterate through each item in the dialog data grid.
            //Check to see if the user selected a specific rule to be enabled.
            //Set the rule to the same enabled state in PerformanceAdviser using
            //PerformanceAdviser::SetRuleDisabled.
            var testIndex = 0;
            foreach (DataGridViewRow row in testData.Rows)
            {
                var isEnabled = (bool)row.Cells[0].Value;
                m_performanceAdviser.SetRuleEnabled(testIndex, isEnabled);
                Debug.WriteLine("Test Name: " + m_performanceAdviser.GetRuleName(testIndex) + " Enabled? " +
                                !m_performanceAdviser.IsRuleEnabled(testIndex));
                testIndex++;
            }

            //Run all rules that are currently enabled and report errors
            var failures = m_performanceAdviser.ExecuteAllRules(m_document);
            foreach (var fm in failures)
            {
                var tFailure = new Transaction(m_document, "Failure Reporting Transaction");
                tFailure.Start();
                m_document.PostFailure(fm);
                tFailure.Commit();
            }
        }

        /// <summary>
        ///     Called when the user clicks the "Select All" button.
        /// </summary>
        private void btn_SelectAll_Click(object sender, EventArgs e)
        {
            //Set the first column value (the enabled status) to true;
            foreach (DataGridViewRow row in testData.Rows) row.Cells[0].Value = true;
        }

        /// <summary>
        ///     Called when the user clicks the "Deselect All" button.
        /// </summary>
        private void btn_DeselectAll_Click(object sender, EventArgs e)
        {
            //Set the first column value (the enabled status) to false;
            foreach (DataGridViewRow row in testData.Rows) row.Cells[0].Value = false;
        }

        /// <summary>
        ///     Closes the dialog without committing any action
        /// </summary>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     This method is called by UICommand::Execute() and adds test information to the grid
        ///     data object "testData."
        /// </summary>
        /// <param name="name">The rule name</param>
        /// <param name="isOurRule">Whether or not the rule is a the API-defined rule in this project</param>
        /// <param name="isEnabled">Whether or not the rule is currently selected to run</param>
        public void AddData(string name, bool isOurRule, bool isEnabled)
        {
            var data = new object[3];
            data[0] = isEnabled;
            data[1] = name;
            data[2] = isOurRule ? "Yes" : "No";

            testData.Rows.Add(data);
        }
    }
}
