// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.DisplacementElementAnimation.CS
{
    /// <summary>
    ///     The command that initializes and starts the model animation.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DisplacementStructureModelAnimatorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new DisplacementStructureModelAnimator(commandData.Application, true).StartAnimation();

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     The command that initializes and starts the model animation step by step.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class DisplacementStructureModelAnimatorCommandStepByStep : IExternalCommand
    {
        public static DisplacementStructureModelAnimator DisplacementstructuremodelAnimator;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (DisplacementstructuremodelAnimator == null)
            {
                DisplacementstructuremodelAnimator =
                    new DisplacementStructureModelAnimator(commandData.Application, false);
                DisplacementstructuremodelAnimator.StartAnimation();
            }
            else
            {
                DisplacementstructuremodelAnimator.AnimateNextStep();
            }

            return Result.Succeeded;
        }
    }
}
