// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.DirectionCalculation.CS
{
    /// <summary>
    ///     Implementation class for utilities to find south facing exterior walls in a project.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class FindSouthFacingWalls : FindSouthFacingBase
    {
        protected void Execute(bool useProjectLocationNorth)
        {
            var uiDoc = new UIDocument(Document);
            var selElements = new ElementSet();

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

            var elemIdList = new List<ElementId>();
            foreach (Element element in selElements)
                elemIdList.Add(element.Id);
            uiDoc.Selection.SetElementIds(elemIdList);
        }
    }
}
