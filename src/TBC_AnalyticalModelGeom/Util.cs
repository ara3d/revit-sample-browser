using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static ElementId GetAnalyticalElementId(Element e)
        {
            var doc = e.Document;

            var m
                = AnalyticalToPhysicalAssociationManager
                  .GetAnalyticalToPhysicalAssociationManager(
                    doc);

            return null == m
                ? throw new System.ArgumentException(
                    "No AnalyticalToPhysicalAssociationManager found")
                : m.GetAssociatedElementId(e.Id);
        }
    }
}
