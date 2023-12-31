// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.AttachedDetailGroup.CS
{
    /// <summary>
    ///     The external command that hides all of the selected group's attached detail groups.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AttachedDetailGroupHideAllCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation of the command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;
            var activeView = commandData.View;
            var groupHelper = new GroupHelper();
            Group selectedModelGroup;

            if (!groupHelper.GetSelectedModelGroup(uiDoc, out selectedModelGroup, out message)) return Result.Cancelled;

            groupHelper.HideAllAttachedDetailGroups(selectedModelGroup, doc, activeView);
            return Result.Succeeded;
        }
    }
}
