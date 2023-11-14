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

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace Revit.SDK.Samples.DimensionLeaderEnd.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class MoveHorizontally : IExternalCommand
   {
      private double m_delta = -10;
     public virtual Result Execute(ExternalCommandData commandData
          , ref string message, ElementSet elements)
      {
         // Get the handle of current document.
         var uidoc = commandData.Application.ActiveUIDocument;
         var doc = uidoc.Document;

         using (var _transaction_ = new Transaction(doc))
         {
            // Get the element selection of current document.
            var selection = uidoc.Selection;
            var selectedIds = uidoc.Selection.GetElementIds();

            if (0 == selectedIds.Count)
            {
               // If no elements selected.
               TaskDialog.Show("Revit", "You haven't selected any elements.");
            }
            else
            {
               foreach (var id in selectedIds)
               {
                  var dim = doc.GetElement(id) as Dimension;

                  if (null != dim)
                  {
                     var dimLine = dim.Curve as Line;
                     if (dimLine != null)
                     {
                        _transaction_.Start("Set leader end position.");
                        try
                        {
                           var dir = dimLine.Direction;
                           if (dim.Segments.IsEmpty)
                           {
                              var leaderPos = ComputeLeaderPosition(dir, dim.Origin);
                              dim.LeaderEndPosition = leaderPos;
                           }
                           else
                           {
                              foreach (DimensionSegment ds in dim.Segments)
                              {
                                 var leaderPos = ComputeLeaderPosition(dir, ds.Origin);
                                 ds.LeaderEndPosition = leaderPos;
                              }
                           }
                           _transaction_.Commit();
                        }
                        catch (System.Exception ex)
                        {
                           TaskDialog.Show("Can't set dimension leader end point: {0}", ex.Message);
                           _transaction_.RollBack();
                        }
                     }
                  }
               }
            }

            return Result.Succeeded;
         }
      }

      private XYZ ComputeLeaderPosition(XYZ dir, XYZ origin)
      {
         var leaderPos = new XYZ();
         leaderPos = dir * m_delta;
         leaderPos = leaderPos.Add(origin);
         return leaderPos;
      }
   }

   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class MoveToPickedPoint : IExternalCommand
   {
       public virtual Result Execute(ExternalCommandData commandData
          , ref string message, ElementSet elements)
      {
         // Get the handle of current document.
         var uidoc = commandData.Application.ActiveUIDocument;
         var doc = uidoc.Document;
         using (var _transaction_ = new Transaction(doc))
         {

            // Get the element selection of current document.
            var selection = uidoc.Selection;
            var selectedIds = uidoc.Selection.GetElementIds();

            if (0 == selectedIds.Count)
            {
               // If no elements selected.
               TaskDialog.Show("Revit", "You haven't selected any elements.");
            }
            else
            {
               foreach (var id in selectedIds)
               {
                  var dim = doc.GetElement(id) as Dimension;
                  if (null != dim)
                  {
                     var startPoint = selection.PickPoint(ObjectSnapTypes.None, "Pick start");
                      _transaction_.Start("Set leader end point");
                     try
                     {
                        if (dim.Segments.IsEmpty)
                        {
                           dim.LeaderEndPosition = startPoint;
                        }
                        else
                        {
                           var deltaVec = dim.Segments.get_Item(1).Origin.Subtract(dim.Segments.get_Item(0).Origin);
                           var offset = new XYZ();
                           foreach (DimensionSegment ds in dim.Segments)
                           {
                              ds.LeaderEndPosition = startPoint.Add(offset);
                              offset = offset.Add(deltaVec);
                           }
                        }
                        _transaction_.Commit();
                     }
                     catch (System.Exception ex)
                     {
                        TaskDialog.Show("Can't set dimension leader end point: {0}", ex.Message);
                        _transaction_.RollBack();
                     }
                  }
               }
            }

            return Result.Succeeded;
         }
      }
   }
}

