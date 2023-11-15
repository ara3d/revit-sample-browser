// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Globalization;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    /// <summary>
    ///     Type converter for wrapper classes
    /// </summary>
    public class WrapperConverter : ExpandableObjectConverter
    {
        /// <summary>
        ///     Can be converted to string
        /// </summary>
        /// <returns>true if destinationType is string, otherwise false</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType.Equals(typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Converts to string. If value is null, convert it to "(null)".
        ///     if value has a "Name" property, returns its name. otherwise, returns "(...)".
        /// </summary>
        /// <returns>Converted string</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                if (value == null) return "(null)";

                // get its name
                var type = value.GetType();
                var mi = type.GetMethod("get_Name", Type.EmptyTypes);
                return mi != null ? mi.Invoke(value, Array.Empty<object>()).ToString() :
                    // if no name
                    "(...)";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
