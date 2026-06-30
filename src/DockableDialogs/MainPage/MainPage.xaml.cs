// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.APIUtility;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.MainPage
{
    public partial class MainPage : Page, IDockablePaneProvider
    {
        private UIApplication m_application;
        private int m_bottom = 1;
        private readonly ExternalEvent m_exEvent;
        private readonly ApiExternalEventHandler m_handler = new();
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
            m_textWriter = new StandardIoRouter(tb_output);
            Console.SetOut(m_textWriter); //Send all standard output to the text rerouter.
        }

        private UIApplication Application
        {
            set => m_application = value;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            new DockablePaneProviderData();

            data.InitialState = new DockablePaneState
            {
                DockPosition = m_position
            };
            var targetPane = m_targetGuid == Guid.Empty ? null : new DockablePaneId(m_targetGuid);
            switch (m_position)
            {
                case DockPosition.Tabbed:
                    data.InitialState.TabBehind = targetPane;
                    break;
                case DockPosition.Floating:
                    data.InitialState.SetFloatingRectangle(new Rectangle(m_left, m_top, m_right, m_bottom));
                    break;
            }

            DialogHelper.Message("***Intial docking parameters***");
            DialogHelper.Message(ApiUtility.GetDockStateSummary(data.InitialState));
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

        public void UpdateUi(ModelessCommandData data)
        {
            switch (data.CommandType)
            {
                case ModelessCommandType.PrintMainPageStatistics:
                    {
                        DialogHelper.Message("***Main Pane***");
                        DialogHelper.Message(data.WindowSummaryData);
                        break;
                    }

                case ModelessCommandType.PrintSelectedPageStatistics:
                    {
                        DialogHelper.Message("***Selected Pane***");
                        DialogHelper.Message(data.WindowSummaryData);
                        break;
                    }
            }
        }

        private TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is not null and TChildItem item)
                {
                    return item;
                }

                var childOfChild = FindVisualChild<TChildItem>(child);
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
            var data = new ModelessCommandData
            {
                CommandType = ModelessCommandType.PrintMainPageStatistics
            };
            ThisApplication.ThisApp.GetDockableApiUtility().RunModelessCommand(data);
            m_exEvent.Raise();
        }

        private void RaisePrintSpecificSummaryCommand(string guid)
        {
            var data = new ModelessCommandData
            {
                CommandType = ModelessCommandType.PrintSelectedPageStatistics,
                SelectedPaneId = guid
            };
            ThisApplication.ThisApp.GetDockableApiUtility().RunModelessCommand(data);
            m_exEvent.Raise();
        }

        public string GetPageWpfData()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-WFP Page Info-");
            sb.AppendLine($"FrameWorkElement.Width={Width}");
            sb.AppendLine($"FrameWorkElement.Height={Height}");

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
            DialogHelper.Message("***Main Pane WPF info***");
            DialogHelper.Message(ThisApplication.ThisApp.GetMainWindow().GetPageWpfData());
        }

        private void btn_getById_Click(object sender, RoutedEventArgs e)
        {
            var guid = Interaction.InputBox("Enter Pane Guid", SampleBrowserUtils.ApplicationName);
            if (string.IsNullOrEmpty(guid))
                return;
            RaisePrintSpecificSummaryCommand(guid);
        }

        private void btn_listTabs_Click(object sender, RoutedEventArgs e)
        {
            DialogHelper.Message("***Dockable dialogs***");
            DialogHelper.Message($" Main dialog: {SampleBrowserUtils.SmUserDockablePaneId.Guid}");
            DialogHelper.Message($" Element View: {DockablePanes.BuiltInDockablePanes.ElementView.Guid}");
            DialogHelper.Message($" System Navigator: {DockablePanes.BuiltInDockablePanes.SystemNavigator.Guid}");
            DialogHelper.Message($" Link Navigator: {DockablePanes.BuiltInDockablePanes.HostByLinkNavigator.Guid}");
            DialogHelper.Message($" Project Browser: {DockablePanes.BuiltInDockablePanes.ProjectBrowser.Guid}");
            DialogHelper.Message($" Properties Palette: {DockablePanes.BuiltInDockablePanes.PropertiesPalette.Guid}");
            DialogHelper.Message($" Rebar Browser: {DockablePanes.BuiltInDockablePanes.RebarBrowser.Guid}");
        }
    }
}
