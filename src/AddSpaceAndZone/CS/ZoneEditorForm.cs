// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The ZoneEditorForm Class the user interface to edit a Zone element.
    /// </summary>
    public partial class ZoneEditorForm : Form
    {
        /// <summary>
        ///     The default constructor of ZoneEditorForm class.
        /// </summary>
        private ZoneEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The constructor of ZoneEditorForm class.
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="zoneNode"></param>
        public ZoneEditorForm(DataManager dataManager, ZoneNode zoneNode)
        {
            m_dataManager = dataManager;
            m_zoneNode = zoneNode;
            m_zone = m_zoneNode.Zone;
            InitializeComponent();
        }

        /// <summary>
        ///     When the addSpace Button is clicked, the selected spaces will be added to the
        ///     current Zone element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addSpaceButton_Click(object sender, EventArgs e)
        {
            var set = new SpaceSet();
            foreach (SpaceItem item in availableSpacesListView.SelectedItems)
            {
                set.Insert(item.Space);
            }

            m_zone.AddSpaces(set);

            UpdateSpaceList();
        }

        /// <summary>
        ///     When the removeSpace Button is clicked, the selected spaces will be removed from the
        ///     current Zone element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeSpaceButton_Click(object sender, EventArgs e)
        {
            var set = new SpaceSet();
            foreach (SpaceItem item in currentSpacesListView.SelectedItems)
            {
                set.Insert(item.Space);
            }

            m_zone.RemoveSpaces(set);
            UpdateSpaceList();
        }

        /// <summary>
        ///     When the ZoneEditorForm is loaded, update the AvailableSpacesListView and CurrentSpacesListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoneEditorForm_Load(object sender, EventArgs e)
        {
            Text = "Edit Zone : " + m_zone.Name;
            UpdateSpaceList();
        }

        /// <summary>
        ///     Update the AvailableSpacesListView and CurrentSpacesListView
        /// </summary>
        private void UpdateSpaceList()
        {
            availableSpacesListView.Items.Clear();
            currentSpacesListView.Items.Clear();

            // AvailableSpacesListView
            foreach (var space in m_dataManager.GetSpaces())
            {
                if (m_zone.Spaces.Contains(space) == false)
                    availableSpacesListView.Items.Add(new SpaceItem(space));
            }

            // CurrentSpacesListView
            foreach (Space space in m_zone.Spaces)
            {
                currentSpacesListView.Items.Add(new SpaceItem(space));
            }

            availableSpacesListView.Update();
            currentSpacesListView.Update();
        }

        /// <summary>
        ///     When OK button is clicked, close the ZoneEditorForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
