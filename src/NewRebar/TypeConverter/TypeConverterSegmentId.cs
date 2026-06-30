// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.NewRebar.CS.TypeConverter
{
    /// <summary>
    ///     Type converter between int and string is provided for property grid.
    /// </summary>
    public class TypeConverterSegmentId : Int32Converter
    {
        /// <summary>
        ///     Segment count.
        /// </summary>
        public static int SegmentCount = 1;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var segments = new int[SegmentCount];
            for (var i = 0; i < SegmentCount; i++) segments[i] = i;
            return new StandardValuesCollection(segments);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
