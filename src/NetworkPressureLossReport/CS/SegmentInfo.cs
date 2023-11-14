using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace Revit.SDK.Samples.NetworkPressureLossReport
{
   internal class SegmentInfo
   {
      private MEPNetworkSegmentId m_id;
      const double Tolerance = 0.0000001;

      public double Flow { get; }

      public double PressureDrop { get; }

      public MEPAnalyticalSegmentType SegmentType { get; }

      public bool IsCriticalPath { get; }

      public string Id => m_id.ElementId.ToString() + @"_" + m_id.SegmentId.ToString();

      public ElementId RevitElementId => m_id.ElementId;

      public double Length { get; }

      public double Size { get; }

      public double Velocity { get; }

      public double VelocityPressure { get; }

      public double Coefficients { get; }

      public double Friction => Length < Tolerance ? 0.0 : PressureDrop / Length;

      public double ReynoldsNumber { get; }

      public XYZ Start { get; }

      public XYZ End { get; }

      public SegmentInfo(Document doc, MEPAnalyticalSegment seg, MEPNetworkSegmentData data)
      {
         m_id = new MEPNetworkSegmentId(seg.RevitElementId, seg.Id);

         // Be aware that the flow and pressure may be negative.
         // It means the flow is from the end node to the start node.
         SegmentType = seg.SegmentType;
         Size = seg.InnerDiameter;        // Hydraulic diameter for rectangular or oval profile.
         Flow = Math.Abs(data.Flow);
         PressureDrop = data.Flow > 0 ? data.PressureDrop : -1 * data.PressureDrop;

         Velocity = Math.Abs(data.Velocity);
         VelocityPressure = Math.Abs(data.VelocityPressure);
         Coefficients = data.Coefficient;
         IsCriticalPath = data.IsCriticalPath;
         ReynoldsNumber = data.ReynoldsNumber;

         Length = 0.0;
         var thisElem = doc.GetElement(seg.RevitElementId);
         if (thisElem != null)
         {
            var thisModel = MEPAnalyticalModelData.GetMEPAnalyticalModelData(thisElem);
            var start = thisModel.GetNodeById(seg.StartNode);
            var end = thisModel.GetNodeById(seg.EndNode);
            if (start != null && end != null)
            {
               Start = data.Flow > 0 ? start.Location : end.Location;
               End = data.Flow > 0 ? end.Location : start.Location;
               Length = Start.DistanceTo(End);
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
