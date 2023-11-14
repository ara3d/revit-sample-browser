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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RayTraceBounce.CS
{
   /// <summary>
   /// A class inherits IExternalCommand interface.
   /// This class shows how to find intersection between ray and face and create  
   /// connecting lines by Revit API method FindReferencesByDirection.
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      #region Class Memeber Variables
      /// <summary>
      /// revit application
      /// </summary>
      UIApplication m_app;

      /// <summary>
      /// a 3D View
      /// </summary>
      View3D m_view;
      #endregion

      #region Class Interface Implementation
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
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            // should have a line style "bounce" created in the document before running this
            m_app = commandData.Application;
            Get3DView();
            if (m_view == null)
            {
               TaskDialog.Show("Revit", "A default 3D view (named {3D}) must exist before running this command");
               return Result.Cancelled;
            }
            else
            {
               var form = new RayTraceBounceForm(commandData, m_view);
               form.ShowDialog();
            }
            return Result.Succeeded;
         }
         catch (Exception e)
         {
            message = e.ToString();
            return Result.Failed;
         }
      }
      #endregion

      #region Class Implementation
      /// <summary>
      /// Get a 3D view from active document
      /// </summary>
      public void Get3DView()
      {
         var list = new List<Element>();
         var collector = new FilteredElementCollector(m_app.ActiveUIDocument.Document);
         list.AddRange(collector.OfClass(typeof(View3D)).ToElements());
         foreach (View3D v in list)
         {
             // skip view template here because view templates are invisible in project browsers
             if (v != null && !v.IsTemplate && v.Name == "{3D}")
            {
               m_view = v as View3D;
               break;
            }
         }
      }
      #endregion

   }
}
