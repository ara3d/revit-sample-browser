// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters
{
    public class CityConverter : TypeConverter
    {
        public const string UserDefined = "User Defined";

        public static readonly List<City> Cities;

        static CityConverter()
        {
            Cities = new List<City>();
            foreach (City city in RevitStartInfo.RevitApp.Cities)
            {
                Cities.Add(city);
            }
        }

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
            return new StandardValuesCollection(Cities);
        }

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

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var text = value as string;
            if (!string.IsNullOrEmpty(text))
                foreach (var city in Cities)
                {
                    if (city.Name == text)
                        return city;
                }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            if (destinationType == typeof(string))
            {
                if (value == null) return UserDefined;
                var city = value as City;
                return city?.Name;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
