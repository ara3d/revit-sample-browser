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
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion // Namespaces

namespace AdnRme
{
    #region CmdElectricalSystemBrowser
    /// <summary>
    /// Reproduces the Power section of the MEP system browser (panel &gt; system &gt; element) in a modeless tree.
    /// Built on parameter lookups from Revit 2008; prefer CmdElectricalConnectors for new work.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class CmdElectricalSystemBrowser : IExternalCommand
    {
        public const string Unassigned = "Unassigned";

        #region Sorting comparers

        #region NumericalComparer
        class NumericalComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return int.Parse(x) - int.Parse(y);
            }
        }
        #endregion // NumericalComparer

        #region PanelCircuitComparer
        // Sort panel:circuit keys; handles comma-delimited circuit lists (e.g. MDP:1,3,5).
        class PanelCircuitComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var d = x.CompareTo(y);
                if (0 != d)
                {
                    var a = x.Split(':');
                    var b = y.Split(':');
                    d = a[0].CompareTo(b[0]);
                    if (0 == d)
                    {
                        var a2 = a[1].Split(',');
                        var b2 = b[1].Split(',');
                        d = a2.Length - b2.Length;
                        if (0 == d)
                        {
                            for (var i = 0; i < a2.Length && 0 == d; ++i)
                            {
                                d = int.Parse(a2[0]) - int.Parse(b2[0]);
                            }
                        }
                    }
                }
                return d;
            }
        }
        #endregion // PanelCircuitComparer

        #region PanelSystemComparer
        // Sort panel:system keys; numeric when the system segment is all digits/commas.
        public class PanelSystemComparer : IComparer<string>
        {
            #region JtIsDigit
            //
            // JtIsDigit is faster than using char.IsDigit() in a lop, cf.
            // http://weblogs.asp.net/justin_rogers/archive/2004/03/29/100982.aspx
            // Performance: Different methods for testing string input for numeric values...
            // by Justin Rogers
            //
            public static bool JtIsDigit(string s)
            {
                foreach (var a in s)
                {
                    if (a is < '0' or > '9')
                    {
                        return false;
                    }
                }
                return true;
            }

            public static bool JtIsDigitOrComma(string s)
            {
                foreach (var a in s)
                {
                    if (a is (< '0' or > '9') and not ',')
                    {
                        return false;
                    }
                }
                return true;
            }
            #endregion // JtIsDigit

            int CompareCommaDelimitedIntegerLists(string x, string y)
            {
                var nx = x.IndexOf(',');
                var ny = y.IndexOf(',');
                var ix = (0 < nx) ? int.Parse(x.Substring(0, nx)) : int.Parse(x);
                var iy = (0 < ny) ? int.Parse(y.Substring(0, ny)) : int.Parse(y);
                var d = ix - iy;
                return (0 == d && 0 < nx && 0 < ny)
                  ? CompareCommaDelimitedIntegerLists(x.Substring(nx + 1), y.Substring(ny + 1))
                  : (0 == d ? nx - ny : d);
            }

            public int Compare(string x, string y)
            {
                var a = x.Split(':');
                var b = y.Split(':');
                var d = a[0].CompareTo(b[0]);
                if (0 == d
                  && 1 < a.GetLength(0)
                  && 1 < b.GetLength(0))
                {
                    d = (JtIsDigitOrComma(a[1]) && JtIsDigitOrComma(b[1]))
                      ? CompareCommaDelimitedIntegerLists(a[1], b[1])
                      : a[1].CompareTo(b[1]);
                }
                return d;
            }
        }
        #endregion // PanelSystemComparer
        #endregion // Sorting comparers

        #region ListEquipment
        static void ListEquipment(
          FamilyInstance elecEqip,
          IDictionary<string, List<Element>> mapPanelAndSystemToEquipment)
        {
            var mepModel = elecEqip.MEPModel;
            //ElectricalSystemSet systems = mepModel.ElectricalSystems; // 2020
            var systems = mepModel.GetElectricalSystems(); // 2021
            var s = string.Empty;
            if (null == systems)
            {
                s = Unassigned + ":" + elecEqip.Name;
            }
            else
            {
                Debug.Assert(1 == systems.Count,
                  "expected equipment to belong to one single panel and system");

                foreach (var system in systems)
                {
                    if (0 < s.Length)
                    {
                        s += ", ";
                    }
                    s += system.PanelName + ":" + system.Name;
                }
            }
            Debug.WriteLine("  " + elecEqip.Name + ": " + s);
            Debug.Assert(!mapPanelAndSystemToEquipment.ContainsKey(s), "expected each panel and system to occur in one equipment element only");
            if (!mapPanelAndSystemToEquipment.ContainsKey(s))
            {
                mapPanelAndSystemToEquipment.Add(s, []);
            }
            mapPanelAndSystemToEquipment[s].Add(elecEqip);
        }
        #endregion // ListEquipment

        public static Result Execute2(
          ExternalCommandData commandData,
          ref String message,
          ElementSet elements,
          bool populateFullHierarchy)
        {
            try
            {
                WaitCursor waitCursor = new();
                var app = commandData.Application;
                var doc = app.ActiveUIDocument.Document;
                //
                // display electrical equipment instance data, 
                // i.e. equipment_name:system_name, and convert it to 
                // a map from key = panel:circuit --> equipment:
                //
                var equipment = Util.GetElectricalEquipment(doc);
                var n = equipment.Count;
                Debug.WriteLine(string.Format("Retrieved {0} electrical equipment instance{1}{2}",
                  n, BuildingCoder.Util.PluralSuffix(n), BuildingCoder.Util.DotOrColon(n)));
                Dictionary<string, List<Element>> mapPanelAndSystemToEquipment = [];
                foreach (FamilyInstance elecEqip in equipment)
                {
                    ListEquipment(elecEqip, mapPanelAndSystemToEquipment);
                }
                //
                // determine mapping from panel to circuit == electrical system:
                //
                Dictionary<string, ISet<ElectricalSystem>> mapPanelToSystems
                  = [];

                var systems = Util.GetElectricalSystems(doc);
                n = systems.Count;
                Debug.WriteLine(string.Format("Retrieved {0} electrical system{1}.",
                  n, BuildingCoder.Util.PluralSuffix(n)));
                //
                // all circuits which are fed from the same family instance have 
                // the same panel name, so you can retrieve all of these circuits.
                //
                // todo: there is an issue here if there are several different panels 
                // with the same name! they will get merged in the tree view, 
                // but they should stay separate. possible workaround: add the 
                // element id to keep them separate, and then remove it again
                // when displaying in tree view.
                //
                foreach (ElectricalSystem system in systems)
                {
                    var panelName = system.PanelName;

                    Debug.WriteLine("  system " + system.Name + ": panel " + panelName
                      + " load classifications " + system.LoadClassifications);

                    if (!mapPanelToSystems.ContainsKey(panelName))
                    {
                        mapPanelToSystems.Add(panelName,
                          new SortedSet<ElectricalSystem>());
                    }
                    mapPanelToSystems[panelName].Add(system);
                }
                n = mapPanelToSystems.Count;
                //Debug.WriteLine( string.Format( "Mapping from the {0} panel{1} to systems, system name :circuit name(connectors/unused connectors):", n, BuildingCoder.Util.PluralSuffix( n ) ) );
                Debug.WriteLine(string.Format("Mapping from the {0} panel{1} to electrical systems == circuits:",
                  n, BuildingCoder.Util.PluralSuffix(n)));
                List<string> keys = [.. mapPanelToSystems.Keys];
                keys.Sort();
                string s;
                foreach (var panelName in keys)
                {
                    s = string.Empty;
                    foreach (var system in mapPanelToSystems[panelName])
                    {
                        var cmgr = system.ConnectorManager;

                        // the connector manager does not include any logical connectors
                        // in the Revit 2009 fcs and wu1 API, only physical ones:
                        //Debug.Assert( 0 == cmgr.Connectors.Size, 
                        //  "electrical connector count is always zero" );

                        Debug.Assert(cmgr.UnusedConnectors.Size <= cmgr.Connectors.Size,
                          "unused connectors is a subset of connectors");

                        Debug.Assert(system.Name.Equals(system.CircuitNumber),
                          "ElectricalSystem Name and CircuitNumber properties are always identical");

                        //s += ( 0 < s.Length ? ", " : ": " ) + system.Name;

                        s += (0 < s.Length ? ", " : ": ") + system.Name // + ":" + system.CircuitNumber
                          + "(" + cmgr.Connectors.Size.ToString()
                          + "/" + cmgr.UnusedConnectors.Size.ToString() + ")";
                    }
                    Debug.WriteLine("  " + panelName + s);
                }
                /*
                Debug.WriteLine( "Mapping from panels to systems to connected elements:" );
                foreach( string panelName in keys )
                {
                  Debug.WriteLine( "  panel " + panelName + ":" );
                  foreach( ElectricalSystem system in mapPanelToSystems[panelName] )
                  {
                    ConnectorManager cmgr = system.ConnectorManager;
                    n = cmgr.Connectors.Size;
                    Debug.WriteLine( string.Format( "    system {0} has {1} connector{2}{3}", system.Name, n, BuildingCoder.Util.PluralSuffix( n ), BuildingCoder.Util.DotOrColon( n ) ) );
                    foreach( Connector connector in system.ConnectorManager.Connectors )
                    {
                      Element owner = connector.Owner;
                      Debug.WriteLine( string.Format( "    owner {0} {1}, domain {2}", owner.Name, owner.Id.Value, connector.Domain ) );
                    }
                  }
                }
                */

                //
                // list all circuit elements:
                //
                // this captures all elements in circuits H-2: 2, 4, 6 etc, 
                // but not the element T2 in H-2:1,3,5, because it has no circuit number,
                // just a panel number.
                //
                var bipPanel = BuiltInParameter.RBS_ELEC_CIRCUIT_PANEL_PARAM;
                var bipCircuit = BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER;
                var circuitElements = Util.GetCircuitElements(doc);
                n = circuitElements.Count;
                Debug.WriteLine(string.Format("Retrieved {0} circuit element{1}{2}",
                  n, BuildingCoder.Util.PluralSuffix(n), BuildingCoder.Util.DotOrColon(n)));
                Dictionary<string, List<Element>> mapPanelAndCircuitToElements = [];
                foreach (var e in circuitElements)
                {
                    var circuitName = e.get_Parameter(bipCircuit).AsString();
                    //
                    // do not map an electrical system to itself:
                    //
                    if (!(e is ElectricalSystem && e.Name.Equals(circuitName)))
                    {
                        var panelName = e.get_Parameter(bipPanel).AsString();
                        var key = panelName + ":" + circuitName;
                        Debug.WriteLine(string.Format("  {0} <{1} {2}> panel:circuit {3}", e.GetType().Name, e.Name, e.Id.Value, key));
                        if (!mapPanelAndCircuitToElements.ContainsKey(key))
                        {
                            mapPanelAndCircuitToElements.Add(key, []);
                        }
                        mapPanelAndCircuitToElements[key].Add(e);
                    }
                }
                n = mapPanelAndCircuitToElements.Count;
                Debug.WriteLine(string.Format("Mapped circuit elements to {0} panel:circuit{1}{2}",
                  n, BuildingCoder.Util.PluralSuffix(n), BuildingCoder.Util.DotOrColon(n)));
                keys.Clear();
                keys.AddRange(mapPanelAndCircuitToElements.Keys);
                keys.Sort(new PanelCircuitComparer());
                foreach (var panelAndCircuit in keys)
                {
                    List<string> a = new(mapPanelAndCircuitToElements[panelAndCircuit].Count);
                    foreach (var e in mapPanelAndCircuitToElements[panelAndCircuit])
                    {
                        a.Add((e is not FamilyInstance inst ? e.Category.Name : inst.Symbol.Family.Name) + " " + e.Name);
                    }
                    a.Sort();
                    s = string.Join(", ", a.ToArray());
                    Debug.WriteLine("  " + panelAndCircuit + ": " + s);
                }

                #region Aborted attempt to use RBS_ELEC_CIRCUIT_PANEL_PARAM
#if USE_RBS_ELEC_CIRCUIT_PANEL_PARAM
        //
        // list all panel elements:
        //
        // selecting all elements with a BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER
        // captures all elements in circuits H-2: 2, 4, 6 etc, but not the element 
        // T2 in H-2:1,3,5, because it has no circuit number, just a panel number.
        //
        // so grab everything with a panel number instead.
        //
        // all this added to the selection was lots of wires, so forget it again.
        //
        BuiltInParameter bipCircuit = BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER;
        BuiltInParameter bipPanel = BuiltInParameter.RBS_ELEC_CIRCUIT_PANEL_PARAM;
        List<Element> circuitElements = new List<Element>();
        Util.GetElementsWithParameter( circuitElements, bipPanel, app );
        n = circuitElements.Count;
        Debug.WriteLine( string.Format( "Retrieved {0} circuit element{1}{2}",
          n, BuildingCoder.Util.PluralSuffix( n ), BuildingCoder.Util.DotOrColon( n ) ) );
        Dictionary<string, List<Element>> mapCircuitToElements = new Dictionary<string, List<Element>>();
        foreach( Element e in circuitElements )
        {
          string panelName = e.get_Parameter( bipPanel ).AsString();
          Parameter p = e.get_Parameter( bipCircuit );
          if( null == p )
          {
            Debug.WriteLine( string.Format( "  {0} <{1} {2}> panel:circuit {3}:null", e.GetType().Name, e.Name, e.Id.Value, panelName ) );
          }
          else
          {
            string circuitName = p.AsString();
            //
            // do not map an electrical system to itself:
            //
            if( !( e is ElectricalSystem && e.Name.Equals( circuitName ) ) )
            {
              string key = panelName + ":" + circuitName;
              Debug.WriteLine( string.Format( "  {0} <{1} {2}> panel:circuit {3}", e.GetType().Name, e.Name, e.Id.Value, key ) );
              if( !mapCircuitToElements.ContainsKey( key ) )
              {
                mapCircuitToElements.Add( key, new List<Element>() );
              }
              mapCircuitToElements[key].Add( e );
            }
          }
        }
        n = mapCircuitToElements.Count;
        Debug.WriteLine( string.Format( "Mapped circuit elements to {0} panel:circuit{1}{2}",
          n, BuildingCoder.Util.PluralSuffix( n ), BuildingCoder.Util.DotOrColon( n ) ) );
        keys.Clear();
        keys.AddRange( mapCircuitToElements.Keys );
        keys.Sort( new PanelCircuitComparer() );
        foreach( string circuitName in keys )
        {
          List<string> a = new List<string>( mapCircuitToElements[circuitName].Count );
          foreach( Element e in mapCircuitToElements[circuitName] )
          {
            FamilyInstance inst = e as FamilyInstance;
            a.Add( ( null == inst ? e.Category.Name : inst.Symbol.Family.Name ) + " " + e.Name );
          }
          a.Sort();
          s = string.Join( ", ", a.ToArray() );
          Debug.WriteLine( "  " + circuitName + ": " + s );
        }
#endif // USE_RBS_ELEC_CIRCUIT_PANEL_PARAM
                #endregion // Aborted attempt to use RBS_ELEC_CIRCUIT_PANEL_PARAM

                //
                // merge the two trees of equipment and circuit elements
                // to reproduce the content of the system browser ... the 
                // hardest part of this is setting up the PanelSystemComparer
                // to generate the same sort order as the system browser:
                //
                //n = mapPanelAndSystemToEquipment.Count + mapPanelAndCircuitToElements.Count;
                //Dictionary<string, List<Element>> mapSystemBrowser = new Dictionary<string, List<Element>>( n );
                Dictionary<string, List<Element>> mapSystemBrowser = new(mapPanelAndCircuitToElements);
                foreach (var pair in mapPanelAndSystemToEquipment)
                {
                    mapSystemBrowser[pair.Key] = pair.Value;
                }
                n = mapSystemBrowser.Count;
                Debug.WriteLine(string.Format("Mapped equipment + circuit elements to {0} panel:system{1}{2}",
                  n, BuildingCoder.Util.PluralSuffix(n), BuildingCoder.Util.DotOrColon(n)));
                keys.Clear();
                keys.AddRange(mapSystemBrowser.Keys);
                keys.Sort(new PanelSystemComparer());
                foreach (var panelAndSystem in keys)
                {
                    List<string> a = new(mapSystemBrowser[panelAndSystem].Count);
                    foreach (var e in mapSystemBrowser[panelAndSystem])
                    {
                        a.Add(Util.BrowserDescription(e));
                    }
                    a.Sort();
                    s = string.Join(", ", a.ToArray());
                    Debug.WriteLine(string.Format("  {0}({1}): ", panelAndSystem, a.Count) + s);
                }
                //
                // get the electrical equipment category id:
                //
                var categories = doc.Settings.Categories;
                var electricalEquipmentCategoryId = categories.get_Item(BuiltInCategory.OST_ElectricalEquipment).Id;
                //
                // we have assembled the required information and structured it
                // sufficiently for the tree view, so now let us go ahead and display it:
                //
                CmdInspectElectricalForm dialog = new(mapSystemBrowser, electricalEquipmentCategoryId, populateFullHierarchy);
                dialog.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        public Result Execute(
          ExternalCommandData commandData,
          ref String message,
          ElementSet elements)
        {
            return Execute2(commandData, ref message, elements, false);
        }
    }
    #endregion // CmdElectricalSystemBrowser

    #region CmdElectricalHierarchy
    [Transaction(TransactionMode.ReadOnly)]
    public class CmdElectricalHierarchy : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref String message,
          ElementSet elements)
        {
            return CmdElectricalSystemBrowser.Execute2(commandData, ref message, elements, true);
        }
    }
    #endregion // CmdElectricalHierarchy
}
