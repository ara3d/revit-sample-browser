// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.CurvedBeam.CS
{
    public partial class CurvedBeamForm : Form
    {
        private CurvedBeamForm()
        {
            InitializeComponent();
        }

        public CurvedBeamForm(Command dataBuffer)
        {
            m_dataBuffer = dataBuffer;
            InitializeComponent();
        }

        private void newArcButton_Click(object sender, EventArgs e)
        {
            var locLev = LevelCB.SelectedValue as Level;
            var beamArc = m_dataBuffer.CreateArc(locLev.Elevation);
            var succeed = m_dataBuffer.CreateCurvedBeam(BeamTypeCB.SelectedValue as FamilySymbol,
                beamArc, locLev);
            if (succeed) TaskDialog.Show("Revit", "Succeeded to create Arc beam.");
        }

        private void newNurbSplineButton_Click(object sender, EventArgs e)
        {
            var locLev = LevelCB.SelectedValue as Level;
            var beamNs = m_dataBuffer.CreateNurbSpline(locLev.Elevation);
            var succeed = m_dataBuffer.CreateCurvedBeam(BeamTypeCB.SelectedValue as FamilySymbol,
                beamNs, locLev);
            if (succeed) TaskDialog.Show("Revit", "Succeeded to create NurbSpline beam.");
        }

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
