// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS
{
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
                    $@"{directoryName}\GetSetDefaultTypes.dll", "Ara3D.RevitSampleBrowser.GetSetDefaultTypes.CS.ThisCommand");
                var button = panel.AddItem(data) as PushButton;
                button.LargeImage = new BitmapImage(new Uri($"{directoryName}\\Resources\\type.png"));

                DefaultFamilyTypesPane = new DefaultFamilyTypes();
                DefaultElementTypesPane = new DefaultElementTypes();
                application.RegisterDockablePane(DefaultFamilyTypes.PaneId, "Default Family Types",
                    DefaultFamilyTypesPane);
                application.RegisterDockablePane(DefaultElementTypes.PaneId, "Default Element Types",
                    DefaultElementTypesPane);

                application.ViewActivated += application_ViewActivated;

                return Result.Succeeded;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Default Type Selector");
                return Result.Failed;
            }
        }

        private void application_ViewActivated(object sender, ViewActivatedEventArgs e)
        {
            if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
                !DockablePane.PaneExists(DefaultElementTypes.PaneId))
                return;

            if (sender is not UIApplication uiApp)
                return;

            DefaultFamilyTypesPane?.SetDocument(e.Document);
            DefaultElementTypesPane?.SetDocument(e.Document);
        }
    }
}
