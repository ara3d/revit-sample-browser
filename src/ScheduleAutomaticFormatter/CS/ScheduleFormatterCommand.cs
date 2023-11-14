// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ScheduleAutomaticFormatter.CS
{
    /// <summary>
    ///     An external command that formats the columns of the schedule automatically.  After this has taken place,
    ///     the schedule formatting will be automatically updated when the schedule changes.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class ScheduleFormatterCommand : IExternalCommand
    {
        /// <summary>
        ///     The formatter used by the command when updating the schedule.
        ///     Created upon first use.
        /// </summary>
        private ScheduleFormatter m_theFormatter;

        /// <summary>
        ///     The command implementation.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var viewSchedule = commandData.View as ViewSchedule;

            // Setup info needed for the updater
            var schema = GetOrCreateSchema();

            // Setup formatter for the schedule, if not setup. 
            if (m_theFormatter == null)
            {
                m_theFormatter = new ScheduleFormatter
                {
                    Schema = schema,
                    AddInId = commandData.Application.ActiveAddInId
                };
            }

            using (var t = new Transaction(viewSchedule.Document, "Format columns"))
            {
                t.Start();
                // Make formatting changes
                m_theFormatter.FormatScheduleColumns(viewSchedule);

                // Mark schedule to be formatted
                AddMarkerEntity(viewSchedule, schema);
                t.Commit();
            }

            // Add updater to listen to further changes
            AddUpdater(m_theFormatter);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Adds an entity to the schedule, indicating that the schedule should be formatted by this tool.
        /// </summary>
        /// <param name="viewSchedule">The schedule.</param>
        /// <param name="schema">The schema used for the entity.</param>
        private void AddMarkerEntity(ViewSchedule viewSchedule, Schema schema)
        {
            // Is entity already present?
            var entity = viewSchedule.GetEntity(schema);

            // If not, add it.
            if (!entity.IsValid())
            {
                entity = new Entity(schema);
                entity.Set("Formatted", true);
                viewSchedule.SetEntity(entity);
            }
        }

        /// <summary>
        ///     Set up the schema used to mark the schedules as formatted.
        /// </summary>
        /// <returns></returns>
        private static Schema GetOrCreateSchema()
        {
            var schemaId = new Guid("98017A5F-F4A7-451C-8807-EF137B587C50");

            var schema = Schema.Lookup(schemaId);
            if (schema == null)
            {
                var builder = new SchemaBuilder(schemaId);
                builder.SetSchemaName("ScheduleFormatterFlag");
                builder.AddSimpleField("Formatted", typeof(bool));
                schema = builder.Finish();
            }

            return schema;
        }

        /// <summary>
        ///     Add the updater to watch for formatted schedule changes.
        /// </summary>
        /// <param name="formatter">The schedule formatter.</param>
        private static void AddUpdater(ScheduleFormatter formatter)
        {
            // If not registered, register the updater
            if (!UpdaterRegistry.IsUpdaterRegistered(formatter.GetUpdaterId()))
            {
                // Filter on: schedule type, and extensible storage entity of the target schema
                var classFilter = new ElementClassFilter(typeof(ViewSchedule));
                var esFilter = new ExtensibleStorageFilter(formatter.Schema.GUID);
                var filter = new LogicalAndFilter(classFilter, esFilter);

                // Register and add trigger for updater.
                UpdaterRegistry.RegisterUpdater(formatter);
                UpdaterRegistry.AddTrigger(formatter.GetUpdaterId(), filter, Element.GetChangeTypeAny());
            }
        }
    }
}
