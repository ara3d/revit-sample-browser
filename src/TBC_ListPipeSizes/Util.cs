using Autodesk.Revit.DB;
using System.IO;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ListPipeSizes sample.</summary>
    internal static partial class Util
    {
        public static string ListPipeSizesFootToMmString(double a)
        {
            return FootToMm(a)
                .ToString("0.##")
                .PadLeft(8);
        }
        public static void GetPipeSegmentSizes(
            Document doc,
            string filename)
        {
            var segments
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Segment));

            using StreamWriter file = new(
                filename, true);
            foreach (Segment segment in segments)
            {
                file.WriteLine(segment.Name);

                foreach (var size in segment.GetSizes())
                    file.WriteLine("  {0} {1} {2}",
                        ListPipeSizesFootToMmString(size.NominalDiameter),
                        ListPipeSizesFootToMmString(size.InnerDiameter),
                        ListPipeSizesFootToMmString(size.OuterDiameter));
            }
        }
    }
}
