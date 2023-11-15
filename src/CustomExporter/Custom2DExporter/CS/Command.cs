// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Custom2DExporter.CS
{
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
                if (!IsExportableView(activeView))
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

                        Utilities.DisplayExport(activeView, points);

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

        private bool IsExportableView(View view)
        {
            if (!view.CanBePrinted || view.IsTemplate)
                return false;

            var exportableTypes = GetExportableViewTypes();
            return exportableTypes.Contains(view.ViewType);
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

            resultsSummary = new ResultsSummary
            {
                NumElements = context.NumElements,
                NumTexts = context.NumTexts,
                Texts = context.Texts
            };
        }

        /// <summary>
        ///     Displays the results from a run of path of travel creation using a TaskDialog.
        /// </summary>
        /// <param name="resultsSummary"></param>
        private static void ShowResults(ResultsSummary resultsSummary)
        {
            var td = new TaskDialog("Results of 2D export");
            td.MainInstruction = $"2D exporter exported {resultsSummary.NumElements} elements";
            var details = $"There were {resultsSummary.NumTexts} text nodes exported.\n\n";

            if (resultsSummary.NumTexts > 0 && resultsSummary.Texts.Length > 0)
                details += "Exported text nodes:\n" + resultsSummary.Texts;

            td.MainContent = details;

            td.Show();
        }

        /// <summary>
        ///     Class that aggregates the results of the export.
        /// </summary>
        private class ResultsSummary
        {
            public int NumElements { get; set; }

            public int NumTexts { get; set; }

            public string Texts { get; set; }
        }
    }
}
