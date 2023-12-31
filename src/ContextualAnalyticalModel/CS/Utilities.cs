// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    /// <summary>
    ///     Utility methods
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Selects a Revit element and returns its ElementId
        /// </summary>
        /// <param name="uiDoc">UIDocument</param>
        /// <param name="msg">status message</param>
        /// <returns>ElementId of the selected element</returns>
        public static ElementId GetSelectedObject(UIDocument uiDoc, string msg)
        {
            var selectedElementId = ElementId.InvalidElementId;
            var refElem = uiDoc.Selection.PickObject(ObjectType.Element, msg);
            if (refElem != null) selectedElementId = refElem.ElementId;

            return selectedElementId;
        }

        /// <summary>
        ///     Selects multiple Revit elements and returns the ElementIds
        /// </summary>
        /// <param name="uiDoc">UIDocument</param>
        /// <param name="msg">status message</param>
        /// <returns>List of ElementId of the selected elements</returns>
        public static ISet<ElementId> GetSelectedObjects(UIDocument uiDoc, string msg)
        {
            ISet<ElementId> selectedElementIdList = new HashSet<ElementId>();
            var refElemList = uiDoc.Selection.PickObjects(ObjectType.Element, msg);
            if (refElemList != null) selectedElementIdList = refElemList.Select(x => x.ElementId).ToHashSet();

            return selectedElementIdList;
        }
    }
}
