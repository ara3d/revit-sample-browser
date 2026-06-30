// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    /// <summary>
    /// Subscribes and unsubscribes application events via ControlledApplication (UI trigger points).
    /// Use += / -=; Application or Document work the same when the trigger is API-level.
    /// </summary>
    public class EventManager
    {
        private readonly List<string> m_historySelection;

        private readonly UIControlledApplication m_app;

        private EventManager()
        {
        }

        public EventManager(UIControlledApplication app)
        {
            m_app = app;
            m_historySelection = new List<string>();
        }

        public void Update(List<string> selection)
        {
            // Unsubscribe events removed since last selection.
            foreach (var eventname in m_historySelection)
            {
                if (!selection.Contains(eventname))
                    SubtractEvents(eventname);
            }

            // Subscribe newly selected events.
            foreach (var eventname in selection)
            {
                if (!m_historySelection.Contains(eventname))
                    AddEvents(eventname);
            }

            m_historySelection.Clear();
            foreach (var eventname in selection)
            {
                m_historySelection.Add(eventname);
            }
        }

        private void AddEvents(string eventName)
        {
            switch (eventName)
            {
                case "DocumentCreating":
                    m_app.ControlledApplication.DocumentCreating += app_eventsHandlerMethod;
                    break;
                case "DocumentCreated":
                    m_app.ControlledApplication.DocumentCreated += app_eventsHandlerMethod;
                    break;
                case "DocumentOpening":
                    m_app.ControlledApplication.DocumentOpening += app_eventsHandlerMethod;
                    break;
                case "DocumentOpened":
                    m_app.ControlledApplication.DocumentOpened += app_eventsHandlerMethod;
                    break;
                case "DocumentClosing":
                    m_app.ControlledApplication.DocumentClosing += app_eventsHandlerMethod;
                    break;
                case "DocumentClosed":
                    m_app.ControlledApplication.DocumentClosed += app_eventsHandlerMethod;
                    break;
                case "DocumentSavedAs":
                    m_app.ControlledApplication.DocumentSavedAs += app_eventsHandlerMethod;
                    break;
                case "DocumentSavingAs":
                    m_app.ControlledApplication.DocumentSavingAs += app_eventsHandlerMethod;
                    break;
                case "DocumentSaving":
                    m_app.ControlledApplication.DocumentSaving += app_eventsHandlerMethod;
                    break;
                case "DocumentSaved":
                    m_app.ControlledApplication.DocumentSaved += app_eventsHandlerMethod;
                    break;
                case "DocumentSynchronizingWithCentral":
                    m_app.ControlledApplication.DocumentSynchronizingWithCentral += app_eventsHandlerMethod;
                    break;
                case "DocumentSynchronizedWithCentral":
                    m_app.ControlledApplication.DocumentSynchronizedWithCentral += app_eventsHandlerMethod;
                    break;
                case "FileExporting":
                    m_app.ControlledApplication.FileExporting += app_eventsHandlerMethod;
                    break;
                case "FileExported":
                    m_app.ControlledApplication.FileExported += app_eventsHandlerMethod;
                    break;
                case "FileImporting":
                    m_app.ControlledApplication.FileImporting += app_eventsHandlerMethod;
                    break;
                case "FileImported":
                    m_app.ControlledApplication.FileImported += app_eventsHandlerMethod;
                    break;
                case "DocumentPrinting":
                    m_app.ControlledApplication.DocumentPrinting += app_eventsHandlerMethod;
                    break;
                case "DocumentPrinted":
                    m_app.ControlledApplication.DocumentPrinted += app_eventsHandlerMethod;
                    break;
                case "ViewPrinting":
                    m_app.ControlledApplication.ViewPrinting += app_eventsHandlerMethod;
                    break;
                case "ViewPrinted":
                    m_app.ControlledApplication.ViewPrinted += app_eventsHandlerMethod;
                    break;
                case "ViewActivating":
                    m_app.ViewActivating += app_eventsHandlerMethod;
                    break;
                case "ViewActivated":
                    m_app.ViewActivated += app_eventsHandlerMethod;
                    break;
                case "ProgressChanged":
                    m_app.ControlledApplication.ProgressChanged += app_eventsHandlerMethod;
                    break;
                case "SelectionChanged":
                    m_app.SelectionChanged += app_eventsHandlerMethod;
                    break;
            }
        }

        private void SubtractEvents(string eventName)
        {
            switch (eventName)
            {
                case "DocumentCreating":
                    m_app.ControlledApplication.DocumentCreating -= app_eventsHandlerMethod;
                    break;
                case "DocumentCreated":
                    m_app.ControlledApplication.DocumentCreated -= app_eventsHandlerMethod;
                    break;
                case "DocumentOpening":
                    m_app.ControlledApplication.DocumentOpening -= app_eventsHandlerMethod;
                    break;
                case "DocumentOpened":
                    m_app.ControlledApplication.DocumentOpened -= app_eventsHandlerMethod;
                    break;
                case "DocumentClosing":
                    m_app.ControlledApplication.DocumentClosing -= app_eventsHandlerMethod;
                    break;
                case "DocumentClosed":
                    m_app.ControlledApplication.DocumentClosed -= app_eventsHandlerMethod;
                    break;
                case "DocumentSavedAs":
                    m_app.ControlledApplication.DocumentSavedAs -= app_eventsHandlerMethod;
                    break;
                case "DocumentSavingAs":
                    m_app.ControlledApplication.DocumentSavingAs -= app_eventsHandlerMethod;
                    break;
                case "DocumentSaving":
                    m_app.ControlledApplication.DocumentSaving -= app_eventsHandlerMethod;
                    break;
                case "DocumentSaved":
                    m_app.ControlledApplication.DocumentSaved -= app_eventsHandlerMethod;
                    break;
                case "DocumentSynchronizingWithCentral":
                    m_app.ControlledApplication.DocumentSynchronizingWithCentral -= app_eventsHandlerMethod;
                    break;
                case "DocumentSynchronizedWithCentral":
                    m_app.ControlledApplication.DocumentSynchronizedWithCentral -= app_eventsHandlerMethod;
                    break;
                case "FileExporting":
                    m_app.ControlledApplication.FileExporting -= app_eventsHandlerMethod;
                    break;
                case "FileExported":
                    m_app.ControlledApplication.FileExported -= app_eventsHandlerMethod;
                    break;
                case "FileImporting":
                    m_app.ControlledApplication.FileImporting -= app_eventsHandlerMethod;
                    break;
                case "FileImported":
                    m_app.ControlledApplication.FileImported -= app_eventsHandlerMethod;
                    break;
                case "DocumentPrinting":
                    m_app.ControlledApplication.DocumentPrinting -= app_eventsHandlerMethod;
                    break;
                case "DocumentPrinted":
                    m_app.ControlledApplication.DocumentPrinted -= app_eventsHandlerMethod;
                    break;
                case "ViewPrinting":
                    m_app.ControlledApplication.ViewPrinting -= app_eventsHandlerMethod;
                    break;
                case "ViewPrinted":
                    m_app.ControlledApplication.ViewPrinted -= app_eventsHandlerMethod;
                    break;
                case "ViewActivating":
                    m_app.ViewActivating -= app_eventsHandlerMethod;
                    break;
                case "ViewActivated":
                    m_app.ViewActivated -= app_eventsHandlerMethod;
                    break;
                case "ProgressChanged":
                    m_app.ControlledApplication.ProgressChanged -= app_eventsHandlerMethod;
                    break;
                case "SelectionChanged":
                    m_app.SelectionChanged -= app_eventsHandlerMethod;
                    break;
            }
        }

        public void app_eventsHandlerMethod(object obj, EventArgs args)
        {
            ExternalApplication.EventLogManager.TrackEvent(obj, args);
            ExternalApplication.EventLogManager.WriteLogFile(obj, args);
        }
    }
}
