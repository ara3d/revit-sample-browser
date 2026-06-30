// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    public class TimeZoneConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Returns a collection of standard values from the default context for the
        ///     data type this type converter is designed for.
        /// </summary>
        /// <returns>Element collection retrieved through filtering current Revit document elements</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(RevitStartInfo.TimeZones);
        }
    }
}
