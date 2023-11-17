// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The DataManager Class is used to obtain, create or edit the Space elements and Zone elements.
    /// </summary>
    public class DataManager
    {
        private readonly ExternalCommandData m_commandData;
        private Level m_currentLevel;
        private readonly Phase m_defaultPhase;
        private readonly List<Level> m_levels;
        private SpaceManager m_spaceManager;
        private ZoneManager m_zoneManager;

        /// <summary>
        ///     The constructor of DataManager class.
        /// </summary>
        /// <param name="commandData">The ExternalCommandData</param>
        public DataManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_levels = new List<Level>();
            Initialize();
            m_currentLevel = m_levels[0];
            var para =
                commandData.Application.ActiveUIDocument.Document.ActiveView.get_Parameter(BuiltInParameter.VIEW_PHASE);
            var phaseId = para.AsElementId();
            m_defaultPhase = commandData.Application.ActiveUIDocument.Document.GetElement(phaseId) as Phase;
        }

        /// <summary>
        ///     Get the Level elements.
        /// </summary>
        public ReadOnlyCollection<Level> Levels => new ReadOnlyCollection<Level>(m_levels);

        /// <summary>
        ///     Initialize the data member, obtain the Space and Zone elements.
        /// </summary>
        private void Initialize()
        {
            var spaceDictionary = new Dictionary<ElementId, List<Space>>();
            var zoneDictionary = new Dictionary<ElementId, List<Zone>>();

            var activeDoc = m_commandData.Application.ActiveUIDocument.Document;

            foreach (var level in activeDoc.GetFilteredElements<Level>())
            {
                m_levels.Add(level);
                spaceDictionary.Add(level.Id, new List<Space>());
                zoneDictionary.Add(level.Id, new List<Zone>());
            }

            foreach (var space in activeDoc.GetFilteredElements<Space>())
            {
                spaceDictionary[space.LevelId].Add(space);
            }

            foreach (var zone in activeDoc.GetFilteredElements<Zone>())
            {
                if (activeDoc.GetElement(zone.LevelId) != null) 
                    zoneDictionary[zone.LevelId].Add(zone);
            }

            m_spaceManager = new SpaceManager(m_commandData, spaceDictionary);
            m_zoneManager = new ZoneManager(m_commandData, zoneDictionary);
        }

        /// <summary>
        ///     Create a Zone element.
        /// </summary>
        public void CreateZone()
        {
            if (m_defaultPhase == null)
            {
                TaskDialog.Show("Revit", "The phase of the active view is null, you can't create zone in a null phase");
                return;
            }

            try
            {
                m_zoneManager.CreateZone(m_currentLevel, m_defaultPhase);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }

        /// <summary>
        ///     Create some spaces.
        /// </summary>
        public void CreateSpaces()
        {
            if (m_defaultPhase == null)
            {
                TaskDialog.Show("Revit",
                    "The phase of the active view is null, you can't create spaces in a null phase");
                return;
            }

            try
            {
                if (m_commandData.Application.ActiveUIDocument.Document.ActiveView.ViewType == ViewType.FloorPlan)
                    m_spaceManager.CreateSpaces(m_currentLevel, m_defaultPhase);
                else
                    TaskDialog.Show("Revit", "You can not create spaces in this plan view");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }

        /// <summary>
        ///     Get the Space elements.
        /// </summary>
        /// <returns>A space list in current level.</returns>
        public List<Space> GetSpaces()
        {
            return m_spaceManager.GetSpaces(m_currentLevel);
        }

        /// <summary>
        ///     Get the Zone elements.
        /// </summary>
        /// <returns>A Zone list in current level.</returns>
        public List<Zone> GetZones()
        {
            return m_zoneManager.GetZones(m_currentLevel);
        }

        /// <summary>
        ///     Update the current level.
        /// </summary>
        /// <param name="level"></param>
        public void Update(Level level)
        {
            m_currentLevel = level;
        }
    }
}
