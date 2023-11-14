// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.Samples.MaterialQuantities
{
    /// <summary>
    ///     Outputs an analysis of the materials that make up walls, floors, and roofs, and displays the output in Excel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private static AddInId _appId = new AddInId(new Guid("7E5CAC0D-F3D8-4040-89D6-0828D681561B"));

        private Document m_doc;

        private TextWriter m_writer;

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            m_doc = revit.Application.ActiveUIDocument.Document;

            var filename = "CalculateMaterialQuantities.txt";

            m_writer = new StreamWriter(filename);

            ExecuteCalculationsWith<RoofMaterialQuantityCalculator>();
            ExecuteCalculationsWith<WallMaterialQuantityCalculator>();
            ExecuteCalculationsWith<FloorMaterialQuantityCalculator>();

            m_writer.Close();

            // This operation doesn't change the model, so return cancelled to cancel the transaction
            return Result.Cancelled;
        }

        /// <summary>
        ///     Executes a calculator for one type of Revit element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void ExecuteCalculationsWith<T>() where T : MaterialQuantityCalculator, new()
        {
            var calculator = new T();
            calculator.SetDocument(m_doc);
            calculator.CalculateMaterialQuantities();
            calculator.ReportResults(m_writer);
        }
    }

    /// <summary>
    ///     The wall material quantity calculator specialized class.
    /// </summary>
    internal class WallMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            // filter for non-symbols that match the desired category so that inplace elements will also be found
            var collector = new FilteredElementCollector(Doc);
            ElementsToProcess = collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Wall";
        }
    }

    /// <summary>
    ///     The floor material quantity calculator specialized class.
    /// </summary>
    internal class FloorMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            var collector = new FilteredElementCollector(Doc);
            ElementsToProcess = collector.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Floor";
        }
    }

    /// <summary>
    ///     The roof material quantity calculator specialized class.
    /// </summary>
    internal class RoofMaterialQuantityCalculator : MaterialQuantityCalculator
    {
        protected override void CollectElements()
        {
            var collector = new FilteredElementCollector(Doc);
            ElementsToProcess = collector.OfCategory(BuiltInCategory.OST_Roofs).WhereElementIsNotElementType()
                .ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Roof";
        }
    }

    /// <summary>
    ///     The base material quantity calculator for all element types.
    /// </summary>
    internal abstract class MaterialQuantityCalculator
    {
        /// <summary>
        ///     Flag indicating the mode of the calculation.
        /// </summary>
        private bool m_calculatingGrossQuantities;

        protected Document Doc;

        /// <summary>
        ///     The list of elements for material quantity extraction.
        /// </summary>
        protected IList<Element> ElementsToProcess;

        /// <summary>
        ///     A storage of material quantities per individual element.
        /// </summary>
        private readonly Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>> m_quantitiesPerElement =
            new Dictionary<ElementId, Dictionary<ElementId, MaterialQuantities>>();

        /// <summary>
        ///     A storage of material quantities for the entire project.
        /// </summary>
        private readonly Dictionary<ElementId, MaterialQuantities> m_totalQuantities =
            new Dictionary<ElementId, MaterialQuantities>();

        /// <summary>
        ///     A collection of warnings generated due to failure to delete elements in advance of gross quantity calculations.
        /// </summary>
        private readonly List<string> m_warningsForGrossQuantityCalculations = new List<string>();

        /// <summary>
        ///     Override this to populate the list of elements for material quantity extraction.
        /// </summary>
        protected abstract void CollectElements();

        /// <summary>
        ///     Override this to return the name of the element type calculated by this calculator.
        /// </summary>
        protected abstract string GetElementTypeName();

        /// <summary>
        ///     Sets the document for the calculator class.
        /// </summary>
        public void SetDocument(Document d)
        {
            Doc = d;
        }

        /// <summary>
        ///     Executes the calculation.
        /// </summary>
        public void CalculateMaterialQuantities()
        {
            CollectElements();
            CalculateNetMaterialQuantities();
            CalculateGrossMaterialQuantities();
        }

        /// <summary>
        ///     Calculates net material quantities for the target elements.
        /// </summary>
        private void CalculateNetMaterialQuantities()
        {
            foreach (var e in ElementsToProcess) CalculateMaterialQuantitiesOfElement(e);
        }

        /// <summary>
        ///     Calculates gross material quantities for the target elements (material quantities with
        ///     all openings, doors and windows removed).
        /// </summary>
        private void CalculateGrossMaterialQuantities()
        {
            m_calculatingGrossQuantities = true;
            var t = new Transaction(Doc);
            t.SetName("Delete Cutting Elements");
            t.Start();
            DeleteAllCuttingElements();
            Doc.Regenerate();
            foreach (var e in ElementsToProcess) CalculateMaterialQuantitiesOfElement(e);
            t.RollBack();
        }

        /// <summary>
        ///     Delete all elements that cut out of target elements, to allow for calculation of gross material quantities.
        /// </summary>
        private void DeleteAllCuttingElements()
        {
            new List<ElementFilter>();
            var collector = new FilteredElementCollector(Doc);

            // (Type == FamilyInstance && (Category == Door || Category == Window) || Type == Opening
            var filterFamilyInstance = new ElementClassFilter(typeof(FamilyInstance));
            var filterWindowCategory = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            var filterDoorCategory = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var filterDoorOrWindowCategory = new LogicalOrFilter(filterWindowCategory, filterDoorCategory);
            var filterDoorWindowInstance = new LogicalAndFilter(filterDoorOrWindowCategory, filterFamilyInstance);

            var filterOpening = new ElementClassFilter(typeof(Opening));

            var filterCuttingElements = new LogicalOrFilter(filterOpening, filterDoorWindowInstance);
            ICollection<Element> cuttingElementsList = collector.WherePasses(filterCuttingElements).ToElements();

            foreach (var e in cuttingElementsList)
                // Doors in curtain grid systems cannot be deleted.  This doesn't actually affect the calculations because
                // material quantities are not extracted for curtain systems.
                if (e.Category != null)
                {
                    if (e.Category.BuiltInCategory == BuiltInCategory.OST_Doors)
                    {
                        var door = e as FamilyInstance;
                        var host = door.Host as Wall;

                        if (host.CurtainGrid != null)
                            continue;
                    }

                    var deletedElements = Doc.Delete(e.Id);

                    // Log failed deletion attempts to the output.  (These may be other situations where deletion is not possible but 
                    // the failure doesn't really affect the results.
                    if (deletedElements == null || deletedElements.Count < 1)
                        m_warningsForGrossQuantityCalculations.Add(
                            string.Format("   The tool was unable to delete the {0} named {2} (id {1})",
                                e.GetType().Name, e.Id, e.Name));
                }
        }

        /// <summary>
        ///     Store calculated material quantities in the storage collection.
        /// </summary>
        /// <param name="materialId">The material id.</param>
        /// <param name="volume">The extracted volume.</param>
        /// <param name="area">The extracted area.</param>
        /// <param name="quantities">The storage collection.</param>
        private void StoreMaterialQuantities(ElementId materialId, double volume, double area,
            Dictionary<ElementId, MaterialQuantities> quantities)
        {
            MaterialQuantities materialQuantityPerElement;
            var found = quantities.TryGetValue(materialId, out materialQuantityPerElement);
            if (found)
            {
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume += volume;
                    materialQuantityPerElement.GrossArea += area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume += volume;
                    materialQuantityPerElement.NetArea += area;
                }
            }
            else
            {
                materialQuantityPerElement = new MaterialQuantities();
                if (m_calculatingGrossQuantities)
                {
                    materialQuantityPerElement.GrossVolume = volume;
                    materialQuantityPerElement.GrossArea = area;
                }
                else
                {
                    materialQuantityPerElement.NetVolume = volume;
                    materialQuantityPerElement.NetArea = area;
                }

                quantities.Add(materialId, materialQuantityPerElement);
            }
        }

        /// <summary>
        ///     Calculate and store material quantities for a given element.
        /// </summary>
        /// <param name="e">The element.</param>
        private void CalculateMaterialQuantitiesOfElement(Element e)
        {
            var elementId = e.Id;
            var materials = e.GetMaterialIds(false);

            foreach (var materialId in materials)
            {
                var volume = e.GetMaterialVolume(materialId);
                var area = e.GetMaterialArea(materialId, false);

                if (volume > 0.0 || area > 0.0)
                {
                    StoreMaterialQuantities(materialId, volume, area, m_totalQuantities);

                    Dictionary<ElementId, MaterialQuantities> quantityPerElement;
                    var found = m_quantitiesPerElement.TryGetValue(elementId, out quantityPerElement);
                    if (found)
                    {
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                    }
                    else
                    {
                        quantityPerElement = new Dictionary<ElementId, MaterialQuantities>();
                        StoreMaterialQuantities(materialId, volume, area, quantityPerElement);
                        m_quantitiesPerElement.Add(elementId, quantityPerElement);
                    }
                }
            }
        }

        /// <summary>
        ///     Write results in CSV format to the indicated output writer.
        /// </summary>
        /// <param name="writer">The output text writer.</param>
        public void ReportResults(TextWriter writer)
        {
            if (m_totalQuantities.Count == 0)
                return;

            var legendLine = "Gross volume(cubic ft),Net volume(cubic ft),Gross area(sq ft),Net area(sq ft)";

            writer.WriteLine();
            writer.WriteLine("Totals for {0} elements,{1}", GetElementTypeName(), legendLine);

            // If unexpected deletion failures occurred, log the warnings to the output.
            if (m_warningsForGrossQuantityCalculations.Count > 0)
            {
                writer.WriteLine(
                    "WARNING: Calculations for gross volume and area may not be completely accurate due to the following warnings: ");
                foreach (var s in m_warningsForGrossQuantityCalculations)
                    writer.WriteLine(s);
                writer.WriteLine();
            }

            ReportResultsFor(m_totalQuantities, writer);

            foreach (var keyId in m_quantitiesPerElement.Keys)
            {
                var id = keyId;
                var e = Doc.GetElement(id);

                writer.WriteLine();
                writer.WriteLine("Totals for {0} element {1} (id {2}),{3}", GetElementTypeName(),
                    e.Name.Replace(',', ':'), id, legendLine);

                var quantities = m_quantitiesPerElement[id];

                ReportResultsFor(quantities, writer);
            }
        }

        /// <summary>
        ///     Write the contents of one storage collection to the indicated output writer.
        /// </summary>
        /// <param name="quantities">The storage collection for material quantities.</param>
        /// <param name="writer">The output writer.</param>
        private void ReportResultsFor(Dictionary<ElementId, MaterialQuantities> quantities, TextWriter writer)
        {
            foreach (var keyMaterialId in quantities.Keys)
            {
                var materialId = keyMaterialId;
                var quantity = quantities[materialId];

                var material = Doc.GetElement(materialId) as Material;

                //writer.WriteLine(String.Format("   {0} Net: [{1:F2} cubic ft {2:F2} sq. ft]  Gross: [{3:F2} cubic ft {4:F2} sq. ft]", material.Name, quantity.NetVolume, quantity.NetArea, quantity.GrossVolume, quantity.GrossArea));
                writer.WriteLine("{0},{3:F2},{1:F2},{4:F2},{2:F2}", material.Name.Replace(',', ':'), quantity.NetVolume,
                    quantity.NetArea, quantity.GrossVolume, quantity.GrossArea);
            }
        }
    }

    /// <summary>
    ///     A storage class for the extracted material quantities.
    /// </summary>
    internal class MaterialQuantities
    {
        /// <summary>
        ///     Gross volume (cubic ft)
        /// </summary>
        public double GrossVolume { get; set; }

        /// <summary>
        ///     Gross area (sq. ft)
        /// </summary>
        public double GrossArea { get; set; }

        /// <summary>
        ///     Net volume (cubic ft)
        /// </summary>
        public double NetVolume { get; set; }

        /// <summary>
        ///     Net area (sq. ft)
        /// </summary>
        public double NetArea { get; set; }
    }
}
