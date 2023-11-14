// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ThisApplication : IExternalApplication
    {
        internal static ThisApplication thisApp;
        private APIUtility m_APIUtility;


        private MainPage m_mainPage;


        public DockablePaneId MainPageDockablePaneId => Globals.sm_UserDockablePaneId;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Add UI for registering, showing, and hiding dockable panes.
        /// </summary>
        public Result OnStartup(UIControlledApplication application)
        {
            thisApp = this;
            m_APIUtility = new APIUtility();

            application.CreateRibbonTab(Globals.DiagnosticsTabName);
            var panel = application.CreateRibbonPanel(Globals.DiagnosticsTabName, Globals.DiagnosticsPanelName);

            panel.AddSeparator();

            var pushButtonRegisterPageData = new PushButtonData(Globals.RegisterPage, Globals.RegisterPage,
                FileUtility.GetAssemblyFullName(), typeof(ExternalCommandRegisterPage).FullName)
            {
                LargeImage = new BitmapImage(new Uri(FileUtility.GetApplicationResourcesPath() + "Register.png"))
            };
            var pushButtonRegisterPage = panel.AddItem(pushButtonRegisterPageData) as PushButton;
            pushButtonRegisterPage.AvailabilityClassName = typeof(ExternalCommandRegisterPage).FullName;


            var pushButtonShowPageData = new PushButtonData(Globals.ShowPage, Globals.ShowPage,
                FileUtility.GetAssemblyFullName(), typeof(ExternalCommandShowPage).FullName)
            {
                LargeImage = new BitmapImage(new Uri(FileUtility.GetApplicationResourcesPath() + "Show.png"))
            };
            var pushButtonShowPage = panel.AddItem(pushButtonShowPageData) as PushButton;
            pushButtonShowPage.AvailabilityClassName = typeof(ExternalCommandShowPage).FullName;


            var pushButtonHidePageData = new PushButtonData(Globals.HidePage, Globals.HidePage,
                FileUtility.GetAssemblyFullName(), typeof(ExternalCommandHidePage).FullName)
            {
                LargeImage = new BitmapImage(new Uri(FileUtility.GetApplicationResourcesPath() + "Hide.png"))
            };
            var pushButtonHidePage = panel.AddItem(pushButtonHidePageData) as PushButton;
            pushButtonHidePage.AvailabilityClassName = typeof(ExternalCommandHidePage).FullName;

            return Result.Succeeded;
        }

        /// <summary>
        ///     Register a dockable Window
        /// </summary>
        public void RegisterDockableWindow(UIApplication application, Guid mainPageGuid)
        {
            Globals.sm_UserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(Globals.sm_UserDockablePaneId, Globals.ApplicationName,
                thisApp.GetMainWindow());
        }

        /// <summary>
        ///     Register a dockable Window
        /// </summary>
        public void RegisterDockableWindow(UIControlledApplication application, Guid mainPageGuid)
        {
            Globals.sm_UserDockablePaneId = new DockablePaneId(mainPageGuid);
            application.RegisterDockablePane(Globals.sm_UserDockablePaneId, Globals.ApplicationName,
                thisApp.GetMainWindow());
        }

        /// <summary>
        ///     Create the new WPF Page that Revit will dock.
        /// </summary>
        public void CreateWindow()
        {
            m_mainPage = new MainPage();
        }

        /// <summary>
        ///     Show or hide a dockable pane.
        /// </summary>
        public void SetWindowVisibility(UIApplication application, bool state)
        {
            var pane = application.GetDockablePane(Globals.sm_UserDockablePaneId);
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

        public MainPage GetMainWindow()
        {
            if (!IsMainWindowAvailable())
                throw new InvalidOperationException("Main window not constructed.");
            return m_mainPage;
        }

        public APIUtility GetDockableAPIUtility()
        {
            return m_APIUtility;
        }
    }
}
