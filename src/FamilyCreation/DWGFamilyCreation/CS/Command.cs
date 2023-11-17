// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.DWGFamilyCreation.CS
{
    /// <summary>
    ///     To add an external command to Autodesk Revit
    ///     the developer should implement an object that
    ///     supports the IExternalCommand interface.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     Revit application
        /// </summary>
        private UIApplication m_app;

        /// <summary>
        ///     Revit document
        /// </summary>
        private Document m_doc;

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
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dwgFile = "Desk.dwg";
                var dwgFullPath = Path.Combine(assemblyDirectory, dwgFile);

                var transaction = new Transaction(m_doc, "DWGFamilyCreation");
                transaction.Start();
                // Import the dwg file into current family document
                var options = new DWGImportOptions
                {
                    Placement = ImportPlacement.Origin,
                    OrientToView = true
                };
                m_doc.Import(dwgFullPath, options, view, out _);

                // Add type parameters to the family
                AddParameters(dwgFile);
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
        ///     Add type parameters to the family
        /// </summary>
        /// <param name="dwgFileName">Name of imported dwg file</param>
        private void AddParameters(string dwgFileName)
        {
            // Get the family manager
            var familyMgr = m_doc.FamilyManager;

            // Add parameter 1: DWGFileName
            familyMgr.NewType("DWGFamilyCreation");
            var paraFileName = familyMgr.AddParameter("DWGFileName", new ForgeTypeId(),
                SpecTypeId.String.Text, false);
            familyMgr.Set(paraFileName, dwgFileName);

            // Add parameter 2: ImportTime
            var time = DateTime.Now.ToString("yyyy-MM-dd");
            var paraImportTime = familyMgr.AddParameter("ImportTime", new ForgeTypeId(),
                SpecTypeId.String.Text, false);
            familyMgr.Set(paraImportTime, time);
        }

        /// <summary>
        ///     Get the view where the dwg file will be imported
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
