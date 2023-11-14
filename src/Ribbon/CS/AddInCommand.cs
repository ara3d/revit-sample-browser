//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Ribbon.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand, create a wall
   /// all the properties for new wall comes from user selection in Ribbon
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class CreateWall : IExternalCommand
   {
      public static ElementSet CreatedWalls = new ElementSet(); //restore all the walls created by API.

      #region IExternalCommand Members Implementation
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
         { newWall = Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id, true); }
         else { newWall = Wall.Create(app.ActiveUIDocument.Document, newWallShape, newWallType.Id, newWallLevel.Id, false); }
         if (null != newWall)
         {
            newWall.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).Set(newWallMark); //set new wall's mark
            CreatedWalls.Insert(newWall);
         }
         trans.Commit();
         return Result.Succeeded;
      }

      #endregion IExternalCommand Members Implementation

      #region protected methods
      protected WallType GetNewWallType(UIApplication app)
      {
         var myPanel = app.GetRibbonPanels()[0];
         var radioGroupTypeSelector =
             GetRibbonItemByName(myPanel, "WallTypeSelector") as RadioButtonGroup;
         if (null == radioGroupTypeSelector) { throw new InvalidCastException("Cannot get Wall Type selector!"); }
         var wallTypeName = radioGroupTypeSelector.Current.ItemText;
         WallType newWallType = null;
         var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
         ICollection<Element> founds = collector.OfClass(typeof(WallType)).ToElements();
         foreach (var elem in founds)
         {
            var wallType = elem as WallType;
            if (wallType.Name.StartsWith(wallTypeName))
            {
               newWallType = wallType; break;
            }
         }

         return newWallType;
      }

      protected Level GetNewWallLevel(UIApplication app)
      {
         var myPanel = app.GetRibbonPanels()[0];
         var comboboxLevel =
             GetRibbonItemByName(myPanel, "LevelsSelector") as ComboBox;
         if (null == comboboxLevel) { throw new InvalidCastException("Cannot get Level selector!"); }
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
               newWallLevel = level; break;
            }
         }

         return newWallLevel;
      }

      protected List<Curve> GetNewWallShape(UIApplication app)
      {
         var myPanel = app.GetRibbonPanels()[0];
         var comboboxWallShape =
             GetRibbonItemByName(myPanel, "WallShapeComboBox") as ComboBox;
         if (null == comboboxWallShape) { throw new InvalidCastException("Cannot get Wall Shape Gallery!"); }
         var wallShape = comboboxWallShape.Current.ItemText;
         if ("SquareWall" == wallShape) { return GetSquareWallShape(app.Application.Create); }
         else if ("CircleWall" == wallShape) { return GetCircleWallShape(app.Application.Create); }
         else if ("TriangleWall" == wallShape) { return GetTriangleWallShape(app.Application.Create); }
         else { return GetRectangleWallShape(app.Application.Create); }
      }

      protected string GetNewWallMark(UIApplication app)
      {
         var myPanel = app.GetRibbonPanels()[0];
         var textBox =
             GetRibbonItemByName(myPanel, "WallMark") as TextBox;
         if (null == textBox) { throw new InvalidCastException("Cannot get Wall Mark TextBox!"); }
         string newWallMark;
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
               {
                  try
                  {
                     var index = Convert.ToInt32(strings[strings.Length - 1]);
                     if (index > newWallIndex) { newWallIndex = index; }
                  }
                  catch (Exception)
                  {
                     continue;
                  }
               }
            }
         }
         newWallMark = textBox.Value.ToString() + '_' + (newWallIndex + 1);
         return newWallMark;
      }

      protected List<Curve> GetRectangleWallShape(Autodesk.Revit.Creation.Application creApp)
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

      protected List<Curve> GetSquareWallShape(Autodesk.Revit.Creation.Application creApp)
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

      protected List<Curve> GetCircleWallShape(Autodesk.Revit.Creation.Application creApp)
      {
         //calculate size of Structural and NonStructural walls
         var WallsSize = CreatedWalls.Size + CreatedWalls.Size;
         var curves = new List<Curve>();
         //15: distance from each wall, 40: diameter of circle  
         var arc = Arc.Create(new XYZ(WallsSize * 15, 20, 0), new XYZ(WallsSize * 15, 20, 40), new XYZ(WallsSize * 15, 40, 20));
         var arc2 = Arc.Create(new XYZ(WallsSize * 15, 20, 0), new XYZ(WallsSize * 15, 20, 40), new XYZ(WallsSize * 15, 0, 20));
         curves.Add(arc);
         curves.Add(arc2);
         return curves;
      }

      protected List<Curve> GetTriangleWallShape(Autodesk.Revit.Creation.Application creApp)
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
      #endregion

      /// <summary>
      /// return the RibbonItem by the input name in a specific panel
      /// </summary>
      /// <param name="panelRibbon">RibbonPanel which contains the RibbonItem </param>
      /// <param name="itemName">name of RibbonItem</param>
      /// <return>RibbonItem whose name is same with input string</param>
      public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
      {
         foreach (var item in panelRibbon.GetItems())
         {
            if (itemName == item.Name)
            {
               return item;
            }
         }

         return null;
      }
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand,create a structural wall
   /// all the properties for new wall comes from user selection in Ribbon
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class CreateStructureWall : CreateWall
   {
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand, 
   /// delete all the walls which create by Ribbon sample
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class DeleteWalls : IExternalCommand
   {
      #region IExternalCommand Members Implementation
      public Result Execute(ExternalCommandData revit,
                                             ref string message,
                                             ElementSet elements)
      {
         // delete all the walls which create by RibbonSample
         var wallSet = CreateWall.CreatedWalls;
         var trans = new Transaction(revit.Application.ActiveUIDocument.Document, "DeleteWalls");
         trans.Start();
         foreach (Element e in wallSet)
         {
             revit.Application.ActiveUIDocument.Document.Delete(e.Id);
         }
         CreateWall.CreatedWalls.Clear();
         trans.Commit();
         return Result.Succeeded;
      }
      #endregion IExternalCommand Members Implementation
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand,Move walls, X direction
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class XMoveWalls : IExternalCommand
   {
      #region IExternalCommand Members Implementation

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
            if (null != wall)
            {
               wall.Location.Move(new XYZ(12, 0, 0));
            }
         }
         trans.Commit();
         return Result.Succeeded;
      }
      #endregion IExternalCommand Members Implementation
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand,Move walls, Y direction
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class YMoveWalls : IExternalCommand
   {
      #region IExternalCommand Members Implementation

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
            if (null != wall)
            {
               wall.Location.Move(new XYZ(0, 12, 0));
            }
         }
        trans.Commit();
         return Result.Succeeded;
      }
      #endregion IExternalCommand Members Implementation
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand,
   /// Reset all the Ribbon options to default, such as level, wall type...
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class ResetSetting : IExternalCommand
   {
      #region IExternalCommand Members Implementation

      public Result Execute(ExternalCommandData revit,
                                             ref string message,
                                             ElementSet elements)
      {
         var myPanel = revit.Application.GetRibbonPanels()[0];
         //reset wall type
         var radioGroupTypeSelector =
             GetRibbonItemByName(myPanel, "WallTypeSelector") as RadioButtonGroup;
         if (null == radioGroupTypeSelector) { throw new InvalidCastException("Cannot get Wall Type selector!"); }
         radioGroupTypeSelector.Current = radioGroupTypeSelector.GetItems()[0];

         //reset level
         var comboboxLevel =
             GetRibbonItemByName(myPanel, "LevelsSelector") as ComboBox;
         if (null == comboboxLevel) { throw new InvalidCastException("Cannot get Level selector!"); }
         comboboxLevel.Current = comboboxLevel.GetItems()[0];

         //reset wall shape
         var comboboxWallShape =
             GetRibbonItemByName(myPanel, "WallShapeComboBox") as ComboBox;
         if (null == comboboxLevel) { throw new InvalidCastException("Cannot get wall shape combo box!"); }
         comboboxWallShape.Current = comboboxWallShape.GetItems()[0];

         //get wall mark
         var textBox =
             GetRibbonItemByName(myPanel, "WallMark") as TextBox;
         if (null == textBox) { throw new InvalidCastException("Cannot get Wall Mark TextBox!"); }
         textBox.Value = "new wall";

         return Result.Succeeded;
      }

      /// <summary>
      /// return the RibbonItem by the input name in a specific panel
      /// </summary>
      /// <param name="panelRibbon">RibbonPanel which contains the RibbonItem </param>
      /// <param name="itemName">name of RibbonItem</param>
      /// <return>RibbonItem whose name is same with input string</param>
      public RibbonItem GetRibbonItemByName(RibbonPanel panelRibbon, string itemName)
      {
         foreach (var item in panelRibbon.GetItems())
         {
            if (itemName == item.Name)
            {
               return item;
            }
         }

         return null;
      }

      #endregion IExternalCommand Members Implementation
   }

   /// <summary>
   /// Do Nothing, 
   /// Create this just because ToggleButton have to bind to a ExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class Dummy : IExternalCommand
   {
      #region IExternalCommand Members Implementation

      public Result Execute(ExternalCommandData revit,
                                             ref string message,
                                             ElementSet elements)
      {
         return Result.Succeeded;
      }

      #endregion IExternalCommand Members Implementation
   }
}
