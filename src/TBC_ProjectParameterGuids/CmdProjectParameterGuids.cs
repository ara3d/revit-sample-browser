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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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

            var projectInfoElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ProjectInformation)
                    .FirstElement();

            var firstWallTypeElement
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls)
                    .WhereElementIsElementType()
                    .FirstElement();

            CategorySet categories = null;
            Parameter foundParameter = null;

            var projectParametersData
                = Util.GetProjectParameterData(doc);

            // Shared status/GUID are not available from ParameterBindings; bind temporarily to read them.

            foreach (var projectParameterData
                in projectParametersData)
                if (projectParameterData.Definition != null)
                {
                    categories = projectParameterData.Binding.Categories;
                    if (!categories.Contains(projectInfoElement.Category))
                    {
                        using Transaction tempTransaction
                            = new(doc);
                        tempTransaction.Start("Temporary");

                        if (Util.AddProjectParameterBinding(
                            doc, projectParameterData,
                            projectInfoElement.Category))
                        {

                            foundParameter
                                = projectInfoElement.get_Parameter(
                                    projectParameterData.Definition);

                            if (foundParameter == null)
                            {
                                // Shared type parameters may report ProjectInformation binding but not appear on that element.

                                if (!categories.Contains(
                                    firstWallTypeElement.Category))
                                {
                                    if (Util.AddProjectParameterBinding(
                                            doc, projectParameterData,
                                            firstWallTypeElement.Category))
                                        foundParameter
                                            = firstWallTypeElement.get_Parameter(
                                                projectParameterData.Definition);
                                }
                                else
                                {
                                    foundParameter
                                        = firstWallTypeElement.get_Parameter(
                                            projectParameterData.Definition);
                                }

                                if (foundParameter != null)
                                    Util.PopulateProjectParameterData(
                                        foundParameter,
                                        projectParameterData);
                                else
                                    projectParameterData.IsSharedStatusKnown
                                        = false;
                            }
                            else
                            {
                                Util.PopulateProjectParameterData(
                                    foundParameter,
                                    projectParameterData);
                            }
                        }
                        else
                        {
                            // Non-shared parameters cannot bind to ProjectInformation and have no GUID.

                            projectParameterData.IsShared = false;
                            projectParameterData.IsSharedStatusKnown = true;
                        }

                        tempTransaction.RollBack();
                    }
                    else
                    {
                        foundParameter
                            = projectInfoElement.get_Parameter(
                                projectParameterData.Definition);

                        if (foundParameter != null)
                            Util.PopulateProjectParameterData(
                                foundParameter, projectParameterData);
                        else
                            projectParameterData.IsSharedStatusKnown
                                = false;
                    }
                }

            StringBuilder sb = new();

            sb.AppendLine("PARAMETER NAME\tIS SHARED?\tGUID");

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

            TaskDialog resultsDialog = new(
                "Results are in the Clipboard")
            {
                MainInstruction
                = "Results are in the Clipboard",

                MainContent
                = "Paste the clipboard into a spreadsheet "
                      + "program to see the results."
            };

            resultsDialog.Show();

            return Result.Succeeded;
        }
    }
}