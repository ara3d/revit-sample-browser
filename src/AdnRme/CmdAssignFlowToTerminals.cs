#region Header
// Revit MEP API sample application
//
// Copyright (C) 2007-2021 by Jeremy Tammik, Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software
// for any purpose and without fee is hereby granted, provided
// that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  
// AUTODESK, INC. DOES NOT WARRANT THAT THE OPERATION OF THE 
// PROGRAM WILL BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject
// to restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Header

#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion // Namespaces

namespace AdnRme
{
    [Transaction(TransactionMode.Manual)]
    public class CmdAssignFlowToTerminals : IExternalCommand
    {
        #region Get Terminals Per Space
        class NumericalComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return int.Parse(x) - int.Parse(y);
            }
        }

        static Dictionary<string, List<FamilyInstance>> GetTerminalsPerSpace(
          Document doc)
        {
            var terminals = Util.GetSupplyAirTerminals(doc);

            Dictionary<string, List<FamilyInstance>> terminalsPerSpace
              = [];

            foreach (FamilyInstance terminal in terminals)
            {
                //string roomNr = terminal.Room.Number;
                var spaceNr = terminal.Space.Number; // changed Room to Space
                if (!terminalsPerSpace.ContainsKey(spaceNr))
                {
                    terminalsPerSpace.Add(spaceNr, []);
                }
                terminalsPerSpace[spaceNr].Add(terminal);
            }

            List<string> keys = [.. terminalsPerSpace.Keys];
            keys.Sort(new NumericalComparer());

            string ids;
            List<FamilyInstance> spaceTerminals;
            int n, nTerminals = 0;

            foreach (var key in keys)
            {
                spaceTerminals = terminalsPerSpace[key];
                n = spaceTerminals.Count;
                ids = Util.IdList(spaceTerminals);

                Debug.WriteLine(string.Format(
                  "Space {0} contains {1} air terminal{2}{3} {4}",
                  key, n, Util.PluralSuffix(n), Util.DotOrColon(n), ids));

                nTerminals += n;
            }
            n = terminalsPerSpace.Count;

            Debug.WriteLine(string.Format(
              "Processing a total of {0} space{1} containing {2} air terminal{3}.",
              n, Util.PluralSuffix(n), nTerminals, Util.PluralSuffix(nTerminals)));

            return terminalsPerSpace;
        }
        #endregion // Get Terminals Per Space

        #region AssignFlowToTerminals

        static double RoundFlowTo(double a)
        {
            a /= Const.RoundTerminalFlowTo;
            a = Math.Round(a, 0, MidpointRounding.AwayFromZero);
            a *= Const.RoundTerminalFlowTo;
            return a;
        }

        static void AssignFlowToTerminals(List<FamilyInstance> terminals, double flow)
        {
            foreach (var terminal in terminals)
            {
                var p = Util.GetTerminalFlowParameter(terminal);
                p.Set(flow);
            }
        }

        static void AssignFlowToTerminalsForSpace(
          List<FamilyInstance> terminals,
          Space space)
        {
            Debug.Assert(null != terminals, "expected valid list of terminals");
            var n = terminals.Count;

            var calculatedSupplyAirFlow = Util.GetSpaceParameterValue(
              space, Bip.CalculatedSupplyAirFlow, ParameterName.CalculatedSupplyAirFlow);

            var flowCfm = calculatedSupplyAirFlow * Const.SecondsPerMinute;
            var flowCfmPerOutlet = flowCfm / n;
            var flowCfmPerOutletRounded = RoundFlowTo(flowCfmPerOutlet);
            var flowPerOutlet = flowCfmPerOutletRounded / Const.SecondsPerMinute;

            var format = "Space {0} has calculated supply airflow {1} f^3/s = {2} CFM and {3} terminal{4}"
                         + " --> flow {5} CFM per terminal, rounded to {6} = {7} f^3/s";

            Debug.WriteLine(string.Format(format,
              space.Number, Util.RealString(calculatedSupplyAirFlow), Util.RealString(flowCfm),
              n, Util.PluralSuffix(n), Util.RealString(flowCfmPerOutlet),
              Util.RealString(flowCfmPerOutletRounded), Util.RealString(flowPerOutlet)));

            AssignFlowToTerminals(terminals, flowPerOutlet);
        }
        #endregion // AssignFlowToTerminals

        #region Execute Command
        public Result Execute(
          ExternalCommandData commandData,
          ref String message,
          ElementSet elements)
        {
            try
            {
                WaitCursor waitCursor = new();
                var uiapp = commandData.Application;
                var doc = uiapp.ActiveUIDocument.Document;
                //
                // 1. determine air terminals for each space.
                // determine the relationship between all air terminals and all spaces:
                // extract and group all air terminals per space 
                // (key=space, val=set of air terminals)
                //
                Debug.WriteLine("\nDetermining terminals per space...");
                var terminalsPerSpace = GetTerminalsPerSpace(doc);
                //
                // 2. assign flow to the air terminals depending on the space's calculated supply air flow.
                //
                //ElementFilterIterator it = doc.get_Elements( typeof( Room ) ); // 2008
                //ElementIterator it = doc.get_Elements( typeof( Space ) ); // 2009
                //List<Element> spaces = new List<Element>(); // 2009
                //doc.get_Elements( typeof( Space ), spaces ); // 2009
                //FilteredElementCollector collector = new FilteredElementCollector( doc );
                //collector.OfClass( typeof( Space ) );
                //IList<Element> spaces = collector.ToElements();

                using Transaction tx = new(doc);
                var spaces = Util.GetSpaces(doc);
                var n = spaces.Count;

                var s = "{0} of " + n.ToString() + " spaces processed...";
                var caption = "Assign Flow to Terminals";
                tx.Start(caption);

                using (ProgressForm pf = new(caption, s, n))
                {
                    foreach (var space in spaces)
                    {
                        if (terminalsPerSpace.ContainsKey(space.Number))
                        {
                            AssignFlowToTerminalsForSpace(terminalsPerSpace[space.Number], space);
                        }
                        pf.Increment();
                    }
                }
                tx.Commit();
                Debug.WriteLine("Completed.");
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        #endregion // Execute Command
    }
}
