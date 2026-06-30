using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Document.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Document : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point Document extensions");

            var version = doc.Version;
            N3POutput.Line("Version", version);

            var familiesOk = doc.CheckAllFamilies(out var corruptIds);
            N3POutput.Line("CheckAllFamilies", familiesOk);
            if (!familiesOk)
                N3POutput.Line("  Corrupt family count", corruptIds.Count);

            if (doc.AreGlobalParametersAllowed())
            {
                var globals = doc.GetAllGlobalParameters();
                N3POutput.Line("Global parameters", globals.Count);
            }
            else
            {
                N3POutput.Line("Global parameters", "not allowed in this document");
            }

            return Result.Succeeded;
        }
    }
}
