// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Globalization;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    /// <summary>
    ///     This class is intent to convert CreationData to String.
    /// </summary>
    public class CreationDataTypeConverter : TypeConverter
    {
        /// <summary>
        ///     CreationData can convert to string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        ///     Convert CreationData to string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (value is CreationData cd) return $"Total {cd.EdgesForHostedSweep.Count} Edges";
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
