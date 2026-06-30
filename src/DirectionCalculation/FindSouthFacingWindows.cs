// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    public class FindSouthFacingWindows : FindSouthFacingBase
    {
        protected void Execute(bool useProjectLocationNorth)
        {
            UIDocument uiDoc = new(Document);
            ElementSet selElements = new();
            foreach (var elementId in uiDoc.Selection.GetElementIds())
                selElements.Insert(uiDoc.Document.GetElement(elementId));

            foreach (var window in ElementQuery.CollectWindows(Document))
            {
                var exteriorDirection = ViewHelper.GetWindowDirection(window);
                if (useProjectLocationNorth)
                    exteriorDirection = TransformByProjectLocation(exteriorDirection);
                if (IsSouthFacing(exteriorDirection))
                    selElements.Insert(window);
            }

            List<ElementId> elemIdList = [];
            foreach (Element element in selElements)
                elemIdList.Add(element.Id);
            uiDoc.Selection.SetElementIds(elemIdList);
        }
    }
}
