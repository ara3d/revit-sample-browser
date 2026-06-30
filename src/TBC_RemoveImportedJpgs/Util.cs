#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static bool ElementNameEndsWithJpg(Element e)
        {
            var s = e.Name;

            return 3 < s.Length && s.EndsWith(".jpg");
        }

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
