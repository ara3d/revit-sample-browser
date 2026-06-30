using Autodesk.Revit.DB;

namespace ExcelExporterImporter.Common
{
    internal static class B1ParameterCompat
    {
        public static bool IsYesNo(Definition definition)
        {
            return definition.GetDataType() == SpecTypeId.Boolean.YesNo;
        }

        public static bool IsText(Definition definition)
        {
            return definition.GetDataType() == SpecTypeId.String.Text;
        }

        public static string GetTypeLabel(Definition definition)
        {
            return LabelUtils.GetLabelForSpec(definition.GetDataType());
        }

        public static string GetGroupLabel(Definition definition)
        {
            return LabelUtils.GetLabelForGroup(definition.GetGroupTypeId());
        }
    }
}
