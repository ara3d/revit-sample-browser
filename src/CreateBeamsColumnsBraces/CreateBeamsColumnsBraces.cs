// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using STRUCTURALTYPE = Autodesk.Revit.DB.Structure.StructuralType;

namespace Ara3D.RevitSampleBrowser.CreateBeamsColumnsBraces.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private readonly SortedList m_levels = new SortedList(); //list of list sorted by their elevations

        private UV[,] m_matrixUv; //2D coordinates of matrix
        private UIApplication m_revit;

        public ArrayList ColumnMaps { get; } = new ArrayList();

        public ArrayList BeamMaps { get; } = new ArrayList();

        public ArrayList BraceMaps { get; } = new ArrayList();

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            m_revit = revit.Application;
            var tran = new Transaction(m_revit.ActiveUIDocument.Document, "CreateBeamsColumnsBraces");
            tran.Start();

            try
            {
                //if initialize failed return Result.Failed
                var initializeOk = Initialize();
                if (!initializeOk)
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

        public bool AddInstance(object columnObject, object beamObject, object braceObject, int floorNumber)
        {
            //whether floor number less than levels number
            if (floorNumber >= m_levels.Count)
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
                    var baseLevel = m_levels.GetByIndex(k) as Level;
                    var topLevel = m_levels.GetByIndex(k + 1) as Level;

                    var matrixXSize = m_matrixUv.GetLength(0); //length of matrix's x range
                    var matrixYSize = m_matrixUv.GetLength(1); //length of matrix's y range

                    //iterate coordinate both in x direction and y direction and create beams and braces
                    for (var j = 0; j < matrixYSize; j++)
                    for (var i = 0; i < matrixXSize; i++)
                    {
                        //create beams and braces in x direction
                        if (i != matrixXSize - 1)
                            PlaceBrace(m_matrixUv[i, j], m_matrixUv[i + 1, j], baseLevel, topLevel, braceSymbol, true);
                        //create beams and braces in y direction
                        if (j != matrixYSize - 1)
                            PlaceBrace(m_matrixUv[i, j], m_matrixUv[i, j + 1], baseLevel, topLevel, braceSymbol, false);
                    }

                    for (var j = 0; j < matrixYSize; j++)
                    for (var i = 0; i < matrixXSize; i++)
                    {
                        //create beams and braces in x direction
                        if (i != matrixXSize - 1)
                            PlaceBeam(m_matrixUv[i, j], m_matrixUv[i + 1, j], baseLevel, topLevel, beamSymbol);
                        //create beams and braces in y direction
                        if (j != matrixYSize - 1)
                            PlaceBeam(m_matrixUv[i, j], m_matrixUv[i, j + 1], baseLevel, topLevel, beamSymbol);
                    }

                    //place column of this level
                    foreach (var point2D in m_matrixUv)
                    {
                        PlaceColumn(point2D, columnSymbol, baseLevel, topLevel);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void CreateMatrix(int xNumber, int yNumber, double distance)
        {
            m_matrixUv = new UV[xNumber, yNumber];

            for (var i = 0; i < xNumber; i++)
            for (var j = 0; j < yNumber; j++)
                m_matrixUv[i, j] = new UV(i * distance, j * distance);
        }

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
                    if (i.Current is Level level) m_levels.Add(level.Elevation, level);
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

        private void PlaceBrace(UV point2D1, UV point2D2, Level baseLevel, Level topLevel, FamilySymbol braceType,
            bool isXDirection)
        {
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

    public class SymbolMap
    {
        public SymbolMap(FamilySymbol symbol)
        {
            var familyName = "";
            if (null != symbol.Family) familyName = symbol.Family.Name;
            SymbolName = $"{familyName} : {symbol.Name}";
        }

        public string SymbolName { get; }
    }
}
