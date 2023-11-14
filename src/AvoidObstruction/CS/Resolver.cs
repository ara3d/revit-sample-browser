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


using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;

namespace Revit.SDK.Samples.AvoidObstruction.CS
{
   /// <summary>
   /// This class implement the algorithm to detect the obstruction and resolve it.
   /// </summary>
   class Resolver
   {
      /// <summary>
      /// Revit Document.
      /// </summary>
      private Document m_rvtDoc;

      /// <summary>
      /// Revit Application.
      /// </summary>
      private Application m_rvtApp;

      /// <summary>
      /// Detector to detect the obstructions.
      /// </summary>
      private Detector m_detector;


      PipingSystemType m_pipingSystemType;

      /// <summary>
      /// Constructor, initialize all the fields of this class.
      /// </summary>
      /// <param name="data">Revit ExternalCommandData from external command entrance</param>
      public Resolver(ExternalCommandData data)
      {
         m_rvtDoc = data.Application.ActiveUIDocument.Document;
         m_rvtApp = data.Application.Application;
         m_detector = new Detector(m_rvtDoc);

         var collector = new FilteredElementCollector(m_rvtDoc);
         var pipingSystemTypes = collector.OfClass(typeof(PipingSystemType)).ToElements();
         foreach (PipingSystemType pipingSystemType in pipingSystemTypes)
         {
            if (pipingSystemType.SystemClassification == MEPSystemClassification.SupplyHydronic ||
               pipingSystemType.SystemClassification == MEPSystemClassification.ReturnHydronic)
            {
               m_pipingSystemType = pipingSystemType; 
               break;
            }
         }
      }

      /// <summary>
      /// Detect and resolve the obstructions of all Pipes.
      /// </summary>
      public void Resolve()
      {
         var pipes = new List<Element>();
         var collector = new FilteredElementCollector(m_rvtDoc);
         pipes.AddRange(collector.OfClass(typeof(Pipe)).ToElements());
         foreach (var pipe in pipes)
         {
            Resolve(pipe as Pipe);
         }
      }

      /// <summary>
      /// Calculate the uniform perpendicular directions with inputting direction "dir".
      /// </summary>
      /// <param name="dir">Direction to calculate</param>
      /// <param name="count">How many perpendicular directions will be calculated</param>
      /// <returns>The calculated perpendicular directions with dir</returns>
      private List<XYZ> PerpendicularDirs(XYZ dir, int count)
      {
         var dirs = new List<XYZ>();
         var plane = Plane.CreateByNormalAndOrigin(dir, XYZ.Zero);
         var arc = Arc.Create(plane, 1.0, 0, 6.28);

         var delta = 1.0 / (double)count;
         for (var i = 1; i <= count; i++)
         {
            var pt = arc.Evaluate(delta * i, true);
            dirs.Add(pt);
         }

         return dirs;
      }

      /// <summary>
      /// Detect the obstructions of pipe and resolve them.
      /// </summary>
      /// <param name="pipe">Pipe to resolve</param>
      private void Resolve(Pipe pipe)
      {
         var parameter = pipe.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM);
         var levelId = parameter.AsElementId();
         var systemTypeId = m_pipingSystemType.Id;
         // Get the centerline of pipe.
         var pipeLine = (pipe.Location as LocationCurve).Curve as Line;

         // Calculate the intersection references with pipe's centerline.
         var obstructionRefArr = m_detector.Obstructions(pipeLine);

         // Filter out the references, just allow Pipe, Beam, and Duct.
         Filter(pipe, obstructionRefArr);

         if (obstructionRefArr.Count == 0)
         {
            // There are no intersection found.
            return;
         }

         // Calculate the direction of pipe's centerline.
         var dir = pipeLine.GetEndPoint(1) - pipeLine.GetEndPoint(0);

         // Build the sections from the intersection references.
         var sections = Section.BuildSections(obstructionRefArr, dir.Normalize());

         // Merge the neighbor sections if the distance of them is too close.
         for (var i = sections.Count - 2; i >= 0; i--)
         {
            var detal = sections[i].End - sections[i + 1].Start;
            if (detal.GetLength() < pipe.Diameter * 3)
            {
               sections[i].Refs.AddRange(sections[i + 1].Refs);
               sections.RemoveAt(i + 1);
            }
         }

         // Resolve the obstructions one by one.
         foreach (var sec in sections)
         {
            Resolve(pipe, sec);
         }

         // Connect the neighbor sections with pipe and elbow fittings.
         //
         for (var i = 1; i < sections.Count; i++)
         {
            // Get the end point from the previous section.
            var start = sections[i - 1].End;

            // Get the start point from the current section.
            var end = sections[i].Start;

            // Create a pipe between two neighbor section.
            var tmpPipe = Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, start, end);

            // Copy pipe's parameters values to tmpPipe.
            CopyParameters(pipe, tmpPipe);

            // Create elbow fitting to connect previous section with tmpPipe.
            var conn1 = FindConnector(sections[i - 1].Pipes[2], start);
            var conn2 = FindConnector(tmpPipe, start);
            var fi = m_rvtDoc.Create.NewElbowFitting(conn1, conn2);

            // Create elbow fitting to connect current section with tmpPipe.
            var conn3 = FindConnector(sections[i].Pipes[0], end);
            var conn4 = FindConnector(tmpPipe, end);
            var f2 = m_rvtDoc.Create.NewElbowFitting(conn3, conn4);
         }

         // Find two connectors which pipe's two ends connector connected to. 
         var startConn = FindConnectedTo(pipe, pipeLine.GetEndPoint(0));
         var endConn = FindConnectedTo(pipe, pipeLine.GetEndPoint(1));

         Pipe startPipe = null;
         if (null != startConn)
         {
            // Create a pipe between pipe's start connector and pipe's start section.
            startPipe = Pipe.Create(m_rvtDoc, pipe.PipeType.Id, levelId, startConn, sections[0].Start);
         }
         else
         {
            // Create a pipe between pipe's start point and pipe's start section.
            startPipe = Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, sections[0].Start, pipeLine.GetEndPoint(0));
         }

         // Copy parameters from pipe to startPipe. 
         CopyParameters(pipe, startPipe);

         // Connect the startPipe and first section with elbow fitting.
         var connStart1 = FindConnector(startPipe, sections[0].Start);
         var connStart2 = FindConnector(sections[0].Pipes[0], sections[0].Start);
         var fii = m_rvtDoc.Create.NewElbowFitting(connStart1, connStart2);

         Pipe endPipe = null;
         var count = sections.Count;
         if (null != endConn)
         {
            // Create a pipe between pipe's end connector and pipe's end section.
            endPipe = Pipe.Create(m_rvtDoc, pipe.PipeType.Id, levelId, endConn, sections[count - 1].End);
         }
         else
         {
            // Create a pipe between pipe's end point and pipe's end section.
            endPipe = Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, sections[count - 1].End, pipeLine.GetEndPoint(1));
         }

         // Copy parameters from pipe to endPipe.
         CopyParameters(pipe, endPipe);

         // Connect the endPipe and last section with elbow fitting.
         var connEnd1 = FindConnector(endPipe, sections[count - 1].End);
         var connEnd2 = FindConnector(sections[count - 1].Pipes[2], sections[count - 1].End);
         var fiii = m_rvtDoc.Create.NewElbowFitting(connEnd1, connEnd2);

         // Delete the pipe after resolved.
         m_rvtDoc.Delete(pipe.Id);
      }

      /// <summary>
      /// Filter the inputting References, just allow Pipe, Duct and Beam References.
      /// </summary>
      /// <param name="pipe">Pipe</param>
      /// <param name="refs">References to filter</param>
      private void Filter(Pipe pipe, List<ReferenceWithContext> refs)
      {
         for (var i = refs.Count - 1; i >= 0; i--)
         {
            var cur = refs[i].GetReference();
            var curElem = m_rvtDoc.GetElement(cur);
            if (curElem.Id == pipe.Id ||
            (!(curElem is Pipe) && !(curElem is Duct) &&
            curElem.Category.BuiltInCategory != BuiltInCategory.OST_StructuralFraming))
            {
               refs.RemoveAt(i);
            }
         }
      }

      /// <summary>
      /// This method will find out a route to avoid the obstruction. 
      /// </summary>
      /// <param name="pipe">Pipe to resolve</param>
      /// <param name="section">Pipe's one obstruction</param>
      /// <returns>A route which can avoid the obstruction</returns>
      private Line FindRoute(Pipe pipe, Section section)
      {
         // Perpendicular direction minimal length.
         var minLength = pipe.Diameter * 2;

         // Parallel direction jump step. 
         var jumpStep = pipe.Diameter;

         // Calculate the directions in which to find the solution.
         var dirs = new List<XYZ>();
         XYZ crossDir = null;
         foreach (var gref in section.Refs)
         {
            var elem = m_rvtDoc.GetElement(gref.GetReference());
            var locationLine = (elem.Location as LocationCurve).Curve as Line;
            var refDir = locationLine.GetEndPoint(1) - locationLine.GetEndPoint(0);
            refDir = refDir.Normalize();
            if (refDir.IsAlmostEqualTo(section.PipeCenterLineDirection) || refDir.IsAlmostEqualTo(-section.PipeCenterLineDirection))
            {
               continue;
            }
            crossDir = refDir.CrossProduct(section.PipeCenterLineDirection);
            dirs.Add(crossDir.Normalize());
            break;
         }

         // When all the obstruction are parallel with the centerline of the pipe,
         // We can't calculate the direction from the vector.Cross method.
         if (dirs.Count == 0)
         {
            // Calculate perpendicular directions with dir in four directions.
            var perDirs = PerpendicularDirs(section.PipeCenterLineDirection, 4);
            dirs.Add(perDirs[0]);
            dirs.Add(perDirs[1]);
         }

         Line foundLine = null;
         while (null == foundLine)
         {
            // Extend the section interval by jumpStep.
            section.Inflate(0, jumpStep);
            section.Inflate(1, jumpStep);

            // Find solution in the given directions.
            for (var i = 0; null == foundLine && i < dirs.Count; i++)
            {
               // Calculate the intersections.
               var obs1 = m_detector.Obstructions(section.Start, dirs[i]);
               var obs2 = m_detector.Obstructions(section.End, dirs[i]);

               // Filter out the intersection result.
               Filter(pipe, obs1);
               Filter(pipe, obs2);

               // Find out the minimal intersections in two opposite direction.
               var mins1 = GetClosestSectionsToOrigin(obs1);
               var mins2 = GetClosestSectionsToOrigin(obs2);

               // Find solution in the given direction and its opposite direction.
               for (var j = 0; null == foundLine && j < 2; j++)
               {
                  if (mins1[j] != null && Math.Abs(mins1[j].Proximity) < minLength ||
                      mins2[j] != null && Math.Abs(mins2[j].Proximity) < minLength)
                  {
                     continue;
                  }

                  // Calculate the maximal height that the parallel line can be reached.
                  var maxHight = 1000 * pipe.Diameter;
                  if (mins1[j] != null && mins2[j] != null)
                  {
                     maxHight = Math.Min(Math.Abs(mins1[j].Proximity), Math.Abs(mins2[j].Proximity));
                  }
                  else if (mins1[j] != null)
                  {
                     maxHight = Math.Abs(mins1[j].Proximity);
                  }
                  else if (mins2[j] != null)
                  {
                     maxHight = Math.Abs(mins2[j].Proximity);
                  }

                  var dir = (j == 1) ? dirs[i] : -dirs[i];

                  // Calculate the parallel line which can avoid obstructions.
                  foundLine = FindParallelLine(pipe, section, dir, maxHight);
               }
            }
         }
         return foundLine;
      }

      /// <summary>
      /// Find a line Parallel to pipe's centerline to avoid the obstruction.
      /// </summary>
      /// <param name="pipe">Pipe who has obstructions</param>
      /// <param name="section">Pipe's one obstruction</param>
      /// <param name="dir">Offset Direction of the parallel line</param>
      /// <param name="maxLength">Maximum offset distance</param>
      /// <returns>Parallel line which can avoid the obstruction</returns>
      private Line FindParallelLine(Pipe pipe, Section section, XYZ dir, double maxLength)
      {
         var step = pipe.Diameter;
         var hight = 2 * pipe.Diameter;
         while (hight <= maxLength)
         {
            var detal = dir * hight;
            var line = Line.CreateBound(section.Start + detal, section.End + detal);
            var refs = m_detector.Obstructions(line);
            Filter(pipe, refs);

            if (refs.Count == 0)
            {
               return line;
            }
            hight += step;
         }
         return null;
      }

      /// <summary>
      /// Find out two References, whose ProximityParameter is negative or positive,
      /// And Get the minimal value from all positive reference, and get the maximal value 
      /// from the negative reference. if there are no such reference, using null instead.
      /// </summary>
      /// <param name="refs">References</param>
      /// <returns>Reference array</returns>
      private ReferenceWithContext[] GetClosestSectionsToOrigin(List<ReferenceWithContext> refs)
      {
         var mins = new ReferenceWithContext[2];
         if (refs.Count == 0)
         {
            return mins;
         }

         if (refs[0].Proximity > 0)
         {
            mins[1] = refs[0];
            return mins;
         }

         for (var i = 0; i < refs.Count - 1; i++)
         {
            if (refs[i].Proximity < 0 && refs[i + 1].Proximity > 0)
            {
               mins[0] = refs[i];
               mins[1] = refs[i + 1];
               return mins;
            }
         }

         mins[0] = refs[refs.Count - 1];

         return mins;
      }


      /// <summary>
      /// Resolve one obstruction of Pipe.
      /// </summary>
      /// <param name="pipe">Pipe to resolve</param>
      /// <param name="section">One pipe's obstruction</param>
      private void Resolve(Pipe pipe, Section section)
      {
         // Find out a parallel line of pipe centerline, which can avoid the obstruction.
         var offset = FindRoute(pipe, section);

         // Construct a section line according to the given section.
         var sectionLine = Line.CreateBound(section.Start, section.End);

         // Construct two side lines, which can avoid the obstruction too.
         var side1 = Line.CreateBound(sectionLine.GetEndPoint(0), offset.GetEndPoint(0));
         var side2 = Line.CreateBound(offset.GetEndPoint(1), sectionLine.GetEndPoint(1));

         //
         // Create an "U" shape, which connected with three pipes and two elbows, to round the obstruction.
         //
         var pipeType = pipe.PipeType;
         var start = side1.GetEndPoint(0);
         var startOffset = offset.GetEndPoint(0);
         var endOffset = offset.GetEndPoint(1);
         var end = side2.GetEndPoint(1);

         var parameter = pipe.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM);
         var levelId = parameter.AsElementId();
         // Create three side pipes of "U" shape.
         var systemTypeId = m_pipingSystemType.Id;
         var pipe1 = Pipe.Create(m_rvtDoc, systemTypeId, pipeType.Id, levelId, start, startOffset);
         var pipe2 = Pipe.Create(m_rvtDoc, systemTypeId, pipeType.Id, levelId, startOffset, endOffset);
         var pipe3 = Pipe.Create(m_rvtDoc, systemTypeId, pipeType.Id, levelId, endOffset, end);

         // Copy parameters from pipe to other three created pipes.
         CopyParameters(pipe, pipe1);
         CopyParameters(pipe, pipe2);
         CopyParameters(pipe, pipe3);

         // Add the created three pipes to current section.
         section.Pipes.Add(pipe1);
         section.Pipes.Add(pipe2);
         section.Pipes.Add(pipe3);

         // Create the first elbow to connect two neighbor pipes of "U" shape.
         var conn1 = FindConnector(pipe1, startOffset);
         var conn2 = FindConnector(pipe2, startOffset);
         m_rvtDoc.Create.NewElbowFitting(conn1, conn2);

         // Create the second elbow to connect another two neighbor pipes of "U" shape.
         var conn3 = FindConnector(pipe2, endOffset);
         var conn4 = FindConnector(pipe3, endOffset);
         m_rvtDoc.Create.NewElbowFitting(conn3, conn4);
      }

      /// <summary>
      /// Copy parameters from source pipe to target pipe.
      /// </summary>
      /// <param name="source">Coping source</param>
      /// <param name="target">Coping target</param>
      private void CopyParameters(Pipe source, Pipe target)
      {
         var diameter = source.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).AsDouble();
         target.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).Set(diameter);
      }

      /// <summary>
      /// Find out a connector from pipe with a specified point.
      /// </summary>
      /// <param name="pipe">Pipe to find the connector</param>
      /// <param name="conXYZ">Specified point</param>
      /// <returns>Connector whose origin is conXYZ</returns>
      private Connector FindConnector(Pipe pipe, XYZ conXYZ)
      {
         var conns = pipe.ConnectorManager.Connectors;
         foreach (Connector conn in conns)
         {
            if (conn.Origin.IsAlmostEqualTo(conXYZ))
            {
               return conn;
            }
         }
         return null;
      }

      /// <summary>
      /// Find out the connector which the pipe's specified connector connected to.
      /// The pipe's specified connector is given by point conxyz.
      /// </summary>
      /// <param name="pipe">Pipe to find the connector</param>
      /// <param name="conXYZ">Specified point</param>
      /// <returns>Connector whose origin is conXYZ</returns>
      private Connector FindConnectedTo(Pipe pipe, XYZ conXYZ)
      {
         var connItself = FindConnector(pipe, conXYZ);
         var connSet = connItself.AllRefs;
         foreach (Connector conn in connSet)
         {
            if (conn.Owner.Id != pipe.Id &&
                conn.ConnectorType == ConnectorType.End)
            {
               return conn;
            }
         }
         return null;
      }
   }
}
