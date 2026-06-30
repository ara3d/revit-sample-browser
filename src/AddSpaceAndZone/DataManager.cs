// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public class DataManager
    {
        private readonly ExternalCommandData m_commandData;
        private Level m_currentLevel;
        private readonly Phase m_defaultPhase;
        private readonly List<Level> m_levels;
        private SpaceManager m_spaceManager;
        private ZoneManager m_zoneManager;

        public DataManager(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_levels = [];
            Initialize();
            m_currentLevel = m_levels[0];
            var para =
                commandData.Application.ActiveUIDocument.Document.ActiveView.get_Parameter(BuiltInParameter.VIEW_PHASE);
            var phaseId = para.AsElementId();
            m_defaultPhase = commandData.Application.ActiveUIDocument.Document.GetElement(phaseId) as Phase;
        }

        public ReadOnlyCollection<Level> Levels => new(m_levels);

        private void Initialize()
        {
            Dictionary<ElementId, List<Space>> spaceDictionary = new();
            Dictionary<ElementId, List<Zone>> zoneDictionary = new();

            var activeDoc = m_commandData.Application.ActiveUIDocument.Document;

            foreach (var level in activeDoc.GetElements<Level>())
            {
                m_levels.Add(level);
                spaceDictionary.Add(level.Id, []);
                zoneDictionary.Add(level.Id, []);
            }

            foreach (var space in activeDoc.GetElements<Space>())
            {
                spaceDictionary[space.LevelId].Add(space);
            }

            foreach (var zone in activeDoc.GetElements<Zone>())
            {
                if (activeDoc.GetElement(zone.LevelId) != null)
                    zoneDictionary[zone.LevelId].Add(zone);
            }

            m_spaceManager = new SpaceManager(m_commandData, spaceDictionary);
            m_zoneManager = new ZoneManager(m_commandData, zoneDictionary);
        }

        public void CreateZone()
        {
            if (m_defaultPhase == null)
            {
                DialogHelper.ShowMessage("The phase of the active view is null, you can't create zone in a null phase");
                return;
            }

            try
            {
                m_zoneManager.CreateZone(m_currentLevel, m_defaultPhase);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowMessage(ex.Message);
            }
        }

        public void CreateSpaces()
        {
            if (m_defaultPhase == null)
            {
                DialogHelper.ShowMessage(
                    "The phase of the active view is null, you can't create spaces in a null phase");
                return;
            }

            try
            {
                if (m_commandData.Application.ActiveUIDocument.Document.ActiveView.ViewType == ViewType.FloorPlan)
                    m_spaceManager.CreateSpaces(m_currentLevel, m_defaultPhase);
                else
                    DialogHelper.ShowMessage("You can not create spaces in this plan view");
            }
            catch (Exception ex)
            {
                DialogHelper.ShowMessage(ex.Message);
            }
        }

        public List<Space> GetSpaces()
        {
            return m_spaceManager.GetSpaces(m_currentLevel);
        }

        public List<Zone> GetZones()
        {
            return m_zoneManager.GetZones(m_currentLevel);
        }

        public void Update(Level level)
        {
            m_currentLevel = level;
        }
    }
}
