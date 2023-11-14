//
// (C) Copyright 2003-2019 by Autodesk, Inc. All rights reserved.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

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
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Revit.SDK.Samples.ScheduleAutomaticFormatter.CS
{
    /// <summary>
    ///     A class capable of formatting schedules with alternating background colors on the columns
    /// </summary>
    internal class ScheduleFormatter : IUpdater
    {
        /// <summary>
        ///     Constructs a new schedule formatter utility.
        /// </summary>
        public ScheduleFormatter()
        {
            Schema = null;
            AddInId = null;
        }


        /// <summary>
        ///     The ExtensibleStorage schema for marking schedules which have been formatted.
        /// </summary>
        public Schema Schema { get; set; }

        /// <summary>
        ///     The add-in id.
        /// </summary>
        public AddInId AddInId { get; set; }


        /// <summary>
        ///     GUID of the updater.
        /// </summary>
        private static Guid UpdaterGUID => new Guid("{C8483107-EF6D-4FDB-BB88-AF79E0E62361}");


        /// <summary>
        ///     Implements IUpdater.Execute()
        /// </summary>
        /// <param name="data"></param>
        public void Execute(UpdaterData data)
        {
            // Only previously formatted schedules should trigger - so just reformat them
            foreach (var scheduleId in data.GetModifiedElementIds())
            {
                var schedule = data.GetDocument().GetElement(scheduleId) as ViewSchedule;
                FormatScheduleColumns(schedule);
            }
        }

        /// <summary>
        ///     Implements IUpdater.GetAdditionalInformation()
        /// </summary>
        /// <returns></returns>
        public string GetAdditionalInformation()
        {
            return "Automatic schedule formatter";
        }

        /// <summary>
        ///     Implements IUpdater.GetChangePriority()
        /// </summary>
        /// <returns></returns>
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        /// <summary>
        ///     Implements IUpdater.GetUpdaterId()
        /// </summary>
        /// <returns></returns>
        public UpdaterId GetUpdaterId()
        {
            return new UpdaterId(AddInId, UpdaterGUID);
        }

        /// <summary>
        ///     Implements IUpdater.GetUpdaterName()
        /// </summary>
        /// <returns></returns>
        public string GetUpdaterName()
        {
            return "AutomaticScheduleFormatter";
        }

        /// <summary>
        ///     Formats the schedule with alternating background colors
        /// </summary>
        /// <param name="viewSchedule"></param>
        public void FormatScheduleColumns(ViewSchedule viewSchedule)
        {
            var definition = viewSchedule.Definition;
            var index = 0;

            var white = new Color(0xFF, 0xFF, 0xFF);
            var highlight = new Color(0xd8, 0xd8, 0xd8);
            var applyHighlight = false;

            // Loop over fields in order
            foreach (var id in definition.GetFieldOrder())
            {
                // Index 0, 2, etc use highlight color
                if (index % 2 == 0)
                    applyHighlight = true;
                // Index 1, 3, etc use no background color
                else
                    applyHighlight = false;

                // Get the field style
                var field = definition.GetField(id);
                var style = field.GetStyle();
                var options = style.GetCellStyleOverrideOptions();

                // Set override options for background color per requirement
                options.BackgroundColor = applyHighlight;
                style.SetCellStyleOverrideOptions(options);

                // Set background color per requirement
                style.BackgroundColor = applyHighlight ? highlight : white;
                field.SetStyle(style);

                index++;
            }
        }
    }
}