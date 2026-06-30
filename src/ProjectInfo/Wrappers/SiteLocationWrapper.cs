// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Units;
using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters;
using Autodesk.Revit.DB;
using System.ComponentModel;
namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class SiteLocationWrapper : IWrapper
    {
        private readonly SiteLocation m_siteLocation;

        public SiteLocationWrapper(SiteLocation siteLocation)
        {
            m_siteLocation = siteLocation;
            //m_citys = RevitStartInfo.RevitApp.Cities;
        }

        [DisplayName("Time Zone")]
        [TypeConverter(typeof(TimeZoneConverter))]
        public string TimeZone => ValueFormatting.TimeZoneDoubleToString(m_siteLocation.TimeZone, RevitStartInfo.TimeZones);

        //set
        //{
        //    m_siteLocation.TimeZone = GetTimeZoneFromString(value);
        //}
        [DisplayName("Longitude")]
        [TypeConverter(typeof(AngleConverter))]
        public double Longitude
        {
            get => m_siteLocation.Longitude;
            set => m_siteLocation.Longitude = value;
        }

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

        [Browsable(false)]
        public object Handle => m_siteLocation;

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
