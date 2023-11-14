// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.LevelsProperty.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private ExternalCommandData m_revit;
        public ForgeTypeId UnitTypeId;

        /// <summary>
        ///     Store all levels' datum in system
        /// </summary>
        public List<LevelsDataSource> SystemLevelsDatum { get; set; }

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            m_revit = revit;
            UnitTypeId = m_revit.Application.ActiveUIDocument.Document.GetUnits().GetFormatOptions(SpecTypeId.Length)
                .GetUnitTypeId();
            var documentTransaction = new Transaction(revit.Application.ActiveUIDocument.Document, "Document");
            documentTransaction.Start();
            try
            {
                //Get every level by iterating through all elements
                SystemLevelsDatum = new List<LevelsDataSource>();
                var collector = new FilteredElementCollector(m_revit.Application.ActiveUIDocument.Document);
                ICollection<Element> collection = collector.OfClass(typeof(Level)).ToElements();
                foreach (var element in collection)
                {
                    var systemLevel = element as Level;
                    var levelsDataSourceRow = new LevelsDataSource
                    {
                        LevelIDValue = systemLevel.Id,
                        Name = systemLevel.Name
                    };

                    var elevationPara = systemLevel.get_Parameter(BuiltInParameter.LEVEL_ELEV);

                    var temValue = Unit.CovertFromAPI(UnitTypeId, elevationPara.AsDouble());
                    var temValue2 = double.Parse(temValue.ToString("#.0"));

                    levelsDataSourceRow.Elevation = temValue2;

                    SystemLevelsDatum.Add(levelsDataSourceRow);
                }

                using (var displayForm = new LevelsForm(this))
                {
                    displayForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                documentTransaction.RollBack();
                return Result.Failed;
            }

            documentTransaction.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        ///     Set Level
        /// </summary>
        /// <param name="levelID">Pass a Level's ID value</param>
        /// <param name="levelName">Pass a Level's Name</param>
        /// <param name="levelElevation">Pass a Level's Elevation</param>
        /// <returns>True if succeed, else return false</returns>
        public bool SetLevel(ElementId levelID, string levelName, double levelElevation)
        {
            try
            {
                var systemLevel = m_revit.Application.ActiveUIDocument.Document.GetElement(levelID) as Level;

                var elevationPara = systemLevel.get_Parameter(BuiltInParameter.LEVEL_ELEV);
                elevationPara.SetValueString(levelElevation.ToString());
                systemLevel.Name = levelName;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Create a level
        /// </summary>
        /// <param name="levelName">Pass a Level's Name</param>
        /// <param name="levelElevation">Pass a Level's Elevation</param>
        public void CreateLevel(string levelName, double levelElevation)
        {
            var newLevel = Level.Create(m_revit.Application.ActiveUIDocument.Document, levelElevation);
            var elevationPara = newLevel.get_Parameter(BuiltInParameter.LEVEL_ELEV);
            elevationPara.SetValueString(levelElevation.ToString());

            newLevel.Name = levelName;
        }

        /// <summary>
        ///     Delete a Level.
        /// </summary>
        /// <param name="IDOfLevel">A Level's ID value</param>
        public void DeleteLevel(ElementId IDOfLevel)
        {
            m_revit.Application.ActiveUIDocument.Document.Delete(IDOfLevel);
        }
    }
}
