// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS.OptionsDialog
{
    public class AddTabCommand
    {
        private readonly UIControlledApplication m_application;

        public AddTabCommand(UIControlledApplication application)
        {
            m_application = application;
        }

        public bool AddTabToOptionsDialog()
        {
            m_application.DisplayingOptionsDialog +=
                Command_DisplayingOptionDialog;
            return true;
        }

        private void Command_DisplayingOptionDialog(object sender, DisplayingOptionsDialogEventArgs e)
        {
            // Actual options
            Options optionsControl = new();
            ContextualHelp ch = new(ContextualHelpType.Url, "http://www.autodesk.com/");
            TabbedDialogExtension extension = new(optionsControl, optionsControl.OnOK)
            {
                OnRestoreDefaultsAction = optionsControl.OnRestoreDefaults
            };
            extension.SetContextualHelp(ch);
            e.AddTab("Demo options", extension);

            // Demo options
            UserControl3 userControl3 = new("Product Information");
            new ContextualHelp(ContextualHelpType.Url, "http://www.google.com/");
            TabbedDialogExtension tdext3 = new(userControl3,
                userControl3.OnOK)
            {
                OnCancelAction = userControl3.OnCancel,
                OnRestoreDefaultsAction = userControl3.OnRestoreDefaults
            };
            tdext3.SetContextualHelp(ch);
            e.AddTab("Product Information", tdext3);

            UserControl2 userControl2 = new("Copy of SteeringWheels");
            TabbedDialogExtension tdext2 = new(userControl2,
                userControl2.OnOK)
            {
                OnCancelAction = userControl2.OnCancel
            };
            e.AddTab("SteeringWheels(Copy)", tdext2);

            UserControl1 userControl1 = new();
            new ContextualHelp(ContextualHelpType.Url, "http://www.google.com/");
            TabbedDialogExtension tdext1 = new(userControl1,
                userControl1.OnOK)
            {
                OnCancelAction = userControl1.OnCancel,
                OnRestoreDefaultsAction = userControl1.OnRestoreDefaults
            };
            tdext1.SetContextualHelp(ch);
            e.AddTab("WPF components", tdext1);
        }
    }
}
