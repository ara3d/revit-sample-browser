// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                var selectedCurves = GetSelectedCurves(commandData.Application.ActiveUIDocument.Document);

                var gridCreationOption = new GridCreationOptionData(!selectedCurves.IsEmpty);
                using (var gridCreationOptForm = new GridCreationOptionForm(gridCreationOption))
                {
                    var result = gridCreationOptForm.ShowDialog();
                    if (result == DialogResult.Cancel) return Result.Cancelled;

                    var labels = GetAllLabelsOfGrids(document);
                    var unit = GetLengthUnitType(document);
                    switch (gridCreationOption.CreateGridsMode)
                    {
                        case CreateMode.Select:
                            var data = new CreateWithSelectedCurvesData(commandData.Application, selectedCurves,
                                labels);
                            using (var createWithSelected = new CreateWithSelectedCurvesForm(data))
                            {
                                result = createWithSelected.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    var transaction = new Transaction(document, "CreateGridsWithSelectedCurves");
                                    transaction.Start();
                                    data.CreateGrids();
                                    transaction.Commit();
                                }
                            }

                            break;

                        case CreateMode.Orthogonal:
                            var orthogonalData = new CreateOrthogonalGridsData(commandData.Application, unit, labels);
                            using (var orthogonalGridForm = new CreateOrthogonalGridsForm(orthogonalData))
                            {
                                result = orthogonalGridForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    var transaction = new Transaction(document, "CreateOrthogonalGrids");
                                    transaction.Start();
                                    orthogonalData.CreateGrids();
                                    transaction.Commit();
                                }
                            }

                            break;

                        case CreateMode.RadialAndArc:
                            var radArcData = new CreateRadialAndArcGridsData(commandData.Application, unit, labels);
                            using (var radArcForm = new CreateRadialAndArcGridsForm(radArcData))
                            {
                                result = radArcForm.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    var transaction = new Transaction(document, "CreateRadialAndArcGrids");
                                    transaction.Start();
                                    radArcData.CreateGrids();
                                    transaction.Commit();
                                }
                            }

                            break;
                    }

                    return result == DialogResult.OK 
                        ? Result.Succeeded : Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

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
                switch (element)
                {
                    case ModelLine _:
                    case ModelArc _:
                    {
                        var modelCurve = element as ModelCurve;
                        var curve = modelCurve.GeometryCurve;
                        if (curve != null) selectedCurves.Append(curve);
                        break;
                    }
                    case DetailLine _:
                    case DetailArc _:
                    {
                        var detailCurve = element as DetailCurve;
                        var curve = detailCurve.GeometryCurve;
                        if (curve != null) selectedCurves.Append(curve);
                        break;
                    }
                }
            }

            return selectedCurves;
        }

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
                if (element is ModelLine || element is ModelArc || element is DetailLine || element is DetailArc)
                    tmpSet.Insert(element);
            }

            return tmpSet;
        }

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

        private static ArrayList GetAllLabelsOfGrids(Document document)
        {
            var labels = new ArrayList();
            var itor = new FilteredElementCollector(document).OfClass(typeof(Grid)).GetElementIterator();
            itor.Reset();
            for (; itor.MoveNext();)
            {
                if (itor.Current is Grid grid) labels.Add(grid.Name);
            }

            return labels;
        }
    }
}
