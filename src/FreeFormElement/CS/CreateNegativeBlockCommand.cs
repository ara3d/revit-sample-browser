// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.FreeFormElement.CS
{
    /// <summary>
    ///     A command to create a new family block representing a negative of a selected element.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CreateNegativeBlockCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Select target element
            var target = uiDoc.Selection.PickObject(ObjectType.Element,
                new TargetElementSelectionFilter(),
                "Select target");
            var targetElement = doc.GetElement(target);

            // Get height for block based on target element.
            var bbox = targetElement.get_BoundingBox(null);

            // Select boundaries
            var boundaries = uiDoc.Selection.PickObjects(ObjectType.Element,
                new BoundarySelectionFilter(),
                "Select boundary");
            var familyPath = FreeFormElementUtils.FindGenericModelTemplate(doc.Application.FamilyTemplatePath);

            if (string.IsNullOrEmpty(familyPath))
            {
                message = "Unable to find a template named 'GenericModel.rft' in family template path.";
                return Result.Failed;
            }

            var condition = FreeFormElementUtils.CreateNegativeBlock(targetElement, boundaries,
                UIDocument.GetRevitUIFamilyLoadOptions(), familyPath);

            // Show error message for failure condition
            if (condition != FreeFormElementUtils.FailureCondition.Success)
            {
                switch (condition)
                {
                    case FreeFormElementUtils.FailureCondition.CurvesNotContigous:
                        message =
                            "Could not create the block as the boundary curves do not make a contiguous closed boundary.";
                        break;
                    case FreeFormElementUtils.FailureCondition.CurveLoopAboveTarget:
                        message = "Could not create the block as the boundary curves lie above their target element.";
                        break;
                    case FreeFormElementUtils.FailureCondition.NoIntersection:
                        message = "Could not create the block as the curves and the target element does not intersect.";
                        break;
                }

                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }

    /// <summary>
    ///     Selection filter for selection of a target object to use as a template for the negative block.
    /// </summary>
    internal class TargetElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            // Element must have at least one usable solid
            var solids = FreeFormElementUtils.GetTargetSolids(element);

            return solids.Count > 0;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

    /// <summary>
    ///     Selection filter for selection of the boundary curves for the block extents.
    /// </summary>
    internal class BoundarySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            // Allow only curve elements
            if (!(element is CurveElement curveElement))
                return false;

            var curve = curveElement.GeometryCurve;

            // Curves must support the utilities used by the tool (e.g. ReverseCurve)
            return FreeFormElementUtils.SupportsLoopUtilities(curve) &&
                   // Curves must be in XY plane
                   FreeFormElementUtils.IsCurveInXyPlane(curve);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
}
