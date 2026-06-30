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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace AdnRme
{
  #region CmdElectricalSystemBrowser
  /// <summary>
  /// Reproduces the Power section of the MEP system browser (panel &gt; system &gt; element) in a modeless tree.
  /// Built on parameter lookups from Revit 2008; prefer CmdElectricalConnectors for new work.
  /// </summary>
  [Transaction( TransactionMode.ReadOnly )]
  public class CmdElectricalSystemBrowser : IExternalCommand
  {
    public const string Unassigned = "Unassigned";

    #region Sorting comparers

    #region NumericalComparer
    class NumericalComparer : IComparer<string>
    {
      public int Compare( string x, string y )
      {
        return int.Parse( x ) - int.Parse( y );
      }
    }
    #endregion // NumericalComparer

    #region PanelCircuitComparer
    // Sort panel:circuit keys; handles comma-delimited circuit lists (e.g. MDP:1,3,5).
    class PanelCircuitComparer : IComparer<string>
    {
      public int Compare( string x, string y )
      {
        int d = x.CompareTo( y );
        if( 0 != d )
        {
          string[] a = x.Split( ':' );
          string[] b = y.Split( ':' );
          d = a[0].CompareTo( b[0] );
          if( 0 == d )
          {
            string[] a2 = a[1].Split( ',' );
            string[] b2 = b[1].Split( ',' );
            d = a2.Length - b2.Length;
            if( 0 == d )
            {
              for( int i = 0; i < a2.Length && 0 == d; ++i )
              {
                d = int.Parse( a2[0] ) - int.Parse( b2[0] );
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
      public static bool JtIsDigit( string s )
      {
        foreach( char a in s )
        {
          if( a < '0' || '9' < a )
          {
            return false;
          }
        }
        return true;
      }

      public static bool JtIsDigitOrComma( string s )
      {
        foreach( char a in s )
        {
          if( (a < '0' || '9' < a) && (',' != a) )
          {
            return false;
          }
        }
        return true;
      }
      #endregion // JtIsDigit

      int CompareCommaDelimitedIntegerLists( string x, string y )
      {
        int nx = x.IndexOf( ',' );
        int ny = y.IndexOf( ',' );
        int ix = ( 0 < nx ) ? int.Parse( x.Substring( 0, nx ) ) : int.Parse( x );
        int iy = ( 0 < ny ) ? int.Parse( y.Substring( 0, ny ) ) : int.Parse( y );
        int d = ix - iy;
        return ( 0 == d && 0 < nx && 0 < ny )
          ? CompareCommaDelimitedIntegerLists( x.Substring( nx + 1 ), y.Substring( ny + 1 ) )
          : (0 == d ? nx - ny : d);
      }

      public int Compare( string x, string y )
      {
        string[] a = x.Split( ':' );
        string[] b = y.Split( ':' );
        int d = a[0].CompareTo( b[0] );
        if( 0 == d 
          && 1 < a.GetLength( 0 ) 
          && 1 < b.GetLength( 0 ) )
        {
          d = ( JtIsDigitOrComma( a[1] ) && JtIsDigitOrComma( b[1] ) )
            ? CompareCommaDelimitedIntegerLists( a[1], b[1] )
            : a[1].CompareTo( b[1] );
        }
        return d;
      }
    }
    #endregion // PanelSystemComparer
    #endregion // Sorting comparers

    #region ListEquipment
    static void ListEquipment( 
      FamilyInstance elecEqip, 
      IDictionary<string, List<Element>> mapPanelAndSystemToEquipment )
    {
      MEPModel mepModel = elecEqip.MEPModel;
      //ElectricalSystemSet systems = mepModel.ElectricalSystems; // 2020
      ISet<ElectricalSystem> systems = mepModel.GetElectricalSystems(); // 2021
      string s = string.Empty;
      if( null == systems )
      {
        s = Unassigned + ":" + elecEqip.Name;
      }
      else
      {
        Debug.Assert( 1 == systems.Count, 
          "expected equipment to belong to one single panel and system" );

        foreach( ElectricalSystem system in systems )
        {
          if( 0 < s.Length )
          {
            s += ", ";
          }
          s += system.PanelName + ":" + system.Name;
        }
      }
      Debug.WriteLine( "  " + elecEqip.Name + ": " + s );
      Debug.Assert( !mapPanelAndSystemToEquipment.ContainsKey( s ), "expected each panel and system to occur in one equipment element only" );
      if( !mapPanelAndSystemToEquipment.ContainsKey( s ) )
      {
        mapPanelAndSystemToEquipment.Add( s, new List<Element>() );
      }
      mapPanelAndSystemToEquipment[s].Add( elecEqip );
    }
    #endregion // ListEquipment

    static public Result Execute2(
      ExternalCommandData commandData,
      ref String message,
      ElementSet elements,
      bool populateFullHierarchy )
    {
      try
      {
        WaitCursor waitCursor = new WaitCursor();
        UIApplication app = commandData.Application;
        Document doc = app.ActiveUIDocument.Document;
        //
        // display electrical equipment instance data, 
        // i.e. equipment_name:system_name, and convert it to 
        // a map from key = panel:circuit --> equipment:
        //
        List<Element> equipment = Util.GetElectricalEquipment( doc );
        int n = equipment.Count;
        Debug.WriteLine( string.Format( "Retrieved {0} electrical equipment instance{1}{2}", 
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
        Dictionary<string, List<Element>> mapPanelAndSystemToEquipment = new Dictionary<string, List<Element>>();
        foreach( FamilyInstance elecEqip in equipment )
        {
          ListEquipment( elecEqip, mapPanelAndSystemToEquipment );
        }
        //
        // determine mapping from panel to circuit == electrical system:
        //
        Dictionary<string, ISet<ElectricalSystem>> mapPanelToSystems 
          = new Dictionary<string, ISet<ElectricalSystem>>();

        IList<Element> systems = Util.GetElectricalSystems( doc );
        n = systems.Count;
        Debug.WriteLine( string.Format( "Retrieved {0} electrical system{1}.", 
          n, Util.PluralSuffix( n ) ) );
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
        foreach( ElectricalSystem system in systems )
        {
          string panelName = system.PanelName;

          Debug.WriteLine( "  system " + system.Name + ": panel " + panelName 
            + " load classifications " + system.LoadClassifications );

          if( !mapPanelToSystems.ContainsKey( panelName ) )
          {
            mapPanelToSystems.Add( panelName, 
              new SortedSet<ElectricalSystem>() );
          }
          mapPanelToSystems[panelName].Add( system );
        }
        n = mapPanelToSystems.Count;
        //Debug.WriteLine( string.Format( "Mapping from the {0} panel{1} to systems, system name :circuit name(connectors/unused connectors):", n, Util.PluralSuffix( n ) ) );
        Debug.WriteLine( string.Format( "Mapping from the {0} panel{1} to electrical systems == circuits:", 
          n, Util.PluralSuffix( n ) ) );
        List<string> keys = new List<string>( mapPanelToSystems.Keys );
        keys.Sort();
        string s;
        foreach( string panelName in keys )
        {
          s = string.Empty;
          foreach( ElectricalSystem system in mapPanelToSystems[panelName] )
          {
            ConnectorManager cmgr = system.ConnectorManager;
            
            // the connector manager does not include any logical connectors
            // in the Revit 2009 fcs and wu1 API, only physical ones:
            //Debug.Assert( 0 == cmgr.Connectors.Size, 
            //  "electrical connector count is always zero" );

            Debug.Assert( cmgr.UnusedConnectors.Size <= cmgr.Connectors.Size, 
              "unused connectors is a subset of connectors" );

            Debug.Assert( system.Name.Equals( system.CircuitNumber ), 
              "ElectricalSystem Name and CircuitNumber properties are always identical" );

            //s += ( 0 < s.Length ? ", " : ": " ) + system.Name;

            s += ( 0 < s.Length ? ", " : ": " ) + system.Name // + ":" + system.CircuitNumber
              + "(" + cmgr.Connectors.Size.ToString()
              + "/" + cmgr.UnusedConnectors.Size.ToString() + ")";
          }
          Debug.WriteLine( "  " + panelName + s );
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
            Debug.WriteLine( string.Format( "    system {0} has {1} connector{2}{3}", system.Name, n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
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
        BuiltInParameter bipPanel = BuiltInParameter.RBS_ELEC_CIRCUIT_PANEL_PARAM;
        BuiltInParameter bipCircuit = BuiltInParameter.RBS_ELEC_CIRCUIT_NUMBER;
        IList<Element> circuitElements = Util.GetCircuitElements( doc );
        n = circuitElements.Count;
        Debug.WriteLine( string.Format( "Retrieved {0} circuit element{1}{2}",
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
        Dictionary<string, List<Element>> mapPanelAndCircuitToElements = new Dictionary<string, List<Element>>();
        foreach( Element e in circuitElements )
        {
          string circuitName = e.get_Parameter( bipCircuit ).AsString();
          //
          // do not map an electrical system to itself:
          //
          if( !(e is ElectricalSystem && e.Name.Equals( circuitName )) )
          {
            string panelName = e.get_Parameter( bipPanel ).AsString();
            string key = panelName + ":" + circuitName;
            Debug.WriteLine( string.Format( "  {0} <{1} {2}> panel:circuit {3}", e.GetType().Name, e.Name, e.Id.Value, key ) );
            if( !mapPanelAndCircuitToElements.ContainsKey( key ) )
            {
              mapPanelAndCircuitToElements.Add( key, new List<Element>() );
            }
            mapPanelAndCircuitToElements[key].Add( e );
          }
        }
        n = mapPanelAndCircuitToElements.Count;
        Debug.WriteLine( string.Format( "Mapped circuit elements to {0} panel:circuit{1}{2}",
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
        keys.Clear();
        keys.AddRange( mapPanelAndCircuitToElements.Keys );
        keys.Sort( new PanelCircuitComparer() );
        foreach( string panelAndCircuit in keys )
        {
          List<string> a = new List<string>( mapPanelAndCircuitToElements[panelAndCircuit].Count );
          foreach( Element e in mapPanelAndCircuitToElements[panelAndCircuit] )
          {
            FamilyInstance inst = e as FamilyInstance;
            a.Add( ( null == inst ? e.Category.Name : inst.Symbol.Family.Name) + " " + e.Name );
          }
          a.Sort();
          s = string.Join( ", ", a.ToArray() );
          Debug.WriteLine( "  " + panelAndCircuit + ": " + s );
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
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
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
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
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
        Dictionary<string, List<Element>> mapSystemBrowser = new Dictionary<string, List<Element>>( mapPanelAndCircuitToElements );
        foreach( KeyValuePair<string, List<Element>> pair in mapPanelAndSystemToEquipment )
        {
          mapSystemBrowser[pair.Key] = pair.Value;
        }
        n = mapSystemBrowser.Count;
        Debug.WriteLine( string.Format( "Mapped equipment + circuit elements to {0} panel:system{1}{2}",
          n, Util.PluralSuffix( n ), Util.DotOrColon( n ) ) );
        keys.Clear();
        keys.AddRange( mapSystemBrowser.Keys );
        keys.Sort( new PanelSystemComparer() );
        foreach( string panelAndSystem in keys )
        {
          List<string> a = new List<string>( mapSystemBrowser[panelAndSystem].Count );
          foreach( Element e in mapSystemBrowser[panelAndSystem] )
          {
            a.Add( Util.BrowserDescription( e ) );
          }
          a.Sort();
          s = string.Join( ", ", a.ToArray() );
          Debug.WriteLine( string.Format( "  {0}({1}): ", panelAndSystem, a.Count ) + s );
        }
        //
        // get the electrical equipment category id:
        //
        Categories categories = doc.Settings.Categories;
        ElementId electricalEquipmentCategoryId = categories.get_Item( BuiltInCategory.OST_ElectricalEquipment ).Id;
        //
        // we have assembled the required information and structured it
        // sufficiently for the tree view, so now let us go ahead and display it:
        //
        CmdInspectElectricalForm dialog = new CmdInspectElectricalForm( mapSystemBrowser, electricalEquipmentCategoryId, populateFullHierarchy );
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
      ElementSet elements )
    {
      return Execute2( commandData, ref message, elements, false );
    }
  }
  #endregion // CmdElectricalSystemBrowser

  #region CmdElectricalHierarchy
  [Transaction( TransactionMode.ReadOnly )]
  public class CmdElectricalHierarchy : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref String message,
      ElementSet elements )
    {
      return CmdElectricalSystemBrowser.Execute2( commandData, ref message, elements, true );
    }
  }
  #endregion // CmdElectricalHierarchy
}
