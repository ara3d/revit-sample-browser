using Autodesk.Revit.DB;
using System.Reflection;

namespace BuildingCoder
{
    internal static partial class Util
    {
        // Uses reflection on FamilyParameter.m_Parameter to read IsShared/GUID.
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
