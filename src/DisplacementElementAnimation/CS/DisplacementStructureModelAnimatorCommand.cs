// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DisplacementElementAnimation.CS
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
        public static DisplacementStructureModelAnimator m_displacementstructuremodelAnimator;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (m_displacementstructuremodelAnimator == null)
            {
                m_displacementstructuremodelAnimator =
                    new DisplacementStructureModelAnimator(commandData.Application, false);
                m_displacementstructuremodelAnimator.StartAnimation();
            }
            else
            {
                m_displacementstructuremodelAnimator.AnimateNextStep();
            }

            return Result.Succeeded;
        }
    }
}
