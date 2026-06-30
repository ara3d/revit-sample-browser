// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.Journaling.CS
{
    public class Journaling
    {
        private readonly bool m_canReadData; // Indicate whether has journal data

        // Private members
        private readonly ExternalCommandData m_commandData; // Store the reference of command data
        private Level m_createlevel; // Store the level which the created wall on
        private WallType m_createType; // Store the type of the created wall
        private XYZ m_endPoint; // Store the end point of the created wall

        private readonly List<Level> m_levelList; // Store all levels in revit

        private XYZ m_startPoint; // Store the start point of the created wall
        private List<WallType> m_wallTypeList; // Store all wall types in revit

        // Methods
        public Journaling(ExternalCommandData commandData)
        {
            // Initialize the data members
            m_commandData = commandData;
            m_canReadData = commandData.JournalData.Count > 0;

            // Initialize the two list data members
            m_levelList = new List<Level>();
            m_wallTypeList = new List<WallType>();
            InitializeListData();
        }

        // Properties
        /// <summary>
        ///     Give all levels in revit, and this information can be showed in UI
        /// </summary>
        public ReadOnlyCollection<Level> Levels => new ReadOnlyCollection<Level>(m_levelList);

        /// <summary>
        ///     Give all wall types in revit, and this information can be showed in UI
        /// </summary>
        public ReadOnlyCollection<WallType> WallTypes => new ReadOnlyCollection<WallType>(m_wallTypeList);

        /// <summary>
        ///     This is the main deal method in this sample.
        ///     It invoke methods to read and write journal data and create a wall using these data
        /// </summary>
        public void Run()
        {
            // According to it has journal data or not, this sample create wall in two ways
            if (m_canReadData) // if it has journal data
            {
                ReadJournalData(); // read the journal data
                CreateWall(); // create a wall using the data
            }
            else // if it doesn't have journal data
            {
                if (!DisplayUi()) // display a form to collect some necessary data
                    return; // if the user cancels the form, only return

                CreateWall(); // create a wall using the collected data
                WriteJournalData(); // write the journal data
            }
        }

        public void SetNecessaryData(XYZ startPoint, XYZ endPoint, Level level, WallType type)
        {
            m_startPoint = startPoint; // start point
            m_endPoint = endPoint; // end point
            m_createlevel = level; // the level information
            m_createType = type; // the wall type
        }

        private void InitializeListData()
        {
            // Assert the lists have been constructed
            if (null == m_wallTypeList || null == m_levelList)
                throw new Exception("necessary data members don't initialize.");

            // Get all wall types from revit
            var document = m_commandData.Application.ActiveUIDocument.Document;
            var filteredElementCollector = new FilteredElementCollector(document);
            filteredElementCollector.OfClass(typeof(WallType));
            m_wallTypeList = filteredElementCollector.Cast<WallType>().ToList();

            // Sort the wall type list by the name property
            var comparer = new WallTypeComparer();
            m_wallTypeList.Sort(comparer);

            // Get all levels from revit 
            var iter = new FilteredElementCollector(document).OfClass(typeof(Level)).GetElementIterator();
            iter.Reset();
            while (iter.MoveNext())
            {
                if (!(iter.Current is Level level)) continue;
                m_levelList.Add(level);
            }
        }

        /// <summary>
        ///     Read the journal data from the journal.
        ///     All journal data is stored in commandData.Data.
        /// </summary>
        private void ReadJournalData()
        {
            // Get the journal data map from API
            var doc = m_commandData.Application.ActiveUIDocument.Document;
            var dataMap = m_commandData.JournalData;

            var dataValue = // store the journal data value temporarily
                // Get the wall type from the journal
                SampleBrowserUtils.GetSpecialData(dataMap, "Wall Type Name"); // get wall type name
            foreach (var type in m_wallTypeList) // get the wall type by the name
            {
                if (dataValue == type.Name)
                {
                    m_createType = type;
                    break;
                }
            }

            if (null == m_createType) // assert the wall type is exist
                throw new InvalidDataException("Can't find the wall type from the journal.");

            // Get the level information from the journal
            dataValue = SampleBrowserUtils.GetSpecialData(dataMap, "Level Id"); // get the level id
            var id = ElementId.Parse(dataValue); // get the level by its id

            m_createlevel = doc.GetElement(id) as Level;
            if (null == m_createlevel) // assert the level is exist
                throw new InvalidDataException("Can't find the level from the journal.");

            // Get the start point information from the journal
            dataValue = SampleBrowserUtils.GetSpecialData(dataMap, "Start Point");
            m_startPoint = XyzMath.StringToXyz(dataValue);

            dataValue = SampleBrowserUtils.GetSpecialData(dataMap, "End Point");
            m_endPoint = XyzMath.StringToXyz(dataValue);

            // Create wall don't allow the start point equals end point
            if (m_startPoint.Equals(m_endPoint)) throw new InvalidDataException("Start point is equal to end point.");
        }

        /// <summary>
        ///     Display the UI form to collect some necessary information for create wall.
        ///     The information will be write into the journal
        /// </summary>
        /// <returns></returns>
        private bool DisplayUi()
        {
            // Display the form and allow the user to input some information for wall creation
            using (var displayForm = new JournalingForm(this))
            {
                displayForm.ShowDialog();
                if (DialogResult.OK != displayForm.DialogResult) return false;
            }

            return true;
        }

        private void CreateWall()
        {
            // Get the create classes.

            // Create geometry line(curve)
            var geometryLine = Line.CreateBound(m_startPoint, m_endPoint);
            if (null == geometryLine) // assert the creation is successful
                throw new Exception("Create the geometry line failed.");

            // Create the wall using the wall type, level and created geometry line
            var createdWall = Wall.Create(m_commandData.Application.ActiveUIDocument.Document, geometryLine,
                m_createType.Id, m_createlevel.Id,
                15, m_startPoint.Z + m_createlevel.Elevation, true, true);
            if (null == createdWall) // assert the creation is successful
                throw new Exception("Create the wall failed.");
        }

        /// <summary>
        ///     Write the support data into the journal
        /// </summary>
        private void WriteJournalData()
        {
            // Get the StringStringMap class which can write support into.
            var dataMap = m_commandData.JournalData;
            dataMap.Clear();

            // Begin to add the support data
            dataMap.Add("Wall Type Name", m_createType.Name); // add wall type name
            dataMap.Add("Level Id", m_createlevel.Id.ToString()); // add level id
            dataMap.Add("Start Point", XyzMath.XyzToString(m_startPoint));
            dataMap.Add("End Point", XyzMath.XyzToString(m_endPoint));
        }

        private class WallTypeComparer : IComparer<WallType>
        {
            int IComparer<WallType>.Compare(WallType first, WallType second) =>
                string.Compare(first.Name, second.Name);
        }
    }
}
