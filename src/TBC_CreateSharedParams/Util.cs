using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        private const string SharedParamsFilename = "C:/tmp/SharedParams.txt";
        private const string SharedParamsGroupname = "The Building Coder Parameters";
        private const string SharedParamsDefname = "SP";
        private static readonly ForgeTypeId SharedParamsDeftype = SpecTypeId.Number;

        internal static Category GetCategoryForSharedParam(
            Document doc,
            BuiltInCategory target)
        {
            Category cat = null;

            if (target.Equals(BuiltInCategory.OST_IOSModelGroups))
            {
                var collector
                    = GetElementsOfType(doc, typeof(Group),
                        BuiltInCategory.OST_IOSModelGroups);

                var modelGroups = collector.ToElements();

                if (0 == modelGroups.Count)
                {
                    ErrorMsg("Please insert a model group.");
                    return cat;
                }

                cat = modelGroups[0].Category;
            }
            else
            {
                try
                {
                    cat = doc.Settings.Categories.get_Item(target);
                }
                catch (Exception ex)
                {
                    ErrorMsg($"Error obtaining document {target.ToString()} category: {ex.Message}");
                    return cat;
                }
            }

            if (null == cat)
                ErrorMsg($"Unable to obtain the document {target.ToString()} category.");
            return cat;
        }

        internal static bool CreateSharedParameter(
            Document doc,
            Category cat,
            int nameSuffix,
            bool typeParameter)
        {
            var app = doc.Application;

            var ca
                = app.Create;

            var filename
                = app.SharedParametersFilename;

            if (0 == filename.Length)
            {
                var path = SharedParamsFilename;
                StreamWriter stream;
                stream = new StreamWriter(path);
                stream.Close();
                app.SharedParametersFilename = path;
                filename = app.SharedParametersFilename;
            }

            var file
                = app.OpenSharedParameterFile();

            if (null == file)
            {
                ErrorMsg(
                    "Error getting the shared params file.");

                return false;
            }

            var group
                = file.Groups.get_Item(SharedParamsGroupname);

            if (null == group) group = file.Groups.Create(SharedParamsGroupname);

            if (null == group)
            {
                ErrorMsg(
                    "Error getting the shared params group.");

                return false;
            }

            var visible = cat.AllowsBoundParameters;

            var defname = SharedParamsDefname + nameSuffix;

            var definition = group.Definitions.get_Item(
                defname);

            if (null == definition)
            {
                var opt
                    = new ExternalDefinitionCreationOptions(
                        defname, SharedParamsDeftype);

                opt.Visible = visible;

                definition = group.Definitions.Create(opt);
            }

            if (null == definition)
            {
                ErrorMsg(
                    "Error creating shared parameter.");

                return false;
            }

            var catSet = ca.NewCategorySet();
            catSet.Insert(cat);

            try
            {
                var binding = typeParameter
                    ? ca.NewTypeBinding(catSet)
                    : ca.NewInstanceBinding(catSet) as Binding;

                doc.ParameterBindings.Insert(definition, binding);

                Debug.Print(
                    "Created a shared {0} parameter '{1}' for the {2} category.",
                    typeParameter ? "type" : "instance",
                    defname, cat.Name);
            }
            catch (Exception ex)
            {
                ErrorMsg($"Error binding shared parameter to category {cat.Name}: {ex.Message}");
                return false;
            }

            return true;
        }

        internal static void SetInstanceParamVaryBetweenGroupsBehaviour(
            Document doc,
            Guid guid,
            bool allowVaryBetweenGroups = true)
        {
            try
            {
                var sp
                    = SharedParameterElement.Lookup(doc, guid);

                if (null == sp) return;

                var def = sp.GetDefinition();

                if (def.VariesAcrossGroups != allowVaryBetweenGroups)
                    def.SetAllowVaryBetweenGroups(doc, allowVaryBetweenGroups);
            }
            catch
            {
            }
        }

        private class IdForSynchro
        {
            public ElementId RevitId { get; set; }
            public int Param1 { get; set; }
            public string Param2 { get; set; }
            public double Param3 { get; set; }
        }

        private static void ModifySharedParameterValues(
            Document doc, IList<IdForSynchro> data)
        {
            using var tr = new Transaction(doc);
            var guid1 = Guid.Empty;
            var guid2 = Guid.Empty;
            var guid3 = Guid.Empty;

            tr.Start("synchro");

            foreach (var d in data)
            {
                var e = doc.GetElement(d.RevitId);

                if (Guid.Empty == guid1)
                {
                    guid1 = e.LookupParameter("PLUGIN_PARAM1").GUID;
                    guid2 = e.LookupParameter("PLUGIN_PARAM2").GUID;
                    guid3 = e.LookupParameter("PLUGIN_PARAM3").GUID;
                }

                e.get_Parameter(guid1).Set(d.Param1);
                e.get_Parameter(guid2).Set(d.Param2);
                e.get_Parameter(guid3).Set(d.Param3);
            }

            tr.Commit();
        }
    }
}
