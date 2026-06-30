using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions.UI;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.N3P_UIApplication.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_UIApplication : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uiapp = data.Application;

            N3POutput.Header("Nice3point UIApplication / Ribbon extensions");

            var controlled = uiapp.AsControlledApplication();
            N3POutput.Line("UIApplication.AsControlledApplication()", controlled != null);

            var panels = uiapp.GetRibbonPanels();
            N3POutput.Line("Ribbon panel count", panels.Count);

            foreach (var panel in panels.Take(5))
                N3POutput.Line("  Panel", panel.Name);

            N3POutput.Line("Note", "Ribbon authoring (CreatePanel, SetImage) runs in IExternalApplication.OnStartup.");

            return Result.Succeeded;
        }
    }
}
