// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter
{
    /// <summary>
    ///     Type converter between RebarShapeParameter and string is provided for property grid.
    /// </summary>
    public class TypeConverterRebarShapeParameter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        ///     RebarShape parameters list.
        /// </summary>
        public static List<RebarShapeParameter> RebarShapeParameters;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        ///     Converts the given value object to the specified type, using the specified
        ///     context and culture information.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context.
        /// </param>
        /// <param name="culture">
        ///     A System.Globalization.CultureInfo. If null is passed,
        ///     the current culture is assumed.
        /// </param>
        /// <param name="value">The System.Object to convert.</param>
        /// <param name="destinationType">
        ///     The System.Type to convert the value parameter
        ///     to.
        /// </param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
            object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is RebarShapeParameter parameter)
            {
                if (null != parameter) return parameter.Name;
            }

            throw new Exception("Can't be converted to other types except string.");
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        ///     Converts the given object to the type of this converter, using the specified
        ///     context and culture information.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context.
        /// </param>
        /// <param name="culture">
        ///     The System.Globalization.CultureInfo to use as the
        ///     current culture.
        /// </param>
        /// <param name="value">The System.Object to convert.</param>
        /// <returns>An System.Object that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
            object value)
        {
            if (value is string)
                foreach (var param in
                         RebarShapeParameters)
                {
                    if (param.Name.Equals(value))
                        return param;
                }

            throw new Exception("Can't be converted from other types except from string.");
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(RebarShapeParameters);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
