// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        private static int _stairsIndex;
        private static readonly int[] StairsConfigs = { 0, 3, 4, 1, 2 };

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDocument = commandData.Application.ActiveUIDocument;
            var document = activeDocument.Document;

            // Create an automation utility with a hardcoded stairs configuration number
            var utility = StairsAutomationUtility.Create(document, StairsConfigs[_stairsIndex]);

            // Generate the stairs
            utility.GenerateStairs();

            _stairsIndex = _stairsIndex > 4 ? 0 : _stairsIndex + 1;

            return Result.Succeeded;
        }
    }
}
