// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Revit.SDK.Samples.UIAPI.CS.OptionsDialog;

namespace Revit.SDK.Samples.UIAPI.CS
{
    public class AddTabCommand
    {
        private readonly UIControlledApplication _application;

        public AddTabCommand(UIControlledApplication application)
        {
            _application = application;
        }

        public bool AddTabToOptionsDialog()
        {
            _application.DisplayingOptionsDialog +=
                Command_DisplayingOptionDialog;
            return true;
        }

        private void Command_DisplayingOptionDialog(object sender, DisplayingOptionsDialogEventArgs e)
        {
            // Actual options
            var optionsControl = new Options();
            var ch = new ContextualHelp(ContextualHelpType.Url, "http://www.autodesk.com/");
            var extension = new TabbedDialogExtension(optionsControl, optionsControl.OnOK)
 {
     OnRestoreDefaultsAction = optionsControl.OnRestoreDefaults
 };
            extension.SetContextualHelp(ch);
            e.AddTab("Demo options", extension);

            // Demo options
            var userControl3 = new UserControl3("Product Information");
            new ContextualHelp(ContextualHelpType.Url, "http://www.google.com/");
            var tdext3 = new TabbedDialogExtension(userControl3,
                userControl3.OnOK)
            {
                OnCancelAction = userControl3.OnCancel,
                OnRestoreDefaultsAction = userControl3.OnRestoreDefaults
            };
            tdext3.SetContextualHelp(ch);
            e.AddTab("Product Information", tdext3);

            var userControl2 = new UserControl2("Copy of SteeringWheels");
            var tdext2 = new TabbedDialogExtension(userControl2,
                userControl2.OnOK)
            {
                OnCancelAction = userControl2.OnCancel
            };
            e.AddTab("SteeringWheels(Copy)", tdext2);

            var userControl1 = new UserControl1();
            new ContextualHelp(ContextualHelpType.Url, "http://www.google.com/");
            var tdext1 = new TabbedDialogExtension(userControl1,
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
