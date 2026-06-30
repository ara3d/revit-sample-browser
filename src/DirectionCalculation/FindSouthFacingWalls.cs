// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindSouthFacingWalls : FindSouthFacingBase
    {
        protected void Execute(bool useProjectLocationNorth)
        {
            UIDocument uiDoc = new(Document);
            ElementSet selElements = new();

            foreach (var elementId in uiDoc.Selection.GetElementIds())
                selElements.Insert(uiDoc.Document.GetElement(elementId));

            foreach (var wall in ElementQuery.CollectExteriorWalls(Document))
            {
                var exteriorDirection = ViewHelper.GetExteriorWallDirection(wall);
                if (useProjectLocationNorth)
                    exteriorDirection = TransformByProjectLocation(exteriorDirection);
                if (IsSouthFacing(exteriorDirection))
                    selElements.Insert(wall);
            }

            List<ElementId> elemIdList = [];
            foreach (Element element in selElements)
                elemIdList.Add(element.Id);
            uiDoc.Selection.SetElementIds(elemIdList);
        }
    }
}
