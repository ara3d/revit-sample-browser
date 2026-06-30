// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public class SpaceManager
    {
        private readonly ExternalCommandData m_commandData;
        private readonly Dictionary<ElementId, List<Space>> m_spaceDictionary;

        public SpaceManager(ExternalCommandData data, Dictionary<ElementId, List<Space>> spaceData)
        {
            m_commandData = data;
            m_spaceDictionary = spaceData;
        }

        public List<Space> GetSpaces(Level level)
        {
            return m_spaceDictionary[level.Id];
        }

        public void CreateSpaces(Level level, Phase phase)
        {
            try
            {
                var elements = m_commandData.Application.ActiveUIDocument.Document.Create.NewSpaces2(level, phase,
                    m_commandData.Application.ActiveUIDocument.Document.ActiveView);
                foreach (var elem in elements)
                {
                    if (m_commandData.Application.ActiveUIDocument.Document.GetElement(elem) is Space space) m_spaceDictionary[level.Id].Add(space);
                }

                if (elements == null || elements.Count == 0)
                    DialogHelper.ShowMessage($"There is no enclosed loop in {level.Name}");
            }
            catch (Exception ex)
            {
                DialogHelper.ShowMessage(ex.Message);
            }
        }
    }
}
