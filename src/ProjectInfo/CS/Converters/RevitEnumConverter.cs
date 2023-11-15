// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    ///     Converter for Enumeration types of RevitAPI
    /// </summary>
    public abstract class RevitEnumConverter : EnumConverter
    {
        /// <summary>
        ///     Dictionary contains enum and string map
        /// </summary>
        private readonly Dictionary<object, string> m_map;

        /// <summary>
        ///     Initialize private variables
        /// </summary>
        /// <param name="type">Enumeration type</param>
        public RevitEnumConverter(Type type)
            : base(type)
        {
            m_map = EnumMap;
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected abstract Dictionary<object, string> EnumMap { get; }

        /// <summary>
        ///     Returns a collection of standard values from the default context for the
        ///     data type this type converter is designed for.
        /// </summary>
        /// <returns>All enum items</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(m_map.Keys);
        }

        /// <summary>
        ///     Returns an enum item from a string
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext
        ///     that provides a format context.
        /// </param>
        /// <param name="culture">
        ///     An optional System.Globalization.CultureInfo.
        ///     If not supplied, the current culture is assumed.
        /// </param>
        /// <param name="value">string to be converted to</param>
        /// <returns>An enum item</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var enumValue = value;
            var valueText = value.ToString();
            foreach (var pair in m_map)
                if (pair.Value == valueText)
                    enumValue = pair.Key.ToString();
            return base.ConvertFrom(context, culture, enumValue);
        }

        /// <summary>
        ///     Convert enum item to string
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context. </param>
        /// <param name="culture">A CultureInfo. If null is passed, the current culture is assumed. </param>
        /// <param name="value">The Object to convert. </param>
        /// <param name="destinationType">The Type to convert the value parameter to. </param>
        /// <returns>Corresponding string related with the enum item</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            var enumValue = base.ConvertTo(context, culture, value, destinationType);
            var enumObject = Enum.Parse(EnumType, enumValue.ToString());
            return m_map[enumObject];
        }
    }

    /// <summary>
    ///     Converter for BuildingType
    /// </summary>
    public class BuildingTypeConverter : RevitEnumConverter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">enumeration type</param>
        public BuildingTypeConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected override Dictionary<object, string> EnumMap => RevitStartInfo.BuildingTypeMap;
    }

    /// <summary>
    ///     Converter for ExportComplexityConverter
    /// </summary>
    public class ExportComplexityConverter : RevitEnumConverter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">enumeration type</param>
        public ExportComplexityConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected override Dictionary<object, string> EnumMap => RevitStartInfo.ExportComplexityMap;
    }

    /// <summary>
    ///     Converter for ServiceType
    /// </summary>
    public class ServiceTypeConverter : RevitEnumConverter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">enumeration type</param>
        public ServiceTypeConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected override Dictionary<object, string> EnumMap => RevitStartInfo.ServiceTypeMap;
    }

    /// <summary>
    ///     Converter for HVACLoadLoadsReportType
    /// </summary>
    public class HvacLoadLoadsReportTypeConverter : RevitEnumConverter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">enumeration type</param>
        public HvacLoadLoadsReportTypeConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected override Dictionary<object, string> EnumMap => RevitStartInfo.HvacLoadLoadsReportTypeMap;
    }

    /// <summary>
    ///     Converter for HVACLoadConstructionClass
    /// </summary>
    public class HvacLoadConstructionClassConverter : RevitEnumConverter
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="type">enumeration type</param>
        public HvacLoadConstructionClassConverter(Type type) : base(type)
        {
        }

        /// <summary>
        ///     Gets the enum-string map
        /// </summary>
        protected override Dictionary<object, string> EnumMap => RevitStartInfo.HvacLoadConstructionClassMap;
    }
}
