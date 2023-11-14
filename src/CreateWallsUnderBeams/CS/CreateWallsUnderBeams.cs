// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RevitMultiSample.CreateWallsUnderBeams.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateWallsUnderBeams : IExternalCommand
    {
        private const double Precision = 0.0000000001; // Define a precision of double data

        // Private Members
        private readonly ArrayList m_beamCollection; // Store the selection of beams in Revit
        private string m_errorInformation; // Store the error information
        private Level m_level; // Store the level which wall create on
        private WallType m_selectedWallType; // Store the selected wall type

        // Methods
        /// <summary>
        ///     Default constructor of CreateWallsUnderBeams
        /// </summary>
        public CreateWallsUnderBeams()
        {
            WallTypes = new List<WallType>();
            m_beamCollection = new ArrayList();
            IsSturctual = true;
        }

        // Properties
        /// <summary>
        ///     Inform all the wall types can be created in current document
        /// </summary>
        public IList<WallType> WallTypes { get; private set; }

        /// <summary>
        ///     Inform the wall type selected by the user
        /// </summary>
        public object SelectedWallType
        {
            set => m_selectedWallType = value as WallType;
        }

        /// <summary>
        ///     Inform whether the user want to create structural or architecture walls
        /// </summary>
        public bool IsSturctual { get; set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var project = revit.ActiveUIDocument;

            // Find the selection of beams in Revit
            var selection = new ElementSet();
            foreach (var elementId in project.Selection.GetElementIds())
                selection.Insert(project.Document.GetElement(elementId));
            foreach (Element e in selection)
            {
                if (e is FamilyInstance m)
                    if (StructuralType.Beam == m.StructuralType)
                        // Store all the beams the user selected in Revit
                        m_beamCollection.Add(e);
            }

            if (0 == m_beamCollection.Count)
            {
                message = "Can not find any beams.";
                return Result.Failed;
            }

            // Make sure all the beams have horizontal analytical line
            if (!CheckBeamHorizontal())
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // Search all the wall types in the Revit
            var filteredElementCollector = new FilteredElementCollector(project.Document);
            filteredElementCollector.OfClass(typeof(WallType));
            WallTypes = filteredElementCollector.Cast<WallType>().ToList();

            // Show the dialog for the user select the wall style
            using (var displayForm = new CreateWallsUnderBeamsForm(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return Result.Failed;
            }

            // Create the walls which along and under the path of the beams.
            if (!BeginCreate(project.Document))
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // If everything goes right, return succeeded.
            return Result.Succeeded;
        }

        /// <summary>
        ///     Create the walls which along and under the path of the selected beams
        /// </summary>
        /// <param name="project"> A reference of current document</param>
        /// <returns>true if there is no error in process; otherwise, false.</returns>
        private bool BeginCreate(Document project)
        {
            // Begin to create walls along and under each beam
            foreach (var beam in m_beamCollection)
            {
                // Get each selected beam.
                if (!(beam is FamilyInstance m))
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                // the wall will be created using beam's model line as path.   
                if (!(m.Location is LocationCurve curve))
                {
                    m_errorInformation = "The beam should have location curve.";
                    return false;
                }

                var beamCurve = curve.Curve;

                // Get the level using the beam's reference level
                var levelId = m.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();
                m_level = project.GetElement(levelId) as Level;
                if (null == m_level)
                {
                    m_errorInformation = "The program should not go here.";
                    return false;
                }

                var t = new Transaction(project, Guid.NewGuid().GetHashCode().ToString());
                t.Start();
                var createdWall = Wall.Create(project, beamCurve, m_selectedWallType.Id,
                    m_level.Id, 10, 0, true, IsSturctual);
                if (null == createdWall)
                {
                    m_errorInformation = "Can not create the walls";
                    return false;
                }

                // Modify some parameters of the created wall to make it look better.
                var offset = beamCurve.GetEndPoint(0).Z - m_level.Elevation;
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(levelId);
                createdWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(offset - 3000 / 304.8);
                createdWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(levelId);
                t.Commit();
            }

            return true;
        }

        /// <summary>
        ///     Check whether all the beams have horizontal analytical line
        /// </summary>
        /// <returns>true if each beam has horizontal analytical line; otherwise, false.</returns>
        private bool CheckBeamHorizontal()
        {
            foreach (var beam in m_beamCollection)
            {
                // Get the analytical curve of each selected beam.
                // And check if Z coordinate of start point and end point of the curve are same.
                var m = beam as FamilyInstance;
                var beamCurve = m.Location is LocationCurve curve ? curve.Curve : null;
                if (null == beamCurve)
                {
                    m_errorInformation = "The beam should have location curve.";
                    return false;
                }

                if (Precision <= beamCurve.GetEndPoint(0).Z - beamCurve.GetEndPoint(1).Z
                    || -Precision >= beamCurve.GetEndPoint(0).Z - beamCurve.GetEndPoint(1).Z)
                {
                    m_errorInformation = "Please only select horizontal beams.";
                    return false;
                }
            }

            return true;
        }
    }
}
