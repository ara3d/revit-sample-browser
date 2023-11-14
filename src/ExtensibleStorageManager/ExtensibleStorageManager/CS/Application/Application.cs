// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Reflection;
using Autodesk.Revit.UI;

namespace ExtensibleStorageManager
{
    public class Application : IExternalApplication
    {
        /// <summary>
        ///     The Last Schema Guid value used in the UICommand dialog is stored here for future retrieval
        ///     after the dialog is closed.
        /// </summary>
        public static string LastGuid { get; set; }

        /// <summary>
        ///     There is no cleanup needed in this application  -- default implementation
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        ///     Add a button to the Ribbon and attach it to the IExternalCommand defined in Command.cs
        /// </summary>
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
