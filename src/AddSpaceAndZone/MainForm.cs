// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public partial class MainForm : Form
    {
        private MainForm()
        {
            InitializeComponent();
        }

        public MainForm(DataManager dataManager)
        {
            m_dataManager = dataManager;
            InitializeComponent();
        }

        private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update(levelComboBox.SelectedItem as Level);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            levelComboBox.DataSource = m_dataManager.Levels;
            levelComboBox.DisplayMember = "Name";
            levelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            editZoneButton.Enabled = false;
        }

        private void createSpacesButton_Click(object sender, EventArgs e)
        {
            m_dataManager.CreateSpaces();
            Update(levelComboBox.SelectedItem as Level);
        }

        private void editZoneButton_Click(object sender, EventArgs e)
        {
            if (zonesTreeView.SelectedNode is ZoneNode zoneNode)
                using (ZoneEditorForm zoneEditorForm = new(m_dataManager, zoneNode))
                {
                    zoneEditorForm.ShowDialog();
                }

            Update(levelComboBox.SelectedItem as Level);
        }

        private void Update(Level level)
        {
            spacesListView.Items.Clear();
            zonesTreeView.Nodes.Clear();

            m_dataManager.Update(level);

            var spaces = m_dataManager.GetSpaces();
            foreach (var space in spaces)
            {
                spacesListView.Items.Add(new SpaceItem(space));
            }

            var zones = m_dataManager.GetZones();
            foreach (var zone in zones)
            {
                var nodeIndex = zonesTreeView.Nodes.Add(new ZoneNode(zone));
                foreach (Space spaceInZone in zone.Spaces)
                {
                    zonesTreeView.Nodes[nodeIndex].Nodes.Add(new SpaceNode(spaceInZone));
                }
            }

            zonesTreeView.ExpandAll();

            zonesTreeView.Update();
            spacesListView.Update();
        }

        private void createZoneButton_Click(object sender, EventArgs e)
        {
            m_dataManager.CreateZone();
            Update(levelComboBox.SelectedItem as Level);
        }

        private void zonesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            editZoneButton.Enabled = e.Node is ZoneNode;
        }
    }
}
