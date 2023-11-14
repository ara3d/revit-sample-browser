// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Revit.SDK.Samples.PlacementOptions.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Show the OptionsForm for options selection
                using (var optionsForm = new OptionsForm())
                {
                    if (DialogResult.OK == optionsForm.ShowDialog())
                    {
                        var famSymbolList = new List<FamilySymbol>();
                        if (optionsForm.OptionType == PlacementOptionsEnum.FaceBased)
                        {
                            famSymbolList = FindProperFamilySymbol(document, BuiltInCategory.OST_GenericModel);
                            if (famSymbolList == null || famSymbolList.Count == 0)
                            {
                                TaskDialog.Show("Error",
                                    "There is no Face-Based family symbol, please load one first.");
                                return Result.Cancelled;
                            }

                            // Show the FacebasedForm for setting the face based family symbol and FaceBasedPlacementType option.
                            using (var facebaseForm = new FacebasedForm(famSymbolList))
                            {
                                if (DialogResult.OK == facebaseForm.ShowDialog())
                                    commandData.Application.ActiveUIDocument.PromptForFamilyInstancePlacement(
                                        facebaseForm.SelectedFamilySymbol, facebaseForm.FIPlacementOptions);
                            }
                        }
                        else
                        {
                            famSymbolList = FindProperFamilySymbol(document, BuiltInCategory.OST_StructuralFraming);
                            if (famSymbolList == null || famSymbolList.Count == 0)
                            {
                                TaskDialog.Show("Error",
                                    "There is no Sketch-Based family symbol, please load one first.");
                                return Result.Cancelled;
                            }

                            // Show the FacebasedForm for setting the face based family symbol and SketchGalleryOptions option.
                            using (var sketchbasedForm = new SketchbasedForm(famSymbolList))
                            {
                                if (DialogResult.OK == sketchbasedForm.ShowDialog())
                                    commandData.Application.ActiveUIDocument.PromptForFamilyInstancePlacement(
                                        sketchbasedForm.SelectedFamilySymbol, sketchbasedForm.FIPlacementOptions);
                            }
                        }
                    }
                }

                return Result.Succeeded;
            }
            catch (OperationCanceledException)
            {
                // UI preempted operation is finished peacefully, so we consider it successful
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Find proper family symbol for family instance placement.
        /// </summary>
        /// <param name="document">The Revit document.</param>
        /// <param name="category">The category of the family symbol.</param>
        /// <returns>The list of family symbol.</returns>
        private List<FamilySymbol> FindProperFamilySymbol(Document document, BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(document);
            // OST_GenericModel for looking up the face based family symbol.
            // OST_StructuralFraming for looking up the beam framing symbol, which is sketch based.
            var filter = new ElementCategoryFilter(category);
            collector.WherePasses(filter);
            var symbollList = collector.ToElements().ToList();
            if (!symbollList.Any())
                return null;

            var famSymbolList = new List<FamilySymbol>();
            foreach (var elem in symbollList)
            {
                if (elem is FamilySymbol famSymbol)
                    famSymbolList.Add(famSymbol);
            }

            return famSymbolList;
        }
    }

    /// <summary>
    ///     The placement options for user to place the family instance.
    /// </summary>
    public enum PlacementOptionsEnum
    {
        /// <summary>
        ///     Face based
        /// </summary>
        FaceBased,

        /// <summary>
        ///     Sketch based
        /// </summary>
        SketchBased
    }
}
