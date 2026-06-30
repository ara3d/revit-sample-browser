using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using ExcelExporterImporter.Views;
using log4net;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Interop;

namespace ExcelExporterImporter
{
    [Transaction(TransactionMode.Manual)]
    public class CmdExcelExport : IExternalCommand
    {
        private static readonly ILog Logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            try
            {
                var doc = commandData.Application.ActiveUIDocument?.Document;
                if (doc == null)
                    return Result.Cancelled;

                ExportWindow dlg = new(doc);
                WindowInteropHelper window = new(dlg)
                {
                    Owner = ComponentManager.ApplicationWindow
                };
                dlg.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
