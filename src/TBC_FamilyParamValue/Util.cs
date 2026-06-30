using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_FamilyParamValue sample.</summary>
    internal static partial class Util
    {
        internal static string FamilyParamValueString(
            FamilyType t,
            FamilyParameter fp,
            Document doc)
        {
            var value = t.AsValueString(fp);
            switch (fp.StorageType)
            {
                case StorageType.Double:
                    value = $"{RealString((double) t.AsDouble(fp))} (double)";
                    break;

                case StorageType.ElementId:
                    var id = t.AsElementId(fp);
                    var e = doc.GetElement(id);
                    value = $"{id.Value} ({ElementDescription(e)})";
                    break;

                case StorageType.Integer:
                    value = $"{t.AsInteger(fp)} (int)";
                    break;

                case StorageType.String:
                    value = $"'{t.AsString(fp)}' (string)";
                    break;
            }

            return value;
        }

        /// <summary>
        ///     Non-working sample code for setting family parameter values.
        /// </summary>
        internal static void SetFamilyParameterValueFails(
            Document doc,
            string paramNameToAmend)
        {
            var mgr = doc.FamilyManager;
            var familyTypes = mgr.Types;
            var familyTypeItor = familyTypes.ForwardIterator();
            familyTypeItor.Reset();
            while (familyTypeItor.MoveNext())
            {
                var familyParam = mgr.get_Parameter(paramNameToAmend);

                if (familyParam != null)
                {
                    var familyType = familyTypeItor.Current as FamilyType;
                    Debug.Print(familyType.Name);
                    mgr.Set(familyParam, 2);
                }
            }
        }

        /// <summary>
        ///     Working sample code for setting family parameter values.
        /// </summary>
        internal static void SetFamilyParameterValueWorks(
            Document doc,
            string paramNameToAmend)
        {
            var mgr = doc.FamilyManager;
            var familyParam = mgr.get_Parameter(paramNameToAmend);

            if (familyParam != null)
                foreach (FamilyType familyType in mgr.Types)
                {
                    Debug.Print(familyType.Name);
                    mgr.CurrentType = familyType;
                    mgr.Set(familyParam, 2);
                }
        }
    }
}
