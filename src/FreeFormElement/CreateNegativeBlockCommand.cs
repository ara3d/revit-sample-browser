// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.FreeFormElement.CS
{
    /// <summary>
    /// Creates a family block representing the negative of a selected element.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreateNegativeBlockCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;

            var target = uiDoc.Selection.PickObject(ObjectType.Element,
                new TargetElementSelectionFilter(),
                "Select target");
            var targetElement = doc.GetElement(target);

            var bbox = targetElement.get_BoundingBox(null);

            var boundaries = uiDoc.Selection.PickObjects(ObjectType.Element,
                new BoundarySelectionFilter(),
                "Select boundary");
            var familyPath = ViewHelper.FindGenericModelTemplate(doc.Application.FamilyTemplatePath);

            if (string.IsNullOrEmpty(familyPath))
            {
                message = "Unable to find a template named 'GenericModel.rft' in family template path.";
                return Result.Failed;
            }

            var condition = SampleBrowserUtils.CreateNegativeBlock(targetElement, boundaries,
                UIDocument.GetRevitUIFamilyLoadOptions(), familyPath);

            if (condition != SampleBrowserUtils.FailureCondition.Success)
            {
                switch (condition)
                {
                    case SampleBrowserUtils.FailureCondition.CurvesNotContigous:
                        message =
                            "Could not create the block as the boundary curves do not make a contiguous closed boundary.";
                        break;
                    case SampleBrowserUtils.FailureCondition.CurveLoopAboveTarget:
                        message = "Could not create the block as the boundary curves lie above their target element.";
                        break;
                    case SampleBrowserUtils.FailureCondition.NoIntersection:
                        message = "Could not create the block as the curves and the target element does not intersect.";
                        break;
                }

                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }

    public class TargetElementSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            var solids = SampleBrowserUtils.GetTargetSolids(element);

            return solids.Count > 0;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

    public class BoundarySelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (!(element is CurveElement curveElement))
                return false;

            var curve = curveElement.GeometryCurve;

            return SampleBrowserUtils.SupportsLoopUtilities(curve) &&
                   XyzMath.IsCurveInXyPlane(curve);
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }
}
