using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.N3P_Families.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Families : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point Families and modeling extensions");

            var family = doc.CollectElements().OfClass<Family>().FirstOrDefault();
            if (family == null)
            {
                message = "No loadable families found in the document.";
                return Result.Failed;
            }

            var symbols = doc.CollectElements()
                .FamilySymbols((Family)family)
                .ToElements();
            N3POutput.Line($"Family '{family.Name}' symbol count", symbols.Count);

            var instances = doc.CollectElements()
                .FamilyInstances(doc, family.Id)
                .Take(3);
            N3POutput.Line("Sample instance ids", string.Join(", ", instances.Select(e => e.Id)));

            var wall = doc.CollectElements().OfClass<Wall>().FirstOrDefault();
            if (wall != null)
            {
                var joined = wall.GetJoinedElements();
                N3POutput.Line($"Wall {wall.Id} joined elements", joined.Count);
            }

            return Result.Succeeded;
        }
    }
}
