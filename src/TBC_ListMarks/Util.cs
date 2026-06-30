using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ListMarks sample.</summary>
    internal static partial class Util
    {
        public static void NumberStructuralFraming(Document doc)
        {
            var beams
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsNotElementType();

            using Transaction t = new(doc);
            t.Start("Renumber marks");

            var mark_number = 3;

            foreach (FamilyInstance beam in beams)
            {
                var p = beam.get_Parameter(
                    BuiltInParameter.ALL_MODEL_MARK);

                p.Set(mark_number++.ToString());
            }

            t.Commit();
        }
    }
}
