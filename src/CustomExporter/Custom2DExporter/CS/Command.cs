//
// (C) Copyright 2003-2016 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

namespace Revit.SDK.Samples.Custom2DExporter.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var activeView = uiDoc.ActiveView;
                if (!isExportableView(activeView))
                {
                    var td = new TaskDialog("Cannot export view.");
                    td.MainInstruction = "Only plans, elevations and sections can be exported.";

                    td.Show();

                    return Result.Succeeded;
                }

                using (var exportForm = new Export2DView())
                {
                    if (DialogResult.OK == exportForm.ShowDialog())
                    {
                        IList<XYZ> points = null;
                        ResultsSummary resSummary = null;
                        ExportView(activeView,
                            activeView.DisplayStyle /*display with current display style*/,
                            true /* always export some geometry */,
                            exportForm.ViewExportOptions.ExportAnnotationObjects,
                            exportForm.ViewExportOptions.ExportPatternLines,
                            out points,
                            out resSummary);

                        Utilities.displayExport(activeView, points);

                        ShowResults(resSummary);
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Generates the list of view types supported by the Exporter.
        /// </summary>
        /// <returns>List of types that are valid view types for export. </returns>
        public static ICollection<ViewType> GetExportableViewTypes()
        {
            return new[]
            {
                ViewType.FloorPlan,
                ViewType.CeilingPlan,
                ViewType.Section,
                ViewType.Elevation,
                ViewType.Detail,
                ViewType.AreaPlan,
                ViewType.EngineeringPlan
            };
        }

        private bool isExportableView(View view)
        {
            if (!view.CanBePrinted || view.IsTemplate)
                return false;

            var exportableTypes = GetExportableViewTypes();
            if (!exportableTypes.Contains(view.ViewType))
                return false;

            return true;
        }

        private static void ExportView(View exportableView,
            DisplayStyle displayStyle,
            bool includeGeometricObjects,
            bool export2DIncludingAnnotationObjects,
            bool export2DGeometricObjectsIncludingPatternLines,
            out IList<XYZ> points,
            out ResultsSummary resultsSummary)
        {
            var context = new TessellatedGeomAndText2DExportContext(out points);
            var exporter = new CustomExporter(exportableView.Document, context);
            exporter.IncludeGeometricObjects = includeGeometricObjects;
            exporter.Export2DIncludingAnnotationObjects = export2DIncludingAnnotationObjects;
            exporter.Export2DGeometricObjectsIncludingPatternLines = export2DGeometricObjectsIncludingPatternLines;
            exporter.ShouldStopOnError = true;
            exporter.Export(exportableView);
            exporter.Dispose();

            resultsSummary = new ResultsSummary();
            resultsSummary.numElements = context.NumElements;
            resultsSummary.numTexts = context.NumTexts;
            resultsSummary.texts = context.Texts;
        }

        /// <summary>
        ///     Displays the results from a run of path of travel creation using a TaskDialog.
        /// </summary>
        /// <param name="resultsSummary"></param>
        private static void ShowResults(ResultsSummary resultsSummary)
        {
            var td = new TaskDialog("Results of 2D export");
            td.MainInstruction = string.Format("2D exporter exported {0} elements", resultsSummary.numElements);
            var details = string.Format("There were {0} text nodes exported.\n\n",
                resultsSummary.numTexts);

            if (resultsSummary.numTexts > 0 && resultsSummary.texts.Length > 0)
                details += "Exported text nodes:\n" + resultsSummary.texts;

            td.MainContent = details;

            td.Show();
        }

        /// <summary>
        ///     Class that aggregates the results of the export.
        /// </summary>
        private class ResultsSummary
        {
            public int numElements { get; set; }
            public int numTexts { get; set; }
            public string texts { get; set; }
        }
    }
}