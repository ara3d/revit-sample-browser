// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS
{
    /// <summary>
    ///     this class is used to get, set and manage information about Location
    /// </summary>
    public class CoordinateSystemData
    {
        private const double Modulus = 0.0174532925199433; //a modulus for degree convert to pi 
        private const int Precision = 3; //default precision 
        private readonly UIApplication m_application; //the revit application reference
        private readonly ExternalCommandData m_command; // the ExternalCommandData reference

        public CoordinateSystemData(ExternalCommandData commandData)
        {
            m_command = commandData;
            m_application = m_command.Application;
        }

        public double AngleOffset { get; private set; }

        /// <summary>
        ///     return the East to West offset
        /// </summary>
        public double EastWestOffset { get; private set; }

        /// <summary>
        ///     return the North to South offset
        /// </summary>
        public double NorthSouthOffset { get; private set; }

        public double PositionElevation { get; private set; }

        public string LocationName { get; set; }

        public List<string> LocationNames { get; } = [];

        public void GatData()
        {
            GetLocationData();
        }

        public void GetLocationData()
        {
            LocationNames.Clear();
            var currentLocation = m_application.ActiveUIDocument.Document.ActiveProjectLocation;
            LocationName = currentLocation.Name;
            //Retrieve all the project locations associated with this project
            var locations = m_application.ActiveUIDocument.Document.ProjectLocations;

            var iter = locations.ForwardIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var locationTransform = iter.Current as ProjectLocation;
                var transformName = locationTransform.Name;
                LocationNames.Add(transformName);
            }
        }

        /// <summary>
        ///     duplicate a new project location
        /// </summary>
        /// <param name="locationName">old location name</param>
        /// <param name="newLocationName">new location name</param>
        public void DuplicateLocation(string locationName, string newLocationName)
        {
            var locationSet = m_application.ActiveUIDocument.Document.ProjectLocations;
            foreach (ProjectLocation projectLocation in locationSet)
            {
                if (projectLocation.Name == locationName ||
                    $"{projectLocation.Name} (current)" == locationName)
                {
                    //duplicate a new project location
                    projectLocation.Duplicate(newLocationName);
                    break;
                }
            }
        }

        /// <summary>
        ///     change the current project location
        /// </summary>
        /// <param name="locationName"></param>
        public void ChangeCurrentLocation(string locationName)
        {
            var locations = m_application.ActiveUIDocument.Document.ProjectLocations;
            foreach (ProjectLocation projectLocation in locations)
            //find the project location which is selected by user and
            {
                if (projectLocation.Name == locationName)
                {
                    m_application.ActiveUIDocument.Document.ActiveProjectLocation = projectLocation;
                    LocationName = locationName;
                    break;
                }
            }
        }

        /// <summary>
        ///     get the offset values of the project position
        /// </summary>
        /// <param name="locationName"></param>
        public void GetOffset(string locationName)
        {
            var locationSet = m_application.ActiveUIDocument.Document.ProjectLocations;
            foreach (ProjectLocation projectLocation in locationSet)
            {
                if (projectLocation.Name == locationName ||
                    $"{projectLocation.Name} (current)" == locationName)
                {
                    XYZ origin = new(0, 0, 0);
                    var pp = projectLocation.GetProjectPosition(origin);
                    AngleOffset = pp.Angle /= Modulus; //convert to unit degree  
                    EastWestOffset = pp.EastWest; //East to West offset
                    NorthSouthOffset = pp.NorthSouth; //north to south offset
                    PositionElevation = pp.Elevation; //Elevation above ground level
                    break;
                }
            }

            ChangePrecision();
        }

        /// <summary>
        ///     change the offset value for the project position
        /// </summary>
        /// <param name="locationName">location name</param>
        /// <param name="newAngle">angle from true north</param>
        /// <param name="newEast">East to West offset</param>
        /// <param name="newNorth">north to south offset</param>
        /// <param name="newElevation">Elevation above ground level</param>
        public void EditPosition(string locationName, double newAngle, double newEast,
            double newNorth, double newElevation)
        {
            var locationSet = m_application.ActiveUIDocument.Document.ProjectLocations;
            foreach (ProjectLocation location in locationSet)
            {
                if (location.Name == locationName ||
                    $"{location.Name} (current)" == locationName)
                {
                    XYZ origin = new(0, 0, 0);
                    var projectPosition = location.GetProjectPosition(origin);
                    //change the offset value of the project position
                    projectPosition.Angle = newAngle * Modulus; //convert the unit 
                    projectPosition.EastWest = newEast;
                    projectPosition.NorthSouth = newNorth;
                    projectPosition.Elevation = newElevation;
                    location.SetProjectPosition(origin, projectPosition);
                }
            }
        }

        private void ChangePrecision()
        {
            AngleOffset = SampleBrowserUtils.DealPrecision(AngleOffset, Precision);
            EastWestOffset = SampleBrowserUtils.DealPrecision(EastWestOffset, Precision);
            NorthSouthOffset = SampleBrowserUtils.DealPrecision(NorthSouthOffset, Precision);
            PositionElevation = SampleBrowserUtils.DealPrecision(PositionElevation, Precision);
        }
    }
}
