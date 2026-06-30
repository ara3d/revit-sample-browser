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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace AdnRme
{
  [Transaction( TransactionMode.Manual )]
  public class CmdResetDemo : IExternalCommand
  {
    #region Execute Command
    public Result Execute(
      ExternalCommandData commandData,
      ref String message,
      ElementSet elements )
    {
      try
      {
        UIApplication app = commandData.Application;
        Document doc = app.ActiveUIDocument.Document;

        using( Transaction tx = new Transaction( doc ) )
        {
          tx.Start( "Reset Demo" );
          ResetSupplyAirTerminals( doc );
          SetSpaceCfmPerSfToZero( doc );
          tx.Commit();
        }
        return Result.Succeeded;
      }
      catch( Exception ex )
      {
        message = ex.Message;
        return Result.Failed;
      }
    }

    private void SetSpaceCfmPerSfToZero( Document doc )
    {
      //FilteredElementCollector collector = new FilteredElementCollector( doc );
      //collector.OfClass( typeof( Space ) );
      //IList<Element> spaces = collector.ToElements();
      //int n = spaces.Count;

      List<Space> spaces = Util.GetSpaces( doc );
      int n = spaces.Count;

      string s = "{0} of " + n.ToString() + " spaces reset...";

      using( ProgressForm pf = new ProgressForm( "Reset parameter", s, n ) )
      {
        foreach( Space space in spaces )
        {
          SetCfmPerSf( space, 0.0 );
          pf.Increment();
        }
      }
    }

    static void SetCfmPerSf( Space space, double value )
    {
      Parameter pCfmPerSf = Util.GetSpaceParameter( space, ParameterName.CfmPerSf );
      pCfmPerSf.Set( value );
    }

    private void ResetSupplyAirTerminals( Document doc )
    {
      WaitCursor waitCursor = new WaitCursor();
      FilteredElementCollector collector = Util.GetSupplyAirTerminals( doc );

      IList<Element> terminals = collector.ToElements();
      int n = terminals.Count;

      string s = "{0} of " + n.ToString() + " terminals reset...";
      string caption = "Resetting Supply Air Termainal Flows and Sizes";

      using( ProgressForm pf = new ProgressForm( caption, s, n ) )
      {
        foreach( FamilyInstance terminal in terminals )
        {

          Parameter p = Util.GetTerminalFlowParameter( terminal );
          p.Set( 0 );

          ISet<ElementId> symbolIds 
            = terminal.Symbol.Family
              .GetFamilySymbolIds();

          foreach( ElementId id in symbolIds )
          {
            FamilySymbol sym = doc.GetElement( id ) 
              as FamilySymbol;

            terminal.Symbol = sym; // simply set to first symbol found

            break; // done after getting the first symbol
          }
          pf.Increment();
        }
      }
    }
    #endregion // Execute Command
  }
}
