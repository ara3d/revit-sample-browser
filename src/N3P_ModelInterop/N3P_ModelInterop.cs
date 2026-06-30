using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_ModelInterop.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_ModelInterop : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point Model access and worksharing extensions");

            if (doc.IsWorkshared)
            {
                var centralPath = WorksharingUtils.GetCentralModelPath(doc);
                N3POutput.Line("Central model path", centralPath.ConvertToUserVisiblePath());
            }
            else if (!string.IsNullOrEmpty(doc.PathName))
            {
                N3POutput.Line("Document path", doc.PathName);
            }
            else
            {
                N3POutput.Line("Document", "unsaved or cloud-only");
            }

            var wall = doc.CollectElements().OfClass<Wall>().FirstOrDefault();
            if (wall != null && doc.IsWorkshared)
            {
                var status = wall.GetCheckoutStatus(out var owner);
                N3POutput.Line($"Wall {wall.Id} checkout", status);
                if (!string.IsNullOrEmpty(owner))
                    N3POutput.Line("  Owner", owner);

                N3POutput.Line("  Model updates", wall.GetModelUpdatesStatus());
            }

            return Result.Succeeded;
        }
    }
}
