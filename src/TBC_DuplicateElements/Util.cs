using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DuplicateElements sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Create a new group of the specified elements
        ///     in the current active view at the given offset.
        /// </summary>
        internal static void CreateGroup(
            Document doc,
            ICollection<ElementId> ids,
            XYZ offset)
        {
            var group = doc.Create.NewGroup(ids);

            var location = group.Location
                as LocationPoint;

            var p = location.Point + offset;

            doc.Create.PlaceGroup(
                p, group.GroupType);

            group.UngroupMembers();
        }
    }
}
