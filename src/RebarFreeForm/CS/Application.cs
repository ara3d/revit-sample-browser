// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RebarFreeForm.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        private readonly RebarUpdateServer m_server = new RebarUpdateServer();


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // Register CurveElement updater with revit to trigger regen in rebar for selected lines
            var updater = new CurveElementRegenUpdater(application.ActiveAddInId);
            UpdaterRegistry.RegisterUpdater(updater);
            var modelLineFilter = new ElementClassFilter(typeof(CurveElement));
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), modelLineFilter, Element.GetChangeTypeAny());

            //Register the RebarUpdateServer
            var service = ExternalServiceRegistry.GetService(m_server.GetServiceId());
            service?.AddServer(m_server);
            return Result.Succeeded;
        }
    }
}
