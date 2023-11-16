// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Ara3D.RevitSampleBrowser.NewRebar.CS.Parameters;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter
{
    /// <summary>
    ///     Type converter between RebarShapeParameter and string is provided for property grid.
    /// </summary>
    internal class TypeConverterRebarShapeParameter : System.ComponentModel.TypeConverter
    {
        /// <summary>
        ///     RebarShape parameters list.
        /// </summary>
        public static List<RebarShapeParameter> RebarShapeParameters;

        /// <summary>
        ///     Returns whether this converter can convert the object to the specified type.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context.
        /// </param>
        /// <param name="destinationType">
        ///     A System.Type that represents the type you want
        ///     to convert to.
        /// </param>
        /// <returns></returns>
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

        /// <summary>
        ///     Returns whether this converter can convert an object of the given type to
        ///     the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context.
        /// </param>
        /// <param name="sourceType">
        ///     A System.Type that represents the type you want to
        ///     convert from.
        /// </param>
        /// <returns>
        ///     true if this converter can perform the conversion; otherwise,
        ///     false.
        /// </returns>
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
                    if (param.Name.Equals(value))
                        return param;
            throw new Exception("Can't be converted from other types except from string.");
        }

        /// <summary>
        ///     Returns whether this object supports a standard set of values that can be
        ///     picked from a list.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        ///     true if System.ComponentModel.TypeConverter.GetStandardValues() should be
        ///     called to find a common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Returns a collection of standard values for the data type this type converter
        ///     is designed for when provided with a format context.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context
        ///     that can be used to extract additional information about the environment
        ///     from which this converter is invoked. This parameter or properties of this
        ///     parameter can be null.
        /// </param>
        /// <returns>
        ///     A System.ComponentModel.TypeConverter.StandardValuesCollection that holds
        ///     a standard set of valid values, or null if the data type does not support
        ///     a standard set of values.
        /// </returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(RebarShapeParameters);
        }

        /// <summary>
        ///     Returns whether the collection of standard values returned from
        ///     System.ComponentModel.TypeConverter.GetStandardValues()
        ///     is an exclusive list of possible values, using the specified context.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext that
        ///     provides a format context.
        /// </param>
        /// <returns>
        ///     true if the System.ComponentModel.TypeConverter.StandardValuesCollection
        ///     returned from System.ComponentModel.TypeConverter.GetStandardValues() is
        ///     an exhaustive list of possible values; false if other values are possible.
        /// </returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
