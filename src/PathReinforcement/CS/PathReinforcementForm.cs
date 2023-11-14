//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PathReinforcement.CS
{
    /// <summary>
    ///     Main form,it contains a picture box to display the path of path reinforcement and
    ///     a property grid to display the parameters of path reinforcement.
    /// </summary>
    public partial class PathReinforcementForm : Form
    {
        /// <summary>
        ///     path reinforcement object
        /// </summary>
        private Autodesk.Revit.DB.Structure.PathReinforcement m_pathRein;

        /// <summary>
        ///     profile object
        /// </summary>
        private readonly Profile m_profile;

        private readonly PathReinProperties m_properties;

        /// <summary>
        ///     Default constructor
        /// </summary>
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

        /// <summary>
        ///     update the SelectObject of PropertyGrid control
        /// </summary>
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

        /// <summary>
        ///     Ok button click event handler
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
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