// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    /// <summary>
    ///     Converter for Enumeration types of RevitAPI
    /// </summary>
    public abstract class RevitEnumConverter : EnumConverter
    {
        private readonly Dictionary<object, string> m_map;

        public RevitEnumConverter(Type type)
            : base(type)
        {
            m_map = EnumMap;
        }

        protected abstract Dictionary<object, string> EnumMap { get; }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(m_map.Keys);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var enumValue = value;
            var valueText = value.ToString();
            foreach (var pair in m_map)
            {
                if (pair.Value == valueText)
                    enumValue = pair.Key.ToString();
            }

            return base.ConvertFrom(context, culture, enumValue);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            var enumValue = base.ConvertTo(context, culture, value, destinationType);
            var enumObject = Enum.Parse(EnumType, enumValue.ToString());
            return m_map[enumObject];
        }
    }

    public class BuildingTypeConverter : RevitEnumConverter
    {
        public BuildingTypeConverter(Type type) : base(type)
        {
        }

        protected override Dictionary<object, string> EnumMap => RevitStartInfo.BuildingTypeMap;
    }

    public class ExportComplexityConverter : RevitEnumConverter
    {
        public ExportComplexityConverter(Type type) : base(type)
        {
        }

        protected override Dictionary<object, string> EnumMap => RevitStartInfo.ExportComplexityMap;
    }

    public class ServiceTypeConverter : RevitEnumConverter
    {
        public ServiceTypeConverter(Type type) : base(type)
        {
        }

        protected override Dictionary<object, string> EnumMap => RevitStartInfo.ServiceTypeMap;
    }

    public class HvacLoadLoadsReportTypeConverter : RevitEnumConverter
    {
        public HvacLoadLoadsReportTypeConverter(Type type) : base(type)
        {
        }

        protected override Dictionary<object, string> EnumMap => RevitStartInfo.HvacLoadLoadsReportTypeMap;
    }

    public class HvacLoadConstructionClassConverter : RevitEnumConverter
    {
        public HvacLoadConstructionClassConverter(Type type) : base(type)
        {
        }

        protected override Dictionary<object, string> EnumMap => RevitStartInfo.HvacLoadConstructionClassMap;
    }
}
