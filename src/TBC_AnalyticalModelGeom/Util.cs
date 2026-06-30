using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static ElementId GetAnalyticalElementId(Element e)
        {
            Document doc = e.Document;

            AnalyticalToPhysicalAssociationManager m
                = AnalyticalToPhysicalAssociationManager
                  .GetAnalyticalToPhysicalAssociationManager(
                    doc);

            if (null == m)
            {
                throw new System.ArgumentException(
                    "No AnalyticalToPhysicalAssociationManager found");
            }

            return m.GetAssociatedElementId(e.Id);
        }
    }
}
