// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using STRUCTURALTYPE = Autodesk.Revit.DB.Structure.StructuralType;

namespace Revit.SDK.Samples.CreateBeamsColumnsBraces.CS
{
    /// <summary>
    ///     Create Beams, Columns and Braces according to user's input information
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private readonly SortedList levels = new SortedList(); //list of list sorted by their elevations

        private UV[,] m_matrixUV; //2D coordinates of matrix
        private UIApplication m_revit;

        /// <summary>
        ///     list of all type of columns
        /// </summary>
        public ArrayList ColumnMaps { get; } = new ArrayList();

        /// <summary>
        ///     list of all type of beams
        /// </summary>
        public ArrayList BeamMaps { get; } = new ArrayList();

        /// <summary>
        ///     list of all type of braces
        /// </summary>
        public ArrayList BraceMaps { get; } = new ArrayList();

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            m_revit = revit.Application;
            var tran = new Transaction(m_revit.ActiveUIDocument.Document, "CreateBeamsColumnsBraces");
            tran.Start();

            try
            {
                //if initialize failed return Result.Failed
                var initializeOK = Initialize();
                if (!initializeOK)
                {
                    tran.RollBack();
                    return Result.Failed;
                }

                using (var displayForm = new CreateBeamsColumnsBracesForm(this))
                {
                    if (displayForm.ShowDialog() != DialogResult.OK)
                    {
                        tran.RollBack();
                        return Result.Cancelled;
                    }
                }

                tran.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                tran.RollBack();
                return Result.Failed;
            }
        }

        /// <summary>
        ///     check the number of floors is less than the number of levels
        ///     create beams, columns and braces according to selected types
        /// </summary>
        /// <param name="columnObject">type of column</param>
        /// <param name="beamObject">type of beam</param>
        /// <param name="braceObject">type of brace</param>
        /// <param name="floorNumber">number of floor</param>
        /// <returns>number of floors is less than the number of levels and create successfully then return true</returns>
        public bool AddInstance(object columnObject, object beamObject, object braceObject, int floorNumber)
        {
            //whether floor number less than levels number
            if (floorNumber >= levels.Count)
            {
                TaskDialog.Show("Revit", "The number of levels must be added.");
                return false;
            }

            //any symbol is null then the command failed
            if (!(columnObject is FamilySymbol columnSymbol) || !(beamObject is FamilySymbol beamSymbol) || !(braceObject is FamilySymbol braceSymbol)) return false;

            try
            {
                for (var k = 0; k < floorNumber; k++) //iterate levels from lower one to higher
                {
                    var baseLevel = levels.GetByIndex(k) as Level;
                    var topLevel = levels.GetByIndex(k + 1) as Level;

                    var matrixXSize = m_matrixUV.GetLength(0); //length of matrix's x range
                    var matrixYSize = m_matrixUV.GetLength(1); //length of matrix's y range

                    //iterate coordinate both in x direction and y direction and create beams and braces
                    for (var j = 0; j < matrixYSize; j++)
                    for (var i = 0; i < matrixXSize; i++)
                    {
                        //create beams and braces in x direction
                        if (i != matrixXSize - 1)
                            PlaceBrace(m_matrixUV[i, j], m_matrixUV[i + 1, j], baseLevel, topLevel, braceSymbol, true);
                        //create beams and braces in y direction
                        if (j != matrixYSize - 1)
                            PlaceBrace(m_matrixUV[i, j], m_matrixUV[i, j + 1], baseLevel, topLevel, braceSymbol, false);
                    }

                    for (var j = 0; j < matrixYSize; j++)
                    for (var i = 0; i < matrixXSize; i++)
                    {
                        //create beams and braces in x direction
                        if (i != matrixXSize - 1)
                            PlaceBeam(m_matrixUV[i, j], m_matrixUV[i + 1, j], baseLevel, topLevel, beamSymbol);
                        //create beams and braces in y direction
                        if (j != matrixYSize - 1)
                            PlaceBeam(m_matrixUV[i, j], m_matrixUV[i, j + 1], baseLevel, topLevel, beamSymbol);
                    }

                    //place column of this level
                    foreach (var point2D in m_matrixUV) PlaceColumn(point2D, columnSymbol, baseLevel, topLevel);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     generate 2D coordinates of matrix according to parameters
        /// </summary>
        /// <param name="xNumber">Number of Columns in the X direction</param>
        /// <param name="yNumber">Number of Columns in the Y direction</param>
        /// <param name="distance">Distance between columns</param>
        public void CreateMatrix(int xNumber, int yNumber, double distance)
        {
            m_matrixUV = new UV[xNumber, yNumber];

            for (var i = 0; i < xNumber; i++)
            for (var j = 0; j < yNumber; j++)
                m_matrixUV[i, j] = new UV(i * distance, j * distance);
        }

        /// <summary>
        ///     iterate all the symbols of levels, columns, beams and braces
        /// </summary>
        /// <returns>A value that signifies if the initialization was successful for true or failed for false</returns>
        private bool Initialize()
        {
            try
            {
                var i = new FilteredElementCollector(m_revit.ActiveUIDocument.Document).OfClass(typeof(Level))
                    .GetElementIterator();
                i.Reset();
                while (i.MoveNext())
                {
                    //add level to list
                    if (i.Current is Level level) levels.Add(level.Elevation, level);
                }

                i = new FilteredElementCollector(m_revit.ActiveUIDocument.Document).OfClass(typeof(Family))
                    .GetElementIterator();
                while (i.MoveNext())
                {
                    if (i.Current is Family f)
                        foreach (var elementId in f.GetFamilySymbolIds())
                        {
                            object symbol = m_revit.ActiveUIDocument.Document.GetElement(elementId);
                            var familyType = symbol as FamilySymbol;
                            if (familyType?.Category == null) continue;

                            //add symbols of beams and braces to lists 
                            var categoryName = familyType.Category.Name;
                            switch (categoryName)
                            {
                                case "Structural Framing":
                                    BeamMaps.Add(new SymbolMap(familyType));
                                    BraceMaps.Add(new SymbolMap(familyType));
                                    break;
                                case "Structural Columns":
                                    ColumnMaps.Add(new SymbolMap(familyType));
                                    break;
                            }
                        }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     create column of certain type in certain position
        /// </summary>
        /// <param name="point2D">2D coordinate of the column</param>
        /// <param name="columnType">type of column</param>
        /// <param name="baseLevel">the base level of the column</param>
        /// <param name="topLevel">the top level of the column</param>
        private void PlaceColumn(UV point2D, FamilySymbol columnType, Level baseLevel, Level topLevel)
        {
            //create column of certain type in certain level and start point 
            var point = new XYZ(point2D.U, point2D.V, 0);
            var structuralType = STRUCTURALTYPE.Column;
            if (!columnType.IsActive)
                columnType.Activate();
            var column =
                m_revit.ActiveUIDocument.Document.Create.NewFamilyInstance(point, columnType, topLevel, structuralType);

            //set base level & top level of the column
            if (null != column)
            {
                var baseLevelParameter = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                var topLevelParameter = column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                var topOffsetParameter = column.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
                var baseOffsetParameter = column.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);

                if (null != baseLevelParameter)
                {
                    var baseLevelId = baseLevel.Id;
                    baseLevelParameter.Set(baseLevelId);
                }

                if (null != topLevelParameter)
                {
                    var topLevelId = topLevel.Id;
                    topLevelParameter.Set(topLevelId);
                }

                topOffsetParameter?.Set(0.0);

                baseOffsetParameter?.Set(0.0);
            }
        }

        /// <summary>
        ///     create beam of certain type in certain position
        /// </summary>
        /// <param name="point2D1">one point of the location line in 2D</param>
        /// <param name="point2D2">another point of the location line in 2D</param>
        /// <param name="baseLevel">the base level of the beam</param>
        /// <param name="topLevel">the top level of the beam</param>
        /// <param name="beamType">type of beam</param>
        /// <returns>nothing</returns>
        private void PlaceBeam(UV point2D1, UV point2D2, Level baseLevel, Level topLevel, FamilySymbol beamType)
        {
            var height = topLevel.Elevation;
            var startPoint = new XYZ(point2D1.U, point2D1.V, height);
            var endPoint = new XYZ(point2D2.U, point2D2.V, height);

            var line = Line.CreateBound(startPoint, endPoint);
            var structuralType = STRUCTURALTYPE.Beam;
            if (!beamType.IsActive)
                beamType.Activate();
            m_revit.ActiveUIDocument.Document.Create.NewFamilyInstance(line, beamType, topLevel, structuralType);
        }

        /// <summary>
        ///     create brace of certain type in certain position between two adjacent columns
        /// </summary>
        /// <param name="point2D1">one point of the location line in 2D</param>
        /// <param name="point2D2">another point of the location line in 2D</param>
        /// <param name="baseLevel">the base level of the brace</param>
        /// <param name="topLevel">the top level of the brace</param>
        /// <param name="braceType">type of beam</param>
        /// <param name="isXDirection">whether the location line is in x direction</param>
        private void PlaceBrace(UV point2D1, UV point2D2, Level baseLevel, Level topLevel, FamilySymbol braceType,
            bool isXDirection)
        {
            //get the start points and end points of location lines of two braces
            var topHeight = topLevel.Elevation;
            var baseHeight = baseLevel.Elevation;
            var middleElevation = (topHeight + baseHeight) / 2;
            var startPoint = new XYZ(point2D1.U, point2D1.V, middleElevation);
            var endPoint = new XYZ(point2D2.U, point2D2.V, middleElevation);
            XYZ middlePoint;

            if (isXDirection)
                middlePoint = new XYZ((point2D1.U + point2D2.U) / 2, point2D2.V, topHeight);
            else
                middlePoint = new XYZ(point2D2.U, (point2D1.V + point2D2.V) / 2, topHeight);

            //create two brace and set their location line
            var structuralType = STRUCTURALTYPE.Brace;
            var levelId = topLevel.Id;

            var line1 = Line.CreateBound(startPoint, middlePoint);
            if (!braceType.IsActive)
                braceType.Activate();
            var firstBrace =
                m_revit.ActiveUIDocument.Document.Create.NewFamilyInstance(line1, braceType, baseLevel, structuralType);

            var referenceLevel1 = firstBrace.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
            referenceLevel1?.Set(levelId);

            var line2 = Line.CreateBound(endPoint, middlePoint);
            var secondBrace =
                m_revit.ActiveUIDocument.Document.Create.NewFamilyInstance(line2, braceType, baseLevel, structuralType);

            var referenceLevel2 = secondBrace.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
            referenceLevel2?.Set(levelId);
        }
    }

    /// <summary>
    ///     assistant class contains the symbol and its name.
    /// </summary>
    public class SymbolMap
    {
        /// <summary>
        ///     constructor without parameter is forbidden
        /// </summary>
        private SymbolMap()
        {
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
}
