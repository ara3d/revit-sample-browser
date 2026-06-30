// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
namespace Ara3D.RevitSampleBrowser.PlacementOptions.CS
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
                using OptionsForm optionsForm = new();
                if (DialogResult.OK == optionsForm.ShowDialog())
                {
                    List<FamilySymbol> famSymbolList = new();
                    if (optionsForm.OptionType == PlacementOptionsEnum.FaceBased)
                    {
                        famSymbolList = ElementQuery.FindProperFamilySymbol(document, BuiltInCategory.OST_GenericModel);
                        if (famSymbolList == null || famSymbolList.Count == 0)
                        {
                            TaskDialog.Show("Error",
                                "There is no Face-Based family symbol, please load one first.");
                            return Result.Cancelled;
                        }

                        // Show the FacebasedForm for setting the face based family symbol and FaceBasedPlacementType option.
                        using FacebasedForm facebaseForm = new(famSymbolList);
                        if (DialogResult.OK == facebaseForm.ShowDialog())
                            commandData.Application.ActiveUIDocument.PromptForFamilyInstancePlacement(
                                facebaseForm.SelectedFamilySymbol, facebaseForm.FiPlacementOptions);
                    }
                    else
                    {
                        famSymbolList = ElementQuery.FindProperFamilySymbol(document, BuiltInCategory.OST_StructuralFraming);
                        if (famSymbolList == null || famSymbolList.Count == 0)
                        {
                            TaskDialog.Show("Error",
                                "There is no Sketch-Based family symbol, please load one first.");
                            return Result.Cancelled;
                        }

                        // Show the FacebasedForm for setting the face based family symbol and SketchGalleryOptions option.
                        using SketchbasedForm sketchbasedForm = new(famSymbolList);
                        if (DialogResult.OK == sketchbasedForm.ShowDialog())
                            commandData.Application.ActiveUIDocument.PromptForFamilyInstancePlacement(
                                sketchbasedForm.SelectedFamilySymbol, sketchbasedForm.FiPlacementOptions);
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
    }

    /// <summary>
    ///     The placement options for user to place the family instance.
    /// </summary>
    public enum PlacementOptionsEnum
    {
        FaceBased,

        SketchBased
    }
}
