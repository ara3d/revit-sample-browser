using Autodesk.Revit.DB;

namespace ExcelExporterImporter.Common
{
    internal static class B1ParameterCompat
    {
        public static bool IsYesNo(Definition definition) =>
            definition.GetDataType() == SpecTypeId.Boolean.YesNo;

        public static bool IsText(Definition definition) =>
            definition.GetDataType() == SpecTypeId.String.Text;

        public static string GetTypeLabel(Definition definition) =>
            LabelUtils.GetLabelForSpec(definition.GetDataType());

        public static string GetGroupLabel(Definition definition) =>
            LabelUtils.GetLabelForGroup(definition.GetGroupTypeId());
    }
}
