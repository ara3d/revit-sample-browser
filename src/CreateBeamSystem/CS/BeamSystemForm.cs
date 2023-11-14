// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     display beam system to be created and allow user to set its properties
    /// </summary>
    public partial class BeamSystemForm : Form
    {
        /// <summary>
        ///     buffer of data related to UI
        /// </summary>
        private readonly BeamSystemData m_data;

        /// <summary>
        ///     class to draw profile of beam system in PictureBox
        /// </summary>
        private readonly BeamSystemSketch m_sketch;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="data">data related to UI</param>
        public BeamSystemForm(BeamSystemData data)
        {
            InitializeComponent();
            m_data = data;
            // bound PictureBox to display the profile
            m_sketch = new BeamSystemSketch(previewPictureBox);
        }

        /// <summary>
        ///     update PropertyGrid when BeamSystemParams bound to beam system updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParamsUpdated(object sender, EventArgs e)
        {
            beamSystemPropertyGrid.SelectedObject = null;
            beamSystemPropertyGrid.SelectedObject = m_data.Param;
        }

        /// <summary>
        ///     form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeamSystemForm_Load(object sender, EventArgs e)
        {
            // bound PropertyGrid to show beam system's properties
            beamSystemPropertyGrid.SelectedObject = m_data.Param;
            m_data.ParamsUpdated += ParamsUpdated;
            // draw the profile
            m_sketch.DrawProfile(m_data.Lines);
        }

        /// <summary>
        ///     to create beam system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     cancel all command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     change the direction of beam system to the next line in the profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeDirectionButton_Click(object sender, EventArgs e)
        {
            m_data.ChangeProfileDirection();
            m_sketch.DrawProfile(m_data.Lines);
        }
    }
}
