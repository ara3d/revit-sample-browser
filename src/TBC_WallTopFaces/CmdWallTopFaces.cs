#region Header

//
// CmdWallTopFaces.cs - retrieve top faces of selected or all wall
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#define CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
#if CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES
    [Transaction(TransactionMode.Manual)]
#else
  [Transaction( TransactionMode.ReadOnly )]
#endif // CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

    internal class CmdWallTopFaces : IExternalCommand
    {
#if CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

        private static readonly XYZ _offset = XYZ.BasisZ / 12;

        private static readonly Transform _t =
            // Transform.get_Translation( _offset ); // 2013
            Transform.CreateTranslation(_offset); // 2014

#endif // CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            var opt = app.Create.NewGeometryOptions();

            Util.XyzEqualityComparer comparer
                = new(1e-6);

#if CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

            Creator creator = new(doc);

            Transaction t = new(doc);

            t.Start("Create model curve copies of top face edges");

#endif // CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

            IList<Face> topFaces = [];

            int n;
            var nWalls = 0;

            //foreach( Element e in uidoc.Selection.Elements ) // 2014

            foreach (var id in uidoc.Selection.GetElementIds()) // 2015
            {
                var e = doc.GetElement(id);


                if (e is not Wall wall)
                {
                    Debug.Print($"Skipped {Util.ElementDescription(e)}");
                    continue;
                }

                var sideFaces
                    = HostObjectUtils.GetSideFaces(wall,
                        ShellLayerType.Exterior);

                var e2 = doc.GetElement(sideFaces[0]);

                Debug.Assert(e2.Id.Equals(e.Id),
                    "expected side face element to be the wall itself");

                var face = e2.GetGeometryObjectFromReference(
                    sideFaces[0]) as Face;

                if (null == face)
                {
                    Debug.Print($"No side face found for {Util.ElementDescription(e)}");
                    continue;
                }

                // Outer loop: largest-area polygon; see http://thebuildingcoder.typepad.com/blog/2008/12/2d-polygon-areas-and-outer-loop.html
                XYZ normal;
                double area, dist, maxArea = 0;
                EdgeArray outerLoop = null;

                foreach (EdgeArray ea in face.EdgeLoops)
                    if (Util.GetPolygonPlane(
                            ea.GetPolygon(), out normal, out dist, out area)
                        && Math.Abs(area) > Math.Abs(maxArea))
                    {
                        maxArea = area;
                        outerLoop = ea;
                    }

                n = 0;

#if GET_FACES_FROM_OUTER_LOOP
        // With the outermost loop, calculate the top faces

        foreach( Edge edge in outerLoop )
        {
          // For each edge, get the neighbouring
          // face and check its normal

          for( int i = 0; i < 2; ++i )
          {
            PlanarFace pf = edge.get_Face( i )
              as PlanarFace;

            if( null == pf )
            {
              Debug.Print( "Skipped non-planar face on "
                + Util.ElementDescription( e ) );
              continue;
            }

            if( Util.PointsUpwards( pf.Normal, 0.9 ) )
            {
              if( topFaces.Contains( pf ) )
              {
                Debug.Print( "Duplicate face on "
                  + Util.ElementDescription( e ) );
              }
              else
              {
                topFaces.Add( pf );
                ++n;
              }
            }
          }
        }
#endif // GET_FACES_FROM_OUTER_LOOP

                var sideVertices = outerLoop.GetPolygon();

                var solid = wall.get_Geometry(opt)
                    .OfType<Solid>()
                    .First(sol => null != sol);

                foreach (Face f in solid.Faces)
                    if (Util.IsTopFace(f))
                    {
                        var faceVertices
                            = f.Triangulate().Vertices;

                        //if( sideVertices.Exists( v
                        //  => faceVertices.Contains<XYZ>( v, comparer ) ) )
                        //{
                        //  topFaces.Add( f );
                        //  ++n;
                        //}

                        foreach (var v in faceVertices)
                            if (sideVertices.Contains(
                                v, comparer))
                            {
                                topFaces.Add(f);
                                ++n;

#if CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

                                foreach (EdgeArray ea in f.EdgeLoops)
                                {
                                    var curves
                                        = ea.Cast<Edge>()
                                            .Select(
                                                x => x.AsCurve());

                                    foreach (var curve in curves)
                                        //creator.CreateModelCurve( curve.get_Transformed( _t ) ); // 2013
                                        creator.CreateModelCurve(curve.CreateTransformed(_t)); // 2014
                                }

#endif // CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

                                break;
                            }
                    }

                Debug.Print(string.Format(
                        "{0} top face{1} found on {2} ({3})",
                        n, Util.PluralSuffix(n),
                        Util.ElementDescription(e)),
                    nWalls++);
            }

#if CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES
            t.Commit();
#endif // CREATE_MODEL_CURVES_FOR_TOP_FACE_EDGES

            var s = $"{nWalls} wall{Util.PluralSuffix(nWalls)} successfully processed";

            n = topFaces.Count;

            TaskDialog.Show("Wall Top Faces",
                $"{s} with {n} top face{Util.PluralSuffix(n)}.");

            return Result.Succeeded;
        }
    }
}