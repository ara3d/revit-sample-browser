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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DatumsModification.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class DatumStyleModification : IExternalCommand
    {
        #region globle variable
       /// <summary>
       /// 
       /// </summary>
       public static bool showLeftBubble = false;
       /// <summary>
       /// 
       /// </summary>
       public static bool showRightBubble = false;
       /// <summary>
       /// 
       /// </summary>
       public static bool addLeftElbow = false;
       /// <summary>
       /// 
       /// </summary>
       public static bool addRightElbow = false;
       /// <summary>
       /// 
       /// </summary>
       public static bool changeLeftEnd2D = false;
       /// <summary>
       /// 
       /// </summary>
       public static bool changeRightEnd2D = false;
       #endregion

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
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var datums = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
                var view = commandData.Application.ActiveUIDocument.ActiveView;
                if (datums == null || datums.Count == 0)
                   return Result.Cancelled;
                //// Show UI                
                using (var settingForm = new DatumStyleSetting())
                {
                   if (settingForm.ShowDialog() == DialogResult.OK)
                   {
                      using (var tran = new Transaction(document,"StyleModification"))
                      {
                         tran.Start();
                         foreach (var datumRef in datums)
                         {
                            var datum = document.GetElement(datumRef) as DatumPlane;
                            if (showLeftBubble)
                               datum.ShowBubbleInView(DatumEnds.End0, view);
                            else
                               datum.HideBubbleInView(DatumEnds.End0, view);
                            if (showRightBubble)
                               datum.ShowBubbleInView(DatumEnds.End1, view);
                            else
                               datum.HideBubbleInView(DatumEnds.End1, view);
                            if (changeLeftEnd2D)
                               datum.SetDatumExtentType(DatumEnds.End0, view, DatumExtentType.ViewSpecific);
                            else
                               datum.SetDatumExtentType(DatumEnds.End0, view, DatumExtentType.Model);
                            if (changeRightEnd2D)
                               datum.SetDatumExtentType(DatumEnds.End1, view, DatumExtentType.ViewSpecific);
                            else
                               datum.SetDatumExtentType(DatumEnds.End1, view, DatumExtentType.Model);
                            if (addLeftElbow && datum.GetLeader(DatumEnds.End0, view) == null)
                               datum.AddLeader(DatumEnds.End0, view);
                            else if(datum.GetLeader(DatumEnds.End0, view) != null)
                            {
                               var leader =  datum.GetLeader(DatumEnds.End0, view);
                               leader = CalculateLeader(leader, addLeftElbow);
                               datum.SetLeader(DatumEnds.End0, view, leader);
                            }
                                                       
                            if (addRightElbow && datum.GetLeader(DatumEnds.End1, view) == null)
                               datum.AddLeader(DatumEnds.End1, view);
                            else if (datum.GetLeader(DatumEnds.End1, view) != null)
                            {
                               var leader = datum.GetLeader(DatumEnds.End1, view);
                               leader = CalculateLeader(leader, addRightElbow);
                               datum.SetLeader(DatumEnds.End1, view, leader);
                            }
                            
                         }
                         tran.Commit();
                      }                      
                   }                   
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private Leader CalculateLeader(Leader leader,bool addLeader)
        {
           var end = leader.End;
           var anchor = leader.Anchor;
           XYZ elbow = null;
           if (addLeftElbow)
              elbow = new XYZ(leader.Anchor.X + (leader.End.X - leader.Anchor.X) / 2,
                 leader.Anchor.Y + (leader.End.Y - leader.Anchor.Y) / 2,
                 leader.Anchor.Z + (leader.End.Z - leader.Anchor.Z) / 2);
           else
              elbow = new XYZ(leader.Anchor.X, leader.Anchor.Y, leader.Anchor.Z);
 
           leader.Elbow = elbow;
           return leader;
        }
    }

    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class DatumAlignment : IExternalCommand
    {
       #region globle variable
       /// <summary>
       /// 
       /// </summary>
       public static Dictionary<string, DatumPlane> datumDic = new Dictionary<string, DatumPlane>();
       #endregion

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
       public virtual Result Execute(ExternalCommandData commandData
           , ref string message, ElementSet elements)
       {
          try
          {
             var document = commandData.Application.ActiveUIDocument.Document;
             var view = commandData.Application.ActiveUIDocument.ActiveView;
             datumDic.Clear();
             var datums = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
             if (datums == null || datums.Count == 0)
                return Result.Cancelled;

             foreach (var datumRef in datums)
             {
                var datum = document.GetElement(datumRef) as DatumPlane;
                if (!datumDic.Keys.Contains(datum.Name))
                {
                   datumDic.Add(datum.Name, datum);
                }
                
             }

             //// Show UI                
             using (var settingForm = new AlignmentSetting())
             {
                if (settingForm.ShowDialog() == DialogResult.OK)
                {
                   var selectedDatum = datumDic[settingForm.datumList.SelectedItem.ToString()];
                   var baseCurve = selectedDatum.GetCurvesInView(DatumExtentType.ViewSpecific, view).ElementAt(0);
                   var baseLine = baseCurve as Line;
                   var baseDirect = baseLine.Direction;

                   using (var tran = new Transaction(document, "DatumAlignment"))
                   {
                      tran.Start();

                      foreach (var datum in datumDic.Values)
                      {
                         var curve = datum.GetCurvesInView(datum.GetDatumExtentTypeInView(DatumEnds.End0, view),view).ElementAt(0);
                         var direct = (curve as Line).Direction;
                         var newCurve = CalculateCurve(curve,baseLine,baseDirect);
                         datum.SetCurveInView(datum.GetDatumExtentTypeInView(DatumEnds.End0, view), view, newCurve);                         
                      }
                      tran.Commit();
                   }
                }
             }
             return Result.Succeeded;
          }
          catch (Exception ex)
          {
             message = ex.Message;
             return Result.Failed;
          }
       }

       private Curve CalculateCurve(Curve curve,Curve baseCurve, XYZ baseDirect)
       {
          var direct = (curve as Line).Direction;
          Line newCurve = null;
          if (Math.Round(direct.X) == Math.Round(baseDirect.X) && Math.Round(direct.X) == 1)
          {
             newCurve = Line.CreateBound(new XYZ(baseCurve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z)
                , new XYZ(baseCurve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z));
             
          }
          else if (Math.Round(direct.Y) == Math.Round(baseDirect.Y) && Math.Round(direct.Y) == 1)
          {
             newCurve = Line.CreateBound(new XYZ(curve.GetEndPoint(0).X, baseCurve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z)
                , new XYZ(curve.GetEndPoint(1).X, baseCurve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z));            
          }
          else
          {
             newCurve = Line.CreateBound(new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, baseCurve.GetEndPoint(0).Z)
                , new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, baseCurve.GetEndPoint(1).Z));
          }
          return newCurve;
       }
    }

    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class DatumPropagation : IExternalCommand
    {
       #region globle variable
       /// <summary>
       /// 
       /// </summary>
       public static Dictionary<string, ElementId> viewDic = new Dictionary<string, ElementId>();
       #endregion

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
       public virtual Result Execute(ExternalCommandData commandData
           , ref string message, ElementSet elements)
       {
          try
          {
             var document = commandData.Application.ActiveUIDocument.Document;
             var view = commandData.Application.ActiveUIDocument.ActiveView;
             viewDic.Clear();
             var datumRef = commandData.Application.ActiveUIDocument.Selection.GetElementIds().First();
             if (datumRef == null)
                return Result.Cancelled;

             var datum = document.GetElement(datumRef) as DatumPlane;
             var viewList = datum.GetPropagationViews(view);
             foreach (var id in viewList)
             {
                var pView = document.GetElement(id) as Autodesk.Revit.DB.View;
                if (!viewDic.Keys.Contains(pView.ViewType + " : " + pView.Name))
                {
                   viewDic.Add(pView.ViewType + " : " + pView.Name, id);
                }
             }

             //// Show UI                
             using (var settingForm = new PropogateSetting())
             {
                if (settingForm.ShowDialog() == DialogResult.OK)
                {
                   var pViewList = new List<ElementId>() as ISet<ElementId>;
                   foreach (var item in settingForm.propagationViewList.CheckedItems)
                   {
                      var selectedView = viewDic[item.ToString()];
                      pViewList.Add(selectedView);
                   }
                   using (var tran = new Transaction(document, "propagation"))
                   {
                      tran.Start();
                      datum.PropagateToViews(view, pViewList);
                      tran.Commit();
                   }
                   
                }
             }
             return Result.Succeeded;
          }
          catch (Exception ex)
          {
             message = ex.Message;
             return Result.Failed;
          }
       }
    }
}

