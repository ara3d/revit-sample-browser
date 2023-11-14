// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The MainForm Class is the main user interface to manage the Space elements and Zone elements
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///     The default constructor of MainForm.
        /// </summary>
        private MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The constructor of MainForm.
        /// </summary>
        public MainForm(DataManager dataManager)
        {
            m_dataManager = dataManager;
            InitializeComponent();
        }

        /// <summary>
        ///     When the levelComboBox selected index is changed, update the spacesListView and zonesTreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void levelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update(levelComboBox.SelectedItem as Level);
        }

        /// <summary>
        ///     When the MainForm is loaded, initialize related controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // levelComboBox
            levelComboBox.DataSource = m_dataManager.Levels;
            levelComboBox.DisplayMember = "Name";
            levelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // EditZoneButton
            editZoneButton.Enabled = false;
        }

        /// <summary>
        ///     When createSpacesButton is clicked, then create spaces and update the spacesListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createSpacesButton_Click(object sender, EventArgs e)
        {
            m_dataManager.CreateSpaces();
            Update(levelComboBox.SelectedItem as Level);
        }

        /// <summary>
        ///     When editZoneButton is clicked, show the ZoneEditorForm to edit the selected ZoneNode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editZoneButton_Click(object sender, EventArgs e)
        {
            var zoneNode = zonesTreeView.SelectedNode as ZoneNode;
            if (zoneNode != null)
                using (var zoneEditorForm = new ZoneEditorForm(m_dataManager, zoneNode))
                {
                    zoneEditorForm.ShowDialog();
                }

            Update(levelComboBox.SelectedItem as Level);
        }

        /// <summary>
        ///     Update the spacesListView and spacesListView.
        /// </summary>
        /// <param name="level"></param>
        private void Update(Level level)
        {
            spacesListView.Items.Clear();
            zonesTreeView.Nodes.Clear();

            // DataManager
            m_dataManager.Update(level);

            // spacesListView
            var spaces = m_dataManager.GetSpaces();
            foreach (var space in spaces) spacesListView.Items.Add(new SpaceItem(space));

            // zonesTreeView
            var zones = m_dataManager.GetZones();
            foreach (var zone in zones)
            {
                var nodeIndex = zonesTreeView.Nodes.Add(new ZoneNode(zone));
                foreach (Space spaceInZone in zone.Spaces)
                    zonesTreeView.Nodes[nodeIndex].Nodes.Add(new SpaceNode(spaceInZone));
            }

            zonesTreeView.ExpandAll();

            zonesTreeView.Update();
            spacesListView.Update();
        }

        /// <summary>
        ///     When createZoneButton is clicked, then create a new Zone element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createZoneButton_Click(object sender, EventArgs e)
        {
            m_dataManager.CreateZone();
            Update(levelComboBox.SelectedItem as Level);
        }

        private void zonesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is ZoneNode)
                editZoneButton.Enabled = true;
            else
                editZoneButton.Enabled = false;
        }
    }
}
