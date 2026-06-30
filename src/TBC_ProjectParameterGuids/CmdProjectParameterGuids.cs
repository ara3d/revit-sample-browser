#region Header

//
// CmdProjectParameterGuids.cs - determine and report all project parameter GUIDs
//
// Copyright (C) 2015-2020 by CoderBoy and Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
// Written by CoderBoy, cf.
// http://forums.autodesk.com/t5/revit-api/reporting-on-project-parameter-definitions-need-guids/m-p/5947552
// http://forums.autodesk.com/t5/revit-api/create-project-parameter-with-quot-values-can-vary-by-group/m-p/5939455
//

#endregion // Header

#region Namespaces

using System.Text;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdProjectParameterGuids : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            if (doc.IsFamilyDocument)
            {
                message = "The document must be a project document.";
                return Result.Failed;
            }

            // Get the (singleton) element that is the 
            // ProjectInformation object.  It can only have 
            // instance parameters bound to it, and it is 
            // always guaranteed to exist.

            var projectInfoElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ProjectInformation)
                    .FirstElement();

            // Get the first wall type element.  It can only 
            // have type parameters bound to it, and there is 
            // always guaranteed to be at least one of these.

            var firstWallTypeElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsElementType()
                    .FirstElement();

            CategorySet categories = null;
            Parameter foundParameter = null;

            // Get the list of information about all project 
            // parameters, calling our helper method, below.  

            var projectParametersData
                = Util.GetProjectParameterData(doc);

            // In order to be able to query whether or not a 
            // project parameter is shared or not, and if it 
            // is shared then what it's GUID is, we must ensure 
            // it exists in the Parameters collection of an 
            // element.
            // This is because we cannot query this information 
            // directly from the project parameter bindings 
            // object.
            // So each project parameter will attempt to be 
            // temporarily bound to a known object so a 
            // Parameter object created from it will exist 
            // and can be queried for this additional 
            // information.

            foreach (var projectParameterData
                in projectParametersData)
                if (projectParameterData.Definition != null)
                {
                    categories = projectParameterData.Binding.Categories;
                    if (!categories.Contains(projectInfoElement.Category))
                    {
                        // This project parameter is not already 
                        // bound to the ProjectInformation category,
                        // so we must temporarily bind it so we can 
                        // query that object for it.

                        using var tempTransaction
                            = new Transaction(doc);
                        tempTransaction.Start("Temporary");

                        // Try to bind the project parameter do 
                        // the project information category, 
                        // calling our helper method, below.

                        if (Util.AddProjectParameterBinding(
                            doc, projectParameterData,
                            projectInfoElement.Category))
                        {
                            // successfully bound
                            foundParameter
                                = projectInfoElement.get_Parameter(
                                    projectParameterData.Definition);

                            if (foundParameter == null)
                            {
                                // Must be a shared type parameter, 
                                // which the API reports that it binds
                                // to the project information category 
                                // via the API, but doesn't ACTUALLY 
                                // bind to the project information 
                                // category.  (Sheesh!)

                                // So we must use a different, type 
                                // based object known to exist, and 
                                // try again.

                                if (!categories.Contains(
                                    firstWallTypeElement.Category))
                                {
                                    // Add it to walls category as we 
                                    // did with project info for the 
                                    // others, calling our helper 
                                    // method, below.

                                    if (Util.AddProjectParameterBinding(
                                            doc, projectParameterData,
                                            firstWallTypeElement.Category))
                                        // Successfully bound
                                        foundParameter
                                            = firstWallTypeElement.get_Parameter(
                                                projectParameterData.Definition);
                                }
                                else
                                {
                                    // The project parameter was already 
                                    // bound to the Walls category.
                                    foundParameter
                                        = firstWallTypeElement.get_Parameter(
                                            projectParameterData.Definition);
                                }

                                if (foundParameter != null)
                                    Util.PopulateProjectParameterData(
                                        foundParameter,
                                        projectParameterData);
                                else
                                    // Wouldn't bind to the walls 
                                    // category or wasn't found when 
                                    // already bound.
                                    // This should probably never happen?

                                    projectParameterData.IsSharedStatusKnown
                                        = false; // Throw exception?
                            }
                            else
                            {
                                // Found the correct parameter 
                                // instance on the Project 
                                // Information object, so use it.

                                Util.PopulateProjectParameterData(
                                    foundParameter,
                                    projectParameterData);
                            }
                        }
                        else
                        {
                            // The API reports it couldn't bind 
                            // the parameter to the ProjectInformation 
                            // category.
                            // This only happens with non-shared 
                            // Project parameters, which have no 
                            // GUID anyway.

                            projectParameterData.IsShared = false;
                            projectParameterData.IsSharedStatusKnown = true;
                        }

                        tempTransaction.RollBack();
                    }
                    else
                    {
                        // The project parameter was already bound 
                        // to the Project Information category.

                        foundParameter
                            = projectInfoElement.get_Parameter(
                                projectParameterData.Definition);

                        if (foundParameter != null)
                            Util.PopulateProjectParameterData(
                                foundParameter, projectParameterData);
                        else
                            // This will probably never happen.

                            projectParameterData.IsSharedStatusKnown
                                = false; // Throw exception?
                    }
                } // Whether or not the Definition object could be found

            var sb = new StringBuilder();

            // Build column headers

            sb.AppendLine("PARAMETER NAME\tIS SHARED?\tGUID");

            // Add each row.

            foreach (var projectParameterData
                in projectParametersData)
            {
                sb.Append(projectParameterData.Name);
                sb.Append("\t");

                if (projectParameterData.IsSharedStatusKnown)
                    sb.Append(projectParameterData.IsShared.ToString());
                else
                    sb.Append("<Unknown>");

                if (projectParameterData.IsSharedStatusKnown &&
                    projectParameterData.IsShared)
                {
                    sb.Append("\t");
                    sb.Append(projectParameterData.GUID);
                }

                sb.AppendLine();
            }

            Clipboard.Clear();
            Clipboard.SetText(sb.ToString());

            var resultsDialog = new TaskDialog(
                "Results are in the Clipboard");

            resultsDialog.MainInstruction
                = "Results are in the Clipboard";

            resultsDialog.MainContent
                = "Paste the clipboard into a spreadsheet "
                  + "program to see the results.";

            resultsDialog.Show();

            return Result.Succeeded;
        }
    }
}