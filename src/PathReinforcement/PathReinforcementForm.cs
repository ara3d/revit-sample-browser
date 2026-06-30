// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PathReinforcement.CS
{
    /// <summary>
    ///     Main form,it contains a picture box to display the path of path reinforcement and
    ///     a property grid to display the parameters of path reinforcement.
    /// </summary>
    public partial class PathReinforcementForm : Form
    {
        private readonly Autodesk.Revit.DB.Structure.PathReinforcement m_pathRein;

        private readonly Profile m_profile;

        private readonly PathReinProperties m_properties;

        public PathReinforcementForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Overload constructor
        /// </summary>
        /// <param name="pathRein">path reinforcement object</param>
        /// <param name="commandData">revit command data</param>
        public PathReinforcementForm(Autodesk.Revit.DB.Structure.PathReinforcement pathRein,
            ExternalCommandData commandData) : this()
        {
            m_pathRein = pathRein;
            m_properties = new PathReinProperties(pathRein);
            m_properties.UpdateSelectObjEvent += UpdatePropSelectedObject;
            propertyGrid.SelectedObject = m_properties;
            m_profile = new Profile(pathRein, commandData);
        }

        private void UpdatePropSelectedObject()
        {
            propertyGrid.SelectedObject = null;
            propertyGrid.SelectedObject = m_properties;
            propertyGrid.Update();
        }

        /// <summary>
        ///     cancel button click event handler.
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton1_Click(object sender, EventArgs e)
        {
            m_properties.Update();
            Close();
        }

        /// <summary>
        ///     Picture box paint event handler.
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            var size = pictureBox.Size;
            m_profile.Draw(e.Graphics, size, Pens.Red);
        }
    }
}
