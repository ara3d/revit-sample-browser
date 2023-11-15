// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.GenericStructuralConnection.CS
{
    /// <summary>
    ///     Utility class to select connections or connection input elements.
    /// </summary>
    internal class StructuralConnectionSelectionUtils
    {
        /// <summary>
        ///     Static method to select structural connections.
        /// </summary>
        /// <returns>Returns the id of the connection.</returns>
        public static StructuralConnectionHandler SelectConnection(UIDocument document)
        {
            StructuralConnectionHandler conn = null;
            // Create a filter for structural connections.
            var types = new LogicalOrFilter(new List<ElementFilter>
                { new ElementCategoryFilter(BuiltInCategory.OST_StructConnections) });
            var filter = new StructuralConnectionSelectionFilter(types);
            var target = document.Selection.PickObject(ObjectType.Element, filter, "Select connection element :");
            if (target != null)
            {
                var targetElement = document.Document.GetElement(target);
                if (targetElement != null) conn = targetElement as StructuralConnectionHandler;
            }

            return conn;
        }

        /// <summary>
        ///     Static method o select valid input element for the structural connection.
        /// </summary>
        /// <param name="document"> Current document. </param>
        /// <returns>Returns a list of element ids.</returns>
        public static List<ElementId> SelectConnectionElements(UIDocument document)
        {
            var elemIds = new List<ElementId>();

            // Create a filter for the allowed structural connection inputs.
            var connElemTypes = new LogicalOrFilter(new List<ElementFilter>
            {
                new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming),
                new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns),
                new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation),
                new ElementCategoryFilter(BuiltInCategory.OST_Floors),
                new ElementCategoryFilter(BuiltInCategory.OST_Walls)
            });
            var elemFilter = new StructuralConnectionSelectionFilter(connElemTypes);

            var refs = document.Selection
                .PickObjects(ObjectType.Element, elemFilter, "Select elements to add to connection :").ToList();
            elemIds = refs.Select(e => e.ElementId).ToList();

            return elemIds;
        }
    }
}
