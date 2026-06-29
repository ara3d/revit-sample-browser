// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS;

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class SelectionHelper
    {
        public static bool GetSelectedModelGroup(UIDocument uiDoc, out Group modelGroup, out string errorMessage)
                {
                    modelGroup = null;
                    var sel = uiDoc.Selection.GetElementIds();

                    if (sel.Count < 1)
                    {
                        errorMessage = "Please select a single model group.";
                        return false;
                    }

                    if (sel.Count > 1)
                    {
                        errorMessage = "Too many selected elements. Please select a single model group.";
                        return false;
                    }

                    if (!(uiDoc.Document.GetElement(sel.First()) is Group selectedGroup))
                    {
                        errorMessage = "The selected element is not a group. Please select a single model group.";
                        return false;
                    }

                    if (selectedGroup.AttachedParentId != ElementId.InvalidElementId)
                    {
                        errorMessage = "The selected element is an attached detail group. Please select a single model group.";
                        return false;
                    }

                    errorMessage = "";
                    modelGroup = selectedGroup;
                    return true;
                }

        public static StructuralConnectionHandler SelectConnection(UIDocument document)
                {
                    var types = new LogicalOrFilter(new List<ElementFilter>
                        { new ElementCategoryFilter(BuiltInCategory.OST_StructConnections) });
                    var filter = new StructuralConnectionSelectionFilter(types);
                    var target = document.Selection.PickObject(ObjectType.Element, filter, "Select connection element :");
                    if (target == null) return null;
                    return document.Document.GetElement(target) as StructuralConnectionHandler;
                }

        public static List<ElementId> SelectConnectionElements(UIDocument document)
                {
                    var connElemTypes = new LogicalOrFilter(new List<ElementFilter>
                    {
                        new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),
                        new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                        new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation),
                        new ElementCategoryFilter(BuiltInCategory.OST_Floors),
                        new ElementCategoryFilter(BuiltInCategory.OST_Walls)
                    });
                    var elemFilter = new StructuralConnectionSelectionFilter(connElemTypes);
                    return document.Selection
                        .PickObjects(ObjectType.Element, elemFilter, "Select elements to add to connection :")
                        .Select(e => e.ElementId)
                        .ToList();
                }

        public static void MoveElement(Document document, Element elem, ref XYZ pickedPoint, XYZ targetPoint)
                {
                    var vecToMove = targetPoint - pickedPoint;
                    pickedPoint = targetPoint;
                    ElementTransformUtils.MoveElement(document, elem.Id, vecToMove);
                }

        public static List<FamilyInstance> GetSelectedBeams(UIDocument revitDoc)
                {
                    var beamList = new List<FamilyInstance>();
                    foreach (var elementId in revitDoc.Selection.GetElementIds())
                    {
                        if (revitDoc.Document.GetElement(elementId) is FamilyInstance familyInstance &&
                            familyInstance.StructuralType == StructuralType.Beam)
                            beamList.Add(familyInstance);
                    }

                    return beamList;
                }

    }
}