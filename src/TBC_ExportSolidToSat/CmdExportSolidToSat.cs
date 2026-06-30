#region Header

//
// CmdExportSolidToSat.cs - Create a solid in memory and export it to a SAT file
//
// Copyright (C) 2013-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdExportSolidToSat : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            // Retrieve all floors from the model

            var floors
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Floor))
                    .ToElements()
                    .Cast<Floor>()
                    .ToList();

            if (2 != floors.Count)
            {
                message = "Please create two intersected floors";
                return Result.Failed;
            }

            // Retrieve the floor solids

            Options opt = new();

            var geometry1 = floors[0].get_Geometry(opt);
            var geometry2 = floors[1].get_Geometry(opt);

            var solid1 = geometry1.FirstOrDefault() as Solid;
            var solid2 = geometry2.FirstOrDefault() as Solid;

            // Calculate the intersection solid

            var intersectedSolid = BooleanOperationsUtils
                .ExecuteBooleanOperation(solid1, solid2,
                    BooleanOperationsType.Intersect);

            // Search for the metric mass family template file

            var template_path = Util.DirSearch(
                app.FamilyTemplatePath,
                "Metric Mass.rft");

            // Create a new temporary family

            var family_doc = app.NewFamilyDocument(
                template_path);

            // Create a free form element 
            // from the intersection solid

            using (Transaction t = new(family_doc))
            {
                t.Start("Add Free Form Element");

                var freeFormElement = FreeFormElement.Create(
                    family_doc, intersectedSolid);

                t.Commit();
            }

            var dir = Path.GetTempPath();

            var filepath = Path.Combine(dir,
                "floor_intersection_family.rfa");

            SaveAsOptions sao = new()
            {
                OverwriteExistingFile = true
            };

            family_doc.SaveAs(filepath, sao);

            // Create 3D View

            var viewFamilyType
                = new FilteredElementCollector(family_doc)
                    .OfClass(typeof(ViewFamilyType))
                    .OfType<ViewFamilyType>()
                    .FirstOrDefault(x =>
                        x.ViewFamily == ViewFamily.ThreeDimensional);

            View3D threeDView;

            using (Transaction t = new(family_doc))
            {
                t.Start("Create 3D View");

                threeDView = View3D.CreateIsometric(
                    family_doc, viewFamilyType.Id);

                t.Commit();
            }

            // Export to SAT

            List<ElementId> viewSet = new()
            {
                threeDView.Id
            };

            SATExportOptions exportOptions
                = new();

            var res = family_doc.Export(dir,
                "SolidFile.sat", viewSet, exportOptions);

            return Result.Succeeded;
        }
    }
}