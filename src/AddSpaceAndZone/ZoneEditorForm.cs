// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Mechanical;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public partial class ZoneEditorForm : Form
    {
        private ZoneEditorForm()
        {
            InitializeComponent();
        }

        public ZoneEditorForm(DataManager dataManager, ZoneNode zoneNode)
        {
            m_dataManager = dataManager;
            m_zoneNode = zoneNode;
            m_zone = m_zoneNode.Zone;
            InitializeComponent();
        }

        private void addSpaceButton_Click(object sender, EventArgs e)
        {
            SpaceSet set = new();
            foreach (SpaceItem item in availableSpacesListView.SelectedItems)
            {
                set.Insert(item.Space);
            }

            m_zone.AddSpaces(set);

            UpdateSpaceList();
        }

        private void removeSpaceButton_Click(object sender, EventArgs e)
        {
            SpaceSet set = new();
            foreach (SpaceItem item in currentSpacesListView.SelectedItems)
            {
                set.Insert(item.Space);
            }

            m_zone.RemoveSpaces(set);
            UpdateSpaceList();
        }

        private void ZoneEditorForm_Load(object sender, EventArgs e)
        {
            Text = $"Edit Zone : {m_zone.Name}";
            UpdateSpaceList();
        }

        private void UpdateSpaceList()
        {
            availableSpacesListView.Items.Clear();
            currentSpacesListView.Items.Clear();

            foreach (var space in m_dataManager.GetSpaces())
            {
                if (m_zone.Spaces.Contains(space) == false)
                    availableSpacesListView.Items.Add(new SpaceItem(space));
            }

            foreach (Space space in m_zone.Spaces)
            {
                currentSpacesListView.Items.Add(new SpaceItem(space));
            }

            availableSpacesListView.Update();
            currentSpacesListView.Update();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
