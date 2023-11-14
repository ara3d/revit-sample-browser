// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Revit.SDK.Samples.UIAPI.CS.Properties;

namespace Revit.SDK.Samples.UIAPI.CS
{
    public class ExternalApp : IExternalApplication
    {
        private static readonly string addinAssmeblyPath = typeof(ExternalApp).Assembly.Location;

        public UIControlledApplication UIControlledApplication { get; private set; }


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            UIControlledApplication = application;
            ApplicationOptions.Initialize(this);

            createCommandBinding(application);
            createRibbonButton(application);

            // add custom tabs to options dialog.
            var addTabCommand = new AddTabCommand(application);
            addTabCommand.AddTabToOptionsDialog();
            return Result.Succeeded;
        }

        /// <summary>
        ///     Loads the default Mass template automatically rather than showing UI.
        /// </summary>
        /// <param name="application">
        ///     An object that is passed to the external application
        ///     which contains the controlled application.
        /// </param>
        private void createCommandBinding(UIControlledApplication application)
        {
            var wallCreate = RevitCommandId.LookupCommandId("ID_NEW_REVIT_DESIGN_MODEL");
            var binding = application.CreateAddInCommandBinding(wallCreate);
            binding.Executed += binding_Executed;
            binding.CanExecute += binding_CanExecute;
        }


        private BitmapSource convertFromBitmap(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        private void createRibbonButton(UIControlledApplication application)
        {
            application.CreateRibbonTab("AddIn Integration");
            var rp = application.CreateRibbonPanel("AddIn Integration", "Testing");

            var pbd = new PushButtonData("Wall", "Goto WikiHelp for wall creation",
                addinAssmeblyPath,
                "Revit.SDK.Samples.UIAPI.CS.CalcCommand");
            var ch = new ContextualHelp(ContextualHelpType.ContextId, "HID_OBJECTS_WALL");
            pbd.SetContextualHelp(ch);
            pbd.LongDescription = "We redirect the wiki help for this button to Wall creation.";
            pbd.LargeImage = convertFromBitmap(Resources.StrcturalWall);
            pbd.Image = convertFromBitmap(Resources.StrcturalWall_S);

            var pb = rp.AddItem(pbd) as PushButton;
            pb.Enabled = true;
            pb.AvailabilityClassName = "Revit.SDK.Samples.UIAPI.CS.ApplicationAvailabilityClass";

            var pbd1 = new PushButtonData("GotoGoogle", "Go to Google",
                addinAssmeblyPath,
                "Revit.SDK.Samples.UIAPI.CS.CalcCommand");
            var ch1 = new ContextualHelp(ContextualHelpType.Url, "http://www.google.com/");
            pbd1.SetContextualHelp(ch1);
            pbd1.LongDescription = "Go to google.";
            pbd1.LargeImage = convertFromBitmap(Resources.StrcturalWall);
            pbd1.Image = convertFromBitmap(Resources.StrcturalWall_S);
            var pb1 = rp.AddItem(pbd1) as PushButton;
            pb1.AvailabilityClassName = "Revit.SDK.Samples.UIAPI.CS.ApplicationAvailabilityClass";

            var pbd2 = new PushButtonData("GotoRevitAddInUtilityHelpFile", "Go to Revit Add-In Utility",
                addinAssmeblyPath,
                "Revit.SDK.Samples.UIAPI.CS.CalcCommand");

            var ch2 = new ContextualHelp(ContextualHelpType.ChmFile,
                Path.GetDirectoryName(addinAssmeblyPath) + @"\RevitAddInUtility.chm");
            ch2.HelpTopicUrl = @"html/3374f8f0-dccc-e1df-d269-229ed8c60e93.htm";
            pbd2.SetContextualHelp(ch2);
            pbd2.LongDescription = "Go to Revit Add-In Utility.";
            pbd2.LargeImage = convertFromBitmap(Resources.StrcturalWall);
            pbd2.Image = convertFromBitmap(Resources.StrcturalWall_S);
            var pb2 = rp.AddItem(pbd2) as PushButton;
            pb2.AvailabilityClassName = "Revit.SDK.Samples.UIAPI.CS.ApplicationAvailabilityClass";


            var pbd3 = new PushButtonData("PreviewControl", "Preview all views",
                addinAssmeblyPath,
                "Revit.SDK.Samples.UIAPI.CS.PreviewCommand");
            pbd3.LargeImage = convertFromBitmap(Resources.StrcturalWall);
            pbd3.Image = convertFromBitmap(Resources.StrcturalWall_S);
            var pb3 = rp.AddItem(pbd3) as PushButton;
            pb3.AvailabilityClassName = "Revit.SDK.Samples.UIAPI.CS.ApplicationAvailabilityClass";


            var pbd5 = new PushButtonData("Drag_And_Drop", "Drag and Drop", addinAssmeblyPath,
                "Revit.SDK.Samples.UIAPI.CS.DragAndDropCommand");
            pbd5.LargeImage = convertFromBitmap(Resources.StrcturalWall);
            pbd5.Image = convertFromBitmap(Resources.StrcturalWall_S);
            var pb5 = rp.AddItem(pbd5) as PushButton;
            pb5.AvailabilityClassName = "Revit.SDK.Samples.UIAPI.CS.ApplicationAvailabilityClass";
        }

        private void binding_CanExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = true;
        }

        private void binding_Executed(object sender, ExecutedEventArgs e)
        {
            var uiApp = sender as UIApplication;
            if (uiApp == null)
                return;

            var famTemplatePath = uiApp.Application.FamilyTemplatePath;
            var conceptualmassTemplatePath = famTemplatePath + @"\Conceptual Mass\Mass.rft";
            if (File.Exists(conceptualmassTemplatePath))
            {
                //uiApp.OpenAndActivateDocument(conceptualmassTemplatePath);
                var familyDocument = uiApp.Application.NewFamilyDocument(conceptualmassTemplatePath);
                if (null == familyDocument) throw new Exception("Cannot open family document");

                var fileName = Guid.NewGuid() + ".rfa";
                familyDocument.SaveAs(fileName);
                familyDocument.Close();

                uiApp.OpenAndActivateDocument(fileName);

                var collector = new FilteredElementCollector(uiApp.ActiveUIDocument.Document);
                collector = collector.OfClass(typeof(View3D));

                var query = from element in collector
                    where element.Name == "{3D}"
                    select element; // Linq query  

                var views = query.ToList();

                var view3D = views[0] as View3D;
                if (view3D != null)
                    uiApp.ActiveUIDocument.ActiveView = view3D;
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CalcCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Dummy command", "This is a dummy command for buttons associated to contextual help.");
            return Result.Succeeded;
        }
    }
}
