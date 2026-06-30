// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        // An object that is passed to the external application which contains the controlled Revit application.
        private UIControlledApplication m_controlApp;

        public Result OnStartup(UIControlledApplication application)
        {
            m_controlApp = application;

            // Doors are updated from the application level events. 
            // That will insure that the doc is correct when it is saved.
            // Subscribe to related events.
            application.ControlledApplication.DocumentSaving += DocumentSavingHandler;
            application.ControlledApplication.DocumentSavingAs += DocumentSavingAsHandler;

            // The location of this command assembly
            var currentCommandAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // The directory path of buttons' images
            var buttonImageDir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName
                (Path.GetDirectoryName(currentCommandAssemblyPath))));

            // begin to create custom Ribbon panel and command buttons.
            var doorPanel = application.CreateRibbonPanel("Door Swing");

            // the first button in the DoorSwing panel, use to invoke the InitializeCommand.
            var initialCommandBut = doorPanel.AddItem(new PushButtonData("Customize Door Opening",
                    "Customize Door Opening",
                    currentCommandAssemblyPath,
                    typeof(InitializeCommand).FullName))
                as PushButton;
            initialCommandBut.ToolTip = "Customize the expression based on family's geometry and country's standard.";
            initialCommandBut.LargeImage =
                new BitmapImage(new Uri(Path.Combine(buttonImageDir, "InitialCommand_Large.bmp")));
            initialCommandBut.Image =
                new BitmapImage(new Uri(Path.Combine(buttonImageDir, "InitialCommand_Small.bmp")));

            // the second button in the DoorSwing panel, use to invoke the UpdateParamsCommand.
            var updateParamBut = doorPanel.AddItem(new PushButtonData("Update Door Properties",
                    "Update Door Properties",
                    currentCommandAssemblyPath,
                    typeof(UpdateParamsCommand).FullName))
                as PushButton;
            updateParamBut.ToolTip = "Update door properties based on geometry.";
            updateParamBut.LargeImage =
                new BitmapImage(new Uri(Path.Combine(buttonImageDir, "UpdateParameter_Large.bmp")));
            updateParamBut.Image = new BitmapImage(new Uri(Path.Combine(buttonImageDir, "UpdateParameter_Small.bmp")));

            // the third button in the DoorSwing panel, use to invoke the UpdateGeometryCommand.
            var updateGeoBut = doorPanel.AddItem(new PushButtonData("Update Door Geometry",
                    "Update Door Geometry",
                    currentCommandAssemblyPath,
                    typeof(UpdateGeometryCommand).FullName))
                as PushButton;
            updateGeoBut.ToolTip = "Update door geometry based on From/To room property.";
            updateGeoBut.LargeImage =
                new BitmapImage(new Uri(Path.Combine(buttonImageDir, "UpdateGeometry_Large.bmp")));
            updateGeoBut.Image = new BitmapImage(new Uri(Path.Combine(buttonImageDir, "UpdateGeometry_Small.bmp")));

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        private void DocumentSavingHandler(object sender, DocumentSavingEventArgs args)
        {
            var message = "";
            Transaction tran = null;

            try
            {
                var doc = args.Document;
                if (doc.IsModifiable)
                {
                    if (DoorSwingData.UpdateDoorsInfo(args.Document, false, false, ref message) != Result.Succeeded)
                        TaskDialog.Show("Door Swing", message);
                }
                else
                {
                    tran = new Transaction(doc, "Update parameters in Saving event");
                    tran.Start();

                    if (DoorSwingData.UpdateDoorsInfo(args.Document, false, false, ref message) != Result.Succeeded)
                        TaskDialog.Show("Door Swing", message);

                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                // if there are something wrong, give error information message.
                TaskDialog.Show("Door Swing", ex.Message);
                if (null != tran)
                    if (tran.HasStarted() && !tran.HasEnded())
                        tran.RollBack();
            }
        }

        private void DocumentSavingAsHandler(object sender, DocumentSavingAsEventArgs args)
        {
            var message = "";
            Transaction tran = null;
            try
            {
                var doc = args.Document;
                if (doc.IsModifiable)
                {
                    if (DoorSwingData.UpdateDoorsInfo(args.Document, false, false, ref message) != Result.Succeeded)
                        TaskDialog.Show("Door Swing", message);
                }
                else
                {
                    tran = new Transaction(doc, "Update parameters in Saving event");
                    tran.Start();

                    if (DoorSwingData.UpdateDoorsInfo(args.Document, false, false, ref message) != Result.Succeeded)
                        TaskDialog.Show("Door Swing", message);

                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                // if there are something wrong, give error message.
                TaskDialog.Show("Door Swing", ex.Message);

                if (null != tran)
                    if (tran.HasStarted() && !tran.HasEnded())
                        tran.RollBack();
            }
        }
    }
}
