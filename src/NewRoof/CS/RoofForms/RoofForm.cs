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
using System.ComponentModel;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.NewRoof.RoofForms.CS
{
    /// <summary>
    /// The main form to create or delete roof in Revit.
    /// </summary>
    public partial class RoofForm : System.Windows.Forms.Form
    {
        // A reference to the roofs manager
        private RoofsManager.CS.RoofsManager m_roofsManager;
        // To store the extrusion start and extrusion end value for creating extrusion roof.
        private double m_start, m_end;

        /// <summary>
        /// The private construct.
        /// </summary>
        private RoofForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The construct of the RoofForm class.
        /// </summary>
        /// <param name="roofsManager">A reference to the roofs manager</param>
        public RoofForm(RoofsManager.CS.RoofsManager roofsManager)
        {
            m_roofsManager = roofsManager;
            m_start = -10.0;
            m_end = 10.0;

            InitializeComponent();
        }

        /// <summary>
        /// When the RoofForm was loaded, then initialize data of the controls in the form. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoofForm_Load(object sender, EventArgs e)
        {
            foreach (FootPrintRoof roof in m_roofsManager.FootPrintRoofs)
            {
                footPrintRoofsListView.Items.Add(new RoofItem(roof));
            }

            foreach (ExtrusionRoof roof in m_roofsManager.ExtrusionRoofs)
            {
                extrusionRoofsListView.Items.Add(new RoofItem(roof));
            }

            levelsComboBox.DataSource = m_roofsManager.Levels;
            levelsComboBox.DisplayMember = "Name";
            levelsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            roofTypesComboBox.DataSource = m_roofsManager.RoofTypes;
            roofTypesComboBox.DisplayMember = "Name";
            roofTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            refLevelComboBox.DataSource = m_roofsManager.Levels;
            refLevelComboBox.DisplayMember = "Name";
            refLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            extrusionRoofTypesComboBox.DataSource = m_roofsManager.RoofTypes;
            extrusionRoofTypesComboBox.DisplayMember = "Name";
            extrusionRoofTypesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            refPanesComboBox.DataSource = m_roofsManager.ReferencePlanes;
            refPanesComboBox.DisplayMember = "Name";
            refPanesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Book Select Button Click Event
            selectFootPrintButton.Click += new EventHandler(selectFootPrintButton_Click);
            selectProfileButton.Click += new EventHandler(selectProfileButton_Click);

            if (m_roofsManager.RoofKind == RoofsManager.CS.CreateRoofKind.FootPrintRoof)
            {
                roofsTabControl.SelectedTab = footprintRoofTabPage;
            }
            else
            {
                roofsTabControl.SelectedTab = extrusionRoofTabPage;
            }

            footPrintRoofsListView.MultiSelect = false;
            footPrintRoofsListView.FullRowSelect = true;
            footPrintRoofsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            footPrintRoofsListView.MouseDoubleClick += new MouseEventHandler(roofsListView_MouseDoubleClick);

            extrusionRoofsListView.MultiSelect = false;
            extrusionRoofsListView.FullRowSelect = true;
            extrusionRoofsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            extrusionRoofsListView.MouseDoubleClick += new MouseEventHandler(roofsListView_MouseDoubleClick);

            extrusionStartTextBox.Text = m_start.ToString();
            extrusionEndTextBox.Text = m_end.ToString();
        }

        /// <summary>
        /// When selectFootPrintButton was clicked, then select some footprint loops in Revit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectFootPrintButton_Click(object sender, EventArgs e)
        {
            m_roofsManager.RoofKind = RoofsManager.CS.CreateRoofKind.FootPrintRoof;
            Close();
        }

        /// <summary>
        /// When selectFootPrintButton was clicked, then select profile in Revit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectProfileButton_Click(object sender, EventArgs e)
        {
            m_roofsManager.RoofKind = RoofsManager.CS.CreateRoofKind.ExtrusionRoof;
            Close();
        }

        /// <summary>
        /// when createRoofButton was clicked, then create a new roof.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createRoofButton_Click(object sender, EventArgs e)
        {
            if (roofsTabControl.SelectedTab == footprintRoofTabPage)
            {
                createFootPrintRoof();
            }
            else
            {
                createExtrusionRoof();
            }
        }

        /// <summary>
        /// When editRoofButton was click, then create a new RoofEditorForm to edit the selected roof.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editRoofButton_Click(object sender, EventArgs e)
        {
            if (footPrintRoofsListView.SelectedItems.Count != 0
                || extrusionRoofsListView.SelectedItems.Count != 0)
            {
                RoofItem item = null;
                if (roofsTabControl.SelectedTab == footprintRoofTabPage)
                {
                    item = footPrintRoofsListView.SelectedItems[0] as RoofItem;
                }
                else
                {
                    item = extrusionRoofsListView.SelectedItems[0] as RoofItem;
                }

                if (item != null)
                {
                    EditRoofItem(item.ListView, item);
                }
            }
            else
            {
                TaskDialog.Show("Revit", "To edit a roof, you should select a roof or double click a roof in the list first.");
            }
        }

        /// <summary>
        /// Create a new footprint roof.
        /// </summary>
        private void createFootPrintRoof()
        {
            try
            {
                if (m_roofsManager.FootPrint.Size != 0)
                {
                    var level = levelsComboBox.SelectedItem as Level;
                    var roofType = roofTypesComboBox.SelectedItem as RoofType;
                    if (level != null && roofType != null)
                    {
                        var roof = m_roofsManager.CreateFootPrintRoof(level, roofType);
                        if (roof == null)
                        {
                            TaskDialog.Show("Revit", "Invalid footprint2");
                        }
                        else
                        {
                            footPrintRoofsListView.Items.Add(new RoofItem(roof));
                            footPrintRoofsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }
                }
                else
                {
                    TaskDialog.Show("Revit", "You should supply footprint to create footprint roof, click select button to select footprint in Revit.");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message + " : Footprint must be in closed loops.");
            }
        }

        /// <summary>
        /// Create a extrusion roof.
        /// </summary>
        private void createExtrusionRoof()
        {
            try
            {
                if (m_roofsManager.Profile.Size != 0)
                {
                    var level = refLevelComboBox.SelectedItem as Level;
                    var roofType = extrusionRoofTypesComboBox.SelectedItem as RoofType;
                    var refPlane = refPanesComboBox.SelectedItem as Autodesk.Revit.DB.ReferencePlane;
                    if (level != null && roofType != null && refPlane != null)
                    {
                        var roof = m_roofsManager.CreateExtrusionRoof(refPlane, level, roofType, m_start, m_end);
                        if (roof == null)
                        {
                            TaskDialog.Show("Revit", "Invalid profile");
                        }
                        else
                        {
                            extrusionRoofsListView.Items.Add(new RoofItem(roof));
                            extrusionRoofsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                    }
                }
                else
                {
                    TaskDialog.Show("Revit", "You should supply profile to create extrusion roof, click select button to select profile in Revit.");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }

        /// <summary>
        /// Edit a roof's properties.
        /// </summary>
        /// <param name="item">It contains a roof element.</param>
        private void EditRoofItem(object sender, RoofItem item)
        {
            try
            {
                m_roofsManager.BeginTransaction();
                var result = DialogResult.None;
                using (var editorForm = new RoofEditorForm(m_roofsManager, item.Roof))
                {
                    result = editorForm.ShowDialog();
                }
                if (result == DialogResult.OK)
                {
                    if (m_roofsManager.EndTransaction() == TransactionStatus.Committed)
                    {
                        var listView = sender as ListView;
                        if (item.Update())
                        {
                            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        }
                        else
                        {
                            m_roofsManager.FootPrintRoofs.Erase(item.Roof);
                            listView.Items.Remove(item);
                            listView.Refresh();
                        }
                    }
                    else
                    {
                        m_roofsManager.AbortTransaction();
                    }
                }
                else
                {
                    m_roofsManager.AbortTransaction();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
                m_roofsManager.AbortTransaction();
            }
        }

        /// <summary>
        /// When the event occurred, then create a new RoofEditorForm to edit the roof.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void roofsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                RoofItem item = null;
                if (roofsTabControl.SelectedTab == footprintRoofTabPage)
                {
                    item = footPrintRoofsListView.GetItemAt(e.X, e.Y) as RoofItem;
                }
                else
                {
                    item = extrusionRoofsListView.GetItemAt(e.X, e.Y) as RoofItem;
                }

                if (item != null)
                {
                    EditRoofItem(sender, item);
                }
            }
        }

        /// <summary>
        /// Validate the extrusion start value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extrusionStartTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                m_start = double.Parse(extrusionStartTextBox.Text);
            }
            catch
            {
                TaskDialog.Show("Revit", "You should input a decimal value.");
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Validate the extrusion end value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extrusionEndTextBox_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                m_end = double.Parse(extrusionEndTextBox.Text);
            }
            catch
            {
                TaskDialog.Show("Revit", "You should input a decimal value.");
                e.Cancel = true;
            }
        }
    }
}