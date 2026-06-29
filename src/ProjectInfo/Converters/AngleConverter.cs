// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Globalization;
using Ara3D.RevitSampleBrowser.Common.Units;
namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    /// <summary>
    ///     Converts angle with string
    /// </summary>
    public class AngleConverter : TypeConverter
    {
        /// <summary>
        ///     Converts from string.
        /// </summary>
        /// <returns>Converted string</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <returns>Converted string</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Converts from string.
        /// </summary>
        /// <param name="context">
        ///     An System.ComponentModel.ITypeDescriptorContext
        ///     that provides a format context.
        /// </param>
        /// <param name="culture">
        ///     An optional System.Globalization.CultureInfo.
        ///     If not supplied, the current culture is assumed.
        /// </param>
        /// <param name="value">string to be converted to an element</param>
        /// <returns>An element if the element exists, otherwise null</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            return !string.IsNullOrEmpty(text) ? ValueFormatting.AngleStringToDouble(text) : base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Converts to string.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context. </param>
        /// <param name="culture">A CultureInfo. If null is passed, the current culture is assumed. </param>
        /// <param name="value">The Object to convert. </param>
        /// <param name="destinationType">The Type to convert the value parameter to. </param>
        /// <returns>Converted string</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                if (value == null) return string.Empty;
                return ValueFormatting.DoubleToAngleString((double)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
