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
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.InCanvasControlAPI.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    public class Application : IExternalApplication
    {
        private const string TabLabel = "Issues";

        private readonly EventHandler<DocumentClosedEventArgs> m_closedHandler;

        private readonly Dictionary<int, Guid> m_closingDocumentIdToIssueTrackingPairs = new Dictionary<int, Guid>();

        private readonly EventHandler<DocumentClosingEventArgs> m_closingHandler;

        private readonly EventHandler<DocumentCreatedEventArgs> m_createHandler;

        private readonly EventHandler<DocumentOpenedEventArgs> m_openHandler;

        private readonly EventHandler<DocumentChangedEventArgs> m_updateHandler;

        /// <summary>
        ///     Creates external application object and initializes event handlers.
        /// </summary>
        public Application()
        {
            var issueMarkerTrackingManager = IssueMarkerTrackingManager.GetInstance();

            // This event handler moves or deletes the markers based on changes to the tracked elements
            m_updateHandler = (sender, data) => { IssueMarkerUpdater.Execute(data); };

            // This event handler initiates data for the opened document
            m_openHandler = (sender, data) => { issueMarkerTrackingManager.AddTracking(data.Document); };

            // This event handler initiates data for the newly-created document
            m_createHandler = (sender, data) => { issueMarkerTrackingManager.AddTracking(data.Document); };

            // This event handler prepares marker data for the document to be cleaned
            m_closingHandler = (closingSender, closeData) =>
            {
                var track = issueMarkerTrackingManager.GetTracking(closeData.Document);
                if (!m_closingDocumentIdToIssueTrackingPairs.ContainsKey(closeData.DocumentId) &&
                    !closeData.IsCancelled())
                    m_closingDocumentIdToIssueTrackingPairs.Add(closeData.DocumentId, track.Id);
            };

            // This event handler cleans marker data after the document is closed
            m_closedHandler = (closedSender, closedData) =>
            {
                issueMarkerTrackingManager.DeleteTracking(m_closingDocumentIdToIssueTrackingPairs[closedData.DocumentId]);
                m_closingDocumentIdToIssueTrackingPairs.Remove(closedData.DocumentId);
            };
        }

        /// <summary>
        ///     Implements the OnShutdown event. It cleans up events and IssueMarkerTrackingManager
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentChanged -= m_updateHandler;
            application.ControlledApplication.DocumentOpened -= m_openHandler;
            application.ControlledApplication.DocumentCreated -= m_createHandler;
            application.ControlledApplication.DocumentClosing -= m_closingHandler;
            application.ControlledApplication.DocumentClosed -= m_closedHandler;

            IssueMarkerTrackingManager.GetInstance().ClearTrackings();

            return Result.Succeeded;
        }

        /// <summary>
        ///     Implements the OnStartup event. It adds a server to listen for clicks on issue markers, events that manage issue
        ///     marker data based on changes in document,
        ///     and a button that lets user create an issue marker.
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            var click = new IssueSelectHandler();

            //This registers a service. On success, we register a button or event as well.
            var service = ExternalServiceRegistry.GetService(click.GetServiceId());
            if (service != null)
            {
                service.AddServer(click);
                (service as MultiServerService).SetActiveServers(new List<Guid> { click.GetServerId() });

                var ribbonPanel = application.GetRibbonPanels(Tab.AddIns).Find(x => x.Name == TabLabel) ?? application.CreateRibbonPanel(Tab.AddIns, TabLabel);

                RibbonItemData ribbonItemData = new PushButtonData("Create marker", "Create issue marker on an element",
                    Assembly.GetExecutingAssembly().Location, typeof(Command).FullName);

                _ = (PushButton)ribbonPanel.AddItem(ribbonItemData);

                application.ControlledApplication.DocumentChanged += m_updateHandler;
                application.ControlledApplication.DocumentOpened += m_openHandler;
                application.ControlledApplication.DocumentCreated += m_createHandler;
                application.ControlledApplication.DocumentClosing += m_closingHandler;
                application.ControlledApplication.DocumentClosed += m_closedHandler;

                return Result.Succeeded;
            }

            return Result.Failed;
        }
    }
}
