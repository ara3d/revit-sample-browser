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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
   /// <summary>
   /// A simple class to manage an interface into the Revit API.
   /// </summary>
   public partial class APIUtility
   {
      /// <summary>
      /// Store a reference to the application.
      /// </summary>
      public void Initialize(UIApplication uiApplication)
      {
         m_uiApplication = uiApplication;
      }

      /// <summary>
      /// Get the current Modeless command.
      /// </summary>
      public  ModelessCommand ModelessCommand { get; } = new ModelessCommand();

      /// <summary>
      /// Return dockable pane inforamtion, given a dockable pane Guid.
      /// </summary>
      public string GetPaneSummary(string paneGuidString)
      {
         Guid paneGuid;
         try
         {
            paneGuid = new Guid(paneGuidString);
         }
         catch (Exception)
         {
            return "Invalid Guid";
         }
         var paneId = new DockablePaneId(paneGuid);
         return GetPaneSummary(paneId);
      }


      /// <summary>
      /// Return dockable pane inforamtion, given a DockablePaneId
      /// </summary>
      public string GetPaneSummary(DockablePaneId id)
      {
          try
         {
             var pane = m_uiApplication.GetDockablePane(id);
             return GetPaneSummary(pane);
         }
         catch (Exception ex)
         {
            return ex.Message;
         }  
      }

      /// <summary>
      /// Return dockable pane inforamtion, given a DockablePaneId
      /// </summary>
      public static string GetPaneSummary(DockablePane pane)
      {
         var sb = new System.Text.StringBuilder();
         sb.AppendLine("-RevitDockablePane- Title: " + pane.GetTitle() + ", Id-Guid: " + pane.Id.Guid.ToString());
         return sb.ToString();
      }

      /// <summary>
      /// Display docking state information as a string.
      /// </summary>
      public static  string GetDockStateSummary(DockablePaneState paneState)
      { 
         var sb = new System.Text.StringBuilder();
         sb.AppendLine(" -DockablePaneState-");
         sb.AppendLine(" Left: " + paneState.FloatingRectangle.Left.ToString());
         sb.AppendLine(" Right: " + paneState.FloatingRectangle.Right.ToString());
         sb.AppendLine(" Top: " + paneState.FloatingRectangle.Top.ToString());
         sb.AppendLine(" Bottom: " + paneState.FloatingRectangle.Bottom.ToString());
         sb.AppendLine(" Position: " + paneState.DockPosition.ToString());
         sb.AppendLine(" Tab target guid:" + paneState.TabBehind.Guid.ToString());
         return sb.ToString();
      }

      #region Data
      private UIApplication m_uiApplication;

      #endregion

   }
}
