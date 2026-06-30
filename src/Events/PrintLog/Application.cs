// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.PrintLog.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Application : IExternalApplication
    {
        private EventsReactor m_eventsReactor;

        public Result OnStartup(UIControlledApplication application)
        {
            m_eventsReactor = new EventsReactor();
            application.ControlledApplication.ViewPrinting += m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted += m_eventsReactor.AppViewPrinted;
            application.ControlledApplication.DocumentPrinting += m_eventsReactor.AppDocumentPrinting;
            application.ControlledApplication.DocumentPrinted += m_eventsReactor.AppDocumentPrinted;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            m_eventsReactor.CloseLogFiles();
            application.ControlledApplication.ViewPrinting -= m_eventsReactor.AppViewPrinting;
            application.ControlledApplication.ViewPrinted -= m_eventsReactor.AppViewPrinted;
            application.ControlledApplication.DocumentPrinting -= m_eventsReactor.AppDocumentPrinting;
            application.ControlledApplication.DocumentPrinted -= m_eventsReactor.AppDocumentPrinted;
            return Result.Succeeded;
        }
    }
}
