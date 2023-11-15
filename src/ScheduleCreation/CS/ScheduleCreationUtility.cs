// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ScheduleCreation.CS
{
    /// <summary>
    ///     Utility class that contains methods of view schedule creation and schedule sheet instance creation.
    /// </summary>
    internal class ScheduleCreationUtility
    {
        private static readonly BuiltInParameter[] SSkipParameters = { BuiltInParameter.ALL_MODEL_MARK };

        /// <summary>
        ///     Create view schedule(s) and add them to sheet.
        /// </summary>
        /// <param name="uiDocument">UIdocument of revit file.</param>
        public void CreateAndAddSchedules(UIDocument uiDocument)
        {
            var tGroup = new TransactionGroup(uiDocument.Document, "Create schedules and sheets");
            tGroup.Start();

            var schedules = CreateSchedules(uiDocument);

            foreach (var schedule in schedules) AddScheduleToNewSheet(uiDocument.Document, schedule);

            tGroup.Assimilate();
        }

        /// <summary>
        ///     Create a sheet to show the schedule.
        /// </summary>
        /// <param name="document">DBDocument of revit file.</param>
        /// <param name="schedule">View schedule which will be shown on sheet.</param>
        private void AddScheduleToNewSheet(Document document, ViewSchedule schedule)
        {
            //Create a filter to get all the title block types.
            var collector = new FilteredElementCollector(document);
            collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            collector.WhereElementIsElementType();

            var t = new Transaction(document, "Create and populate sheet");
            t.Start();

            //Get ElementId of first title block type.
            var titleBlockId = collector.FirstElementId();

            //Create sheet by gotten title block type.
            var newSheet = ViewSheet.Create(document, titleBlockId);
            newSheet.Name = "Sheet for " + schedule.Name;

            document.Regenerate();

            //Declare a XYZ to be used as the upperLeft point of schedule sheet instance to be created.
            var upperLeft = new XYZ();

            //If there is an existing title block.
            if (titleBlockId != ElementId.InvalidElementId)
            {
                //Find titleblock of the newly created sheet.
                collector = new FilteredElementCollector(document);
                collector.OfCategory(BuiltInCategory.OST_TitleBlocks);
                collector.OwnedByView(newSheet.Id);
                var titleBlock = collector.FirstElement();

                //Get bounding box of the title block.
                var bbox = titleBlock.get_BoundingBox(newSheet);

                //Get upperLeft point of the bounding box.
                upperLeft = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);
                //Move the point to the postion that is 2 inches right and 2 inches down from the original upperLeft point.
                upperLeft = upperLeft + new XYZ(2.0 / 12.0, -2.0 / 12.0, 0);
            }

            //Create a new schedule sheet instance that makes the sheet to show the data of wall view schedule at upperLeft point.
            ScheduleSheetInstance.Create(document, newSheet.Id, schedule.Id, upperLeft);

            t.Commit();
        }

        /// <summary>
        ///     Create a view schedule of wall category and add schedule field, filter and sorting/grouping field to it.
        /// </summary>
        /// <param name="uiDocument">UIdocument of revit file.</param>
        /// <returns>ICollection of created view schedule(s).</returns>
        private ICollection<ViewSchedule> CreateSchedules(UIDocument uiDocument)
        {
            var document = uiDocument.Document;

            var t = new Transaction(document, "Create Schedules");
            t.Start();

            var schedules = new List<ViewSchedule>();

            //Create an empty view schedule of wall category.
            var schedule = ViewSchedule.CreateSchedule(document, new ElementId(BuiltInCategory.OST_Walls),
                ElementId.InvalidElementId);
            schedule.Name = "Wall Schedule 1";
            schedules.Add(schedule);

            //Iterate all the schedulable field gotten from the walls view schedule.
            foreach (var schedulableField in schedule.Definition.GetSchedulableFields())
                //Judge if the FieldType is ScheduleFieldType.Instance.
                if (schedulableField.FieldType == ScheduleFieldType.Instance)
                {
                    //Get ParameterId of SchedulableField.
                    var parameterId = schedulableField.ParameterId;

                    //If the ParameterId is id of BuiltInParameter.ALL_MODEL_MARK then ignore next operation.
                    if (ShouldSkip(parameterId))
                        continue;

                    //Add a new schedule field to the view schedule by using the SchedulableField as argument of AddField method of Autodesk.Revit.DB.ScheduleDefinition class.
                    var field = schedule.Definition.AddField(schedulableField);

                    //Judge if the parameterId is a BuiltInParameter.
                    if (Autodesk.Revit.DB.ParameterUtils.IsBuiltInParameter(parameterId))
                    {
                        var bip = (BuiltInParameter)parameterId.Value;
                        //Get the StorageType of BuiltInParameter.
                        var st = document.get_TypeOfStorage(bip);
                        //if StorageType is String or ElementId, set GridColumnWidth of schedule field to three times of current GridColumnWidth. 
                        //And set HorizontalAlignment property to left.
                        if (st == StorageType.String || st == StorageType.ElementId)
                        {
                            field.GridColumnWidth = 3 * field.GridColumnWidth;
                            field.HorizontalAlignment = ScheduleHorizontalAlignment.Left;
                        }
                        //For other StorageTypes, set HorizontalAlignment property to center.
                        else
                        {
                            field.HorizontalAlignment = ScheduleHorizontalAlignment.Center;
                        }
                    }

                    //Filter the view schedule by volume
                    if (field.ParameterId == new ElementId(BuiltInParameter.HOST_VOLUME_COMPUTED))
                    {
                        var volumeFilterInCubicFt = 0.8 * Math.Pow(3.2808399, 3.0);
                        var filter = new ScheduleFilter(field.FieldId, ScheduleFilterType.GreaterThan,
                            volumeFilterInCubicFt);
                        schedule.Definition.AddFilter(filter);
                    }

                    //Group and sort the view schedule by type
                    if (field.ParameterId == new ElementId(BuiltInParameter.ELEM_TYPE_PARAM))
                    {
                        var sortGroupField = new ScheduleSortGroupField(field.FieldId);
                        sortGroupField.ShowHeader = true;
                        schedule.Definition.AddSortGroupField(sortGroupField);
                    }
                }

            t.Commit();

            uiDocument.ActiveView = schedule;

            return schedules;
        }

        /// <summary>
        ///     Judge if the parameterId should be skipped.
        /// </summary>
        /// <param name="parameterId">ParameterId to be judged.</param>
        /// <returns>Return true if parameterId should be skipped.</returns>
        private bool ShouldSkip(ElementId parameterId)
        {
            foreach (var bip in SSkipParameters)
                if (new ElementId(bip) == parameterId)
                    return true;
            return false;
        }
    }
}
