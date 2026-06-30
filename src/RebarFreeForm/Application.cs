// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.RebarFreeForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private readonly RebarUpdateServer m_server = new();

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // Register CurveElement updater with revit to trigger regen in rebar for selected lines
            CurveElementRegenUpdater updater = new(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            ElementClassFilter modelLineFilter = new(typeof(CurveElement));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), modelLineFilter, Element.GetChangeTypeAny());

            //Register the RebarUpdateServer
            var service = ExternalServiceRegistry.GetService(m_server.GetServiceId());
            service?.AddServer(m_server);
            return Result.Succeeded;
        }
    }
}
