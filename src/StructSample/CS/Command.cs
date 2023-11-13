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
using System.Windows.Forms;

using Autodesk;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.StructSample.CS
{
   /// <summary>
   ///   This command places a set of columns in the selected wall.
   ///   Note that Revit uses Feet as an internal length unit. 
   ///   
   ///   To run this sample, 
   ///   (1) load the column family type of "M_Wood Timber Column", "191 x 292mm" 
   ///       (It is hard-coded in the program.) 
   ///   (2) Draw some walls, and constrain their top and bottom to the levels in the properties dialog. 
   ///   (3) Run the command. 
   ///       It will place columns along each wall with the interval of 5 feet. (The interval is also hard coded.) 
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      #region Interface implementation
      /// <summary>
      /// Implement this method as an external command for Revit.
      /// </summary>
      /// <param name="commandData">An object that is passed to the external application 
      /// which contains data related to the command, 
      /// such as the application object and active view.</param>
      /// <param name="message">A message that can be set by the external application 
      /// which will be displayed if a failure or cancellation is returned by 
      /// the external command.</param>
      /// <param name="elements">A set of elements to which the external application 
      /// can add elements that are to be highlighted in case of failure or cancellation.</param>
      /// <returns>Return the status of the external command. 
      /// A result of Succeeded means that the API external method functioned as expected. 
      /// Cancelled can be used to signify that the user cancelled the external operation 
      /// at some point. Failure should be returned if the application is unable to proceed with 
      /// the operation.</returns>
      public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref String message, Autodesk.Revit.DB.ElementSet elements)
      {
         try
         {
            var rvtApp = commandData.Application;
            var rvtDoc = rvtApp.ActiveUIDocument;
            var ss = new ElementSet();
            foreach (var elementId in rvtDoc.Selection.GetElementIds())
            {
               ss.Insert(rvtDoc.Document.GetElement(elementId));
            }

            var tran = new Transaction(rvtDoc.Document, "StructSample");
            tran.Start();

            var walls = rvtApp.Application.Create.NewElementSet();

            //  iterate through a selection set, and collect walls which are constrained at the top and the bottom.
            foreach (Autodesk.Revit.DB.Element elem in ss)
            {
               if (elem.GetType() == typeof(Autodesk.Revit.DB.Wall))
               {
                  if (elem.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.WALL_HEIGHT_TYPE) != null
                      && elem.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.WALL_BASE_CONSTRAINT) != null)
                  {
                     walls.Insert(elem);
                  }
               }
            }

            //  how many did we get? 
            TaskDialog.Show("Revit", "# of constrained walls in the selection set is " + walls.Size);
            if (walls.Size == 0)
            {
               message = "You must select some walls that are constrained top or bottom";
               return Autodesk.Revit.UI.Result.Failed;
            }

            //  next, we need a column symbol. For simplicity, the symbol name is hard-coded here. 
            var colType = FindFamilySymbol(rvtDoc.Document, "M_Wood Timber Column", "191 x 292mm");
            if (colType == null)
            {
               TaskDialog.Show("Revit", "failed to got a symbol. Please load the M_Wood Timber Column : 191 x 292mm family");
               message = "Please load the M_Wood Timber Column : 191 x 292mm family";

               return Autodesk.Revit.UI.Result.Failed;
            }

            //  place columns.
            double spacing = 5;  //  Spacing in feet hard coded. Note: Revit's internal length unit is feet. 
            foreach (Autodesk.Revit.DB.Wall wall in walls)
            {
               FrameWall(rvtApp.Application, wall, spacing, colType);
            }
            tran.Commit();

            //  return succeeded info. 
            return Autodesk.Revit.UI.Result.Succeeded;
         }
         catch (Exception ex)
         {
            message = ex.ToString();
            return Autodesk.Revit.UI.Result.Failed;
         }
      }
      #endregion


      /// <summary>
      /// find Column which will be used to placed to Wall
      /// </summary>
      /// <param name="rvtDoc">Revit document</param>
      /// <param name="familyName">Family name of Column</param>
      /// <param name="symbolName">Symbol of Column</param>
      /// <returns></returns>
      private FamilySymbol FindFamilySymbol(Document rvtDoc, string familyName, string symbolName)
      {
         var collector = new FilteredElementCollector(rvtDoc);
         var itr = collector.OfClass(typeof(Family)).GetElementIterator();
         itr.Reset();
         while (itr.MoveNext())
         {
            var elem = (Autodesk.Revit.DB.Element)itr.Current;
            if (elem.GetType() == typeof(Autodesk.Revit.DB.Family))
            {
               if (elem.Name == familyName)
               {
                  var family = (Autodesk.Revit.DB.Family)elem;
                  foreach (var symbolId in family.GetFamilySymbolIds())
                  {
                     var symbol = (Autodesk.Revit.DB.FamilySymbol)rvtDoc.GetElement(symbolId);
                     if (symbol.Name == symbolName)
                     {
                        return symbol;
                     }
                  }
               }
            }
         }
         return null;
      }


      /// <summary>
      /// Frame a Wall
      /// </summary>
      /// <param name="rvtApp">Revit application></param>
      /// <param name="wall">Wall as host to place column objects</param>
      /// <param name="spacing">spacing between two columns</param>
      /// <param name="columnType">column type</param>
      private void FrameWall(Autodesk.Revit.ApplicationServices.Application rvtApp, Autodesk.Revit.DB.Wall wall,
          double spacing, Autodesk.Revit.DB.FamilySymbol columnType)
      {
         var rvtDoc = wall.Document;

         // get wall location
         var loc = (Autodesk.Revit.DB.LocationCurve)wall.Location;
         var startPt = loc.Curve.GetEndPoint(0);
         var endPt = loc.Curve.GetEndPoint(1);

         // get wall's vector
         var wallVec = new Autodesk.Revit.DB.UV(
             endPt.X - startPt.X,
             endPt.Y - startPt.Y);

         // get the axis vector
         var axis = new Autodesk.Revit.DB.UV(1.0, 0.0);

         var baseLevelId = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
         var topLevelId = wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();

         // get wall length and vector
         var wallLength = wallVec.GetLength();
         wallVec = wallVec.Normalize();

         // get # of column
         var nmax = (int)(wallLength / spacing);

         TaskDialog.Show("Revit", "wallLength = " + wallLength + "\r\nspacing = " + spacing.ToString() + "\r\nnmax = " + nmax.ToString());

         // get angle of wall and axis
         var angle = wallVec.AngleTo(axis);

         // place all column
         var loc2 = startPt;
         var dx = wallVec.U * spacing;
         var dy = wallVec.V * spacing;
         for (var i = 0; i < nmax; i++)
         {
            PlaceColumn(rvtApp, rvtDoc, loc2, angle, columnType, baseLevelId, topLevelId);

            loc2 = new XYZ(loc2.X + dx,
                loc2.Y + dy,
                loc2.Z);
         }

         // place column at end point of wall
         PlaceColumn(rvtApp, rvtDoc, endPt, angle, columnType, baseLevelId, topLevelId);
      }


      /// <summary>
      /// create a column instance and place it on the wall line.
      /// </summary>
      /// <param name="rvtApp">revit application</param>
      /// <param name="rvtDoc">revit document</param>
      /// <param name="point2">location for placing column</param>
      /// <param name="angle">column angle</param>
      /// <param name="columnType">column type placed in Wall</param>
      /// <param name="baseLevelId">level id for base level where column is placed</param>
      /// <param name="topLevelId">level id for top level where column is placed</param>
      private void PlaceColumn(Autodesk.Revit.ApplicationServices.Application rvtApp, Document rvtDoc, Autodesk.Revit.DB.XYZ point2,
          double angle, FamilySymbol columnType, Autodesk.Revit.DB.ElementId baseLevelId, Autodesk.Revit.DB.ElementId topLevelId)
      {
         var point = point2;

         // Note: Must use level-hosted NewFamilyInstance!
         var instLevel = (Level)rvtDoc.GetElement(baseLevelId);
         if (!columnType.IsActive)
            columnType.Activate();
         var column = rvtDoc.Create.NewFamilyInstance(point, columnType,
             instLevel, Autodesk.Revit.DB.Structure.StructuralType.Column);
         if (column == null)
         {
            TaskDialog.Show("Revit", "failed to create an instance of a column.");
            return;
         }

         // rotate column to place it to right location
         var zVec = new Autodesk.Revit.DB.XYZ(0, 0, 1);
         var axis = Line.CreateUnbound(point, zVec);
         column.Location.Rotate(axis, angle);

         // Set the level Ids
         var baseLevelParameter = column.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
         var topLevelParameter = column.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.FAMILY_TOP_LEVEL_PARAM); ;
         baseLevelParameter.Set(baseLevelId);
         topLevelParameter.Set(topLevelId);
      }
   }
}
