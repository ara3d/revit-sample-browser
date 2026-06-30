using System;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ColumnRound sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Determine the height of a vertical column from
        ///     its top and bottom level.
        /// </summary>
        internal static double GetColumHeightFromLevels(
            Element e)
        {
            if (!IsColumn(e))
                throw new ArgumentException(
                    "Expected a column argument.");

            var doc = e.Document;

            double height = 0;

            if (e != null)
            {
                var topLevel = e.get_Parameter(
                    BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);

                var ip = topLevel.AsElementId();
                var top = doc.GetElement(ip) as Level;
                var tv = top.ProjectElevation;

                var botLevel = e.get_Parameter(
                    BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);

                var bip = botLevel.AsElementId();
                var bot = doc.GetElement(bip) as Level;
                var bv = bot.ProjectElevation;

                height = tv - bv;
            }
            return height;
        }

        /// <summary>
        ///     Determine the height of any given element
        ///     from its bounding box.
        /// </summary>
        internal static double GetElementHeightFromBoundingBox(
            Element e)
        {
            var bb = e.get_BoundingBox(null);

            if (null == bb)
                throw new ArgumentException(
                    "Expected Element 'e' to have a valid bounding box.");

            return bb.Max.Z - bb.Min.Z;
        }

        /// <summary>
        ///     Return true if the given Revit element looks
        ///     like it might be a column family instance.
        /// </summary>
        internal static bool IsColumn(Element e)
        {
            return e is FamilyInstance
                   && null != e.Category
                   && e.Category.Name.ToLower().Contains("column");
        }
    }
}
