using System;
using System.IO;
using System.Text;
using Autodesk.Revit.DB;
using ArgumentException = Autodesk.Revit.Exceptions.ArgumentException;

namespace Revit.SDK.Samples.NetworkPressureLossReport
{
    public class CSVExporter : IDisposable
    {
        private ConnectorDomainType m_connType;
        private Document m_doc;

        public CSVExporter(string csvFilePath, bool? isItemized)
        {
            if (!string.IsNullOrEmpty(csvFilePath)) Writer = new StreamWriter(csvFilePath, false, Encoding.Unicode);
            IsItemized = isItemized;
        }

        public StreamWriter Writer { get; private set; }

        public bool? IsItemized { get; }

        public Document Document
        {
            set => m_doc = value;
        }

        public ConnectorDomainType DomainType
        {
            set => m_connType = value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CSVExporter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Writer != null)
            {
                // Dispose/Close the stream writer.
                Writer.Dispose();
                Writer = null;
            }
        }

        private string SafeGetUnitLabel(FormatOptions opt)
        {
            string unitLabel = null;
            try
            {
                unitLabel = LabelUtils.GetLabelForSymbol(opt.GetSymbolTypeId());
            }
            catch (ArgumentException ex)
            {
                // The unit symbol is None.
            }

            if (string.IsNullOrEmpty(unitLabel))
                try
                {
                    unitLabel = LabelUtils.GetLabelForUnit(opt.GetUnitTypeId());
                }
                catch (ArgumentException ex)
                {
                    // The unit symbol is None.
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