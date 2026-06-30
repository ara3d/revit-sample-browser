#if USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewLineLoad sample.</summary>
    internal static partial class Util
    {
        internal static void CreatePointLoadOnColumnEnd(Document doc)
        {
            var columns
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ColumnAnalytical)
                    .WhereElementIsNotElementType();

            foreach (AnalyticalModel am in columns)
            {
                var curve = am.GetCurve();

                var selector
                    = new AnalyticalModelSelector(curve);

                selector.CurveSelector
                    = AnalyticalCurveSelector.EndPoint;

                var endPointRef
                    = am.GetReference(selector);

                using var tx = new Transaction(doc);
                tx.Start("NewPointBoundaryConditions");

                var newPointBC
                    = doc.Create.NewPointBoundaryConditions(
                        endPointRef,
                        TranslationRotationValue.Fixed, 0,
                        TranslationRotationValue.Spring, 1.0,
                        TranslationRotationValue.Fixed, 0,
                        TranslationRotationValue.Fixed, 0,
                        TranslationRotationValue.Fixed, 0,
                        TranslationRotationValue.Fixed, 0);

                newPointBC.SetOrientTo(
                    BoundaryConditionsOrientTo
                        .HostLocalCoordinateSystem);

                tx.Commit();
            }
        }
    }
}
#endif // USING_ANALYTICAL_MODEL_BEFORE_REVIT_2023
