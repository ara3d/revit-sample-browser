// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ScheduleToHTML.CS
{
    /// <summary>
    ///     The external command exporting the active schedule to HTML.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ScheduleHtmlExportCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation of the command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeView = commandData.View;

            if (activeView is ViewSchedule view)
            {
                var exporter = new ScheduleHtmlExporter(view);
                var revitApplication = commandData.Application.Application;
                var bInteractive = revitApplication.IsJournalPlaying() ? false : true;
                return exporter.ExportToHtml(bInteractive, ref message) ? Result.Succeeded : Result.Cancelled;
            }

            message = "Unable to proceed: Active view must be a schedule.";
            return Result.Cancelled;
        }
    }
}
