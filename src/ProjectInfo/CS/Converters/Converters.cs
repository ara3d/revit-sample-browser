// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.ComponentModel;
using System.Globalization;

namespace Revit.SDK.Samples.ProjectInfo.CS
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
            if (destinationType == null) throw new ArgumentNullException("destinationType");
            if (destinationType == typeof(string))
            {
                if (value == null) return "(null)";

                // get its name
                var type = value.GetType();
                var mi = type.GetMethod("get_Name", new Type[0]);
                if (mi != null) return mi.Invoke(value, new object[0]).ToString();

                // if no name
                return "(...)";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
