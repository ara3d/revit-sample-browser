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

namespace Revit.SDK.Samples.CreateWallinBeamProfile.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateWallinBeamProfile : IExternalCommand
    {
        private const double PRECISION = 0.0000000001; // Define a precision of double data

        // Private Members
        private readonly ArrayList m_beamCollection; // Store the selection of beams in Revit
        private string m_errorInformation; // Store the error information
        private Level m_level; // Store the level which wall create on
        private readonly ArrayList m_lineCollection; // Store the lines of all the beams
        private WallType m_selectedWallType; // Store the selected wall type

        // Methods
        /// <summary>
        ///     Default constructor of CreateWallinBeamProfile
        /// </summary>
        public CreateWallinBeamProfile()
        {
            WallTypes = new List<WallType>();
            m_beamCollection = new ArrayList();
            m_lineCollection = new ArrayList();
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
        ///     Inform whether the user want to create structual or architecture walls
        /// </summary>
        public bool IsSturctual { get; set; }

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var project = revit.ActiveUIDocument;

            // Get necessary data from revit.such as selected beams and level information
            if (!PrepareData(project))
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // Check whether the selected beams can make a a vertical profile
            if (!IsVerticalProfile())
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // Show the dialog for the user select the wall style
            using (var displayForm = new CreateWallinBeamProfileForm(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog()) return Result.Failed;
            }

            // Create the walls using the outline generated by the beams.
            if (!BeginCreate(project.Document))
            {
                message = m_errorInformation;
                return Result.Failed;
            }

            // If everything goes right, return succeeded.
            return Result.Succeeded;
        }

        /// <summary>
        ///     Create the walls using the outline generated by the beams
        /// </summary>
        /// <param name="project"> A reference of current document</param>
        /// <returns>true if no error happens; otherwise, false.</returns>
        private bool BeginCreate(Document project)
        {
            //CurveArray curveArray = new CurveArray();   // store the curves used to create wall
            var curveArray = new List<Curve>();
            var curve = m_lineCollection[0] as Curve;
            curveArray.Add(curve);
            var point = curve.GetEndPoint(1); // used to store the end point of the curve temporarily

            // Sort the curves of analytical model and then add to curveArray.
            // API asks for the curves should be in a sequence, deasil or anticlockwise
            for (var i = 1; i < m_lineCollection.Count; i++)
                foreach (var o in m_lineCollection)
                {
                    var isInclude = false;
                    foreach (var j in curveArray)
                        if (o.Equals(j))
                        {
                            isInclude = true;
                            break;
                        }

                    if (isInclude) continue;

                    curve = o as Curve;
                    if (!EqualPoint(curve.GetEndPoint(0), point)
                        && !EqualPoint(curve.GetEndPoint(1), point))
                        continue;

                    if (EqualPoint(curve.GetEndPoint(0), point))
                    {
                        curveArray.Add(curve);
                        point = curve.GetEndPoint(1);
                        break;
                    }

                    if (EqualPoint(curve.GetEndPoint(1), point))
                    {
                        curveArray.Add(curve);
                        point = curve.GetEndPoint(0);
                        break;
                    }

                    m_errorInformation = "The program should never go here.";
                    return false;
                }

            // If the program goes here, it means the beams can't form a profile.
            if (curveArray.Count != m_lineCollection.Count)
            {
                m_errorInformation = "There are more than one closed profile.";
                return false;
            }

            // Begin to create the wall.
            var t = new Transaction(project, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            var createdWall = Wall.Create(project, curveArray,
                m_selectedWallType.Id, m_level.Id, IsSturctual);

            if (null == createdWall)
            {
                m_errorInformation = "Can not create the wall";
                return false;
            }

            // Modify some parameters of the created wall to make it look better.
            var baseOffset = FindBaseOffset(); // get the base offset from m_level
            var topOffset = FindTopOffset(); // get the top offset from m_level
            var levelId = m_level.Id;
            // Modify the "Base Constraint", "Base Offset", "Top Constraint" and "Top Offset"
            //properties of the created wall.
            createdWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(levelId);
            createdWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(baseOffset);
            createdWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(levelId);
            createdWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(topOffset);
            t.Commit();
            return true;
        }

        /// <summary>
        ///     Get necessary data from revit.such as selected beams, wall types and level information
        /// </summary>
        /// <param name="project">A reference of current document</param>
        /// <returns>true if no error happens; otherwise, false.</returns>
        private bool PrepareData(UIDocument project)
        {
            // Search all the wall types in the Revit
            var filteredElementCollector = new FilteredElementCollector(project.Document);
            filteredElementCollector.OfClass(typeof(WallType));
            WallTypes = filteredElementCollector.Cast<WallType>().ToList();

            // Find the selection of beams in Revit
            var selection = new ElementSet();
            foreach (var elementId in project.Selection.GetElementIds())
                selection.Insert(project.Document.GetElement(elementId));

            foreach (Element e in selection)
            {
                // Use StructuralType property can judge whether it is a beam.
                if (e is FamilyInstance m && StructuralType.Beam == m.StructuralType)
                {
                    m_beamCollection.Add(e); // store the beams

                    if (!(m.Location is LocationCurve curve))
                    {
                        m_errorInformation = "The beam should have location curve.";
                        return false;
                    }

                    m_lineCollection.Add(curve.Curve);
                }
            }

            if (0 == m_beamCollection.Count)
            {
                m_errorInformation = "Can not find any beams.";
                return false;
            }

            // Get the level which will be used in create method
            var collector = new FilteredElementCollector(project.Document);
            m_level = collector.OfClass(typeof(Level)).FirstElement() as Level;
            return true;
        }

        /// <summary>
        ///     Check whether the selected beams can make a a vertical profile.
        /// </summary>
        /// <returns>true if selected beams create a vertical profile; otherwise, false.</returns>
        private bool IsVerticalProfile()
        {
            // First check whether all the beams are in a same vertical plane
            if (!IsInVerticalPlane())
            {
                m_errorInformation = "Not all the beam in a vertical plane.";
                return false;
            }

            // Second check whether a closed profile can be created by all the beams
            if (!CanCreateProfile())
            {
                m_errorInformation = "All the beams should create only one profile.";
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Check whether the input two points are the same
        /// </summary>
        /// <param name="first"> The first point</param>
        /// <param name="second"> The second point</param>
        /// <returns>true if two points are the same; otherwise, false</returns>
        private bool EqualPoint(XYZ first, XYZ second)
        {
            if (-PRECISION <= first.X - second.X && PRECISION >= first.X - second.X
                                                 && -PRECISION <= first.Y - second.Y && PRECISION >= first.Y - second.Y
                                                 && -PRECISION <= first.Z - second.Z && PRECISION >= first.Z - second.Z)
                return true;
            return false;
        }

        /// <summary>
        ///     Check whether the two double data are the same
        /// </summary>
        /// <param name="first">The first double data</param>
        /// <param name="second">The second double data</param>
        /// <returns>true if two double data are the same; otherwise, false</returns>
        private bool EqualDouble(double first, double second)
        {
            return -PRECISION <= first - second && PRECISION >= first - second;
        }

        /// <summary>
        ///     Check whether all the beams are in a same vertical plane
        /// </summary>
        /// <returns>true if they are in same vertical plane; otherwise, false</returns>
        private bool IsInVerticalPlane()
        {
            var startPoint = new XYZ();
            var endPoint = new XYZ();
            var sign = 0; // used as a symbol,
            double slope = 0; // record slope of the lines' projection on X-Y plane

            // When all the beams in the X-Z plane or Y-Z plane, the deal is especial
            // So I use 3 ways to judge whether all the beams are in same vertical plane
            var curve = m_lineCollection[0] as Curve;
            startPoint = curve.GetEndPoint(0);
            endPoint = curve.GetEndPoint(1);
            if (EqualDouble(startPoint.X, endPoint.X))
                sign = 1; // All the beams may be in Y-Z plane
            else if (EqualDouble(startPoint.Y, endPoint.Y))
                sign = 2; // All the beams may be in X-Z plane
            else
                slope = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);

            // Begin to compare each analytical line and judge whether they are in same vertical plane
            for (var i = 1; i < m_lineCollection.Count; i++)
            {
                curve = m_lineCollection[i] as Curve;
                startPoint = curve.GetEndPoint(0);
                endPoint = curve.GetEndPoint(1);

                switch (sign)
                {
                    case 0: // Judge whether the slope of beam's projection on X-Y plane are same.
                        var anotherSlope = (startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X);
                        if (!EqualDouble(slope, anotherSlope)) return false;
                        break;
                    case 1: // Judge whether the beams are in Y-Z plane
                        if (!EqualDouble(startPoint.X, endPoint.X)) return false;
                        break;
                    case 2: // Judge whether the beams are in X-Z plane
                        if (!EqualDouble(startPoint.Y, endPoint.Y)) return false;
                        break;
                    default: // If it go here, there must be something error.
                        TaskDialog.Show("Revit", "Should not come here.");
                        break;
                }
            }

            return true;
        }

        /// <summary>
        ///     Check whether a closed profile can be created by all the beams
        /// </summary>
        /// <returns>true if one profile found; otherwise, false</returns>
        private bool CanCreateProfile()
        {
            // Only allow all the beams compose a close profile.
            // As we all know, a close profile is composed by borders and points,
            // and the number of borders should be equal to points'.
            // So, the judgement use this way. 
            var startPoint = new XYZ();
            var endPoint = new XYZ();
            var pointArray = new ArrayList();

            // Find out all the points in the curves, the same point only count once.
            foreach (var line in m_lineCollection)
            {
                var curve = line as Curve;
                startPoint = curve.GetEndPoint(0);
                endPoint = curve.GetEndPoint(1);
                var hasStartpoint = false; // Judge whether start point has been counted.
                // indicate whether start point is in the array
                var hasEndPoint = false; // Judge whether end point has been counted.
                // indicate whether end point is in the array
                if (0 == pointArray.Count)
                {
                    pointArray.Add(startPoint);
                    pointArray.Add(endPoint);
                    continue;
                }

                // Judge whether the points of this curve have been counted.
                foreach (var o in pointArray)
                {
                    var point = (XYZ)o;
                    if (EqualPoint(startPoint, point)) hasStartpoint = true;
                    if (EqualPoint(endPoint, point)) hasEndPoint = true;
                }

                // If not, add the points into the array.
                if (!hasStartpoint) pointArray.Add(startPoint);
                if (!hasEndPoint) pointArray.Add(endPoint);
            }

            return pointArray.Count == m_lineCollection.Count;
        }

        /// <summary>
        ///     Find the offset from the elevation of the lowest point to m_level's elevation
        /// </summary>
        /// <returns> The length of the offset </returns>
        private double FindBaseOffset()
        {
            // Initialize the data.
            var curve = m_lineCollection[0] as Curve;
            var lowestElevation = curve.GetEndPoint(0).Z; // the elevation of the lowest point

            // Find out the elevation of the lowest point.
            foreach (Curve c in m_lineCollection)
            {
                if (c.GetEndPoint(0).Z < lowestElevation) lowestElevation = c.GetEndPoint(0).Z;
                if (c.GetEndPoint(1).Z < lowestElevation) lowestElevation = c.GetEndPoint(1).Z;
            }

            // Count the offset and return.
            var baseOffset = lowestElevation - m_level.Elevation; // the offset from the m_level's elevation
            return baseOffset;
        }

        /// <summary>
        ///     Find the offset from the elevation of the highest point to m_level's elevation
        /// </summary>
        /// <returns>The length of the offset</returns>
        private double FindTopOffset()
        {
            // Initialize the data
            var curve = m_lineCollection[0] as Curve;
            var highestElevation = curve.GetEndPoint(0).Z; // the elevation of the highest point

            // Find out the elevation of the highest point.
            foreach (Curve c in m_lineCollection)
            {
                if (c.GetEndPoint(0).Z > highestElevation) highestElevation = c.GetEndPoint(0).Z;
                if (c.GetEndPoint(1).Z > highestElevation) highestElevation = c.GetEndPoint(1).Z;
            }

            // Count the offset and return.
            var topOffset = highestElevation - m_level.Elevation; // the offset from the m_level's elevation
            return topOffset;
        }
    }
}
