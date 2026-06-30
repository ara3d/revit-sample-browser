// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ListImportInstances by Nikolay Shulga and Jeremy Tammik (MIT).
// https://github.com/jeremytammik/ListImportInstances

using System.Collections.Specialized;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ListImportInstances.CS
{
    /// <summary>
    /// Lists ImportInstance elements in the active project and writes a report file.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ListImportInstancesCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            ListImports(uidoc.Document);
            return Result.Succeeded;
        }

        static void ListImports(Document doc)
        {
            var col = new FilteredElementCollector(doc)
                .OfClass(typeof(ImportInstance));

            var listOfViewSpecificImports = new NameValueCollection();
            var listOfModelImports = new NameValueCollection();
            var listOfUnidentifiedImports = new NameValueCollection();

            foreach (Element e in col)
            {
                if (e.ViewSpecific)
                {
                    string viewName;
                    try
                    {
                        var viewElement = doc.GetElement(e.OwnerViewId);
                        viewName = viewElement?.Name ?? e.OwnerViewId.ToString();
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentNullException)
                    {
                        viewName = "Invalid View ID: " + e.OwnerViewId;
                    }

                    if (e.Category != null)
                    {
                        listOfViewSpecificImports.Add(
                            ImportCategoryNameToFileName(e.Category.Name), viewName);
                    }
                    else
                    {
                        listOfUnidentifiedImports.Add(e.Id.ToString(), viewName);
                    }
                }
                else
                {
                    listOfModelImports.Add(
                        ImportCategoryNameToFileName(e.Category.Name), e.Name);
                }
            }

            var logOutput = new SimpleTextFileBasedReporter();
            if (!logOutput.Init(doc.PathName))
            {
                TaskDialog.Show("FindImports", "Unable to create report file");
                return;
            }

            if (listOfViewSpecificImports.HasKeys())
            {
                logOutput.StartReportSection("View Specific Imports");
                ListResults(listOfViewSpecificImports, logOutput);
            }

            if (listOfModelImports.HasKeys())
            {
                logOutput.StartReportSection("Model Imports");
                ListResults(listOfModelImports, logOutput);
            }

            if (listOfUnidentifiedImports.HasKeys())
            {
                logOutput.StartReportSection("Unknown import instances");
                ListResults(listOfUnidentifiedImports, logOutput);
            }

            if (!SanityCheckViewSpecific(listOfViewSpecificImports, logOutput))
                logOutput.SetWarning();

            logOutput.Done();
        }

        /// <summary>
        /// Import categories are created from CAD file names with a numeric suffix in parentheses.
        /// Strip the suffix to use the file name as the report key.
        /// </summary>
        static string ImportCategoryNameToFileName(string catName)
        {
            var fileName = catName.Trim();
            if (!fileName.EndsWith(")"))
                return fileName;

            var lastLeftBracket = fileName.LastIndexOf('(');
            if (lastLeftBracket != -1)
                fileName = fileName.Remove(lastLeftBracket);

            return fileName.Trim();
        }

        static void ListResults(NameValueCollection listOfImports, IReportImportData logFile)
        {
            foreach (var key in listOfImports.AllKeys)
                logFile.LogItem(key + ": " + listOfImports.Get(key));
        }

        /// <summary>
        /// Flags view-specific imports that appear in more than one view.
        /// </summary>
        static bool SanityCheckViewSpecific(
            NameValueCollection listOfImports,
            IReportImportData logFile)
        {
            logFile.StartReportSection("Sanity check report for view-specific imports");

            var status = true;
            foreach (var key in listOfImports.AllKeys)
            {
                var levels = listOfImports.GetValues(key);
                if (levels != null && levels.Length > 1)
                {
                    logFile.LogItem(
                        "CAD data " + key
                        + " appears to have been imported in Current View Only mode multiple times. "
                        + "It is present in views "
                        + listOfImports.Get(key));
                    status = false;
                }
            }

            return status;
        }
    }
}
