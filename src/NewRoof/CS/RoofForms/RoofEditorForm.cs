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
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace Revit.SDK.Samples.NewRoof.RoofForms.CS
{
    /// <summary>
    ///     The RoofEditorForm is the main edit form to edit a roof data.
    /// </summary>
    public partial class RoofEditorForm : Form
    {
        // To store the ExtrusionRoofWrapper data of the roof.
        private readonly ExtrusionRoofWrapper m_extrusionRoofWrapper;

        // To store the FootPrintRoofWrapper data of the roof.
        private readonly FootPrintRoofWrapper m_footPrintRoofWrapper;

        // A GraphicsControl to display the roof lines of footprint roof.
        private GraphicsControl m_graphicsControl;

        // To store the roof which will be edited.
        private readonly RoofBase m_roof;

        // A reference to the roofs manager
        private readonly RoofsManager.CS.RoofsManager m_roofsManager;

        /// <summary>
        ///     The private construct.
        /// </summary>
        private RoofEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The construct of the RoofEditorForm class.
        /// </summary>
        /// <param name="roofsManager">A reference to the roofs manager</param>
        /// <param name="roof">The roof which will be edited.</param>
        public RoofEditorForm(RoofsManager.CS.RoofsManager roofsManager, RoofBase roof)
        {
            m_roofsManager = roofsManager;
            m_roof = roof;
            InitializeComponent();

            m_footPrintRoofWrapper = null;
            m_extrusionRoofWrapper = null;

            if (m_roof is FootPrintRoof)
                m_footPrintRoofWrapper = new FootPrintRoofWrapper(m_roof as FootPrintRoof);
            else
                m_extrusionRoofWrapper = new ExtrusionRoofWrapper(m_roof as ExtrusionRoof);
        }

        /// <summary>
        ///     When the RoofEditorForm was loaded, then initialize data of the controls in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoofEditorForm_Load(object sender, EventArgs e)
        {
            roofTypesComboBox.DataSource = m_roofsManager.RoofTypes;
            roofTypesComboBox.DisplayMember = "Name";
            roofTypesComboBox.ValueMember = "Id";
            roofTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            roofTypesComboBox.SelectedValue = m_roof.RoofType.Id;

            if (m_roof is FootPrintRoof)
            {
                roofEditorPropertyGrid.SelectedObject = m_footPrintRoofWrapper;
                Size = new Size(814, 515);

                var label = new Label();
                label.Text = "Footprint roof lines:";
                label.AutoSize = true;
                label.Location = new Point(398, 12);
                Controls.Add(label);

                m_graphicsControl = new GraphicsControl(m_footPrintRoofWrapper);
                m_graphicsControl.Location = new Point(398, 36);
                m_graphicsControl.Size = new Size(400, 440);
                Controls.Add(m_graphicsControl);
            }
            else
            {
                roofEditorPropertyGrid.SelectedObject = m_extrusionRoofWrapper;
            }

            roofEditorPropertyGrid.ExpandAllGridItems();
        }

        /// <summary>
        ///     When the OK button was clicked, update the roof type of the editing roof.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            m_roof.RoofType = roofTypesComboBox.SelectedItem as RoofType;
        }
    }
}