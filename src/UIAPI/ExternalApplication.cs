// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.UIAPI.CS.OptionsDialog;
using Ara3D.RevitSampleBrowser.UIAPI.CS.Properties;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Ara3D.RevitSampleBrowser.UIAPI.CS
{
    public class ExternalApp : IExternalApplication
    {
        private static readonly string AddinAssmeblyPath = typeof(ExternalApp).Assembly.Location;
        private readonly UIControlledApplication m_uiControlledApplication;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            ApplicationOptions.Initialize(this);

            CreateCommandBinding(application);
            CreateRibbonButton(application);

            // add custom tabs to options dialog.
            AddTabCommand addTabCommand = new(application);
            addTabCommand.AddTabToOptionsDialog();
            return Result.Succeeded;
        }

        private void CreateCommandBinding(UIControlledApplication application)
        {
            var wallCreate = RevitCommandId.LookupCommandId("ID_NEW_REVIT_DESIGN_MODEL");
            var binding = application.CreateAddInCommandBinding(wallCreate);
            binding.Executed += binding_Executed;
            binding.CanExecute += binding_CanExecute;
        }

        private void CreateRibbonButton(UIControlledApplication application)
        {
            application.CreateRibbonTab("AddIn Integration");
            var rp = application.CreateRibbonPanel("AddIn Integration", "Testing");

            PushButtonData pbd = new("Wall", "Goto WikiHelp for wall creation",
                AddinAssmeblyPath,
                "Ara3D.RevitSampleBrowser.UIAPI.CS.CalcCommand");
            ContextualHelp ch = new(ContextualHelpType.ContextId, "HID_OBJECTS_WALL");
            pbd.SetContextualHelp(ch);
            pbd.LongDescription = "We redirect the wiki help for this button to Wall creation.";
            pbd.LargeImage = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall);
            pbd.Image = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall_S);

            var pb = rp.AddItem(pbd) as PushButton;
            pb.Enabled = true;
            pb.AvailabilityClassName = "Ara3D.RevitSampleBrowser.UIAPI.CS.ApplicationAvailabilityClass";

            PushButtonData pbd1 = new("GotoGoogle", "Go to Google",
                AddinAssmeblyPath,
                "Ara3D.RevitSampleBrowser.UIAPI.CS.CalcCommand");
            ContextualHelp ch1 = new(ContextualHelpType.Url, "http://www.google.com/");
            pbd1.SetContextualHelp(ch1);
            pbd1.LongDescription = "Go to google.";
            pbd1.LargeImage = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall);
            pbd1.Image = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall_S);
            var pb1 = rp.AddItem(pbd1) as PushButton;
            pb1.AvailabilityClassName = "Ara3D.RevitSampleBrowser.UIAPI.CS.ApplicationAvailabilityClass";

            PushButtonData pbd2 = new("GotoRevitAddInUtilityHelpFile", "Go to Revit Add-In Utility",
                AddinAssmeblyPath,
                "Ara3D.RevitSampleBrowser.UIAPI.CS.CalcCommand");

            ContextualHelp ch2 = new(ContextualHelpType.ChmFile,
                $@"{Path.GetDirectoryName(AddinAssmeblyPath)}\RevitAddInUtility.chm")
            {
                HelpTopicUrl = @"html/3374f8f0-dccc-e1df-d269-229ed8c60e93.htm"
            };
            pbd2.SetContextualHelp(ch2);
            pbd2.LongDescription = "Go to Revit Add-In Utility.";
            pbd2.LargeImage = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall);
            pbd2.Image = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall_S);
            var pb2 = rp.AddItem(pbd2) as PushButton;
            pb2.AvailabilityClassName = "Ara3D.RevitSampleBrowser.UIAPI.CS.ApplicationAvailabilityClass";

            PushButtonData pbd3 = new("PreviewControl", "Preview all views",
                AddinAssmeblyPath,
                "Ara3D.RevitSampleBrowser.UIAPI.CS.PreviewCommand")
            {
                LargeImage = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall),
                Image = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall_S)
            };
            var pb3 = rp.AddItem(pbd3) as PushButton;
            pb3.AvailabilityClassName = "Ara3D.RevitSampleBrowser.UIAPI.CS.ApplicationAvailabilityClass";

            PushButtonData pbd5 = new("Drag_And_Drop", "Drag and Drop", AddinAssmeblyPath,
                "Ara3D.RevitSampleBrowser.UIAPI.CS.DragAndDropCommand")
            {
                LargeImage = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall),
                Image = BitmapHelper.ConvertFromBitmap(Resources.StrcturalWall_S)
            };
            var pb5 = rp.AddItem(pbd5) as PushButton;
            pb5.AvailabilityClassName = "Ara3D.RevitSampleBrowser.UIAPI.CS.ApplicationAvailabilityClass";
        }

        private void binding_CanExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = true;
        }

        private void binding_Executed(object sender, ExecutedEventArgs e)
        {
            if (sender is not UIApplication uiApp)
                return;

            var famTemplatePath = uiApp.Application.FamilyTemplatePath;
            var conceptualmassTemplatePath = $@"{famTemplatePath}\Conceptual Mass\Mass.rft";
            if (File.Exists(conceptualmassTemplatePath))
            {
                //uiApp.OpenAndActivateDocument(conceptualmassTemplatePath);
                var familyDocument = uiApp.Application.NewFamilyDocument(conceptualmassTemplatePath);
                if (null == familyDocument) throw new Exception("Cannot open family document");

                var fileName = $"{Guid.NewGuid()}.rfa";
                familyDocument.SaveAs(fileName);
                familyDocument.Close();

                uiApp.OpenAndActivateDocument(fileName);

                FilteredElementCollector collector = new(uiApp.ActiveUIDocument.Document);
                collector = collector.OfClass(typeof(View3D));

                var query = from element in collector
                                             where element.Name == "{3D}"
                                             select element; // Linq query  

                var views = query.ToList();

                if (views[0] is View3D view3D)
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
