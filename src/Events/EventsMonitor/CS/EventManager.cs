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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.EventsMonitor.CS
{
    /// <summary>
    ///     This class is a manager for application events.
    ///     In this class, user can subscribe and remove the events according to what he select.
    ///     This class used the controlled Application as the sender.
    ///     If you want to use Application or Document as the sender, the usage is same.
    ///     "+=" is used to register event and "-=" is used to remove event.
    /// </summary>
    public class EventManager
    {
        /// <summary>
        ///     This list is used to store what user select last time.
        /// </summary>
        private readonly List<string> historySelection;

        /// <summary>
        ///     Revit application
        /// </summary>
        private readonly UIControlledApplication m_app;

        /// <summary>
        ///     Prevent the compiler from generating a default constructor.
        /// </summary>
        private EventManager()
        {
            // none any codes, just declare it as private to obey the .Net design rule
        }

        /// <summary>
        ///     Constructor for application event manager.
        /// </summary>
        /// <param name="app"></param>
        public EventManager(UIControlledApplication app)
        {
            m_app = app;
            historySelection = new List<string>();
        }

        /// <summary>
        ///     A public method used to update the events subscription
        /// </summary>
        /// <param name="selection"></param>
        public void Update(List<string> selection)
        {
            // If event has been in history list and not in current selection,
            // it means user doesn't select this event again, and it should be move.
            foreach (var eventname in historySelection)
                if (!selection.Contains(eventname))
                    subtractEvents(eventname);

            // Contrarily,if event has been in current selection and not in history list,
            // it means this event should be subscribed.
            foreach (var eventname in selection)
                if (!historySelection.Contains(eventname))
                    addEvents(eventname);

            // generate the history list.
            historySelection.Clear();
            foreach (var eventname in selection) historySelection.Add(eventname);
        }

        /// <summary>
        ///     Register event according to event name.
        ///     The generic handler app_eventsHandlerMethod will be subscribed to this event.
        /// </summary>
        /// <param name="eventName"></param>
        private void addEvents(string eventName)
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

        /// <summary>
        ///     Remove registered event by its name.
        /// </summary>
        /// <param name="eventName">Event name to be subtracted.</param>
        private void subtractEvents(string eventName)
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

        /// <summary>
        ///     Generic event handler can be subscribed to any events.
        ///     It will dump events information(sender and EventArgs) to log window and log file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public void app_eventsHandlerMethod(object obj, EventArgs args)
        {
            // generate event information and set to information window 
            // to track what event be touch off.
            ExternalApplication.EventLogManager.TrackEvent(obj, args);
            // write log file.
            ExternalApplication.EventLogManager.WriteLogFile(obj, args);
        }
    }
}