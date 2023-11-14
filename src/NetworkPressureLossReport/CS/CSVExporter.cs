using System;
using System.IO;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.NetworkPressureLossReport
{
   public class CSVExporter : IDisposable
   {
      private Document m_doc;
      private bool? m_isItemized;
      private ConnectorDomainType m_connType;
      private StreamWriter m_streamWriter;

      public StreamWriter Writer => m_streamWriter;

      public bool? IsItemized => m_isItemized;

      public Document Document
      {
         set => m_doc = value;
      }
      public ConnectorDomainType DomainType
      {
         set => m_connType = value;
      }
      public CSVExporter(string csvFilePath, bool? isItemized)
      {
         if(!string.IsNullOrEmpty(csvFilePath))
         {
            m_streamWriter = new StreamWriter(csvFilePath, false, System.Text.Encoding.Unicode);
         }
         m_isItemized = isItemized;
      }
      ~CSVExporter() => Dispose(false);
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }
      protected virtual void Dispose(bool disposing)
      {
         if(m_streamWriter != null)
         {
            // Dispose/Close the stream writer.
            m_streamWriter.Dispose();     
            m_streamWriter = null;
         }
      }
      private string SafeGetUnitLabel(FormatOptions opt)
      {
         string msg = null;
         string unitLabel = null;
         try
         {
            unitLabel = LabelUtils.GetLabelForSymbol(opt.GetSymbolTypeId());
         }
         catch (Autodesk.Revit.Exceptions.ArgumentException ex)
         {
            // The unit symbol is None.
            msg = ex.Message;
         }
         if(string.IsNullOrEmpty(unitLabel))
         {
            try
            {
               unitLabel = LabelUtils.GetLabelForUnit(opt.GetUnitTypeId());
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
               // The unit symbol is None.
               msg = ex.Message;
            }
         }
         return unitLabel;
      }
      private ForgeTypeId GetFlowTypeId()
      {
         ForgeTypeId typeId = null;
         if (m_connType == ConnectorDomainType.Piping)
            typeId = SpecTypeId.Flow;
         else if (m_connType == ConnectorDomainType.Hvac)
            typeId = SpecTypeId.AirFlow;
         return typeId;
      }
      public double ConvertFromInternalFlow(double flow)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetFlowTypeId());
         return UnitUtils.ConvertFromInternalUnits(flow, formatOption.GetUnitTypeId());
      }
      public string GetFlowUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetFlowTypeId());
         return SafeGetUnitLabel(formatOption);
      }
      private ForgeTypeId GetSizeTypeId()
      {
         ForgeTypeId typeId = null;
         if (m_connType == ConnectorDomainType.Piping)
            typeId = SpecTypeId.PipeSize;
         else if (m_connType == ConnectorDomainType.Hvac)
            typeId = SpecTypeId.DuctSize;
         return typeId;
      }
      public double ConvertFromInternalSize(double value)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetSizeTypeId());
         return UnitUtils.ConvertFromInternalUnits(value, formatOption.GetUnitTypeId());
      }
      public string GetSizeUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetSizeTypeId());
         return SafeGetUnitLabel(formatOption);
      }
      private ForgeTypeId GetVelocityTypeId()
      {
         ForgeTypeId typeId = null;
         if (m_connType == ConnectorDomainType.Piping)
            typeId = SpecTypeId.PipingVelocity;
         else if (m_connType == ConnectorDomainType.Hvac)
            typeId = SpecTypeId.HvacVelocity;
         return typeId;
      }
      public double ConvertFromInternalVelocity(double value)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetVelocityTypeId());
         return UnitUtils.ConvertFromInternalUnits(value, formatOption.GetUnitTypeId());
      }
      public string GetVelocityUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetVelocityTypeId());
         return SafeGetUnitLabel(formatOption);
      }
      private ForgeTypeId GetPressureTypeId()
      {
         ForgeTypeId typeId = null;
         if (m_connType == ConnectorDomainType.Piping)
            typeId = SpecTypeId.PipingPressure;
         else if (m_connType == ConnectorDomainType.Hvac)
            typeId = SpecTypeId.HvacPressure;
         return typeId;
      }
      public double ConvertFromInternalPressure(double value)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetPressureTypeId());
         return UnitUtils.ConvertFromInternalUnits(value, formatOption.GetUnitTypeId());
      }
      public string GetPressureUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetPressureTypeId());
         return SafeGetUnitLabel(formatOption);
      }
      private ForgeTypeId GetFrictionTypeId()
      {
         ForgeTypeId typeId = null;
         if (m_connType == ConnectorDomainType.Piping)
            typeId = SpecTypeId.PipingFriction;
         else if (m_connType == ConnectorDomainType.Hvac)
            typeId = SpecTypeId.HvacFriction;
         return typeId;
      }
      public double ConvertFromInternalFriction(double value)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetFrictionTypeId());
         return UnitUtils.ConvertFromInternalUnits(value, formatOption.GetUnitTypeId());
      }
      public string GetFrictionUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(GetFrictionTypeId());
         return SafeGetUnitLabel(formatOption);
      }
      public double ConvertFromInternalLength(double value)
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
         return UnitUtils.ConvertFromInternalUnits(value, formatOption.GetUnitTypeId());
      }
      public string GetLengthUnitSymbol()
      {
         var formatOption = m_doc.GetUnits().GetFormatOptions(SpecTypeId.Length);
         return SafeGetUnitLabel(formatOption);
      }
   }
}
