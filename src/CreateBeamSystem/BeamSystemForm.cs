// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public partial class BeamSystemForm : Form
    {
        private readonly BeamSystemData m_data;

        private readonly BeamSystemSketch m_sketch;

        public BeamSystemForm(BeamSystemData data)
        {
            InitializeComponent();
            m_data = data;
            // bound PictureBox to display the profile
            m_sketch = new BeamSystemSketch(previewPictureBox);
        }

        private void ParamsUpdated(object sender, EventArgs e)
        {
            beamSystemPropertyGrid.SelectedObject = null;
            beamSystemPropertyGrid.SelectedObject = m_data.Param;
        }

        private void BeamSystemForm_Load(object sender, EventArgs e)
        {
            // bound PropertyGrid to show beam system's properties
            beamSystemPropertyGrid.SelectedObject = m_data.Param;
            m_data.ParamsUpdated += ParamsUpdated;
            // draw the profile
            m_sketch.DrawProfile(m_data.Lines);
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

        private void changeDirectionButton_Click(object sender, EventArgs e)
        {
            m_data.ChangeProfileDirection();
            m_sketch.DrawProfile(m_data.Lines);
        }
    }
}
