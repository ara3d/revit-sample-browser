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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.UI;
using System.Text;

namespace Revit.SDK.Samples.DockableDialogs.CS
{

   public partial class MainPage : Page, IDockablePaneProvider
    {
      public MainPage()
        {
           InitializeComponent();
           m_exEvent  = ExternalEvent.Create(m_handler);
           m_textWriter = new StandardIORouter(tb_output); 
           Console.SetOut(m_textWriter);  //Send all standard output to the text rerouter.
        }
      UIApplication Application
      {
         set => m_application = value;
      }
      #region UI State

      private void DozeOff()
      {
         EnableCommands(false);
      }

      private void EnableCommands(bool status)
      {
         if (status == false)
            Cursor = Cursors.Wait;
         else
            Cursor = Cursors.Arrow;
      }

      public void WakeUp()
      {
         EnableCommands(true);
      }
      #endregion
      #region UI Support
      public void UpdateUI(ModelessCommandData data)
      {
         switch (data.CommandType)
         {
            case (ModelessCommandType.PrintMainPageStatistics):
               {
                  Log.Message("***Main Pane***");
                  Log.Message(data.WindowSummaryData);
                  break;
               }

            case (ModelessCommandType.PrintSelectedPageStatistics):
               {
                  Log.Message("***Selected Pane***");
                  Log.Message(data.WindowSummaryData);
                  break;
               }

            default:
               break;
         }
      }
           
           
       private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
           for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
           {
              var child = VisualTreeHelper.GetChild(obj, i);
              if (child != null && child is childItem) return (childItem)child;
              else
              {
              
                 var childOfChild = FindVisualChild<childItem>(child);
                 if (childOfChild != null) return childOfChild;
              }
           }
  
           return null;
        }
        #endregion
      #region UI Support
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
        }


        #endregion
      #region Data
        private UIApplication m_application;
         private ExternalEvent m_exEvent;
         private APIExternalEventHandler m_handler=  new APIExternalEventHandler();
         private System.IO.TextWriter m_textWriter; //Used to re-route any standard IO to the WPF UI.


        #endregion

      /// <summary>
      /// Called by Revit to initialize dockable pane settings set in DockingSetupDialog.
      /// </summary>
      /// <param name="data"></param>
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            var d = new DockablePaneProviderData();


            data.InitialState = new DockablePaneState(); 
            data.InitialState.DockPosition = m_position;
           DockablePaneId targetPane;
           if (m_targetGuid == Guid.Empty)
              targetPane = null;
           else targetPane = new DockablePaneId(m_targetGuid);
            if (m_position == DockPosition.Tabbed)
               data.InitialState.TabBehind = targetPane;

 
           if (m_position == DockPosition.Floating)
            {
               data.InitialState.SetFloatingRectangle(new Autodesk.Revit.DB.Rectangle(m_left, m_top, m_right, m_bottom));
            }

           Log.Message("***Intial docking parameters***");
           Log.Message(APIUtility.GetDockStateSummary(data.InitialState));

        }

        private void PaneInfoButton_Click(object sender, RoutedEventArgs e)
        {
           RaisePrintSummaryCommand();
        }


       private void RaisePrintSummaryCommand()
        {
           var data = new ModelessCommandData();
           data.CommandType = ModelessCommandType.PrintMainPageStatistics;
           ThisApplication.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
           m_exEvent.Raise();
        }

       private void RaisePrintSpecificSummaryCommand(string guid)
       {
          var data = new ModelessCommandData();
          data.CommandType = ModelessCommandType.PrintSelectedPageStatistics;
          data.SelectedPaneId = guid;
          ThisApplication.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
          m_exEvent.Raise();
       }

     
       public string GetPageWpfData()
       {
          var sb = new StringBuilder();
          sb.AppendLine("-WFP Page Info-");
          sb.AppendLine("FrameWorkElement.Width=" + Width);
          sb.AppendLine("FrameWorkElement.Height=" + Height);

          return sb.ToString();
       }
       public void SetInitialDockingParameters(int left, int right, int top, int bottom, DockPosition position, Guid targetGuid)
       {
          m_position = position;
          m_left= left;
          m_right = right;
          m_top = top;
          m_bottom = bottom;
          m_targetGuid = targetGuid;
       }
      #region Data

       private Guid m_targetGuid;
       private DockPosition m_position = DockPosition.Bottom;  
       private int m_left = 1;
       private int m_right = 1;
       private int m_top = 1;
       private int m_bottom = 1;

      #endregion

       private void wpf_stats_Click(object sender, RoutedEventArgs e)
       {
          Log.Message("***Main Pane WPF info***");
          Log.Message(ThisApplication.thisApp.GetMainWindow().GetPageWpfData());
       }


       private void btn_getById_Click(object sender, RoutedEventArgs e)
       {
          var guid = Microsoft.VisualBasic.Interaction.InputBox("Enter Pane Guid", Globals.ApplicationName, "");
          if (string.IsNullOrEmpty(guid))
             return;
          RaisePrintSpecificSummaryCommand(guid);
       }

       private void btn_listTabs_Click(object sender, RoutedEventArgs e)
       {
          Log.Message("***Dockable dialogs***");
        Log.Message(" Main dialog: " + Globals.sm_UserDockablePaneId.Guid.ToString());
        Log.Message(" Element View: " + DockablePanes.BuiltInDockablePanes.ElementView.Guid.ToString());
        Log.Message(" System Navigator: " + DockablePanes.BuiltInDockablePanes.SystemNavigator.Guid.ToString());
        Log.Message(" Link Navigator: " + DockablePanes.BuiltInDockablePanes.HostByLinkNavigator.Guid.ToString());
        Log.Message(" Project Browser: " + DockablePanes.BuiltInDockablePanes.ProjectBrowser.Guid.ToString());
        Log.Message(" Properties Palette: " + DockablePanes.BuiltInDockablePanes.PropertiesPalette.Guid.ToString());
        Log.Message(" Rebar Browser: " + DockablePanes.BuiltInDockablePanes.RebarBrowser.Guid.ToString());


       }

    }
}

