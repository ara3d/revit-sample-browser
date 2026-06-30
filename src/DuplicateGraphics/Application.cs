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

// DirectContext3D server that duplicates element graphics.

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.DuplicateGraphics.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private static Application _sApplicationInstance;
        private HashSet<Document> m_documents;
        private readonly XYZ m_offset = new(0, 0, 45);

        private List<RevitElementDrawingServer> m_servers;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // Register events. 
                application.ControlledApplication.DocumentClosing += OnDocumentClosing;
                m_servers = [];
                m_documents = [];

                _sApplicationInstance = this;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // remove the event.
            application.ControlledApplication.DocumentClosing -= OnDocumentClosing;
            return Result.Succeeded;
        }

        public void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            UnregisterServers(args.Document, false);
        }

        public static void ProcessCommandDuplicateGraphics(Document document)
        {
            _sApplicationInstance?.AddMultipleRevitElementServers(new UIDocument(document));
        }

        public static void ProcessCommandClearExternalGraphics(Document document)
        {
            _sApplicationInstance?.UnregisterServers(null, true);
        }

        public void AddRevitElementServer(UIDocument uidoc)
        {
            var reference = uidoc.Selection.PickObject(ObjectType.Element,
                "Select an element to duplicate with DirectContext3D");
            var elem = uidoc.Document.GetElement(reference);

            var directContext3DService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            RevitElementDrawingServer revitServer = new(uidoc, elem, m_offset);
            directContext3DService.AddServer(revitServer);
            m_servers.Add(revitServer);

            var msDirectContext3DService = directContext3DService as MultiServerService;

            var serverIds = msDirectContext3DService.GetActiveServerIds();

            serverIds.Add(revitServer.GetServerId());

            msDirectContext3DService.SetActiveServers(serverIds);

            m_documents.Add(uidoc.Document);
            uidoc.UpdateAllOpenViews();
        }

        public void AddMultipleRevitElementServers(UIDocument uidoc)
        {
            var references =
                uidoc.Selection.PickObjects(ObjectType.Element, "Select elements to duplicate with DirectContext3D");

            var directContext3DService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            var msDirectContext3DService = directContext3DService as MultiServerService;
            var serverIds = msDirectContext3DService.GetActiveServerIds();

            // Create one server per element.
            foreach (var reference in references)
            {
                var elem = uidoc.Document.GetElement(reference);

                RevitElementDrawingServer revitServer = new(uidoc, elem, m_offset);
                directContext3DService.AddServer(revitServer);
                m_servers.Add(revitServer);

                serverIds.Add(revitServer.GetServerId());
            }

            msDirectContext3DService.SetActiveServers(serverIds);

            m_documents.Add(uidoc.Document);
            uidoc.UpdateAllOpenViews();
        }

        public void UnregisterServers(Document document, bool updateViews)
        {
            var externalDrawerServiceId = ExternalServices.BuiltInExternalServices.DirectContext3DService;
            if (ExternalServiceRegistry.GetService(externalDrawerServiceId) is not MultiServerService externalDrawerService)
                return;

            foreach (var registeredServerId in externalDrawerService.GetRegisteredServerIds())
            {
                if (externalDrawerService.GetServer(registeredServerId) is not RevitElementDrawingServer externalDrawServer)
                    continue;
                if (document != null && !document.Equals(externalDrawServer.Document))
                    continue;
                externalDrawerService.RemoveServer(registeredServerId);
            }

            if (document != null)
            {
                m_servers.RemoveAll(server => document.Equals(server.Document));

                if (updateViews)
                {
                    UIDocument uidoc = new(document);
                    uidoc.UpdateAllOpenViews();
                }

                m_documents.Remove(document);
            }
            else
            {
                m_servers.Clear();

                if (updateViews)
                    foreach (var doc in m_documents)
                    {
                        UIDocument uidoc = new(doc);
                        uidoc.UpdateAllOpenViews();
                    }

                m_documents.Clear();
            }
        }
    }
}
