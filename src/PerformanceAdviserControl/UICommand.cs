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
            //A list of rule information to be used below
            var ruleInfoList = ElementQuery.CollectRuleInfo(PerformanceAdviser.GetPerformanceAdviser());

            //This dialog box will allow the user to select and run performance rules, so it needs
            //the PerformanceAdviser and active document passed to it.
            var tdd = new TestDisplayDialog(PerformanceAdviser.GetPerformanceAdviser(),
                commandData.Application.ActiveUIDocument.Document);

            foreach (var r in ruleInfoList)
                /// Add the rule data we just collected in the previous loop the the dialog box
                /// we are about to show.
            {
                tdd.AddData(r.RuleName, r.IsOurRule, r.IsEnabled);
            }

            tdd.ShowDialog();

            return Result.Succeeded;
        }
    }
}
