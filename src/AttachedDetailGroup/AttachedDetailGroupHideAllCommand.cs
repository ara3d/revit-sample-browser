// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS
{
    [Transaction(TransactionMode.Manual)]
    public class AttachedDetailGroupHideAllCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;
            var activeView = commandData.View;
            if (!SelectionHelper.GetSelectedModelGroup(uiDoc, out var selectedModelGroup, out message)) return Result.Cancelled;

            GroupVisibilityHelper.HideAllAttachedDetailGroups(selectedModelGroup, doc, activeView);
            return Result.Succeeded;
        }
    }
}
