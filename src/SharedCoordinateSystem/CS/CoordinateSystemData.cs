// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">the ExternalCommandData reference</param>
        public CoordinateSystemData(ExternalCommandData commandData)
        {
            m_command = commandData;
            m_application = m_command.Application;
        }

        /// <summary>
        ///     the value of the angle form true north
        /// </summary>
        public double AngleOffset { get; private set; }

        /// <summary>
        ///     return the East to West offset
        /// </summary>
        public double EastWestOffset { get; private set; }

        /// <summary>
        ///     return the North to South offset
        /// </summary>
        public double NorthSouthOffset { get; private set; }

        /// <summary>
        ///     return the Elevation above ground level
        /// </summary>
        public double PositionElevation { get; private set; }

        /// <summary>
        ///     get and set the current project location name of the project
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        ///     get all the project locations' name of the project
        /// </summary>
        public List<string> LocationNames { get; } = new List<string>();

        /// <summary>
        ///     get the shared coordinate system data of the project
        /// </summary>
        public void GatData()
        {
            GetLocationData();
        }

        /// <summary>
        ///     get the information of all the project locations associated with this project
        /// </summary>
        public void GetLocationData()
        {
            LocationNames.Clear();
            var currentLocation = m_application.ActiveUIDocument.Document.ActiveProjectLocation;
            //get the current location name
            LocationName = currentLocation.Name;
            //Retrieve all the project locations associated with this project
            var locations = m_application.ActiveUIDocument.Document.ProjectLocations;

            var iter = locations.ForwardIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var locationTransform = iter.Current as ProjectLocation;
                var transformName = locationTransform.Name;
                LocationNames.Add(transformName); //add the location's name to the list
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
                    projectLocation.Name + " (current)" == locationName)
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
                //set it to the current projecte location 
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
                    projectLocation.Name + " (current)" == locationName)
                {
                    var origin = new XYZ(0, 0, 0);
                    //get the project position
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
                    location.Name + " (current)" == locationName)
                {
                    //get the project position
                    var origin = new XYZ(0, 0, 0);
                    var projectPosition = location.GetProjectPosition(origin);
                    //change the offset value of the project position
                    projectPosition.Angle = newAngle * Modulus; //convert the unit 
                    projectPosition.EastWest = newEast;
                    projectPosition.NorthSouth = newNorth;
                    projectPosition.Elevation = newElevation;
                    //set the value of the project position
                    location.SetProjectPosition(origin, projectPosition);
                }
            }
        }

        /// <summary>
        ///     change the Precision of the value
        /// </summary>
        private void ChangePrecision()
        {
            AngleOffset = UnitConversion.DealPrecision(AngleOffset, Precision);
            EastWestOffset = UnitConversion.DealPrecision(EastWestOffset, Precision);
            NorthSouthOffset = UnitConversion.DealPrecision(NorthSouthOffset, Precision);
            PositionElevation = UnitConversion.DealPrecision(PositionElevation, Precision);
        }
    }
}
