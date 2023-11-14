// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CurvedBeam.CS
{
    /// <summary>
    ///     This class inherits from IExternalCommand interface, and implements the Execute method to create Arc, BSpline
    ///     beams.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class Command : IExternalCommand
    {
        private UIApplication m_revit;

        /// <summary>
        ///     list of all type of beams
        /// </summary>
        public ArrayList BeamMaps { get; } = new ArrayList();

        /// <summary>
        ///     list of all levels
        /// </summary>
        public ArrayList LevelMaps { get; } = new ArrayList();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_revit = commandData.Application;
            var tran = new Transaction(m_revit.ActiveUIDocument.Document, "CurvedBeam");
            tran.Start();

            // if initialize failed return Result.Failed
            var initializeOK = Initialize();
            if (!initializeOK) return Result.Failed;

            // pop up new beam form
            var displayForm = new CurvedBeamForm(this);
            displayForm.ShowDialog();
            tran.Commit();

            return Result.Succeeded;
        }

        /// <summary>
        ///     iterate all the symbols of levels and beams
        /// </summary>
        /// <returns>A value that signifies if the initialization was successful for true or failed for false</returns>
        private bool Initialize()
        {
            try
            {
                var levelFilter = new ElementClassFilter(typeof(Level));
                var famFilter = new ElementClassFilter(typeof(Family));
                var orFilter = new LogicalOrFilter(levelFilter, famFilter);
                var collector = new FilteredElementCollector(m_revit.ActiveUIDocument.Document);
                var i = collector.WherePasses(orFilter).GetElementIterator();
                i.Reset();
                var moreElement = i.MoveNext();
                while (moreElement)
                {
                    object o = i.Current;

                    // add level to list
                    if (o is Level level)
                    {
                        LevelMaps.Add(new LevelMap(level));
                        goto nextLoop;
                    }

                    // get
                    if (!(o is Family f)) goto nextLoop;

                    foreach (var elementId in f.GetFamilySymbolIds())
                    {
                        object symbol = m_revit.ActiveUIDocument.Document.GetElement(elementId);
                        var familyType = symbol as FamilySymbol;
                        if (familyType?.Category == null) goto nextLoop;

                        // add symbols of beams and braces to lists 
                        var categoryName = familyType.Category.Name;
                        if ("Structural Framing" == categoryName) BeamMaps.Add(new SymbolMap(familyType));
                    }

                    nextLoop:
                    moreElement = i.MoveNext();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return true;
        }

        /// <summary>
        ///     create an horizontal arc instance with specified z coordinate value
        /// </summary>
        public Arc CreateArc(double z)
        {
            var center = new XYZ(0, 0, z);
            var radius = 20.0;
            var startAngle = 0.0;
            var endAngle = 5.0;
            var xAxis = new XYZ(1, 0, 0);
            var yAxis = new XYZ(0, 1, 0);
            return Arc.Create(center, radius, startAngle, endAngle, xAxis, yAxis);
        }

        /// <summary>
        ///     create a horizontal partial ellipse instance with specified z coordinate value
        /// </summary>
        public Curve CreateEllipse(double z)
        {
            var center = new XYZ(0, 0, z);
            double radX = 30;
            double radY = 50;
            var xVec = new XYZ(1, 0, 0);
            var yVec = new XYZ(0, 1, 0);
            var param0 = 0.0;
            var param1 = 3.1415;
            var ellpise = Ellipse.CreateCurve(center, radX, radY, xVec, yVec, param0, param1);
            m_revit.ActiveUIDocument.Document.Regenerate();
            return ellpise;
        }

        /// <summary>
        ///     create a horizontal nurbspline instance with specified z coordinate value
        /// </summary>
        public Curve CreateNurbSpline(double z)
        {
            // create control points with same z value
            var ctrPoints = new List<XYZ>();
            var xyz1 = new XYZ(-41.887503610431267, -9.0290629129782189, z);
            var xyz2 = new XYZ(-9.27600019217055, 0.32213521486563046, z);
            var xyz3 = new XYZ(9.27600019217055, 0.32213521486563046, z);
            var xyz4 = new XYZ(41.887503610431267, 9.0290629129782189, z);

            ctrPoints.Add(xyz1);
            ctrPoints.Add(xyz2);
            ctrPoints.Add(xyz3);
            ctrPoints.Add(xyz4);

            IList<double> weights = new List<double>();
            double w1 = 1, w2 = 1, w3 = 1, w4 = 1;
            weights.Add(w1);
            weights.Add(w2);
            weights.Add(w3);
            weights.Add(w4);

            IList<double> knots = new List<double>();
            double k0 = 0, k1 = 0, k2 = 0, k3 = 0, k4 = 34.425128, k5 = 34.425128, k6 = 34.425128, k7 = 34.425128;

            knots.Add(k0);
            knots.Add(k1);
            knots.Add(k2);
            knots.Add(k3);
            knots.Add(k4);
            knots.Add(k5);
            knots.Add(k6);
            knots.Add(k7);

            var detailNurbSpline = NurbSpline.CreateCurve(3, knots, ctrPoints, weights);
            m_revit.ActiveUIDocument.Document.Regenerate();

            return detailNurbSpline;
        }

        /// <summary>
        ///     create a curved beam
        /// </summary>
        /// <param name="fsBeam">beam type</param>
        /// <param name="curve">Curve of this beam.</param>
        /// <param name="level">beam's reference level</param>
        /// <returns></returns>
        public bool CreateCurvedBeam(FamilySymbol fsBeam, Curve curve, Level level)
        {
            try
            {
                if (!fsBeam.IsActive)
                    fsBeam.Activate();
                var beam = m_revit.ActiveUIDocument.Document.Create.NewFamilyInstance(curve, fsBeam, level,
                    StructuralType.Beam);

                // get beam location curve
                if (!(beam?.Location is LocationCurve beamCurve)) return false;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.ToString());
                return false;
            }

            // regenerate document
            m_revit.ActiveUIDocument.Document.Regenerate();
            return true;
        }
    }

    /// <summary>
    ///     assistant class contains symbol and it's name
    /// </summary>
    public class SymbolMap
    {
        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private SymbolMap()
        {
            // no operation 
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="symbol">family symbol</param>
        public SymbolMap(FamilySymbol symbol)
        {
            ElementType = symbol;
            var familyName = "";
            if (null != symbol.Family) familyName = symbol.Family.Name;
            SymbolName = familyName + " : " + symbol.Name;
        }

        /// <summary>
        ///     SymbolName property
        /// </summary>
        public string SymbolName { get; } = "";

        /// <summary>
        ///     ElementType property
        /// </summary>
        public FamilySymbol ElementType { get; }
    }

    /// <summary>
    ///     assistant class contains level and it's name
    /// </summary>
    public class LevelMap
    {
        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private LevelMap()
        {
            // no operation
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="level">level</param>
        public LevelMap(Level level)
        {
            Level = level;
            LevelName = level.Name;
        }

        /// <summary>
        ///     LevelName property
        /// </summary>
        public string LevelName { get; } = "";

        /// <summary>
        ///     Level property
        /// </summary>
        public Level Level { get; }
    }
}
