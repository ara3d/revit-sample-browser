#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static void ReadTitleBlockLabelParameters(
            Document doc)
        {
            var title_block_instances
                = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .OfClass(typeof(FamilyInstance));

            Parameter p;

            Debug.Print("Title block instances:");

            foreach (FamilyInstance tb in title_block_instances)
            {
                var typeId = tb.GetTypeId();
                var type = doc.GetElement(typeId);

                p = tb.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER);

                Debug.Assert(null != p,
                    "expected valid sheet number");

                var sheet_number = p.AsString();

                p = tb.get_Parameter(
                    BuiltInParameter.PROJECT_AUTHOR);

                Debug.Assert(null != p,
                    "expected valid project author");

                var project_author = p.AsValueString();

                p = tb.get_Parameter(
                    BuiltInParameter.CLIENT_NAME);

                Debug.Assert(null != p,
                    "expected valid client name");

                var client_name = p.AsValueString();

                Debug.Print(
                    "Title block {0} <{1}> of type {2} <{3}>: "
                    + "{4} project author {5} for client {6}",
                    tb.Name, tb.Id.Value,
                    type.Name, typeId.Value,
                    sheet_number, project_author,
                    client_name);
            }
        }

        public static string GetParameterValueString(
            Element e,
            BuiltInParameter bip)
        {
            var p = e.get_Parameter(bip);

            var s = string.Empty;

            if (null != p)
            {
                switch (p.StorageType)
                {
                    case StorageType.Integer:
                        s = p.AsInteger().ToString();
                        break;

                    case StorageType.ElementId:
                        s = p.AsElementId().Value.ToString();
                        break;

                    case StorageType.Double:
                        s = RealString(p.AsDouble());
                        break;

                    case StorageType.String:
                        s = $"{p.AsValueString()} ({RealString(p.AsDouble())})";
                        break;

                    default:
                        s = "";
                        break;
                }

                s = $", {bip}={s}";
            }

            return s;
        }
    }
}
