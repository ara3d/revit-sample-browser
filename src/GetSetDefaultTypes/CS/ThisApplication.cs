// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace RevitMultiSample.GetSetDefaultTypes.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalApplication
    /// </summary>
    public class ThisApplication : IExternalApplication
    {
        public static DefaultFamilyTypes DefaultFamilyTypesPane;
        public static DefaultElementTypes DefaultElementTypesPane;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var str = "Default Type Selector";
                var panel = application.CreateRibbonPanel(str);
                var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var data = new PushButtonData("Default Type Selector", "Default Type Selector",
                    directoryName + @"\GetSetDefaultTypes.dll", "RevitMultiSample.GetSetDefaultTypes.CS.ThisCommand");
                var button = panel.AddItem(data) as PushButton;
                button.LargeImage = new BitmapImage(new Uri(directoryName + "\\Resources\\type.png"));

                // register dockable Windows on startup.
                DefaultFamilyTypesPane = new DefaultFamilyTypes();
                DefaultElementTypesPane = new DefaultElementTypes();
                application.RegisterDockablePane(DefaultFamilyTypes.PaneId, "Default Family Types",
                    DefaultFamilyTypesPane);
                application.RegisterDockablePane(DefaultElementTypes.PaneId, "Default Element Types",
                    DefaultElementTypesPane);

                // register view active event
                application.ViewActivated += application_ViewActivated;

                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Default Type Selector");
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Show dockable panes when view active.
        /// </summary>
        private void application_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
                !DockablePane.PaneExists(DefaultElementTypes.PaneId))
                return;

            if (!(sender is UIApplication uiApp))
                return;

            DefaultFamilyTypesPane?.SetDocument(e.Document);
            DefaultElementTypesPane?.SetDocument(e.Document);
        }
    }
}
