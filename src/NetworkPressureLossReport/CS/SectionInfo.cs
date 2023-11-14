// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace RevitMultiSample.NetworkPressureLossReport
{
    internal class SectionInfo
    {
        private const double
            Epsilon = 0.0001; // The small tolerance within which two flow values may be considered equal.

        private readonly IList<SegmentInfo> m_segments;

        public SectionInfo()
        {
            TotalPressureLoss = 0.0;
            m_segments = new List<SegmentInfo>();
        }

        public double TotalPressureLoss { get; set; }

        public int NumberOfSegments => m_segments.Count;

        public int NumberOfStraights
        {
            get { return m_segments.Count(x => x.SegmentType == MEPAnalyticalSegmentType.Segment); }
        }

        public int NumberOfFittingsOrAccessories
        {
            get { return m_segments.Count(x => x.SegmentType == MEPAnalyticalSegmentType.Fitting); }
        }

        public double Flow => m_segments.FirstOrDefault().Flow;

        public double Size => m_segments.FirstOrDefault().Size;

        public double Velocity => m_segments.FirstOrDefault().Velocity;

        public double VelocityPressure => m_segments.FirstOrDefault().VelocityPressure;

        public double Friction => m_segments.FirstOrDefault().Friction;

        public bool IsCriticalPath => m_segments.FirstOrDefault().IsCriticalPath;

        public SegmentInfo AddSegment(Document doc, MEPAnalyticalSegment segment, MEPNetworkSegmentData segmentData)
        {
            var newSegmentInfo = new SegmentInfo(doc, segment, segmentData);
            m_segments.Add(newSegmentInfo);
            return newSegmentInfo;
        }

        public void ExportCsv(CsvExporter ex, int sectionNumber)
        {
            // "Section, Type/No, Element, Flow, Size, Velocity, Velocity Pressure, Length, Coefficients, Friction, Pressure Loss, Section Pressure Loss");
            string sNull = null;
            int straightCount = 0, fittingCount = 0;
            var totalStraightLength = 0.0;
            var totalFittingCoeff = 0.0;
            var totalStraightLoss = 0.0;
            var totalFittingLoss = 0.0;

            foreach (var segInfo in m_segments)
            {
                if (ex.IsItemized == true)
                    ex.Writer.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}", sectionNumber,
                        segInfo.SegmentType, segInfo.Id, ex.ConvertFromInternalFlow(segInfo.Flow),
                        ex.ConvertFromInternalSize(segInfo.Size), ex.ConvertFromInternalVelocity(segInfo.Velocity),
                        ex.ConvertFromInternalPressure(segInfo.VelocityPressure),
                        ex.ConvertFromInternalLength(segInfo.Length), segInfo.Coefficients,
                        ex.ConvertFromInternalFriction(segInfo.Friction),
                        ex.ConvertFromInternalPressure(segInfo.PressureDrop), sNull);
                switch (segInfo.SegmentType)
                {
                    case MEPAnalyticalSegmentType.Segment:
                        straightCount++;
                        totalStraightLength += segInfo.Length;
                        totalStraightLoss += segInfo.PressureDrop;
                        break;
                    case MEPAnalyticalSegmentType.Fitting:
                        fittingCount++;
                        totalFittingCoeff += segInfo.Coefficients;
                        totalFittingLoss = segInfo.PressureDrop;
                        break;
                }
            }

            TotalPressureLoss = totalStraightLoss + totalFittingLoss;

            // "Section, Type/No, Element, Flow, Size, Velocity, Velocity Pressure, Length, Coefficients, Friction, Pressure Loss, Section Pressure Loss");
            ex.Writer.WriteLine("{0}, {1}, Straights, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", sectionNumber,
                straightCount, ex.ConvertFromInternalFlow(Flow), ex.ConvertFromInternalSize(Size),
                ex.ConvertFromInternalVelocity(Velocity), sNull, ex.ConvertFromInternalLength(totalStraightLength),
                sNull, ex.ConvertFromInternalFriction(Friction), ex.ConvertFromInternalPressure(totalStraightLoss),
                sNull);

            ex.Writer.WriteLine("{0}, {1}, Fittings, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}", sectionNumber,
                fittingCount, ex.ConvertFromInternalFlow(Flow), ex.ConvertFromInternalSize(Size),
                ex.ConvertFromInternalVelocity(Velocity), ex.ConvertFromInternalPressure(VelocityPressure), sNull,
                totalFittingCoeff, sNull, ex.ConvertFromInternalPressure(totalFittingLoss),
                ex.ConvertFromInternalPressure(TotalPressureLoss));
        }

        public void UpdateView(AvfViewer viewer, List<XYZ> points, List<VectorAtPoint> valList, double maxFlow)
        {
            var coeff = viewer.Scale / 12.0;

            var maxX = -double.MaxValue;
            var maxY = -double.MaxValue;
            var maxZ = -double.MaxValue;

            foreach (var seg in m_segments)
            {
                // With the flow value being the scaled vector length, the fittings typically get much longer vector than its actual length.
                // As such, we skip the fitting flow display. 
                if (seg.SegmentType != MEPAnalyticalSegmentType.Segment)
                    continue;

                // Skip the zero flow segments.
                if (seg.Flow < 0.0000001)
                    continue;

                var p0 = seg.Start;
                var p1 = seg.End;

                maxX = Math.Max(Math.Max(maxX, p0.X), p1.X);
                maxY = Math.Max(Math.Max(maxY, p0.Y), p1.Y);
                maxZ = Math.Max(Math.Max(maxZ, p0.Z), p1.Z);

                points.Add(p1);

                var xyzList = new List<XYZ>();
                var vec = (p1 - p0) / coeff; // This is the exact segment length at the current view scale.

                // Convert the vector length to the flow value in scale.
                vec = vec.Normalize();
                vec *= seg.Flow / maxFlow;

                xyzList.Add(vec);

                valList.Add(new VectorAtPoint(xyzList));

                if (points.Count >= 1000) // 1000 is the limit on the number of points for one spatial field primitive
                    viewer.AddData(points, valList);

                // Only display the first segment in the section unless checked "Itemized", since the flow value and direction are the same in one section.
                if (!viewer.IsItemized)
                    break;
            }

            viewer.AddCorner(maxX, maxY, maxZ);
        }
    }
}
