// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.APIUtility;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.TopLevelCommands;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Windows.Media.Imaging;
namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ThisApplication : IExternalApplication
    {
        public static ThisApplication ThisApp;
        private ApiUtility m_apiUtility;

        private MainPage.MainPage m_mainPage;

        public DockablePaneId MainPageDockablePaneId => SampleBrowserUtils.SmUserDockablePaneId;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            ThisApp = this;
            m_apiUtility = new ApiUtility();

            application.CreateRibbonTab(SampleBrowserUtils.DiagnosticsTabName);
            var panel = application.CreateRibbonPanel(SampleBrowserUtils.DiagnosticsTabName, SampleBrowserUtils.DiagnosticsPanelName);

            panel.AddSeparator();

            PushButtonData pushButtonRegisterPageData = new(SampleBrowserUtils.RegisterPage, SampleBrowserUtils.RegisterPage,
                AssemblyPathHelper.GetAssemblyFullName(), typeof(ExternalCommandRegisterPage).FullName)
            {
                LargeImage = new BitmapImage(new Uri($"{AssemblyPathHelper.GetApplicationResourcesPath()}Register.png"))
            };
            var pushButtonRegisterPage = panel.AddItem(pushButtonRegisterPageData) as PushButton;
            pushButtonRegisterPage.AvailabilityClassName = typeof(ExternalCommandRegisterPage).FullName;

            PushButtonData pushButtonShowPageData = new(DialogHelper.ShowPage, DialogHelper.ShowPage,
                AssemblyPathHelper.GetAssemblyFullName(), typeof(ExternalCommandShowPage).FullName)
            {
                LargeImage = new BitmapImage(new Uri($"{AssemblyPathHelper.GetApplicationResourcesPath()}Show.png"))
            };
            var pushButtonShowPage = panel.AddItem(pushButtonShowPageData) as PushButton;
            pushButtonShowPage.AvailabilityClassName = typeof(ExternalCommandShowPage).FullName;

            PushButtonData pushButtonHidePageData = new(SampleBrowserUtils.HidePage, SampleBrowserUtils.HidePage,
                AssemblyPathHelper.GetAssemblyFullName(), typeof(ExternalCommandHidePage).FullName)
            {
                LargeImage = new BitmapImage(new Uri($"{AssemblyPathHelper.GetApplicationResourcesPath()}Hide.png"))
            };
            var pushButtonHidePage = panel.AddItem(pushButtonHidePageData) as PushButton;
            pushButtonHidePage.AvailabilityClassName = typeof(ExternalCommandHidePage).FullName;

            return Result.Succeeded;
        }

        public void RegisterDockableWindow(UIApplication application, Guid mainPageGuid)
        {
            SampleBrowserUtils.SmUserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(SampleBrowserUtils.SmUserDockablePaneId, SampleBrowserUtils.ApplicationName,
                ThisApp.GetMainWindow());
        }

        public void RegisterDockableWindow(UIControlledApplication application, Guid mainPageGuid)
        {
            SampleBrowserUtils.SmUserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(SampleBrowserUtils.SmUserDockablePaneId, SampleBrowserUtils.ApplicationName,
                ThisApp.GetMainWindow());
        }

        public void CreateWindow()
        {
            m_mainPage = new MainPage.MainPage();
        }

        public void SetWindowVisibility(UIApplication application, bool state)
        {
            var pane = application.GetDockablePane(SampleBrowserUtils.SmUserDockablePaneId);
            if (pane != null)
            {
                if (state)
                    pane.Show();
                else
                    pane.Hide();
            }
        }

        public bool IsMainWindowAvailable()
        {
            if (m_mainPage == null)
                return false;

            var isAvailable = true;
            try
            {
            }
            catch (Exception)
            {
                isAvailable = false;
            }

            return isAvailable;
        }

        public MainPage.MainPage GetMainWindow()
        {
            return !IsMainWindowAvailable() ? throw new InvalidOperationException("Main window not constructed.") : m_mainPage;
        }

        public ApiUtility GetDockableApiUtility()
        {
            return m_apiUtility;
        }
    }
}
