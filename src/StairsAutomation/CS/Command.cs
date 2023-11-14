// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.StairsAutomation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        private static int stairsIndex;
        private static readonly int[] stairsConfigs = { 0, 3, 4, 1, 2 };

        /// <summary>
        ///     The implementation of the automatic stairs creation command.
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDocument = commandData.Application.ActiveUIDocument;
            var document = activeDocument.Document;

            // Create an automation utility with a hardcoded stairs configuration number
            var utility = StairsAutomationUtility.Create(document, stairsConfigs[stairsIndex]);

            // Generate the stairs
            utility.GenerateStairs();

            stairsIndex++;
            if (stairsIndex > 4)
                stairsIndex = 0;

            return Result.Succeeded;
        }
    }
}
