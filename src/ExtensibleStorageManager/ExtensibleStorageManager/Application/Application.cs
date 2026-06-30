// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Reflection;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.Application
{
    public class Application : IExternalApplication
    {
        /// <summary>Last schema Guid entered in the UI dialog; persisted across dialog sessions.</summary>
        public static string LastGuid { get; set; }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var rp = application.CreateRibbonPanel("Extensible Storage Manager");
            var currentAssembly = Assembly.GetAssembly(GetType()).Location;

            _ = rp.AddItem(new PushButtonData("Extensible Storage Manager", "Extensible Storage Manager",
                currentAssembly, "ExtensibleStorageManager.Command")) as PushButton;

            return Result.Succeeded;
        }
    }
}
