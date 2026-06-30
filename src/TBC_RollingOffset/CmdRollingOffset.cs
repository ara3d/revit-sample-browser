#region Header

//
// CmdRollingOffset.cs - calculate a rolling offset pipe segment between two existing pipes and hook them up
//
// Copyright (C) 2013-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdRollingOffset : IExternalCommand
    {
#pragma warning disable IDE1006

        private const string _prompt
            = "Please run this in a model containing "
              + "exactly two parallel offset pipe elements, "
              + "and they will be automatically selected. "
              + "Alternatively, pre-select two pipe elements "
              + "before launching this command, or post-select "
              + "them when prompted.";

        private const BuiltInParameter bipDiameter
            = BuiltInParameter.RBS_PIPE_DIAMETER_PARAM;

        private static readonly bool _place_model_line = false;

        private static readonly bool _place_fittings = false;

        private static readonly bool _use_static_pipe_create = true;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            //// Select all pipes in the entire model.

            //List<Pipe> pipes = new List<Pipe>(
            //  new FilteredElementCollector( doc )
            //    .OfClass( typeof( Pipe ) )
            //    .ToElements()
            //    .Cast<Pipe>() );

            //int n = pipes.Count;

            //// If there are less than two, 
            //// there is nothing we can do.

            //if( 2 > n )
            //{
            //  message = _prompt;
            //  return Result.Failed;
            //}

            //// If there are exactly two, pick those.

            //if( 2 < n )
            //{
            //  // Else, check for a pre-selection.

            //  pipes.Clear();

            //  Selection sel = uidoc.Selection;

            //  //n = sel.Elements.Size; // 2014

            //  ICollection<ElementId> ids
            //    = sel.GetElementIds(); // 2015

            //  n = ids.Count; // 2015

            //  Debug.Print( "{0} pre-selected elements.",
            //    n );

            //  // If two or more model pipes were pre-
            //  // selected, use the first two encountered.

            //  if( 1 < n )
            //  {
            //    //foreach( Element e in sel.Elements ) // 2014

            //    foreach( ElementId id in ids ) // 2015
            //    {
            //      Pipe c = doc.GetElement( id ) as Pipe;

            //      if( null != c )
            //      {
            //        pipes.Add( c );

            //        if( 2 == pipes.Count )
            //        {
            //          Debug.Print( "Found two model pipes, "
            //            + "ignoring everything else." );

            //          break;
            //        }
            //      }
            //    }
            //  }

            //  // Else, prompt for an 
            //  // interactive post-selection.

            //  if( 2 != pipes.Count )
            //  {
            //    pipes.Clear();

            //    try
            //    {
            //      Reference r = sel.PickObject(
            //        ObjectType.Element,
            //        new PipeElementSelectionFilter(),
            //        "Please pick first pipe." );

            //      pipes.Add( doc.GetElement( r.ElementId )
            //        as Pipe );
            //    }
            //    catch( Autodesk.Revit.Exceptions
            //      .OperationCanceledException )
            //    {
            //      return Result.Cancelled;
            //    }

            //    try
            //    {
            //      Reference r = sel.PickObject(
            //        ObjectType.Element,
            //        new PipeElementSelectionFilter(),
            //        "Please pick second pipe." );

            //      pipes.Add( doc.GetElement( r.ElementId )
            //        as Pipe );
            //    }
            //    catch( Autodesk.Revit.Exceptions
            //      .OperationCanceledException )
            //    {
            //      return Result.Cancelled;
            //    }
            //  }
            //}

            JtPairPicker<Pipe> picker
                = new(uidoc);

            var rc = picker.Pick();

            if (Result.Failed == rc) message = _prompt;

            if (Result.Succeeded != rc) return rc;

            var pipes = picker.Selected;

            var systemTypeId
                = pipes[0].MEPSystem.GetTypeId();

            Debug.Assert(pipes[1].MEPSystem.GetTypeId()
                    .Value.Equals(
                        systemTypeId.Value),
                "expected two similar pipes");

            var levelId = pipes[0].LevelId;

            Debug.Assert(
                pipes[1].LevelId.Value.Equals(
                    levelId.Value),
                "expected two pipes on same level");

            var wall_thickness = Util.GetWallThickness(pipes[0]);

            Debug.Print("{0} has wall thickness {1}",
                Util.ElementDescription(pipes[0]),
                Util.RealString(wall_thickness));

            var c0 = pipes[0].GetCurve();
            var c1 = pipes[1].GetCurve();

            if (c0 is not Line || c1 is not Line)
            {
                message = $"{_prompt} Expected straight pipes.";

                return Result.Failed;
            }

            var p00 = c0.GetEndPoint(0);
            var p01 = c0.GetEndPoint(1);

            var p10 = c1.GetEndPoint(0);
            var p11 = c1.GetEndPoint(1);

            var v0 = p01 - p00;
            var v1 = p11 - p10;

            if (!Util.IsParallel(v0, v1))
            {
                message = $"{_prompt} Expected parallel pipes.";

                return Result.Failed;
            }

            var p0 = p00.DistanceTo(p10) > p01.DistanceTo(p10)
                ? p00
                : p01;

            var p1 = p10.DistanceTo(p0) > p11.DistanceTo(p0)
                ? p10
                : p11;

            var pm = 0.5 * (p0 + p1);

            var v = p1 - p0;

            if (Util.IsParallel(v, v0))
            {
                message = "The selected pipes are colinear.";
                return Result.Failed;
            }

            var z = v.CrossProduct(v1);

            var w = z.CrossProduct(v1).Normalize();

            var distanceAcross = Math.Abs(
                v.DotProduct(w));

            var distanceAlong = Math.Abs(
                v.DotProduct(v1.Normalize()));

            Debug.Assert(Util.IsEqual(v.GetLength(),
                    Math.Sqrt((distanceAcross * distanceAcross)
                              + (distanceAlong * distanceAlong))),
                "expected Pythagorean equality here");

            var angle = 45 * Math.PI / 180.0;

            var angle2 = (0.5 * Math.PI) - angle;

            var length = distanceAcross * Math.Tan(angle2);

            var halfLength = 0.5 * length;

            var remainingPipeLength
                = 0.5 * (distanceAlong - length);

            if (0 > v1.DotProduct(v)) v1.Negate();

            v1 = v1.Normalize();

            var q0 = p0 + (remainingPipeLength * v1);

            var q1 = p1 - (remainingPipeLength * v1);

            using Transaction tx = new(doc);

            var pipe = pipes[0];

            var diameter = pipe
                .get_Parameter(bipDiameter) // "Diameter"
                .AsDouble();

            var pipe_type_standard
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(PipeType))
                    .Cast<PipeType>()
                    .Where(e
                        => e.Name.Equals("Standard"))
                    .FirstOrDefault();

            Debug.Assert(
                pipe_type_standard.Id.Value.Equals(
                    pipe.PipeType.Id.Value),
                "expected all pipes in this simple "
                + "model to use the same pipe type");

            tx.Start("Rolling Offset");

            if (_place_model_line)
            {
                (pipes[0].Location as LocationCurve).Curve
                    = Line.CreateBound(p0, q0);

                (pipes[1].Location as LocationCurve).Curve
                    = Line.CreateBound(p1, q1);

                Creator creator = new(doc);

                var line = Line.CreateBound(q0, q1);

                creator.CreateModelCurve(line);

                pipe = null;
            }
            else if (_place_fittings)
            {
                // Setting the active work plane does not affect fitting placement or rotation.
                //
                //Plane plane = new Plane( z, q0 );
                //
                //SketchPlane sp = SketchPlane.Create( 
                //  doc, plane );
                //
                //uidoc.ActiveView.SketchPlane = sp;
                //uidoc.ActiveView.ShowActiveWorkPlane();

                var symbol
                    = new FilteredElementCollector(doc)
                        .OfClass(typeof(FamilySymbol))
                        .OfCategory(BuiltInCategory.OST_PipeFitting)
                        .Cast<FamilySymbol>()
                        .Where(e
                            => e.Family.Name.Contains("Elbow - Generic"))
                        .FirstOrDefault();

                var fitting0 = doc.Create
                    .NewFamilyInstance(q0, symbol,
                        StructuralType.NonStructural);

                fitting0.LookupParameter("Angle").Set(
                    45.0 * Math.PI / 180.0);

                //fitting0.get_Parameter( bipDiameter ) // does not exist
                //  .Set( diameter );

                fitting0.LookupParameter("Nominal Radius")
                    .Set(0.5 * diameter);

                var axis = Line.CreateBound(p0, q0);
                angle = z.AngleTo(XYZ.BasisZ);

                ElementTransformUtils.RotateElement(
                    doc, fitting0.Id, axis, Math.PI - angle);

                var con0 = Util.GetConnectorClosestTo(
                    fitting0, p0);

                (pipes[0].Location as LocationCurve).Curve
                    = Line.CreateBound(p0, con0.Origin);

                Util.Connect(con0.Origin, pipe, fitting0);

                var fitting1 = doc.Create
                    .NewFamilyInstance(q1, symbol,
                        StructuralType.NonStructural);

                //fitting1.get_Parameter( "Angle" ).Set( 45.0 * Math.PI / 180.0 ); // 2014
                //fitting1.get_Parameter( "Nominal Radius" ).Set( 0.5 * diameter ); // 2014

                fitting1.LookupParameter("Angle").Set(45.0 * Math.PI / 180.0); // 2015
                fitting1.LookupParameter("Nominal Radius").Set(0.5 * diameter); // 2015

                axis = Line.CreateBound(
                    q1, q1 + XYZ.BasisZ);

                ElementTransformUtils.RotateElement(
                    doc, fitting1.Id, axis, Math.PI);

                axis = Line.CreateBound(q1, p1);

                ElementTransformUtils.RotateElement(
                    doc, fitting1.Id, axis, Math.PI - angle);

                var con1 = Util.GetConnectorClosestTo(
                    fitting1, p1);

                (pipes[1].Location as LocationCurve).Curve
                    = Line.CreateBound(con1.Origin, p1);

                Util.Connect(con1.Origin, fitting1, pipes[1]);

                con0 = Util.GetConnectorClosestTo(
                    fitting0, pm);

                con1 = Util.GetConnectorClosestTo(
                    fitting1, pm);

                // Connecting fittings directly does not insert a pipe segment between them.
                //
                //con0.ConnectTo( con1 );

                pipe = Pipe.Create(doc,
                    pipe_type_standard.Id, levelId, con0, con1); // 2015

                pipe.get_Parameter(bipDiameter)
                    .Set(diameter);

                Util.Connect(con0.Origin, fitting0, pipe);
                Util.Connect(con1.Origin, pipe, fitting1);
            }
            else
            {
                if (_use_static_pipe_create)
                {
                    ElementId idSystem;
                    ElementId idType;
                    ElementId idLevel;

                    // pipe.MEPSystem.Id and similar values are invalid for Pipe.Create's systemTypeId.
                    var idSystem1 = pipe.MEPSystem.Id;
                    var idSystem2 = ElementId.InvalidElementId;
                    var idSystem3 = PipingSystem.Create(
                            doc, pipe.MEPSystem.GetTypeId(), "Tbc")
                        .Id;

                    // Throws: "The systemTypeId is not valid piping system type."
                    //pipe = Pipe.Create( doc, idSystem,
                    //  idType, idLevel, q0, q1 );

                    var pipingSystemType
                        = new FilteredElementCollector(doc)
                            .OfClass(typeof(PipingSystemType))
                            .OfType<PipingSystemType>()
                            .FirstOrDefault(st
                                => st.SystemClassification
                                   == MEPSystemClassification
                                       .SupplyHydronic);

                    if (null == pipingSystemType)
                    {
                        message = "Could not find hydronic supply piping system type";
                        return Result.Failed;
                    }

                    idSystem = pipingSystemType.Id;

                    Debug.Assert(pipe.get_Parameter(
                                BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM)
                            .AsElementId().Value.Equals(
                                idSystem.Value),
                        "expected same piping system element id");

                    var pipeType =
                        new FilteredElementCollector(doc)
                            .OfClass(typeof(PipeType))
                            .OfType<PipeType>()
                            .FirstOrDefault();

                    if (null == pipeType)
                    {
                        message = "Could not find pipe type";
                        return Result.Failed;
                    }

                    idType = pipeType.Id;

                    Debug.Assert(pipe.get_Parameter(
                                BuiltInParameter.ELEM_TYPE_PARAM)
                            .AsElementId().Value.Equals(
                                idType.Value),
                        "expected same pipe type element id");

                    Debug.Assert(pipe.PipeType.Id.Value
                            .Equals(idType.Value),
                        "expected same pipe type element id");

                    // pipe.LevelId is not the reference level for Pipe.Create.
                    idLevel = pipe.get_Parameter(
                            BuiltInParameter.RBS_START_LEVEL_PARAM)
                        .AsElementId();

                    pipe = Pipe.Create(doc,
                        idSystem, idType, idLevel, q0, q1);
                }
                else
                {
                    //pipe = doc.Create.NewPipe( q0, q1, pipe_type_standard ); // 2014

                    pipe = Pipe.Create(doc, systemTypeId,
                        pipe_type_standard.Id, levelId, q0, q1); // 2015
                }

                pipe.get_Parameter(bipDiameter)
                    .Set(diameter);

                //Util.Connect( q0, pipes[0], pipe );
                //Util.Connect( q1, pipe, pipes[1] );

                var con0 = Util.GetConnectorClosestTo(
                    pipes[0], q0);

                var con = Util.GetConnectorClosestTo(
                    pipe, q0);

                doc.Create.NewElbowFitting(con0, con);

                var con1 = Util.GetConnectorClosestTo(
                    pipes[1], q1);

                con = Util.GetConnectorClosestTo(
                    pipe, q1);

                doc.Create.NewElbowFitting(con, con1);
            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}