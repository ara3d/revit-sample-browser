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
using Autodesk.Revit.UI.Events;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;
using System.Reflection;

namespace Revit.SDK.Samples.GetSetDefaultTypes.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalApplication
   /// </summary>
   public class ThisApplication : IExternalApplication
   {
      #region IExternalApplication Members

      public Result OnShutdown(UIControlledApplication application)
      {
         return Result.Succeeded;
      }

      public Result OnStartup(UIControlledApplication application)
      {
         try
         {
            var str = "Default Type Selector";
            var panel = application.CreateRibbonPanel(str);
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var data = new PushButtonData("Default Type Selector", "Default Type Selector", directoryName + @"\GetSetDefaultTypes.dll", "Revit.SDK.Samples.GetSetDefaultTypes.CS.ThisCommand");
            var button = panel.AddItem(data) as PushButton;
            button.LargeImage = new BitmapImage(new Uri(directoryName + "\\Resources\\type.png"));

            // register dockable Windows on startup.
            DefaultFamilyTypesPane = new DefaultFamilyTypes();
            DefaultElementTypesPane = new DefaultElementTypes();
            application.RegisterDockablePane(DefaultFamilyTypes.PaneId, "Default Family Types", DefaultFamilyTypesPane);
            application.RegisterDockablePane(DefaultElementTypes.PaneId, "Default Element Types", DefaultElementTypesPane);

            // register view active event
            application.ViewActivated += new EventHandler<ViewActivatedEventArgs>(application_ViewActivated);

            return Result.Succeeded;
         }
         catch (Exception exception)
         {
            MessageBox.Show(exception.ToString(), "Default Type Selector");
            return Result.Failed;
         }
      }

      #endregion

      public static DefaultFamilyTypes DefaultFamilyTypesPane;
      public static DefaultElementTypes DefaultElementTypesPane;


      /// <summary>
      /// Show dockable panes when view active.
      /// </summary>
      void application_ViewActivated(object sender, ViewActivatedEventArgs e)
      {
         if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
             !DockablePane.PaneExists(DefaultElementTypes.PaneId))
            return;

         var uiApp = sender as UIApplication;
         if (uiApp == null)
            return;

         if (DefaultFamilyTypesPane != null)
            DefaultFamilyTypesPane.SetDocument(e.Document);
         if (DefaultElementTypesPane != null)
            DefaultElementTypesPane.SetDocument(e.Document);
      }

   }
}
