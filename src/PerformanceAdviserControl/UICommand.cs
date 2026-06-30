// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
namespace Ara3D.RevitSampleBrowser.PerformanceAdviserControl.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class UiCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var ruleInfoList = ElementQuery.CollectRuleInfo(PerformanceAdviser.GetPerformanceAdviser());

            var tdd = new TestDisplayDialog(PerformanceAdviser.GetPerformanceAdviser(),
                commandData.Application.ActiveUIDocument.Document);

            foreach (var r in ruleInfoList)
            {
                tdd.AddData(r.RuleName, r.IsOurRule, r.IsEnabled);
            }

            tdd.ShowDialog();

            return Result.Succeeded;
        }
    }
}
