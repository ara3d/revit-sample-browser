// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PerformanceAdviserControl.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    internal class UICommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //A list of rule information to be used below
            var ruleInfoList = new List<RuleInfo>();


            ///Here, we query the information about rules registered in PerformanceAdviser so that
            ///we can later decide in a dialog which rules we want to run.

            var performanceAdviser = PerformanceAdviser.GetPerformanceAdviser();


            ICollection<PerformanceAdviserRuleId> allIds = performanceAdviser.GetAllRuleIds();
            foreach (var ruleID in allIds)
            {
                var ruleName = performanceAdviser.GetRuleName(ruleID);
                var ruleDescription = performanceAdviser.GetRuleDescription(ruleID);
                var isEnabled = performanceAdviser.IsRuleEnabled(ruleID);

                //We want to mark user-defined (API) rules, so we check to see if the current rule ID is
                //equal to the rule ID we created.
                var isOurRule = ruleID == FlippedDoorCheck.Id;

                var oneRule = new RuleInfo(ruleID, isOurRule, ruleName, ruleDescription, isEnabled);
                ruleInfoList.Add(oneRule);
            }


            //This dialog box will allow the user to select and run performance rules, so it needs
            //the PerformanceAdviser and active document passed to it.
            var tdd = new TestDisplayDialog(PerformanceAdviser.GetPerformanceAdviser(),
                commandData.Application.ActiveUIDocument.Document);

            foreach (var r in ruleInfoList)
                /// Add the rule data we just collected in the previous loop the the dialog box
                /// we are about to show.
                tdd.AddData(r.RuleName, r.IsOurRule, r.IsEnabled);

            tdd.ShowDialog();

            return Result.Succeeded;
        }
    }
}
