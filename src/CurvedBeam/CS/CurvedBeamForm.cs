// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.CurvedBeam.CS
{
    /// <summary>
    ///     new beam form
    /// </summary>
    public partial class CurvedBeamForm : Form
    {
        /// <summary>
        ///     default construction is forbidden
        /// </summary>
        private CurvedBeamForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     get relevant data from Revit
        /// </summary>
        /// <param name="dataBuffer">relevant review data</param>
        public CurvedBeamForm(Command dataBuffer)
        {
            m_dataBuffer = dataBuffer;
            InitializeComponent();
        }

        /// <summary>
        ///     create Arc beam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newArcButton_Click(object sender, EventArgs e)
        {
            var locLev = LevelCB.SelectedValue as Level;
            var beamArc = m_dataBuffer.CreateArc(locLev.Elevation);
            var succeed = m_dataBuffer.CreateCurvedBeam(BeamTypeCB.SelectedValue as FamilySymbol,
                beamArc, locLev);
            if (succeed) TaskDialog.Show("Revit", "Succeeded to create Arc beam.");
        }

        /// <summary>
        ///     create Nurbspline beam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newNurbSplineButton_Click(object sender, EventArgs e)
        {
            var locLev = LevelCB.SelectedValue as Level;
            var beamNs = m_dataBuffer.CreateNurbSpline(locLev.Elevation);
            var succeed = m_dataBuffer.CreateCurvedBeam(BeamTypeCB.SelectedValue as FamilySymbol,
                beamNs, locLev);
            if (succeed) TaskDialog.Show("Revit", "Succeeded to create NurbSpline beam.");
        }

        /// <summary>
        ///     create nurb spline beam
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newEllipseButton_Click(object sender, EventArgs e)
        {
            var locLev = LevelCB.SelectedValue as Level;

            var beamEllipse = m_dataBuffer.CreateEllipse(locLev.Elevation);
            var succeed = m_dataBuffer.CreateCurvedBeam(BeamTypeCB.SelectedValue as FamilySymbol,
                beamEllipse, locLev);
            if (succeed) TaskDialog.Show("Revit", "Succeeded to create Ellipse beam.");
        }
    }
}
