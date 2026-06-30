// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Linq;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using ToolkitExternalCommand = Nice3point.Revit.Toolkit.External.ExternalCommand;

namespace Ara3D.RevitSampleBrowser.N3P_Scopes.CS
{
    [Transaction(TransactionMode.Manual)]
    public class N3P_Scopes : ToolkitExternalCommand
    {
        public override void Execute()
        {
            var doc = Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point RevitToolkit scopes");

            var walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .ToElementIds();

            if (walls.Count < 1)
            {
                N3POutput.Line("Walls", "none found — open a model with walls");
                return;
            }

            var sourceId = walls.First();

            using (RevitToolkitScopes.BeginFailureSuppression())
            using (RevitToolkitScopes.BeginDialogSuppression(TaskDialogResult.Ok))
            using (var transaction = new Transaction(doc, "N3P copy with suppressed failures"))
            {
                transaction.Start();
                var copied = ElementTransformUtils.CopyElement(doc, sourceId, XYZ.BasisX * 5);
                transaction.Commit();
                N3POutput.Line("Copied element id", copied);
            }

            N3POutput.Line("Failure suppression", "completed");
            N3POutput.Line("Dialog suppression", "completed");
        }
    }
}
