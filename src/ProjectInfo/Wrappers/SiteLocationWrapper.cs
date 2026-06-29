// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Units;
namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    /// <summary>
    ///     Wrapper class for SiteLocation
    /// </summary>
    public class SiteLocationWrapper : IWrapper
    {
        /// <summary>
        ///     SiteLocation
        /// </summary>
        private readonly SiteLocation m_siteLocation;

        /// <summary>
        ///     Initializes private variables.
        /// </summary>
        /// <param name="siteLocation"></param>
        public SiteLocationWrapper(SiteLocation siteLocation)
        {
            m_siteLocation = siteLocation;
            //m_citys = RevitStartInfo.RevitApp.Cities;
        }

        /// <summary>
        ///     Gets or sets TimeZone
        /// </summary>
        [DisplayName("Time Zone")]
        [TypeConverter(typeof(TimeZoneConverter))]
        public string TimeZone => ValueFormatting.TimeZoneDoubleToString(m_siteLocation.TimeZone, RevitStartInfo.TimeZones);

        //set
        //{
        //    m_siteLocation.TimeZone = GetTimeZoneFromString(value);
        //}
        /// <summary>
        ///     Gets or sets Longitude
        /// </summary>
        [DisplayName("Longitude")]
        [TypeConverter(typeof(AngleConverter))]
        public double Longitude
        {
            get => m_siteLocation.Longitude;
            set => m_siteLocation.Longitude = value;
        }

        /// <summary>
        ///     Gets or sets Latitude
        /// </summary>
        [DisplayName("Latitude")]
        [TypeConverter(typeof(AngleConverter))]
        public double Latitude
        {
            get => m_siteLocation.Latitude;
            set => m_siteLocation.Latitude = value;
        }

        [DisplayName("City")]
        [TypeConverter(typeof(CityConverter))]
        public City City
        {
            get => GetCityFromPosition(Latitude, Longitude);
            set
            {
                m_siteLocation.Latitude = value.Latitude;
                m_siteLocation.Longitude = value.Longitude;
                m_siteLocation.TimeZone = value.TimeZone;
            }
        }

        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        [Browsable(false)]
        public object Handle => m_siteLocation;

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get => m_siteLocation.Name;
            set => m_siteLocation.Name = value;
        }

        private City GetCityFromPosition(double latitude, double longitude)
        {
            foreach (City city in RevitStartInfo.RevitApp.Cities)
            {
                if (XyzMath.DoubleEquals(city.Latitude, latitude) && XyzMath.DoubleEquals(city.Longitude, longitude))
                    return city;
            }

            return null;
        }
    }
}
