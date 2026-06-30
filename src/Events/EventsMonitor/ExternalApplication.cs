// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        // UI trigger points require ControlledApplication; API triggers use Application or Document with the same += / -= syntax.
        private static UIControlledApplication _ctrlApp;

        private static LogManager _logManager;

        private static EventsInfoWindows _infWindows;

        private static EventsSettingForm _settingDialog;

        private static List<string> _appEventsSelection;

        private static EventManager _appEventMgr;

#if !(Debug || DEBUG)
        // Release: journal replay/autotest via JournalProcessor and Current.xml.
        private static JournalProcessor m_journalProcessor;
#endif

        public static EventsInfoWindows InfoWindows
        {
            get => _infWindows ?? (_infWindows = new EventsInfoWindows(EventLogManager));
            set => _infWindows = value;
        }

        public static EventsSettingForm SettingDialog 
            => _settingDialog ?? (_settingDialog = new EventsSettingForm());

        public static LogManager EventLogManager 
            => _logManager ?? (_logManager = new LogManager());

        public static List<string> ApplicationEvents
        {
            get => _appEventsSelection ?? (_appEventsSelection = new List<string>());
            set => _appEventsSelection = value;
        }

        public static EventManager AppEventMgr 
            => _appEventMgr ?? (_appEventMgr = new EventManager(_ctrlApp));

#if !(Debug || DEBUG)
        public static JournalProcessor JnlProcessor
        {
            get
            {
                if (null == m_journalProcessor)
                {
                    m_journalProcessor = new JournalProcessor();
                }
                return m_journalProcessor;
            }
        }
#endif

        public Result OnStartup(UIControlledApplication application)
        {
            _ctrlApp = application;
            _logManager = new LogManager();
            _infWindows = new EventsInfoWindows(_logManager);
            _settingDialog = new EventsSettingForm();
            _appEventsSelection = new List<string>();
            _appEventMgr = new EventManager(_ctrlApp);

#if !(Debug || DEBUG)
            m_journalProcessor = new JournalProcessor();
#endif

            try
            {
#if !(Debug || DEBUG)
                if (m_journalProcessor.IsReplay)
                {
                    _appEventsSelection = m_journalProcessor.EventsList;
                }
                else
                {
#endif
                _settingDialog.ShowDialog();
                if (DialogResult.OK == _settingDialog.DialogResult)
                    _appEventsSelection = _settingDialog.AppSelectionList;

#if !(Debug || DEBUG)
                    m_journalProcessor.DumpEventsListToFile(_appEventsSelection);
                }
#endif

                _appEventMgr.Update(_appEventsSelection);

                _infWindows.Show();

                AddCustomPanel(application);
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            Dispose();
            return Result.Succeeded;
        }

        public static void Dispose()
        {
            if (_infWindows != null)
            {
                _infWindows.Close();
                _infWindows = null;
            }

            if (_settingDialog != null)
            {
                _settingDialog.Close();
                _settingDialog = null;
            }

            _appEventMgr = null;
            _logManager.CloseLogFile();
            _logManager = null;
        }

        private static void AddCustomPanel(UIControlledApplication application)
        {
            var panelName = "Events Monitor";
            var ribbonPanelPushButtons = application.CreateRibbonPanel(panelName);
            var pushButtonData = new PushButtonData("EventsSetting",
                "Set Events", Assembly.GetExecutingAssembly().Location,
                "Ara3D.RevitSampleBrowser.EventsMonitor.CS.Command");
            var pushButtonCreateWall = ribbonPanelPushButtons.AddItem(pushButtonData) as PushButton;
            pushButtonCreateWall.ToolTip = "Setting Events";
        }
    }
}
