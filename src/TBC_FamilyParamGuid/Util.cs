using System.Reflection;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_FamilyParamGuid sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Get family parameter IsShared and GUID properties.
        /// </summary>
        internal static bool GetFamilyParamGuid(
            FamilyParameter fp,
            out string guid)
        {
            guid = string.Empty;
            var isShared = false;

            var fi = fp.GetType().GetField("m_Parameter",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (null != fi)
            {
                var p = fi.GetValue(fp) as Parameter;

                isShared = p.IsShared;

                if (isShared) guid = p.GUID.ToString();
            }

            return isShared;
        }
    }
}
