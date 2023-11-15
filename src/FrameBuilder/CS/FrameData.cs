// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    /// <summary>
    ///     data class contains information to create framing
    /// </summary>
    public class FrameData
    {
        private const int XNumberMaxValue = 50; // maximum number of Columns in the X Direction
        private const int XNumberMinValue = 2; // minimum number of Columns in the X Direction
        private const int XNumberDefault = 3; // default number of Columns in the X Direction
        private const int YNumberMaxValue = 50; // maximum number of Columns in the Y Direction
        private const int YNumberMinValue = 2; // minimum number of Columns in the Y Direction
        private const int YNumberDefault = 3; // default number of Columns in the Y Direction
        private const int TotalMaxValue = 200; // maximum total number of Columns to create
        private const int FloorNumberMinValue = 1; // minimum number of floors
        private const int FloorNumberMaxValue = 200; // maximum number of floors
        private const double DistanceMaxValue = 3000; // maxinum distance between 2 adjoining columns
        private const double DistanceMinValue = 1; // mininum distance between 2 adjoining columns
        private const double DistanceDefault = 5; // default distance between 2 adjoining columns
        private const double LevelHeightMaxValue = 100; // maximum height between 2 new levels
        private const double LevelHeightMinValue = 1; // minimum height between 2 new levels
        private const int DigitPrecision = 4; // the number of significant digits(precision)
        private double m_distance = DistanceDefault; // distance between 2 adjoining columns
        private int m_floorNumber; // number of floors
        private double m_levelHeight; // increaced height of autogenerated levels

        private int m_xNumber = XNumberDefault; // number of Columns in the X Direction
        private int m_yNumber = XNumberDefault; // number of Columns in the Y Direction

        /// <summary>
        ///     it is only used for object factory method
        /// </summary>
        /// <param name="commandData"></param>
        private FrameData(ExternalCommandData commandData)
        {
            // initialize members
            CommandData = commandData;
            ColumnSymbolsMgr = new FrameTypesMgr(commandData);
            BeamSymbolsMgr = new FrameTypesMgr(commandData);
            Levels = new SortedList<double, Level>();
            OriginalLevelSize = Levels.Count;
            m_yNumber = YNumberDefault;
            m_xNumber = XNumberDefault;
            m_distance = DistanceDefault;
        }

        /// <summary>
        ///     command data pass from entry point
        /// </summary>
        public ExternalCommandData CommandData { get; }

        /// <summary>
        ///     object manage all column types
        /// </summary>
        public FrameTypesMgr ColumnSymbolsMgr { get; }

        /// <summary>
        ///     object manage all beam types
        /// </summary>
        public FrameTypesMgr BeamSymbolsMgr { get; }

        /// <summary>
        ///     object manage all brace types
        /// </summary>
        public FrameTypesMgr BraceSymbolsMgr => BeamSymbolsMgr;

        /// <summary>
        ///     number of Columns in the Y Direction
        /// </summary>
        public int YNumber
        {
            get => m_yNumber;
            set
            {
                if (value < YNumberMinValue || value > YNumberMaxValue)
                {
                    var message = "Number of Columns in the Y Direction should no less than "
                                  + YNumberMinValue + " and no more than "
                                  + YNumberMaxValue;
                    throw new ErrorMessageException(message);
                }

                CheckTotalNumber(value * XNumber * (m_floorNumber - 1));
                m_yNumber = value;
            }
        }

        /// <summary>
        ///     number of Columns in the X Direction
        /// </summary>
        public int XNumber
        {
            get => m_xNumber;
            set
            {
                if (value < XNumberMinValue || value > XNumberMaxValue)
                {
                    var message = "Number of Columns in the X Direction should no less than "
                                  + XNumberMinValue + " and no more than "
                                  + XNumberMaxValue;
                    throw new ErrorMessageException(message);
                }

                CheckTotalNumber(value * YNumber * (m_floorNumber - 1));
                m_xNumber = value;
            }
        }

        /// <summary>
        ///     distance between 2 adjoining columns
        /// </summary>
        public double Distance
        {
            get => m_distance;
            set
            {
                if (value < DistanceMinValue || value > DistanceMaxValue)
                {
                    var message = "The distance between columns shoule no less than "
                                  + DistanceMinValue + "and no more than "
                                  + DistanceMaxValue;
                    throw new ErrorMessageException(message);
                }

                m_distance = value;
            }
        }

        /// <summary>
        ///     number of floors
        /// </summary>
        public int FloorNumber
        {
            get => m_floorNumber;
            set
            {
                if (value < FloorNumberMinValue || value > FloorNumberMaxValue)
                {
                    var message = "Number of floors should no less than "
                                  + FloorNumberMinValue + " and no more than "
                                  + FloorNumberMaxValue;
                    throw new ErrorMessageException(message);
                }

                CheckTotalNumber(XNumber * YNumber * (value - 1));
                m_floorNumber = value;
            }
        }

        /// <summary>
        ///     increased height of autogenerated levels
        /// </summary>
        public double LevelHeight
        {
            get => Math.Round(m_levelHeight, DigitPrecision);
            set
            {
                if (value < LevelHeightMinValue || value > LevelHeightMaxValue)
                {
                    var message = "The distance between columns shoule no less than "
                                  + LevelHeightMinValue + "and no more than "
                                  + LevelHeightMaxValue;
                    throw new ErrorMessageException(message);
                }

                m_distance = value;
            }
        }

        /// <summary>
        ///     bottom left point of the frame
        /// </summary>
        public UV FrameOrigin { get; set; } = new UV(0.0, 0.0);

        /// <summary>
        ///     the angle to rotate around bottom left point
        /// </summary>
        public double FrameOriginAngle { get; set; }

        /// <summary>
        ///     column's type
        /// </summary>
        public FamilySymbol ColumnSymbol { get; private set; }

        /// <summary>
        ///     beam's type
        /// </summary>
        public FamilySymbol BeamSymbol { get; private set; }

        /// <summary>
        ///     brace's type
        /// </summary>
        public FamilySymbol BraceSymbol { get; private set; }

        /// <summary>
        ///     list of all levels in the ordr of Elevation
        /// </summary>
        public SortedList<double, Level> Levels { get; }

        /// <summary>
        ///     the number of levels before invoke external command
        /// </summary>
        public int OriginalLevelSize { get; private set; }

        /// <summary>
        ///     create FramingData object. applicationException will throw out,
        ///     if current Revit document doesn't satisfy the condition to create framing
        /// </summary>
        /// <param name="commandData"></param>
        /// <returns></returns>
        public static FrameData CreateInstance(ExternalCommandData commandData)
        {
            var data = new FrameData(commandData);
            data.Initialize();
            data.Validate();

            // initialize members after checking precondition
            data.m_floorNumber = data.Levels.Count - 1 > 0 ? data.Levels.Count - 1 : 1;
            data.ColumnSymbol = data.ColumnSymbolsMgr.FramingSymbols[0];
            data.BeamSymbol = data.BeamSymbolsMgr.FramingSymbols[0];
            data.BraceSymbol = data.BeamSymbolsMgr.FramingSymbols[0];
            data.m_levelHeight = data.Levels.Values[data.Levels.Count - 1].Elevation
                                 - data.Levels.Values[data.Levels.Count - 2].Elevation;

            return data;
        }

        /// <summary>
        ///     cast object to FamilySymbol and set as column's type
        /// </summary>
        /// <param name="obj">FamilySymbol object</param>
        /// <returns>failed to cast and set</returns>
        public bool SetColumnSymbol(object obj)
        {
            if (!(obj is FamilySymbol symbol)) return false;
            ColumnSymbol = symbol;
            return true;
        }

        /// <summary>
        ///     cast object to FamilySymbol and set as beam's type
        /// </summary>
        /// <param name="obj">FamilySymbol object</param>
        /// <returns>failed to cast and set</returns>
        public bool SetBeamSymbol(object obj)
        {
            if (!(obj is FamilySymbol symbol)) return false;
            BeamSymbol = symbol;
            return true;
        }

        /// <summary>
        ///     cast object to FamilySymbol and set as brace's type
        /// </summary>
        /// <param name="obj">FamilySymbol object</param>
        /// <returns>failed to cast and set</returns>
        public bool SetBraceSymbol(object obj)
        {
            if (!(obj is FamilySymbol symbol)) return false;
            BraceSymbol = symbol;
            return true;
        }

        /// <summary>
        ///     add more levels so that level number can meet floor number
        /// </summary>
        public void UpdateLevels()
        {
            var baseElevation = Levels.Values[Levels.Count - 1].Elevation;
            var doc = CommandData.Application.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(doc);
            var viewFamilyTypes = collector.OfClass(typeof(ViewFamilyType)).ToElements();
            var floorPlanId = ElementId.InvalidElementId;
            foreach (var e in viewFamilyTypes)
            {
                if (e is ViewFamilyType v && v.ViewFamily == ViewFamily.FloorPlan)
                {
                    floorPlanId = e.Id;
                    break;
                }
            }

            var newLevelSize = m_floorNumber + 1 - Levels.Count;
            if (newLevelSize == 0) return;

            for (var ii = 0; ii < newLevelSize; ii++)
            {
                var elevation = baseElevation + m_levelHeight * (ii + 1);
                var newLevel = Level.Create(CommandData.Application.ActiveUIDocument.Document, elevation);
                //createDoc.NewViewPlan(newLevel.Name, newLevel, Autodesk.Revit.DB.ViewPlanType.FloorPlan);
                var viewPlan = ViewPlan.Create(doc, floorPlanId, newLevel.Id);
                viewPlan.Name = newLevel.Name;
                Levels.Add(elevation, newLevel);
            }

            OriginalLevelSize = Levels.Count;
        }

        /// <summary>
        ///     check the total number of columns to create less than certain value
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static void CheckTotalNumber(int number)
        {
            if (number > TotalMaxValue)
            {
                var message = "The total number of columns should less than "
                              + TotalMaxValue;
                throw new ErrorMessageException(message);
            }
        }

        /// <summary>
        ///     initialize list of column, beam and brace's type;
        ///     initialize list of level
        /// </summary>
        private void Initialize()
        {
            var doc = CommandData.Application.ActiveUIDocument.Document;

            var collector1 = new FilteredElementCollector(doc);
            var a1 = collector1.OfClass(typeof(Level)).ToElements();

            foreach (Level lev in a1) Levels.Add(lev.Elevation, lev);

            a1.Clear();

            var categories = doc.Settings.Categories;
            var bipColumn = BuiltInCategory.OST_StructuralColumns;
            var bipFraming = BuiltInCategory.OST_StructuralFraming;
            var idColumn = categories.get_Item(bipColumn).Id;
            var idFraming = categories.get_Item(bipFraming).Id;

            var filterColumn = new ElementCategoryFilter(bipColumn);
            var filterFraming = new ElementCategoryFilter(bipFraming);
            var orFilter = new LogicalOrFilter(filterColumn, filterFraming);

            var filterSymbol = new ElementClassFilter(typeof(FamilySymbol));
            var andFilter = new LogicalAndFilter(orFilter, filterSymbol);
            //
            // without filtering for the structural categories,
            // our sample project was returning over 500 family symbols;
            // adding the category filters reduced this number to 40:
            //
            var collector2 = new FilteredElementCollector(doc);
            var a2 = collector2.WherePasses(andFilter).ToElements();

            foreach (FamilySymbol symbol in a2)
            {
                var categoryId = symbol.Category.Id;

                if (idFraming.Equals(categoryId))
                    BeamSymbolsMgr.AddSymbol(symbol);
                else if (idColumn.Equals(categoryId)) ColumnSymbolsMgr.AddSymbol(symbol);
            }
        }

        /// <summary>
        ///     validate the precondition to create framing
        /// </summary>
        private void Validate()
        {
            // level shouldn't less than 2
            if (Levels.Count < 2)
                throw new ErrorMessageException("The level's number in active document is less than 2.");
            // no Structural Column family is loaded in current document
            if (ColumnSymbolsMgr.Size == 0)
                throw new ErrorMessageException("No Structural Column family is loaded in current project.");
            // no Structural Beam family is loaded in current document
            if (BeamSymbolsMgr.Size == 0)
                throw new ErrorMessageException("No Structural Beam family is loaded in current project.");
        }
    }
}
