// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ReferencePlane.CS
{
    /// <summary>
    ///     A form display all reference planes, and allow user to create
    ///     reference plane with a button.
    /// </summary>
    public partial class ReferencePlaneForm : Form
    {
        //A object to manage reference plane.
        private readonly ReferencePlaneMgr m_refPlaneMgr;

        /// <summary>
        ///     A form object constructor.
        /// </summary>
        /// <param name="refPlaneMgr">A ReferencePlaneMgr buffer.</param>
        public ReferencePlaneForm(ReferencePlaneMgr refPlaneMgr)
        {
            Debug.Assert(null != refPlaneMgr);
            InitializeComponent();

            m_refPlaneMgr = refPlaneMgr;

            // Set up the data source.
            refPlanesDataGridView.DataSource = m_refPlaneMgr.ReferencePlanes;

            refPlanesDataGridView.Columns[0].Width = (int)(refPlanesDataGridView.Width * 0.13);
            refPlanesDataGridView.Columns[1].Width = (int)(refPlanesDataGridView.Width * 0.29);
            refPlanesDataGridView.Columns[2].Width = (int)(refPlanesDataGridView.Width * 0.29);
            refPlanesDataGridView.Columns[3].Width = (int)(refPlanesDataGridView.Width * 0.29);
        }

        /// <summary>
        ///     Notify revit to generate a reference plane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            m_refPlaneMgr.Create();
        }
    }
}
