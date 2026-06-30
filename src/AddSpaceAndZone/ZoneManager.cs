// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public class ZoneManager
    {
        private readonly ExternalCommandData m_commandData;
        private Level m_currentLevel;
        private Zone m_currentZone;
        private readonly Dictionary<ElementId, List<Zone>> m_zoneDictionary;

        public ZoneManager(ExternalCommandData commandData, Dictionary<ElementId, List<Zone>> zoneData)
        {
            m_commandData = commandData;
            m_zoneDictionary = zoneData;
        }

        public Zone CurrentZone
        {
            get => CurrentZone;
            set => m_currentZone = value;
        }

        public void CreateZone(Level level, Phase phase)
        {
            var zone = m_commandData.Application.ActiveUIDocument.Document.Create.NewZone(level, phase);
            if (zone != null) m_zoneDictionary[level.Id].Add(zone);
        }

        public void AddSpaces(SpaceSet spaces)
        {
            m_currentZone.AddSpaces(spaces);
        }

        public void RemoveSpaces(SpaceSet spaces)
        {
            m_currentZone.RemoveSpaces(spaces);
        }

        public List<Zone> GetZones(Level level)
        {
            m_currentLevel = level;
            return m_zoneDictionary[level.Id];
        }
    }
}
