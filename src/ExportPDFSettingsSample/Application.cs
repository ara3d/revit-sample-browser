// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS
{
    /// <summary>
    /// Ribbon UI for creating and editing document <see cref="ExportPDFSettings"/> (stored <see cref="PDFExportOptions"/>).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExportPdfSettingsSampleApplication : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var panel = application.CreateRibbonPanel("ExportPDFSettings testing");

                var assembly = Assembly.GetExecutingAssembly();

                panel.AddItem(new PushButtonData("CreateExportPDFSettingsInstance",
                    "Create ExportPDFSettings Instance",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS.CreateExportPDFSettingsCommand"));

                panel.AddSeparator();

                panel.AddItem(new PushButtonData("ModifyExportPDFSettingsInstance",
                    "Modify ExportPDFSettings Instance",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS.ModifyExportPDFSettingsCommand"));

                panel.AddSeparator();

                panel.AddItem(new PushButtonData("AddNamingRule",
                    "Add Naming Rule",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS.AddNamingRuleCommand"));

                panel.AddSeparator();

                panel.AddItem(new PushButtonData("ModifyNamingRule",
                    "Mofidy Naming Rule",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS.MofidyNamingRuleCommand"));

                panel.AddSeparator();

                panel.AddItem(new PushButtonData("DeleteNamingRule",
                    "Delete Naming Rule",
                    assembly.Location,
                    "Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS.DeleteNamingRuleCommand"));
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception from OnStartup", e.ToString());
            }

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CreateExportPdfSettingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Create ExportPDFSettings");
            trans.Start();

            try
            {
                var options = new PDFExportOptions();
                var name = "sample";
                ExportPDFSettings.Create(doc, name, options);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class ModifyExportPdfSettingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Modify ExportPDFSettings");
            trans.Start();

            try
            {
                var settings = PrintHelper.FindSampleSettings(doc);
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();
                options.PaperFormat = ExportPaperFormat.ISO_A4;
                options.HideCropBoundaries = false;
                options.Combine = false;
                settings.SetOptions(options);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class AddNamingRuleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Add a naming rule");
            trans.Start();

            try
            {
                var settings = PrintHelper.FindSampleSettings(doc);
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                // PDFExportOptions ignores naming-rule changes while Combine is true.
                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                var namingRule = options.GetNamingRule();

                var param = BuiltInParameter.SHEET_APPROVED_BY;
                var categoryId = Category.GetCategory(doc, BuiltInCategory.OST_Sheets).Id;
                var paramId = new ElementId(param);
                var itemSheetApprovedBy = TableCellCombinedParameterData.Create();
                itemSheetApprovedBy.CategoryId = categoryId;
                itemSheetApprovedBy.ParamId = paramId;
                itemSheetApprovedBy.Prefix = "-";
                itemSheetApprovedBy.Separator = "-";
                itemSheetApprovedBy.SampleValue = param.ToString();
                namingRule.Add(itemSheetApprovedBy);
                options.SetNamingRule(namingRule);
                settings.SetOptions(options);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class MofidyNamingRuleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Modify a naming rule");
            trans.Start();

            try
            {
                var settings = PrintHelper.FindSampleSettings(doc);
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                var namingRule = options.GetNamingRule();

                var param = BuiltInParameter.SHEET_APPROVED_BY;
                var categoryId = Category.GetCategory(doc, BuiltInCategory.OST_Sheets).Id;
                var paramId = new ElementId(param);
                var rule = namingRule.SingleOrDefault(r => r.CategoryId == categoryId && r.ParamId == paramId);
                if (rule == null)
                {
                    message = "No such rule in naming rule";
                    trans.RollBack();
                    return Result.Failed;
                }

                rule.SampleValue = "Modify my sample value";
                namingRule =
                    namingRule.OrderBy(data => data.SampleValue)
                        .ToList();
                options.SetNamingRule(namingRule);
                settings.SetOptions(options);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class DeleteNamingRuleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Delete a naming rule");
            trans.Start();

            try
            {
                var settings = PrintHelper.FindSampleSettings(doc);
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                var namingRule = options.GetNamingRule();

                var param = BuiltInParameter.SHEET_APPROVED_BY;
                var categoryId = Category.GetCategory(doc, BuiltInCategory.OST_Sheets).Id;
                var paramId = new ElementId(param);
                var rule = namingRule.SingleOrDefault(r => r.CategoryId == categoryId && r.ParamId == paramId);

                namingRule.Remove(rule);
                options.SetNamingRule(namingRule);
                settings.SetOptions(options);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
