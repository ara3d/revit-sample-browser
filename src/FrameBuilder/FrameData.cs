// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public class FrameData
    {
        private const int XNumberMaxValue = 50;
        private const int XNumberMinValue = 2;
        private const int XNumberDefault = 3;
        private const int YNumberMaxValue = 50;
        private const int YNumberMinValue = 2;
        private const int YNumberDefault = 3;
        private const int FloorNumberMinValue = 1;
        private const int FloorNumberMaxValue = 200;
        private const double DistanceMaxValue = 3000;
        private const double DistanceMinValue = 1;
        private const double DistanceDefault = 5;
        private const double LevelHeightMaxValue = 100;
        private const double LevelHeightMinValue = 1;
        private const int DigitPrecision = 4;
        private double m_distance = DistanceDefault;
        private int m_floorNumber;
        private double m_levelHeight;

        private int m_xNumber = XNumberDefault;
        private int m_yNumber = XNumberDefault;

        private FrameData(ExternalCommandData commandData)
        {
            CommandData = commandData;
            ColumnSymbolsMgr = new FrameTypesMgr(commandData);
            BeamSymbolsMgr = new FrameTypesMgr(commandData);
            Levels = [];
            OriginalLevelSize = Levels.Count;
            m_yNumber = YNumberDefault;
            m_xNumber = XNumberDefault;
            m_distance = DistanceDefault;
        }

        public ExternalCommandData CommandData { get; }

        public FrameTypesMgr ColumnSymbolsMgr { get; }

        public FrameTypesMgr BeamSymbolsMgr { get; }

        public FrameTypesMgr BraceSymbolsMgr => BeamSymbolsMgr;

        public int YNumber
        {
            get => m_yNumber;
            set
            {
                if (value is < YNumberMinValue or > YNumberMaxValue)
                {
                    var message =
                        $"Number of Columns in the Y Direction should no less than {YNumberMinValue} and no more than {YNumberMaxValue}";
                    throw new ErrorMessageException(message);
                }

                SampleBrowserUtils.CheckTotalNumber(value * XNumber * (m_floorNumber - 1));
                m_yNumber = value;
            }
        }

        public int XNumber
        {
            get => m_xNumber;
            set
            {
                if (value is < XNumberMinValue or > XNumberMaxValue)
                {
                    var message =
                        $"Number of Columns in the X Direction should no less than {XNumberMinValue} and no more than {XNumberMaxValue}";
                    throw new ErrorMessageException(message);
                }

                SampleBrowserUtils.CheckTotalNumber(value * YNumber * (m_floorNumber - 1));
                m_xNumber = value;
            }
        }

        public double Distance
        {
            get => m_distance;
            set
            {
                if (value is < DistanceMinValue or > DistanceMaxValue)
                {
                    var message =
                        $"The distance between columns shoule no less than {DistanceMinValue}and no more than {DistanceMaxValue}";
                    throw new ErrorMessageException(message);
                }

                m_distance = value;
            }
        }

        public int FloorNumber
        {
            get => m_floorNumber;
            set
            {
                if (value is < FloorNumberMinValue or > FloorNumberMaxValue)
                {
                    var message =
                        $"Number of floors should no less than {FloorNumberMinValue} and no more than {FloorNumberMaxValue}";
                    throw new ErrorMessageException(message);
                }

                SampleBrowserUtils.CheckTotalNumber(XNumber * YNumber * (value - 1));
                m_floorNumber = value;
            }
        }

        public double LevelHeight
        {
            get => Math.Round(m_levelHeight, DigitPrecision);
            set
            {
                if (value is < LevelHeightMinValue or > LevelHeightMaxValue)
                {
                    var message =
                        $"The distance between columns shoule no less than {LevelHeightMinValue}and no more than {LevelHeightMaxValue}";
                    throw new ErrorMessageException(message);
                }

                m_distance = value;
            }
        }

        public UV FrameOrigin { get; set; } = new UV(0.0, 0.0);

        public double FrameOriginAngle { get; set; }

        public FamilySymbol ColumnSymbol { get; private set; }

        public FamilySymbol BeamSymbol { get; private set; }

        public FamilySymbol BraceSymbol { get; private set; }

        public SortedList<double, Level> Levels { get; }

        public int OriginalLevelSize { get; private set; }

        public static FrameData CreateInstance(ExternalCommandData commandData)
        {
            FrameData data = new(commandData);
            data.Initialize();
            data.Validate();

            data.m_floorNumber = data.Levels.Count - 1 > 0 ? data.Levels.Count - 1 : 1;
            data.ColumnSymbol = data.ColumnSymbolsMgr.FramingSymbols[0];
            data.BeamSymbol = data.BeamSymbolsMgr.FramingSymbols[0];
            data.BraceSymbol = data.BeamSymbolsMgr.FramingSymbols[0];
            data.m_levelHeight = data.Levels.Values[data.Levels.Count - 1].Elevation
                                 - data.Levels.Values[data.Levels.Count - 2].Elevation;

            return data;
        }

        public bool SetColumnSymbol(object obj)
        {
            if (obj is not FamilySymbol symbol) return false;
            ColumnSymbol = symbol;
            return true;
        }

        public bool SetBeamSymbol(object obj)
        {
            if (obj is not FamilySymbol symbol) return false;
            BeamSymbol = symbol;
            return true;
        }

        public bool SetBraceSymbol(object obj)
        {
            if (obj is not FamilySymbol symbol) return false;
            BraceSymbol = symbol;
            return true;
        }

        public void UpdateLevels()
        {
            var baseElevation = Levels.Values[Levels.Count - 1].Elevation;
            var doc = CommandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector collector = new(doc);
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
                var elevation = baseElevation + (m_levelHeight * (ii + 1));
                var newLevel = Level.Create(CommandData.Application.ActiveUIDocument.Document, elevation);
                var viewPlan = ViewPlan.Create(doc, floorPlanId, newLevel.Id);
                viewPlan.Name = newLevel.Name;
                Levels.Add(elevation, newLevel);
            }

            OriginalLevelSize = Levels.Count;
        }

        private void Initialize()
        {
            var doc = CommandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector collector1 = new(doc);
            var a1 = collector1.OfClass(typeof(Level)).ToElements();

            foreach (Level lev in a1)
            {
                Levels.Add(lev.Elevation, lev);
            }

            a1.Clear();

            var categories = doc.Settings.Categories;
            var bipColumn = BuiltInCategory.OST_StructuralColumns;
            var bipFraming = BuiltInCategory.OST_StructuralFraming;
            var idColumn = categories.get_Item(bipColumn).Id;
            var idFraming = categories.get_Item(bipFraming).Id;

            ElementCategoryFilter filterColumn = new(bipColumn);
            ElementCategoryFilter filterFraming = new(bipFraming);
            LogicalOrFilter orFilter = new(filterColumn, filterFraming);

            ElementClassFilter filterSymbol = new(typeof(FamilySymbol));
            LogicalAndFilter andFilter = new(orFilter, filterSymbol);
            // Category filters reduce family symbols from 500+ to ~40 in the sample project.
            FilteredElementCollector collector2 = new(doc);
            var a2 = collector2.WherePasses(andFilter).ToElements();

            foreach (FamilySymbol symbol in a2)
            {
                var categoryId = symbol.Category.Id;

                if (idFraming.Equals(categoryId))
                    BeamSymbolsMgr.AddSymbol(symbol);
                else if (idColumn.Equals(categoryId)) ColumnSymbolsMgr.AddSymbol(symbol);
            }
        }

        private void Validate()
        {
            if (Levels.Count < 2)
                throw new ErrorMessageException("The level's number in active document is less than 2.");
            if (ColumnSymbolsMgr.Size == 0)
                throw new ErrorMessageException("No Structural Column family is loaded in current project.");
            if (BeamSymbolsMgr.Size == 0)
                throw new ErrorMessageException("No Structural Beam family is loaded in current project.");
        }
    }
}
