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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.ScheduleAutomaticFormatter.CS
{
    /// <summary>
    ///     An external command that formats the columns of the schedule automatically.  After this has taken place,
    ///     the schedule formatting will be automatically updated when the schedule changes.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ScheduleFormatterCommand : IExternalCommand
    {
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
            m_theFormatter ??= new ScheduleFormatter
            {
                Schema = schema,
                AddInId = commandData.Application.ActiveAddInId
            };

            using (Transaction t = new(viewSchedule.Document, "Format columns"))
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

        private static Schema GetOrCreateSchema()
        {
            Guid schemaId = new("98017A5F-F4A7-451C-8807-EF137B587C50");

            var schema = Schema.Lookup(schemaId);
            if (schema == null)
            {
                SchemaBuilder builder = new(schemaId);
                builder.SetSchemaName("ScheduleFormatterFlag");
                builder.AddSimpleField("Formatted", typeof(bool));
                schema = builder.Finish();
            }

            return schema;
        }

        private static void AddUpdater(ScheduleFormatter formatter)
        {
            // If not registered, register the updater
            if (!UpdaterRegistry.IsUpdaterRegistered(formatter.GetUpdaterId()))
            {
                // Filter on: schedule type, and extensible storage entity of the target schema
                ElementClassFilter classFilter = new(typeof(ViewSchedule));
                ExtensibleStorageFilter esFilter = new(formatter.Schema.GUID);
                LogicalAndFilter filter = new(classFilter, esFilter);

                // Register and add trigger for updater.
                UpdaterRegistry.RegisterUpdater(formatter);
                UpdaterRegistry.AddTrigger(formatter.GetUpdaterId(), filter, Element.GetChangeTypeAny());
            }
        }
    }
}
