// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The ZoneManager class is used to manage the Zone elements in the current document.
    /// </summary>
    public class ZoneManager
    {
        private readonly ExternalCommandData m_commandData;
        private Level m_currentLevel;
        private Zone m_currentZone;
        private readonly Dictionary<ElementId, List<Zone>> m_zoneDictionary;

        /// <summary>
        ///     The constructor of ZoneManager class.
        /// </summary>
        /// <param name="commandData">The ExternalCommandData</param>
        /// <param name="zoneData">The spaceData contains all the Zone elements in different level.</param>
        public ZoneManager(ExternalCommandData commandData, Dictionary<ElementId, List<Zone>> zoneData)
        {
            m_commandData = commandData;
            m_zoneDictionary = zoneData;
        }

        /// <summary>
        ///     Get/Set the Current Zone element.
        /// </summary>
        public Zone CurrentZone
        {
            get => CurrentZone;
            set => m_currentZone = value;
        }

        /// <summary>
        ///     Create a zone in a specified level and phase.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="phase"></param>
        public void CreateZone(Level level, Phase phase)
        {
            var zone = m_commandData.Application.ActiveUIDocument.Document.Create.NewZone(level, phase);
            if (zone != null) m_zoneDictionary[level.Id].Add(zone);
        }

        /// <summary>
        ///     Add some spaces to current Zone.
        /// </summary>
        /// <param name="spaces"></param>
        public void AddSpaces(SpaceSet spaces)
        {
            m_currentZone.AddSpaces(spaces);
        }

        /// <summary>
        ///     Remove some spaces to current Zone.
        /// </summary>
        /// <param name="spaces"></param>
        public void RemoveSpaces(SpaceSet spaces)
        {
            m_currentZone.RemoveSpaces(spaces);
        }

        /// <summary>
        ///     Get the Zone elements in a specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>Return a zone list</returns>
        public List<Zone> GetZones(Level level)
        {
            m_currentLevel = level;
            return m_zoneDictionary[level.Id];
        }
    }
}
