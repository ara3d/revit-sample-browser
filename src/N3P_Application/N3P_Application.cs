using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Application.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Application : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var app = data.Application.Application;

            N3POutput.Header("Nice3point Application / optional module extensions");

            N3POutput.Line("IsDgnExportAvailable", app.IsDgnExportAvailable);
            N3POutput.Line("IsDwgExportAvailable", app.IsDwgExportAvailable);
            N3POutput.Line("IsIfcAvailable", app.IsIfcAvailable);
            N3POutput.Line("IsNavisworksExporterAvailable", app.IsNavisworksExporterAvailable);
            N3POutput.Line("IsFbxExportAvailable", app.IsFbxExportAvailable);
            N3POutput.Line("IsGraphicsAvailable", app.IsGraphicsAvailable);

            var controlled = app.AsControlledApplication();
            N3POutput.Line("AsControlledApplication()", controlled != null);

            return Result.Succeeded;
        }
    }
}
