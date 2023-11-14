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
using System.Windows.Forms;
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GridCreation.CS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Get all selected lines and arcs 
                var selectedCurves = GetSelectedCurves(commandData.Application.ActiveUIDocument.Document);

                // Show UI
                var gridCreationOption = new GridCreationOptionData(!selectedCurves.IsEmpty);
                using (var gridCreationOptForm = new GridCreationOptionForm(gridCreationOption))
                {
                    var result = gridCreationOptForm.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        return Result.Cancelled;
                    }

                    var labels = GetAllLabelsOfGrids(document);
                    var unit = GetLengthUnitType(document);
                    switch (gridCreationOption.CreateGridsMode)
                    {
                        case CreateMode.Select: // Create grids with selected lines/arcs
                            var data = new CreateWithSelectedCurvesData(commandData.Application, selectedCurves, labels);
                            using (var createWithSelected = new CreateWithSelectedCurvesForm(data))
                            {
                                result = createWithSelected.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    // Create grids
                                    var transaction = new Transaction(document, "CreateGridsWithSelectedCurves");
                                    transaction.Start();
                                    data.CreateGrids();
                                    transaction.Commit();
                                }  
                            }
                            break;

                        case CreateMode.Orthogonal: // Create orthogonal grids
                            var orthogonalData = new CreateOrthogonalGridsData(commandData.Application, unit, labels);
                            using (var orthogonalGridForm = new CreateOrthogonalGridsForm(orthogonalData))
                            {
                                result = orthogonalGridForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    // Create grids
                                    var transaction = new Transaction(document, "CreateOrthogonalGrids");
                                    transaction.Start();
                                    orthogonalData.CreateGrids();
                                    transaction.Commit();
                                }  
                            }
                            break;

                        case CreateMode.RadialAndArc: // Create radial and arc grids
                            var radArcData = new CreateRadialAndArcGridsData(commandData.Application, unit, labels);
                            using (var radArcForm = new CreateRadialAndArcGridsForm(radArcData))
                            {
                                result = radArcForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    // Create grids
                                    var transaction = new Transaction(document, "CreateRadialAndArcGrids");
                                    transaction.Start();
                                    radArcData.CreateGrids();
                                    transaction.Commit();
                                }  
                            }
                            break;
                    }

                    if (result == DialogResult.OK)
                    {
                        return Result.Succeeded;
                    }
                    else
                    {
                        return Result.Cancelled;
                    }                    
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        /// Get all selected lines and arcs
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>CurveArray contains all selected lines and arcs</returns>
        private static CurveArray GetSelectedCurves(Document document)
        {
            var selectedCurves = new CurveArray();
            var newUIdocument = new UIDocument(document);
            var elements = new ElementSet();
            foreach (var elementId in newUIdocument.Selection.GetElementIds())
            {
               elements.Insert(newUIdocument.Document.GetElement(elementId));
            }
            foreach (Element element in elements)
            {
                if ((element is ModelLine) || (element is ModelArc))
                {
                    var modelCurve = element as ModelCurve;
                    var curve = modelCurve.GeometryCurve;
                    if (curve != null)
                    {
                        selectedCurves.Append(curve);
                    }
                }
                else if ((element is DetailLine) || (element is DetailArc))
                {
                    var detailCurve = element as DetailCurve;
                    var curve = detailCurve.GeometryCurve;
                    if (curve != null)
                    {
                        selectedCurves.Append(curve);
                    }
                }
            }

            return selectedCurves;
        }

        /// <summary>
        /// Get all model and detail lines/arcs within selected elements
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>ElementSet contains all model and detail lines/arcs within selected elements </returns>
        public static ElementSet GetSelectedModelLinesAndArcs(Document document)
        {
            var newUIdocument = new UIDocument(document);
            var elements = new ElementSet();
            foreach (var elementId in newUIdocument.Selection.GetElementIds())
            {
               elements.Insert(newUIdocument.Document.GetElement(elementId));
            }
            var tmpSet = new ElementSet();
            foreach (Element element in elements)
            {
                if ((element is ModelLine) || (element is ModelArc) || (element is DetailLine) || (element is DetailArc))
                {
                    tmpSet.Insert(element);
                }
            }

            return tmpSet;
        }

        /// <summary>
        /// Get current length display unit type
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>Current length display unit type</returns>
        private static ForgeTypeId GetLengthUnitType(Document document)
        {
            var specTypeId = SpecTypeId.Length;
            var projectUnit = document.GetUnits();
            try
            {
                var formatOption = projectUnit.GetFormatOptions(specTypeId);
                return formatOption.GetUnitTypeId();
            }
            catch (Exception /*e*/)
            {
                return UnitTypeId.Feet;
            }
        }

        /// <summary>
        /// Get all grid labels in current document
        /// </summary>
        /// <param name="document">Revit's document</param>
        /// <returns>ArrayList contains all grid labels in current document</returns>
        private static ArrayList GetAllLabelsOfGrids(Document document)
        {
            var labels = new ArrayList();
            var itor = new FilteredElementCollector(document).OfClass(typeof(Grid)).GetElementIterator();
            itor.Reset();
            for (; itor.MoveNext(); )
            {
                var grid = itor.Current as Grid;
                if (null != grid)
                {
                    labels.Add(grid.Name);
                }
            }

            return labels;
        }
    }
}

