using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_AnalyticalModelGeom sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        /// Return the associated analytical element id
        /// for the given element
        /// </summary>
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
