// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ReadonlySharedParameters.CS
{
    public class ReadonlySharedParameterApplication : IExternalApplication
    {
        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        Result IExternalApplication.OnStartup(UIControlledApplication application)
        {
            var panel = application.CreateRibbonPanel("Shared parameters");

            var data = new PushButtonData("BindSP", "Bind Shared\nParameters",
                GetType().Assembly.Location, typeof(BindNewReadonlySharedParametersToDocument).FullName);
            panel.AddItem(data);

            panel.AddSeparator();

            var data1 = new PushButtonData("SetIds1", "Set ids: GUID",
                GetType().Assembly.Location, typeof(SetReadonlyId1).FullName);

            var data2 = new PushButtonData("SetIds2", "Set ids: short",
                GetType().Assembly.Location, typeof(SetReadonlyId2).FullName);

            panel.AddStackedItems(data1, data2);

            panel.AddSeparator();

            data1 = new PushButtonData("SetCosts1", "Set cost: random",
                GetType().Assembly.Location, typeof(SetReadonlyCost1).FullName);

            data2 = new PushButtonData("SetCosts2", "Set cost: sequence",
                GetType().Assembly.Location, typeof(SetReadonlyCost2).FullName);

            panel.AddStackedItems(data1, data2);

            return Result.Succeeded;
        }
    }
}
