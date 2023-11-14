// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Ribbon.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand, create a wall
    ///     all the properties for new wall comes from user selection in Ribbon
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateWall : IExternalCommand
    {
        public static readonly ElementSet CreatedWalls = new ElementSet(); //restore all the walls created by API.

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "CreateWall");
            trans.Start();
            var app = revit.Application;

            var newWallType = GetNewWallType(app); //get WallType from RadioButtonGroup - WallTypeSelector
            var newWallLevel = GetNewWallLevel(app); //get Level from Combobox - LevelsSelector
            var newWallShape = GetNewWallShape(app); //get wall Curve from Combobox - WallShapeComboBox
            var newWallMark = GetNewWallMark(app); //get mark of new wall from Text box - WallMark

            Wall newWall = null;
            if ("CreateStructureWall" == GetType().Name) //decided by SplitButton
                newWall = Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id,
                    true);
            else
                newWall = Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id,
                    false);
            if (null != newWall)
            {
                newWall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(newWallMark); //set new wall's mark
                CreatedWalls.Insert(newWall);
            }

            trans.Commit();
            return Result.Succeeded;
        }


        protected WallType GetNewWallType(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (!(GetRibbonItemByName(myPanel, "WallTypeSelector") is RadioButtonGroup radioGroupTypeSelector)) throw new InvalidCastException("Cannot get Wall Type selector!");
            var wallTypeName = radioGroupTypeSelector.Current.ItemText;
            WallType newWallType = null;
            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(WallType)).ToElements();
            foreach (var elem in founds)
            {
                var wallType = elem as WallType;
                if (wallType.Name.StartsWith(wallTypeName))
                {
                    newWallType = wallType;
                    break;
                }
            }

            return newWallType;
        }

        protected Level GetNewWallLevel(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (!(GetRibbonItemByName(myPanel, "LevelsSelector") is ComboBox comboboxLevel)) throw new InvalidCastException("Cannot get Level selector!");
            var wallLevel = comboboxLevel.Current.ItemText;
            //find wall type in document
            Level newWallLevel = null;
            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
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
            if (!(GetRibbonItemByName(myPanel, "WallShapeComboBox") is ComboBox comboboxWallShape)) throw new InvalidCastException("Cannot get Wall Shape Gallery!");
            var wallShape = comboboxWallShape.Current.ItemText;
            switch (wallShape)
            {
                case "SquareWall":
                    return GetSquareWallShape(app.Application.Create);
                case "CircleWall":
                    return GetCircleWallShape(app.Application.Create);
                case "TriangleWall":
                    return GetTriangleWallShape(app.Application.Create);
                default:
                    return GetRectangleWallShape(app.Application.Create);
            }
        }

        protected string GetNewWallMark(UIApplication app)
        {
            var myPanel = app.GetRibbonPanels()[0];
            if (!(GetRibbonItemByName(myPanel, "WallMark") is TextBox textBox)) throw new InvalidCastException("Cannot get Wall Mark TextBox!");

            var newWallIndex = 0;
            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
            ICollection<Element> founds = collector.OfClass(typeof(Wall)).ToElements();
            foreach (var elem in founds)
            {
                var wall = elem as Wall;
                var wallMark = wall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();
                if (wallMark.StartsWith(textBox.Value.ToString()) && wallMark.Contains('_'))
                {
                    //get the index for new wall (wall_1, wall_2...)
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

            var newWallMark = textBox.Value.ToString() + '_' + (newWallIndex + 1);
            return newWallMark;
        }

        protected List<Curve> GetRectangleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var WallsSize = CreatedWalls.Size + CreatedWalls.Size;
            var curves = new List<Curve>();
            //15: distance from each wall, 60: wall length , 60: wall width 
            var line1 = Line.CreateBound(new XYZ(WallsSize * 15, 0, 0), new XYZ(WallsSize * 15, 60, 0));
            var line2 = Line.CreateBound(new XYZ(WallsSize * 15, 60, 0), new XYZ(WallsSize * 15, 60, 40));
            var line3 = Line.CreateBound(new XYZ(WallsSize * 15, 60, 40), new XYZ(WallsSize * 15, 0, 40));
            var line4 = Line.CreateBound(new XYZ(WallsSize * 15, 0, 40), new XYZ(WallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            curves.Add(line4);
            return curves;
        }

        protected List<Curve> GetSquareWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var WallsSize = CreatedWalls.Size + CreatedWalls.Size;
            var curves = new List<Curve>();
            //15: distance from each wall, 40: wall length  
            var line1 = Line.CreateBound(new XYZ(WallsSize * 15, 0, 0), new XYZ(WallsSize * 15, 40, 0));
            var line2 = Line.CreateBound(new XYZ(WallsSize * 15, 40, 0), new XYZ(WallsSize * 15, 40, 40));
            var line3 = Line.CreateBound(new XYZ(WallsSize * 15, 40, 40), new XYZ(WallsSize * 15, 0, 40));
            var line4 = Line.CreateBound(new XYZ(WallsSize * 15, 0, 40), new XYZ(WallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            curves.Add(line4);
            return curves;
        }

        protected List<Curve> GetCircleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var WallsSize = CreatedWalls.Size + CreatedWalls.Size;
            var curves = new List<Curve>();
            //15: distance from each wall, 40: diameter of circle  
            var arc = Arc.Create(new XYZ(WallsSize * 15, 20, 0), new XYZ(WallsSize * 15, 20, 40),
                new XYZ(WallsSize * 15, 40, 20));
            var arc2 = Arc.Create(new XYZ(WallsSize * 15, 20, 0), new XYZ(WallsSize * 15, 20, 40),
                new XYZ(WallsSize * 15, 0, 20));
            curves.Add(arc);
            curves.Add(arc2);
            return curves;
        }

        protected List<Curve> GetTriangleWallShape(Application creApp)
        {
            //calculate size of Structural and NonStructural walls
            var WallsSize = CreatedWalls.Size + CreatedWalls.Size;
            var curves = new List<Curve>();
            //15: distance from each wall, 40: height of triangle  
            var line1 = Line.CreateBound(new XYZ(WallsSize * 15, 0, 0), new XYZ(WallsSize * 15, 40, 0));
            var line2 = Line.CreateBound(new XYZ(WallsSize * 15, 40, 0), new XYZ(WallsSize * 15, 20, 40));
            var line3 = Line.CreateBound(new XYZ(WallsSize * 15, 20, 40), new XYZ(WallsSize * 15, 0, 0));
            curves.Add(line1);
            curves.Add(line2);
            curves.Add(line3);
            return curves;
        }

        /// <summary>
        ///     return the RibbonItem by the input name in a specific panel
        /// </summary>
        /// <param name="panelRibbon">RibbonPanel which contains the RibbonItem </param>
        /// <param name="itemName">name of RibbonItem</param>
        /// <return>RibbonItem whose name is same with input string</param>
        public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
        {
            foreach (var item in panelRibbon.GetItems())
                if (itemName == item.Name)
                    return item;

            return null;
        }
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand,create a structural wall
    ///     all the properties for new wall comes from user selection in Ribbon
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateStructureWall : CreateWall
    {
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand,
    ///     delete all the walls which create by Ribbon sample
    /// </summary>
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
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "DeleteWalls");
            trans.Start();
            foreach (Element e in wallSet) revit.Application.ActiveUIDocument.Document.Delete(e.Id);
            CreateWall.CreatedWalls.Clear();
            trans.Commit();
            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand,Move walls, X direction
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class XMoveWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "XMoveWalls");
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

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand,Move walls, Y direction
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class YMoveWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "YMoveWalls");
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

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand,
    ///     Reset all the Ribbon options to default, such as level, wall type...
    /// </summary>
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
            if (!(GetRibbonItemByName(myPanel, "WallTypeSelector") is RadioButtonGroup radioGroupTypeSelector)) throw new InvalidCastException("Cannot get Wall Type selector!");
            radioGroupTypeSelector.Current = radioGroupTypeSelector.GetItems()[0];

            //reset level
            if (!(GetRibbonItemByName(myPanel, "LevelsSelector") is ComboBox comboboxLevel)) throw new InvalidCastException("Cannot get Level selector!");
            comboboxLevel.Current = comboboxLevel.GetItems()[0];

            //reset wall shape
            var comboboxWallShape =
                GetRibbonItemByName(myPanel, "WallShapeComboBox") as ComboBox;
            if (null == comboboxLevel) throw new InvalidCastException("Cannot get wall shape combo box!");
            comboboxWallShape.Current = comboboxWallShape.GetItems()[0];

            //get wall mark
            if (!(GetRibbonItemByName(myPanel, "WallMark") is TextBox textBox)) throw new InvalidCastException("Cannot get Wall Mark TextBox!");
            textBox.Value = "new wall";

            return Result.Succeeded;
        }

        /// <summary>
        ///     return the RibbonItem by the input name in a specific panel
        /// </summary>
        /// <param name="panelRibbon">RibbonPanel which contains the RibbonItem </param>
        /// <param name="itemName">name of RibbonItem</param>
        /// <return>RibbonItem whose name is same with input string</param>
        public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
        {
            foreach (var item in panelRibbon.GetItems())
                if (itemName == item.Name)
                    return item;

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
