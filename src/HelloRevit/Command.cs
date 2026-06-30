// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.HelloRevit.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            var mainDialog = new TaskDialog("Hello, Revit!")
            {
                MainInstruction = "Hello, Revit!",
                MainContent =
                "This sample shows how a basic ExternalCommand can be added to the Revit user interface."
                + " It uses a Revit task dialog to communicate information to the interactive user.\n"
                + "The command links below open additional task dialogs with more information."
            };

            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                "View information about the Revit installation");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                "View information about the active document");

            mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
            mainDialog.DefaultButton = TaskDialogResult.Close;

            mainDialog.FooterText =
                "<a href=\"http://usa.autodesk.com/adsk/servlet/index?siteID=123112&id=2484975 \">"
                + "Click here for the Revit API Developer Center</a>";

            var tResult = mainDialog.Show();

            switch (tResult)
            {
                case TaskDialogResult.CommandLink1:
                {
                        var dialogCommandLink1 = new TaskDialog("Revit Build Information")
                        {
                            MainInstruction =
                                $"Revit Version Name is: {app.VersionName}\nRevit Version Number is: {app.VersionNumber}\nRevit Version Build is: {app.VersionBuild}"
                        };

                        dialogCommandLink1.Show();
                    break;
                }
                case TaskDialogResult.CommandLink2:
                    TaskDialog.Show("Active Document Information",
                        $"Active document: {activeDoc.Title}\nActive view name: {activeDoc.ActiveView.Name}");
                    break;
            }

            return Result.Succeeded;
        }
    }
}
