//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notify appears in all copies and
// that both that copyright notify and the limited warranty and
// restricted rights notify below appear in all supporting
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.TransactionControl.CS
{
    /// <summary>
    /// A Form used to create a wall
    /// </summary>
    public partial class CreateWallForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// A reference to the external command data.
        /// </summary>
        private ExternalCommandData m_commandData;
        /// <summary>
        /// The created wall
        /// </summary>
        private Wall m_createdWall = null;

        /// <summary>
        /// The created wall
        /// </summary>
        public Wall CreatedWall
        {
            get
            {
                return m_createdWall;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commandData">the external command data</param>
        public CreateWallForm(ExternalCommandData commandData)
        {
            InitializeComponent();
            m_commandData = commandData;
            Initialize();
        }

        /// <summary>
        /// do some initialization work: get all levels and wall types in active document 
        /// and add them to combo box
        /// </summary>
        private void Initialize()
        {
            //add levels to combo box levelsComboBox
            var document = m_commandData.Application.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(document);
            var filter = new ElementClassFilter(typeof(Level));
            var levels = collector.WherePasses(filter).ToElements();

            foreach (var element in levels)
            {
                var level = element as Level;
                if (level != null)
                {
                    this.levelsComboBox.Items.Add(level);
                }
            }

            if (this.levelsComboBox.Items.Count > 0)
            {
                this.levelsComboBox.DisplayMember = "Name";
                this.levelsComboBox.SelectedIndex = 0;
            }

            //add wall types to combo box wallTypesComboBox
            var filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.OfClass(typeof(WallType));
            foreach (var element in filteredElementCollector)
            {
                var wallType = element as WallType;
                if (null == wallType)
                {
                    continue;
                }
                this.wallTypesComboBox.Items.Add(wallType);
            }
            if (this.wallTypesComboBox.Items.Count > 0)
            {
                this.wallTypesComboBox.DisplayMember = "Name";
                this.wallTypesComboBox.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// try to create a wall. if failed, keep this dialog,
        /// otherwise, close it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var document = m_commandData.Application.ActiveUIDocument.Document;
                double x, y, z;
                Autodesk.Revit.DB.XYZ sPoint;

                //try to convert string to double, and return without closing dialog if failed
                if (Double.TryParse(this.spXTextBox.Text, out x) &&
                   Double.TryParse(this.spYTextBox.Text, out y) &&
                   Double.TryParse(this.spZTextBox.Text, out z))
                {
                    sPoint = new Autodesk.Revit.DB.XYZ(x, y, z);
                }
                else
                {
                    TaskDialog.Show("Revit", "Failed to get the start point.");
                    return;
                }

                //try to convert string to double, and return without closing dialog if failed
                Autodesk.Revit.DB.XYZ ePoint;
                if (Double.TryParse(this.epXTextBox.Text, out x) &&
                    Double.TryParse(this.epYTextBox.Text, out y) &&
                    Double.TryParse(this.epZTextBox.Text, out z))
                {
                    ePoint = new Autodesk.Revit.DB.XYZ(x, y, z);
                }
                else
                {
                    TaskDialog.Show("Revit", "Failed to get the end point.");
                    return;
                }

                if (sPoint.IsAlmostEqualTo(ePoint))
                {
                    TaskDialog.Show("Revit", "The start point and end point cannot have the same location.");
                    return;
                }

                var line = Line.CreateBound(sPoint, ePoint);

                var level = this.levelsComboBox.SelectedItem as Level;
                var wallType = this.wallTypesComboBox.SelectedItem as WallType;

                //check whether parameters used to create wall are not null
                if (null == line)
                {
                    TaskDialog.Show("Revit", "Create line failed.");
                    return;
                }

                if (null == level || null == wallType)
                {
                    TaskDialog.Show("Revit", "Please select a level and a wall type.");
                    return;
                }

                m_createdWall = Wall.Create(document, line, wallType.Id, level.Id, 10, 0, true, true);
                document.Regenerate();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }
    }
}