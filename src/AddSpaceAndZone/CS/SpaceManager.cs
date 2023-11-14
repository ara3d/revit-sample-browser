// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The SpaceManager class is used to manage the Spaces elements in the current document.
    /// </summary>
    internal class SpaceManager
    {
        private readonly ExternalCommandData m_commandData;
        private readonly Dictionary<ElementId, List<Space>> m_spaceDictionary;

        /// <summary>
        ///     The constructor of SpaceManager class.
        /// </summary>
        /// <param name="data">The ExternalCommandData</param>
        /// <param name="spaceData">The spaceData contains all the Space elements in different level.</param>
        public SpaceManager(ExternalCommandData data, Dictionary<ElementId, List<Space>> spaceData)
        {
            m_commandData = data;
            m_spaceDictionary = spaceData;
        }

        /// <summary>
        ///     Get the Spaces elements in a specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>Return a space list</returns>
        public List<Space> GetSpaces(Level level)
        {
            return m_spaceDictionary[level.Id];
        }

        /// <summary>
        ///     Create the space for each closed wall loop or closed space separation in the active view.
        /// </summary>
        /// <param name="level">The level in which the spaces is to exist.</param>
        /// <param name="phase">The phase in which the spaces is to exist.</param>
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
                    TaskDialog.Show("Revit", "There is no enclosed loop in " + level.Name);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
            }
        }
    }
}
