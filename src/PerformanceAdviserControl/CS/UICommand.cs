//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

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