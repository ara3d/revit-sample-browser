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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    public partial class MainPage : Page, IDockablePaneProvider
    {
        private UIApplication m_application;
        private int m_bottom = 1;
        private readonly ExternalEvent m_exEvent;
        private readonly APIExternalEventHandler m_handler = new APIExternalEventHandler();
        private int m_left = 1;
        private DockPosition m_position = DockPosition.Bottom;
        private int m_right = 1;

        private Guid m_targetGuid;
        private readonly TextWriter m_textWriter; //Used to re-route any standard IO to the WPF UI.
        private int m_top = 1;

        public MainPage()
        {
            InitializeComponent();
            m_exEvent = ExternalEvent.Create(m_handler);
            m_textWriter = new StandardIORouter(tb_output);
            Console.SetOut(m_textWriter); //Send all standard output to the text rerouter.
        }

        private UIApplication Application
        {
            set => m_application = value;
        }


        /// <summary>
        ///     Called by Revit to initialize dockable pane settings set in DockingSetupDialog.
        /// </summary>
        /// <param name="data"></param>
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            new DockablePaneProviderData();


            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = m_position;
            DockablePaneId targetPane;
            if (m_targetGuid == Guid.Empty)
                targetPane = null;
            else targetPane = new DockablePaneId(m_targetGuid);
            if (m_position == DockPosition.Tabbed)
                data.InitialState.TabBehind = targetPane;


            if (m_position == DockPosition.Floating)
                data.InitialState.SetFloatingRectangle(new Rectangle(m_left, m_top, m_right, m_bottom));

            Log.Message("***Intial docking parameters***");
            Log.Message(APIUtility.GetDockStateSummary(data.InitialState));
        }

        private void DozeOff()
        {
            EnableCommands(false);
        }

        private void EnableCommands(bool status)
        {
            Cursor = status == false ? Cursors.Wait : Cursors.Arrow;
        }

        public void WakeUp()
        {
            EnableCommands(true);
        }

        public void UpdateUI(ModelessCommandData data)
        {
            switch (data.CommandType)
            {
                case ModelessCommandType.PrintMainPageStatistics:
                {
                    Log.Message("***Main Pane***");
                    Log.Message(data.WindowSummaryData);
                    break;
                }

                case ModelessCommandType.PrintSelectedPageStatistics:
                {
                    Log.Message("***Selected Pane***");
                    Log.Message(data.WindowSummaryData);
                    break;
                }
            }
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }

                var childOfChild = FindVisualChild<childItem>(child);
                if (childOfChild != null) return childOfChild;
            }

            return null;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
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

        public void SetInitialDockingParameters(int left, int right, int top, int bottom, DockPosition position,
            Guid targetGuid)
        {
            m_position = position;
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
            m_targetGuid = targetGuid;
        }


        private void wpf_stats_Click(object sender, RoutedEventArgs e)
        {
            Log.Message("***Main Pane WPF info***");
            Log.Message(ThisApplication.thisApp.GetMainWindow().GetPageWpfData());
        }


        private void btn_getById_Click(object sender, RoutedEventArgs e)
        {
            var guid = Interaction.InputBox("Enter Pane Guid", Globals.ApplicationName);
            if (string.IsNullOrEmpty(guid))
                return;
            RaisePrintSpecificSummaryCommand(guid);
        }

        private void btn_listTabs_Click(object sender, RoutedEventArgs e)
        {
            Log.Message("***Dockable dialogs***");
            Log.Message(" Main dialog: " + Globals.sm_UserDockablePaneId.Guid);
            Log.Message(" Element View: " + DockablePanes.BuiltInDockablePanes.ElementView.Guid);
            Log.Message(" System Navigator: " + DockablePanes.BuiltInDockablePanes.SystemNavigator.Guid);
            Log.Message(" Link Navigator: " + DockablePanes.BuiltInDockablePanes.HostByLinkNavigator.Guid);
            Log.Message(" Project Browser: " + DockablePanes.BuiltInDockablePanes.ProjectBrowser.Guid);
            Log.Message(" Properties Palette: " + DockablePanes.BuiltInDockablePanes.PropertiesPalette.Guid);
            Log.Message(" Rebar Browser: " + DockablePanes.BuiltInDockablePanes.RebarBrowser.Guid);
        }
    }
}