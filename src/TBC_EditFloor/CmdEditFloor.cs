#region Header

//
// CmdEditFloor.cs - read existing floor geometry and create a new floor
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdEditFloor : IExternalCommand
    {
        #region Super simple floor creation

#if BEFORE_FLOOR_CREATE_METHOD
    Result Execute2(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Document doc = uidoc.Document;

      using( Transaction tx = new Transaction( doc ) )
      {
        tx.Start( "Create a Floor" );

        int n = 4;
        XYZ[] points = new XYZ[ n ];
        points[ 0 ] = XYZ.Zero;
        points[ 1 ] = new XYZ( 10.0, 0.0, 0.0 );
        points[ 2 ] = new XYZ( 10.0, 10.0, 0.0 );
        points[ 3 ] = new XYZ( 0.0, 10.0, 0.0 );

        // Code for Revit 2021 using CurveArray:

        CurveArray curve = new CurveArray();

        for( int i = 0; i < n; i++ )
        {
          Line line = Line.CreateBound( points[ i ],
            points[ (i < n - 1) ? i + 1 : 0 ] );

          curve.Append( line );
        }

        doc.Create.NewFloor( curve, true ); // 2021

        tx.Commit();
      }
      return Result.Succeeded;
    }
#endif // BEFORE_FLOOR_CREATE_METHOD

        #endregion // Super simple floor creation

        #region Attempt to include inner loops

#if ATTEMPT_TO_INCLUDE_INNER_LOOPS
    /// <summary>
    /// Convert an EdgeArrayArray to a CurveArray,
    /// possibly including multiple loops.
    /// All non-linear segments are approximated by
    /// the edge curve tesselation.
    /// </summary>
    CurveArray Convert( EdgeArrayArray eaa )
    {
      CurveArray ca = new CurveArray();
      List<XYZ> pts = new List<XYZ>();

      XYZ q;
      string s;
      int iLoop = 0;

      foreach( EdgeArray ea in eaa )
      {
        q = null;
        s = string.Empty;
        pts.Clear();

        foreach( Edge e in ea )
        {
          IList<XYZ> a = e.Tessellate();
          bool first = true;
          //XYZ p0 = null;

          foreach( XYZ p in a )
          {
            if( first )
            {
              if( null == q )
              {
                s += Util.PointString( p );
                pts.Add( p );
              }
              else
              {
                Debug.Assert( p.IsAlmostEqualTo( q ), "expected connected sequential edges" );
              }
              first = false;
              //p0 = p;
              q = p;
            }
            else
            {
              s += " --> " + Util.PointString( p );
              //ca.Append( Line.get_Bound( q, p ) );
              pts.Add( p );
              q = p;
            }
          }
          //ca.Append( Line.get_Bound( q, p0 ) );
        }

        Debug.Print( "{0}: {1}", iLoop++, s );

        // test case: break after first edge loop,
        // which we assume to be the outer:

        //break;

        {
          // try reversing all the inner loops:

          if( 1 < iLoop )
          {
            pts.Reverse();
          }

          bool first = true;

          foreach( XYZ p in pts )
          {
            if( first )
            {
              first = false;
            }
            else
            {
              ca.Append( Line.get_Bound( q, p ) );
            }
            q = p;
          }
        }
      }
      return ca;
    }
#endif // ATTEMPT_TO_INCLUDE_INNER_LOOPS

        #endregion // Attempt to include inner loops

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            // Retrieve selected floors, or all floors, if nothing is selected:

            var floors = new List<Element>();
            if (!Util.GetSelectedElementsOrAll(
                floors, uidoc, typeof(Floor)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some floor elements."
                    : "No floor elements found.";
                return Result.Failed;
            }

            // Determine top face of each selected floor:

            var nNullFaces = 0;
            var topFaces = new List<Face>();
            var opt = app.Application.Create.NewGeometryOptions();

            foreach (Floor floor in floors)
            {
                var geo = floor.get_Geometry(opt);

                //GeometryObjectArray objects = geo.Objects; // 2012

                foreach (var obj in geo)
                {
                    var solid = obj as Solid;
                    if (solid != null)
                    {
                        var f = Util.GetTopFace(solid);
                        if (null == f)
                        {
                            Debug.WriteLine(
                                $"{Util.ElementDescription(floor)} has no top face.");
                            ++nNullFaces;
                        }

                        topFaces.Add(f);
                    }
                }
            }

            using var t = new Transaction(doc);
            t.Start("Create Model Lines and Floor");

            // Create new floors from the top faces found.
            // Before creating the new floor, we would obviously
            // apply whatever modifications are required to the
            // new floor profile:

            var creApp = app.Application.Create;
            var creDoc = doc.Create;

            var i = 0;
            var n = topFaces.Count - nNullFaces;

            Debug.Print(
                "{0} top face{1} found.",
                n, Util.PluralSuffix(n));

            foreach (var f in topFaces)
            {
                var floor = floors[i++] as Floor;

                if (null != f)
                {
                    var eaa = f.EdgeLoops;

                    // Code for Revit 2021 and earlier:

                    //CurveArray profile; // 2021

                    #region Attempt to include inner loops

#if ATTEMPT_TO_INCLUDE_INNER_LOOPS
          bool use_original_loops = true;
          if( use_original_loops )
          {
            profile = Convert( eaa );
          }
          else
#endif // ATTEMPT_TO_INCLUDE_INNER_LOOPS

                    #endregion // Attempt to include inner loops

                    //{
                    //  profile = new CurveArray();

                    //  // Only use first edge array,
                    //  // the outer boundary loop,
                    //  // skip the further items
                    //  // representing holes:

                    //  EdgeArray ea = eaa.get_Item( 0 );
                    //  foreach ( Edge e in ea )
                    //  {
                    //    IList<XYZ> pts = e.Tessellate();
                    //    int m = pts.Count;
                    //    XYZ p = pts[0];
                    //    XYZ q = pts[m - 1];
                    //    Line line = Line.CreateBound( p, q );
                    //    profile.Append( line );
                    //  }
                    //}

                    var loops = new List<CurveLoop>(); // 2022

                    {
                        var loop = new CurveLoop();

                        // Only use first edge array,
                        // the outer boundary loop,
                        // skip the further items
                        // representing holes:

                        var ea = eaa.get_Item(0);
                        foreach (Edge e in ea)
                        {
                            var pts = e.Tessellate();
                            var m = pts.Count;
                            var p = pts[0];
                            var q = pts[m - 1];
                            var line = Line.CreateBound(p, q);
                            loop.Append(line);
                        }

                        loops = new List<CurveLoop>();
                        loops.Add(loop);
                    }

                    //Level level = floor.Level; // 2013

                    //Level level = doc.GetElement( floor.LevelId )
                    //  as Level; // 2014

                    // In this case we have a valid floor type given.
                    // In general, not that NewFloor will only accept 
                    // floor types whose IsFoundationSlab predicate
                    // is false.

                    //floor = creDoc.NewFloor( profile, // 2021
                    //  floor.FloorType, level, true );

                    floor = Floor.Create(doc, loops,
                        floor.FloorType.Id, floor.LevelId); // 2022

                    var v = new XYZ(5, 5, 0);

                    //doc.Move( floor, v ); // 2011

                    ElementTransformUtils.MoveElement(doc, floor.Id, v); // 2012
                }
            }

            t.Commit();

            return Result.Succeeded;
        }
    }
}