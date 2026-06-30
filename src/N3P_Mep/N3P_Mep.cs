using System.Linq;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Mep.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Mep : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            var pipes = doc.CollectElements().OfClass<Pipe>().ToElements().Cast<Pipe>().Take(5).ToList();
            if (pipes.Count == 0)
            {
                message = "No pipes found. Open an MEP model or place pipes first.";
                return Result.Failed;
            }

            N3POutput.Header("Nice3point MEP / Pipe extensions");

            foreach (var pipe in pipes)
            {
                N3POutput.Line($"Pipe {pipe.Id}");
                N3POutput.Line("  HasOpenConnector", pipe.HasOpenConnector);
            }

            return Result.Succeeded;
        }
    }
}
