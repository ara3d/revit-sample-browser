﻿using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Revit.SDK.Samples.NetworkPressureLossReport
{
   internal class SegmentInfo
   {
      private MEPNetworkSegmentId m_id;
      private MEPAnalyticalSegmentType m_segmentType;
      private bool m_isCriticalPath;
      private double m_length;
      private double m_size;
      private double m_flow;
      private double m_velocity;
      private double m_velocityPressure;
      private double m_coefficients;
      private double m_pressureDrop;
      private double m_reynolds;
      private XYZ m_startPt;
      private XYZ m_endPt;
      const double Tolerance = 0.0000001;

      public double Flow => m_flow;

      public double PressureDrop => m_pressureDrop;

      public MEPAnalyticalSegmentType SegmentType => m_segmentType;

      public bool IsCriticalPath => m_isCriticalPath;

      public string Id => m_id.ElementId.ToString() + @"_" + m_id.SegmentId.ToString();

      public ElementId RevitElementId => m_id.ElementId;

      public double Length => m_length;

      public double Size => m_size;

      public double Velocity => m_velocity;

      public double VelocityPressure => m_velocityPressure;

      public double Coefficients => m_coefficients;

      public double Friction => m_length < Tolerance ? 0.0 : m_pressureDrop / m_length;

      public double ReynoldsNumber => m_reynolds;

      public XYZ Start => m_startPt;

      public XYZ End => m_endPt;

      public SegmentInfo(Document doc, MEPAnalyticalSegment seg, MEPNetworkSegmentData data)
      {
         m_id = new MEPNetworkSegmentId(seg.RevitElementId, seg.Id);

         // Be aware that the flow and pressure may be negative.
         // It means the flow is from the end node to the start node.
         m_segmentType = seg.SegmentType;
         m_size = seg.InnerDiameter;        // Hydraulic diameter for rectangular or oval profile.
         m_flow = Math.Abs(data.Flow);
         m_pressureDrop = data.Flow > 0 ? data.PressureDrop : -1 * data.PressureDrop;

         m_velocity = Math.Abs(data.Velocity);
         m_velocityPressure = Math.Abs(data.VelocityPressure);
         m_coefficients = data.Coefficient;
         m_isCriticalPath = data.IsCriticalPath;
         m_reynolds = data.ReynoldsNumber;

         m_length = 0.0;
         var thisElem = doc.GetElement(seg.RevitElementId);
         if (thisElem != null)
         {
            var thisModel = MEPAnalyticalModelData.GetMEPAnalyticalModelData(thisElem);
            var start = thisModel.GetNodeById(seg.StartNode);
            var end = thisModel.GetNodeById(seg.EndNode);
            if (start != null && end != null)
            {
               m_startPt = data.Flow > 0 ? start.Location : end.Location;
               m_endPt = data.Flow > 0 ? end.Location : start.Location;
               m_length = m_startPt.DistanceTo(m_endPt);
            }
         }
      }
   }

   internal class CompareNetworkSegmentId : IEqualityComparer<MEPNetworkSegmentId>
   {
      public bool Equals(MEPNetworkSegmentId left, MEPNetworkSegmentId right)
      {
         return left.ElementId == right.ElementId
             && left.SegmentId == right.SegmentId;
      }

      public int GetHashCode(MEPNetworkSegmentId idSeg)
      {
         // A simple way to combine the element id and segment id into one hash code.
         var hash = 17;
         hash = hash * 31 + idSeg.ElementId.GetHashCode();
         hash = hash * 31 + idSeg.SegmentId.GetHashCode();
         return hash;
      }
   }
}
