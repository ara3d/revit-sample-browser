// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.HelloRevit.CS
{
    /// <summary>
    ///     Demonstrate how a basic ExternalCommand can be added to the Revit user interface.
    ///     And demonstrate how to create a Revit style dialog.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // NOTES: Anything can be done in this method, such as create a message box, 
            // a task dialog or fetch some information from revit and so on.
            // We mainly use the task dialog for example.

            // Get the application and document from external command data.
            var app = commandData.Application.Application;
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            // Study how to create a revit style dialog using task dialog API by following
            // code snippet.  

            // Creates a Revit task dialog to communicate information to the interactive user.
            var mainDialog = new TaskDialog("Hello, Revit!");
            mainDialog.MainInstruction = "Hello, Revit!";
            mainDialog.MainContent =
                "This sample shows how a basic ExternalCommand can be added to the Revit user interface."
                + " It uses a Revit task dialog to communicate information to the interactive user.\n"
                + "The command links below open additional task dialogs with more information.";

            // Add commmandLink to task dialog
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                "View information about the Revit installation");
            mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                "View information about the active document");

            // Set common buttons and default button. If no CommonButton or CommandLink is added,
            // task dialog will show a Close button by default.
            mainDialog.CommonButtons = TaskDialogCommonButtons.Close;
            mainDialog.DefaultButton = TaskDialogResult.Close;

            // Set footer text. Footer text is usually used to link to the help document.
            mainDialog.FooterText =
                "<a href=\"http://usa.autodesk.com/adsk/servlet/index?siteID=123112&id=2484975 \">"
                + "Click here for the Revit API Developer Center</a>";

            var tResult = mainDialog.Show();

            switch (tResult)
            {
                // If the user clicks the first command link, a simple Task Dialog 
                // with only a Close button shows information about the Revit installation. 
                case TaskDialogResult.CommandLink1:
                {
                    var dialogCommandLink1 = new TaskDialog("Revit Build Information");
                    dialogCommandLink1.MainInstruction =
                        "Revit Version Name is: " + app.VersionName + "\n"
                        + "Revit Version Number is: " + app.VersionNumber + "\n"
                        + "Revit Version Build is: " + app.VersionBuild;

                    dialogCommandLink1.Show();
                    break;
                }
                // If the user clicks the second command link, a simple Task Dialog 
                // created by static method shows information about the active document.
                case TaskDialogResult.CommandLink2:
                    TaskDialog.Show("Active Document Information",
                        "Active document: " + activeDoc.Title + "\n"
                        + "Active view name: " + activeDoc.ActiveView.Name);
                    break;
            }

            return Result.Succeeded;
        }
    }
}
