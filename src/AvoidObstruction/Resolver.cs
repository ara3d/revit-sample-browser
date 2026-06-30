// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Mep;
using Ara3D.RevitSampleBrowser.Common.Parameters;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.AvoidObstruction.CS
{
    public class Resolver
    {
        private readonly Detector m_detector;

        private readonly PipingSystemType m_pipingSystemType;

        private readonly Application m_rvtApp;

        private readonly Document m_rvtDoc;

        public Resolver(ExternalCommandData data)
        {
            m_rvtDoc = data.Application.ActiveUIDocument.Document;
            m_rvtApp = data.Application.Application;
            m_detector = new Detector(m_rvtDoc);

            foreach (var pipingSystemType in m_rvtDoc.GetElements<PipingSystemType>())
            {
                if (pipingSystemType.SystemClassification is MEPSystemClassification.SupplyHydronic or
                    MEPSystemClassification.ReturnHydronic)
                {
                    m_pipingSystemType = pipingSystemType;
                    break;
                }
            }
        }

        public void Resolve()
        {
            foreach (var pipe in m_rvtDoc.GetElements<Pipe>())
                Resolve(pipe);
        }

        private void Resolve(Pipe pipe)
        {
            var parameter = pipe.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM);
            var levelId = parameter.AsElementId();
            var systemTypeId = m_pipingSystemType.Id;
            var pipeLine = (pipe.Location as LocationCurve).Curve as Line;

            var obstructionRefArr = m_detector.Obstructions(pipeLine);

            Filter(pipe, obstructionRefArr);

            if (obstructionRefArr.Count == 0)
                // There are no intersection found.
                return;

            var dir = pipeLine.GetEndPoint(1) - pipeLine.GetEndPoint(0);

            var sections = Section.BuildSections(obstructionRefArr, dir.Normalize());

            for (var i = sections.Count - 2; i >= 0; i--)
            {
                var detal = sections[i].End - sections[i + 1].Start;
                if (detal.GetLength() < pipe.Diameter * 3)
                {
                    sections[i].Refs.AddRange(sections[i + 1].Refs);
                    sections.RemoveAt(i + 1);
                }
            }

            foreach (var sec in sections)
            {
                Resolve(pipe, sec);
            }

            //
            for (var i = 1; i < sections.Count; i++)
            {
                var start = sections[i - 1].End;

                var end = sections[i].Start;

                var tmpPipe = Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, start, end);

                ParameterAccess.CopyParameters(pipe, tmpPipe);

                // Create elbow fitting to connect previous section with tmpPipe.
                var conn1 = ConnectorHelper.FindConnector(sections[i - 1].Pipes[2], start);
                var conn2 = ConnectorHelper.FindConnector(tmpPipe, start);
                m_rvtDoc.Create.NewElbowFitting(conn1, conn2);

                // Create elbow fitting to connect current section with tmpPipe.
                var conn3 = ConnectorHelper.FindConnector(sections[i].Pipes[0], end);
                var conn4 = ConnectorHelper.FindConnector(tmpPipe, end);
                m_rvtDoc.Create.NewElbowFitting(conn3, conn4);
            }

            // Find two connectors which pipe's two ends connector connected to. 
            var startConn = ElementQuery.FindConnectedTo(pipe, pipeLine.GetEndPoint(0));
            var endConn = ElementQuery.FindConnectedTo(pipe, pipeLine.GetEndPoint(1));

            var startPipe = null != startConn
                ? Pipe.Create(m_rvtDoc, pipe.PipeType.Id, levelId, startConn, sections[0].Start)
                : Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, sections[0].Start,
                    pipeLine.GetEndPoint(0));
            ParameterAccess.CopyParameters(pipe, startPipe);

            var connStart1 = ConnectorHelper.FindConnector(startPipe, sections[0].Start);
            var connStart2 = ConnectorHelper.FindConnector(sections[0].Pipes[0], sections[0].Start);
            m_rvtDoc.Create.NewElbowFitting(connStart1, connStart2);

            Pipe endPipe = null;
            var count = sections.Count;
            endPipe = null != endConn
                ? Pipe.Create(m_rvtDoc, pipe.PipeType.Id, levelId, endConn, sections[count - 1].End)
                : Pipe.Create(m_rvtDoc, systemTypeId, pipe.PipeType.Id, levelId, sections[count - 1].End,
                    pipeLine.GetEndPoint(1));

            ParameterAccess.CopyParameters(pipe, endPipe);

            var connEnd1 = ConnectorHelper.FindConnector(endPipe, sections[count - 1].End);
            var connEnd2 = ConnectorHelper.FindConnector(sections[count - 1].Pipes[2], sections[count - 1].End);
            m_rvtDoc.Create.NewElbowFitting(connEnd1, connEnd2);

            m_rvtDoc.Delete(pipe.Id);
        }

        private void Filter(Pipe pipe, List<ReferenceWithContext> refs)
        {
            for (var i = refs.Count - 1; i >= 0; i--)
            {
                var cur = refs[i].GetReference();
                var curElem = m_rvtDoc.GetElement(cur);
                if (curElem.Id == pipe.Id ||
                    (curElem is not Pipe && curElem is not Duct &&
                     curElem.Category.BuiltInCategory != BuiltInCategory.OST_StructuralFraming))
                    refs.RemoveAt(i);
            }
        }

        private Line FindRoute(Pipe pipe, Section section)
        {
            // Perpendicular direction minimal length.
            var minLength = pipe.Diameter * 2;

            // Parallel direction jump step. 
            var jumpStep = pipe.Diameter;

            List<XYZ> dirs = new();
            foreach (var gref in section.Refs)
            {
                var elem = m_rvtDoc.GetElement(gref.GetReference());
                var locationLine = (elem.Location as LocationCurve).Curve as Line;
                var refDir = locationLine.GetEndPoint(1) - locationLine.GetEndPoint(0);
                refDir = refDir.Normalize();
                if (refDir.IsAlmostEqualTo(section.PipeCenterLineDirection) ||
                    refDir.IsAlmostEqualTo(-section.PipeCenterLineDirection)) continue;
                var crossDir = refDir.CrossProduct(section.PipeCenterLineDirection);
                dirs.Add(crossDir.Normalize());
                break;
            }

            // When all the obstruction are parallel with the centerline of the pipe,
            // We can't calculate the direction from the vector.Cross method.
            if (dirs.Count == 0)
            {
                // Calculate perpendicular directions with dir in four directions.
                var perDirs = SampleBrowserUtils.PerpendicularDirs(section.PipeCenterLineDirection, 4);
                dirs.Add(perDirs[0]);
                dirs.Add(perDirs[1]);
            }

            Line foundLine = null;
            while (null == foundLine)
            {
                section.Inflate(0, jumpStep);
                section.Inflate(1, jumpStep);

                for (var i = 0; null == foundLine && i < dirs.Count; i++)
                {
                    var obs1 = m_detector.Obstructions(section.Start, dirs[i]);
                    var obs2 = m_detector.Obstructions(section.End, dirs[i]);

                    Filter(pipe, obs1);
                    Filter(pipe, obs2);

                    // Find out the minimal intersections in two opposite direction.
                    var mins1 = SampleBrowserUtils.GetClosestSectionsToOrigin(obs1);
                    var mins2 = SampleBrowserUtils.GetClosestSectionsToOrigin(obs2);

                    for (var j = 0; null == foundLine && j < 2; j++)
                    {
                        if ((mins1[j] != null && Math.Abs(mins1[j].Proximity) < minLength) ||
                            (mins2[j] != null && Math.Abs(mins2[j].Proximity) < minLength))
                            continue;

                        var maxHight = 1000 * pipe.Diameter;
                        if (mins1[j] != null && mins2[j] != null)
                            maxHight = Math.Min(Math.Abs(mins1[j].Proximity), Math.Abs(mins2[j].Proximity));
                        else if (mins1[j] != null)
                            maxHight = Math.Abs(mins1[j].Proximity);
                        else if (mins2[j] != null) maxHight = Math.Abs(mins2[j].Proximity);

                        var dir = j == 1 ? dirs[i] : -dirs[i];

                        foundLine = FindParallelLine(pipe, section, dir, maxHight);
                    }
                }
            }

            return foundLine;
        }

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

                if (refs.Count == 0) return line;
                hight += step;
            }

            return null;
        }

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

            ParameterAccess.CopyParameters(pipe, pipe1);
            ParameterAccess.CopyParameters(pipe, pipe2);
            ParameterAccess.CopyParameters(pipe, pipe3);

            // Add the created three pipes to current section.
            section.Pipes.Add(pipe1);
            section.Pipes.Add(pipe2);
            section.Pipes.Add(pipe3);

            var conn1 = ConnectorHelper.FindConnector(pipe1, startOffset);
            var conn2 = ConnectorHelper.FindConnector(pipe2, startOffset);
            m_rvtDoc.Create.NewElbowFitting(conn1, conn2);

            var conn3 = ConnectorHelper.FindConnector(pipe2, endOffset);
            var conn4 = ConnectorHelper.FindConnector(pipe3, endOffset);
            m_rvtDoc.Create.NewElbowFitting(conn3, conn4);
        }
    }
}
