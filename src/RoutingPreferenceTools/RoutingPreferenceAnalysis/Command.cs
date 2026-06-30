// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceAnalysis
{
    /// <summary>
    ///     Main Revit command
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!RoutingPreferenceHelper.ValidateMep(commandData.Application.Application))
            {
                RoutingPreferenceHelper.MepWarning();
                return Result.Succeeded;
            }

            if (!RoutingPreferenceHelper.ValidatePipesDefined(commandData.Application.ActiveUIDocument.Document))
            {
                RoutingPreferenceHelper.PipesDefinedWarning();
                return Result.Succeeded;
            }

            MainWindow mainWindow = new(commandData.Application);
            mainWindow.ShowDialog();
            return Result.Succeeded;
        }
    }
}
