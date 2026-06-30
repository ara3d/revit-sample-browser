using System;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static XYZ GetSunDirection(View view)
        {
            var doc = view.Document;

            var sunSettings
                = view.SunAndShadowSettings;

            var initialDirection = XYZ.BasisY;

            var altitude = sunSettings.GetFrameAltitude(
                sunSettings.ActiveFrame);

            var altitudeRotation = Transform
                .CreateRotation(XYZ.BasisX, altitude);

            var altitudeDirection = altitudeRotation
                .OfVector(initialDirection);

            var azimuth = sunSettings.GetFrameAzimuth(
                sunSettings.ActiveFrame);

            var projectInfoElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ProjectBasePoint)
                    .FirstElement();

            var bipAtn
                = BuiltInParameter.BASEPOINT_ANGLETON_PARAM;

            var patn = projectInfoElement.get_Parameter(
                bipAtn);

            var trueNorthAngle = patn.AsDouble();

            var actualAzimuth = 2 * Math.PI - azimuth + trueNorthAngle; // adjust for project true north

            var azimuthRotation = Transform
                .CreateRotation(XYZ.BasisZ, actualAzimuth);

            var sunDirection = azimuthRotation.OfVector(
                altitudeDirection);

            return -sunDirection;
        }

        internal static void SetSiteLocationToCity(Document doc)
        {
            var cities = doc.Application.Cities;
            try
            {
                var item = cities.ForwardIterator();
                while (item != null)
                {
                    item.MoveNext();
                    var city = item.Current as City;
                    if (city.Name.Contains("中国") ||
                        city.Name.Contains("China"))
                    {
                        var transaction = new Transaction(doc, "Create Wall");
                        transaction.Start();

                        var projectLocation = doc.ActiveProjectLocation;
                        var site = projectLocation.GetSiteLocation();

                        site.Latitude = city.Latitude;
                        site.Longitude = city.Longitude;
                        site.TimeZone = city.TimeZone;
                        transaction.Commit();
                        break;
                    }
                }
            }
            catch (Exception ret)
            {
                Debug.Print(ret.Message);
            }
        }

        internal static void SetSiteLocationToCity2(Document doc)
        {
            var cities = doc.Application.Cities;

            foreach (City city in cities)
            {
                var s = city.Name;

                if (s.Contains("中国") || s.Contains("China"))
                {
                    using var t = new Transaction(doc);
                    t.Start("Set Site Location to City");

                    var projectLocation = doc.ActiveProjectLocation;
                    var site = projectLocation.GetSiteLocation();

                    site.Latitude = city.Latitude;
                    site.Longitude = city.Longitude;
                    site.TimeZone = city.TimeZone;

                    t.Commit();

                    break;
                }
            }
        }
    }
}
