//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
// 
using System;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DWGFamilyCreation.CS
{
    /// <summary>
    /// To add an external command to Autodesk Revit 
    /// the developer should implement an object that 
    /// supports the IExternalCommand interface.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Revit application
        /// </summary>
        UIApplication m_app;
        /// <summary>
        /// Revit document
        /// </summary>
        Document m_doc;

        public Result Execute(ExternalCommandData commandData,
        ref string message, ElementSet elements)
        {
            try
            {
                m_app = commandData.Application;
                m_doc = commandData.Application.ActiveUIDocument.Document;
                if (null == m_doc)
                {
                    message = "There is no active document.";
                    return Result.Failed;
                }

                if (!m_doc.IsFamilyDocument)
                {
                    message = "Current document is not a family document.";
                    return Result.Failed;
                }

                // Get the view where the dwg file will be imported
                var view = GetView();
                if (null == view)
                {
                    message = "Opened wrong template file, please use the provided family template file.";
                    return Result.Failed;
                }

                // The dwg file which will be imported
                var AssemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var DWGFile = "Desk.dwg";
                var DWGFullPath = Path.Combine(AssemblyDirectory, DWGFile);

                var transaction = new Transaction(m_doc, "DWGFamilyCreation");
                transaction.Start();
                // Import the dwg file into current family document
                var options = new DWGImportOptions();
                options.Placement = ImportPlacement.Origin;
                options.OrientToView = true;
                m_doc.Import(DWGFullPath, options, view, out _);

                // Add type parameters to the family
                AddParameters(DWGFile);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Add type parameters to the family
        /// </summary>
        /// <param name="DWGFileName">Name of imported dwg file</param>
        private void AddParameters(string DWGFileName)
        {
            // Get the family manager
            var familyMgr = m_doc.FamilyManager;

            // Add parameter 1: DWGFileName
            familyMgr.NewType("DWGFamilyCreation");
            var paraFileName = familyMgr.AddParameter("DWGFileName", new ForgeTypeId(),
                SpecTypeId.String.Text, false);
            familyMgr.Set(paraFileName, DWGFileName);

            // Add parameter 2: ImportTime
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            var paraImportTime = familyMgr.AddParameter("ImportTime", new ForgeTypeId(),
                SpecTypeId.String.Text, false);
            familyMgr.Set(paraImportTime, time);
        }

        /// <summary>
        /// Get the view where the dwg file will be imported
        /// </summary>
        /// <returns>The view where the dwg file will be imported</returns>
        private View GetView()
        {
            View view = null;
            var views = new List<Element>();
            var collector = new FilteredElementCollector(m_app.ActiveUIDocument.Document);
            views.AddRange(collector.OfClass(typeof(View)).ToElements());
            foreach (View v in views)
            {
                if (!v.IsTemplate && v.ViewType == ViewType.FloorPlan && v.Name == "Ref. Level")
                {
                    view = v;
                    break;
                }
            }

            return view;
        }
    }
}
