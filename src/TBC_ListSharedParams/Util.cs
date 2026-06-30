using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ListSharedParams sample.</summary>
    internal static partial class Util
    {
        public static Guid SharedParamGuid(
            Application app,
            string defGroup,
            string defName)
        {
            var guid = Guid.Empty;
            try
            {
                var file = app.OpenSharedParameterFile();
                var group = file.Groups.get_Item(defGroup);
                var definition = group.Definitions.get_Item(defName);
                var externalDefinition = definition as ExternalDefinition;
                guid = externalDefinition.GUID;
            }
            catch (Exception)
            {
            }

            return guid;
        }
    }
}
