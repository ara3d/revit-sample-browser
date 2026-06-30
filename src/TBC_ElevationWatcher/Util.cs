using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ElevationWatcher sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return the first elevation view found in the
        ///     given element id collection or null.
        /// </summary>
        internal static View FindElevationView(
            Document doc,
            ICollection<ElementId> ids)
        {
            View view = null;

            foreach (var id in ids)
            {
                view = doc.GetElement(id) as View;

                if (view.IsTemplate
                    && ViewType.Internal == view.ViewType)
                {
                    view = null;
                    continue;
                }

                if (view is {ViewType: ViewType.Elevation})
                    break;

                view = null;
            }

            return view;
        }
    }
}
