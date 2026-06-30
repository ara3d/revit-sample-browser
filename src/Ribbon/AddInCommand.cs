// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.Ribbon.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateWall : IExternalCommand
    {
        public static readonly ElementSet CreatedWalls = new(); //restore all the walls created by API.

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document, "CreateWall");
            trans.Start();
            var app = revit.Application;

            var newWallType = GetNewWallType(app);
            var newWallLevel = GetNewWallLevel(app);
            var newWallShape = GetNewWallShape(app);
            var newWallMark = GetNewWallMark(app);

            var newWall = "CreateStructureWall" == GetType().Name
                ? Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id,
                    true)
                : Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id,
                    false);
            if (null != newWall)
            {
                newWall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(newWallMark);
                CreatedWalls.Insert(newWall);
            }

            trans.Commit();
            return Result.Succeeded;
        }

        protected WallType GetNewWallType(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (GetRibbonItemByName(myPanel, "WallTypeSelector") is not RadioButtonGroup radioGroupTypeSelector) throw new InvalidCastException("Cannot get Wall Type selector!");
            var wallTypeName = radioGroupTypeSelector.Current.ItemText;
            FilteredElementCollector collector = new(app.ActiveUIDocument.Document);
            return collector.OfClass(typeof(WallType)).ToElements().OfType<WallType>().FirstOrDefault(wallType => wallType.Name.StartsWith(wallTypeName));
        }

        protected Level GetNewWallLevel(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (GetRibbonItemByName(myPanel, "LevelsSelector") is not ComboBox comboboxLevel) throw new InvalidCastException("Cannot get Level selector!");
            var wallLevel = comboboxLevel.Current.ItemText;
            //find wall type in document
            Level newWallLevel = null;
            FilteredElementCollector collector = new(app.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(Level)).ToElements();
            foreach (var elem in founds)
            {
                var level = elem as Level;
                if (level.Name.StartsWith(wallLevel))
                {
                    newWallLevel = level;
                    break;
                }
            }

            return newWallLevel;
        }

        protected List<Curve> GetNewWallShape(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (GetRibbonItemByName(myPanel, "WallShapeComboBox") is not ComboBox comboboxWallShape) throw new InvalidCastException("Cannot get Wall Shape Gallery!");
            var wallShape = comboboxWallShape.Current.ItemText;
            return wallShape switch
            {
                "SquareWall" => GetSquareWallShape(app.Application.Create),
                "CircleWall" => GetCircleWallShape(app.Application.Create),
                "TriangleWall" => GetTriangleWallShape(app.Application.Create),
                _ => GetRectangleWallShape(app.Application.Create),
            };
        }

        protected string GetNewWallMark(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (GetRibbonItemByName(myPanel, "WallMark") is not TextBox textBox) throw new InvalidCastException("Cannot get Wall Mark TextBox!");

            var newWallIndex = 0;
            FilteredElementCollector collector = new(app.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(Wall)).ToElements();
            foreach (var elem in founds)
            {
                var wall = elem as Wall;
                var wallMark = wall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                if (wallMark.StartsWith(textBox.Value.ToString()) && wallMark.Contains('_'))
                {
                    char[] chars = { '_' };
                    var strings = wallMark.Split(chars);
                    if (strings.Length >= 2)
                        try
                        {
                            var index = Convert.ToInt32(strings[strings.Length - 1]);
                            if (index > newWallIndex) newWallIndex = index;
                        }
                        catch (Exception)
                        {
                        }
                }
            }

            var newWallMark = $"{textBox.Value}_{newWallIndex + 1}";
            return newWallMark;
        }

        protected List<Curve> GetRectangleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var wallsSize = CreatedWalls.Size + CreatedWalls.Size;
            List<Curve> curves = [];
            //15: distance from each wall, 60: wall length , 60: wall width 
            var line1 = Line.CreateBound(new XYZ(wallsSize * 15, 0, 0), new XYZ(wallsSize * 15, 60, 0));
            var line2 = Line.CreateBound(new XYZ(wallsSize * 15, 60, 0), new XYZ(wallsSize * 15, 60, 40));
            var line3 = Line.CreateBound(new XYZ(wallsSize * 15, 60, 40), new XYZ(wallsSize * 15, 0, 40));
            var line4 = Line.CreateBound(new XYZ(wallsSize * 15, 0, 40), new XYZ(wallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            curves.Add(line4);
            return curves;
        }

        protected List<Curve> GetSquareWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var wallsSize = CreatedWalls.Size + CreatedWalls.Size;
            List<Curve> curves = [];
            //15: distance from each wall, 40: wall length  
            var line1 = Line.CreateBound(new XYZ(wallsSize * 15, 0, 0), new XYZ(wallsSize * 15, 40, 0));
            var line2 = Line.CreateBound(new XYZ(wallsSize * 15, 40, 0), new XYZ(wallsSize * 15, 40, 40));
            var line3 = Line.CreateBound(new XYZ(wallsSize * 15, 40, 40), new XYZ(wallsSize * 15, 0, 40));
            var line4 = Line.CreateBound(new XYZ(wallsSize * 15, 0, 40), new XYZ(wallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            curves.Add(line4);
            return curves;
        }

        protected List<Curve> GetCircleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var wallsSize = CreatedWalls.Size + CreatedWalls.Size;
            List<Curve> curves = [];
            //15: distance from each wall, 40: diameter of circle  
            var arc = Arc.Create(new XYZ(wallsSize * 15, 20, 0), new XYZ(wallsSize * 15, 20, 40),
                new XYZ(wallsSize * 15, 40, 20));
            var arc2 = Arc.Create(new XYZ(wallsSize * 15, 20, 0), new XYZ(wallsSize * 15, 20, 40),
                new XYZ(wallsSize * 15, 0, 20));
            curves.Add(arc);
            curves.Add(arc2);
            return curves;
        }

        protected List<Curve> GetTriangleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var wallsSize = CreatedWalls.Size + CreatedWalls.Size;
            List<Curve> curves = [];
            //15: distance from each wall, 40: height of triangle  
            var line1 = Line.CreateBound(new XYZ(wallsSize * 15, 0, 0), new XYZ(wallsSize * 15, 40, 0));
            var line2 = Line.CreateBound(new XYZ(wallsSize * 15, 40, 0), new XYZ(wallsSize * 15, 20, 40));
            var line3 = Line.CreateBound(new XYZ(wallsSize * 15, 20, 40), new XYZ(wallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            return curves;
        }

        public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
        {
            foreach (var item in panelRibbon.GetItems())
            {
                if (itemName == item.Name)
                    return item;
            }

            return null;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateStructureWall : CreateWall
    {
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeleteWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            // delete all the walls which create by RibbonSample
            var wallSet = CreateWall.CreatedWalls;
            Transaction trans = new(revit.Application.ActiveUIDocument.Document, "DeleteWalls");
            trans.Start();
            foreach (Element e in wallSet)
            {
                revit.Application.ActiveUIDocument.Document.Delete(e.Id);
            }

            CreateWall.CreatedWalls.Clear();
            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class XMoveWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document, "XMoveWalls");
            trans.Start();
            var iter = CreateWall.CreatedWalls.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var wall = iter.Current as Wall;
                wall?.Location.Move(new XYZ(12, 0, 0));
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class YMoveWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            Transaction trans = new(revit.Application.ActiveUIDocument.Document, "YMoveWalls");
            trans.Start();
            var iter = CreateWall.CreatedWalls.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var wall = iter.Current as Wall;
                wall?.Location.Move(new XYZ(0, 12, 0));
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ResetSetting : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var myPanel = revit.Application.GetRibbonPanels()[0];
            //reset wall type
            if (GetRibbonItemByName(myPanel, "WallTypeSelector") is not RadioButtonGroup radioGroupTypeSelector) throw new InvalidCastException("Cannot get Wall Type selector!");
            radioGroupTypeSelector.Current = radioGroupTypeSelector.GetItems()[0];

            //reset level
            if (GetRibbonItemByName(myPanel, "LevelsSelector") is not ComboBox comboboxLevel) throw new InvalidCastException("Cannot get Level selector!");
            comboboxLevel.Current = comboboxLevel.GetItems()[0];

            //reset wall shape
            var comboboxWallShape =
                GetRibbonItemByName(myPanel, "WallShapeComboBox") as ComboBox;
            if (null == comboboxLevel) throw new InvalidCastException("Cannot get wall shape combo box!");
            comboboxWallShape.Current = comboboxWallShape.GetItems()[0];

            if (GetRibbonItemByName(myPanel, "WallMark") is not TextBox textBox) throw new InvalidCastException("Cannot get Wall Mark TextBox!");
            textBox.Value = "new wall";

            return Result.Succeeded;
        }

        public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
        {
            foreach (var item in panelRibbon.GetItems())
            {
                if (itemName == item.Name)
                    return item;
            }

            return null;
        }
    }

    /// <summary>
    ///     Do Nothing,
    ///     Create this just because ToggleButton have to bind to a ExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Dummy : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            return Result.Succeeded;
        }
    }
}
