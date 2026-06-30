#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_RemoveImportedJpgs sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return true if the given
        ///     element name ends in ".jpg".
        /// </summary>
        public static bool ElementNameEndsWithJpg(Element e)
        {
            var s = e.Name;

            return 3 < s.Length && s.EndsWith(".jpg");
        }

        /// <summary>
        ///     Return true if the given element name seems to
        ///     indicate an image file refrerence, i.e. ends in
        ///     ".jpg", ".jpeg", or ".bmp".
        /// </summary>
        public static bool ElementNameMayIndicateImageFileReference(
            Element e)
        {
            var s = e.Name.ToLower();

            return s.EndsWith(".jpg")
                   || s.EndsWith(".jpeg")
                   || s.EndsWith(".bmp");
        }
    }
}
