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

namespace Ara3D.RevitSampleBrowser.ExportPDFSettingsSample.CS
{
    /// <summary>
    ///     ExternalApplication for ExportPDFSettings manipulation through Revit API. ExportPDFSettings is the element to store
    ///     PDFExportOptions in document.
    ///     This sample contains:
    ///     - Create ExportPDFSettings
    ///     - Modify ExportPDFSettings
    ///     - Add naming rule for ExportPDFSettings
    ///     - Modify naming rule for ExportPDFSettings
    ///     - Delete naming rule for ExportPDFSettings
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
                // Create user interface for ExportPDFSettings manipulation
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

    /// <summary>
    ///     ExternalCommand to create an ExportPDFSettings instance.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateExportPdfSettingsCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
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

    /// <summary>
    ///     ExternalCommand to modify an ExportPDFSettings instance.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ModifyExportPdfSettingsCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Modify ExportPDFSettings");
            trans.Start();

            try
            {
                var settings = ExportPDFSettings.FindByName(doc, "sample");
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();
                options.PaperFormat = ExportPaperFormat.ISO_A4; // Change paper format
                options.HideCropBoundaries = false; // Change hide crop boundaries
                options.Combine = false; // Change name into the pattern of naming rule
                settings.SetOptions(options); // Activate changes
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

    /// <summary>
    ///     ExternalCommand to add a naming rule to ExportPDFSettings instance.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class AddNamingRuleCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Add a naming rule");
            trans.Start();

            try
            {
                var settings = ExportPDFSettings.FindByName(doc, "sample");
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                // Naming rule remains the same in silence if exporting is combined
                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                // Get naming rule
                var namingRule = options.GetNamingRule();

                // Add naming parameter Sheets-Approved-By to naming rule
                var param = BuiltInParameter.SHEET_APPROVED_BY;
                var categoryId = Category.GetCategory(doc, BuiltInCategory.OST_Sheets).Id;
                var paramId = new ElementId(param);
                var itemSheetApprovedBy = TableCellCombinedParameterData.Create();
                itemSheetApprovedBy.CategoryId = categoryId;
                itemSheetApprovedBy.ParamId = paramId;
                itemSheetApprovedBy.Prefix = "-"; // You can also add prefix/suffix
                itemSheetApprovedBy.Separator = "-";
                itemSheetApprovedBy.SampleValue = param.ToString();
                namingRule.Add(itemSheetApprovedBy);
                // Don't forget to set naming rule for options
                options.SetNamingRule(namingRule);
                // And save the options for settings
                // Note that naming rule won't be changed if exporting is combined, see the comments of PDFExportOptions.SetOptions
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

    /// <summary>
    ///     ExternalCommand to modify a naming rule from ExportPDFSettings instance.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class MofidyNamingRuleCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Modify a naming rule");
            trans.Start();

            try
            {
                var settings = ExportPDFSettings.FindByName(doc, "sample");
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                // Naming rule remains the same in silence if exporting is combined
                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                // Get naming rule
                var namingRule = options.GetNamingRule();

                // Find SHEET_APPROVED_BY rule
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

                // Mofidy rule
                rule.SampleValue = "Modify my sample value";
                namingRule =
                    namingRule.OrderBy(data => data.SampleValue)
                        .ToList(); // The order of rules is defined by the naming rule list
                options.SetNamingRule(namingRule);
                // Note that naming rule won't be changed if exporting is combined, see the comments of PDFExportOptions.SetOptions
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

    /// <summary>
    ///     ExternalCommand to delete a naming rule from ExportPDFSettings instance.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DeleteNamingRuleCommand : IExternalCommand
    {
        /// <summary>
        ///     The implementation for IExternalCommand.Execute()
        /// </summary>
        /// <param name="commandData">The Revit command data.</param>
        /// <param name="message">The error message (ignored).</param>
        /// <param name="elements">The elements to display in the failure dialog (ignored).</param>
        /// <returns>Result.Succeeded</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Delete a naming rule");
            trans.Start();

            try
            {
                var settings = ExportPDFSettings.FindByName(doc, "sample");
                if (settings == null)
                {
                    message = "Cannot find sample settings";
                    trans.RollBack();
                    return Result.Failed;
                }

                var options = settings.GetOptions();

                // Naming rule remains the same in silence if exporting is combined
                if (options.Combine)
                {
                    message = "Exporting is combined. To change naming rule you need to set exporting not combined.";
                    trans.RollBack();
                    return Result.Failed;
                }

                // Get naming rule
                var namingRule = options.GetNamingRule();

                // Find SHEET_APPROVED_BY rule
                var param = BuiltInParameter.SHEET_APPROVED_BY;
                var categoryId = Category.GetCategory(doc, BuiltInCategory.OST_Sheets).Id;
                var paramId = new ElementId(param);
                var rule = namingRule.SingleOrDefault(r => r.CategoryId == categoryId && r.ParamId == paramId);

                // Delete rule
                namingRule.Remove(rule);
                options.SetNamingRule(namingRule);
                // Note that naming rule won't be changed if exporting is combined, see the comments of PDFExportOptions.SetOptions
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
