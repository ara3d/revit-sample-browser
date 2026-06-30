// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ReferencePlane.CS
{
    public partial class ReferencePlaneForm : Form
    {
        private readonly ReferencePlaneMgr m_refPlaneMgr;

        public ReferencePlaneForm(ReferencePlaneMgr refPlaneMgr)
        {
            Debug.Assert(null != refPlaneMgr);
            InitializeComponent();

            m_refPlaneMgr = refPlaneMgr;

            refPlanesDataGridView.DataSource = m_refPlaneMgr.ReferencePlanes;

            refPlanesDataGridView.Columns[0].Width = (int)(refPlanesDataGridView.Width * 0.13);
            refPlanesDataGridView.Columns[1].Width = (int)(refPlanesDataGridView.Width * 0.29);
            refPlanesDataGridView.Columns[2].Width = (int)(refPlanesDataGridView.Width * 0.29);
            refPlanesDataGridView.Columns[3].Width = (int)(refPlanesDataGridView.Width * 0.29);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_refPlaneMgr.Create();
        }
    }
}
