// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    public class CreationDataTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            return value is CreationData cd
                ? $"Total {cd.EdgesForHostedSweep.Count} Edges"
                : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
