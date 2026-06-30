// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ExportCncFab by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/ExportCncFab

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ara3D.RevitSampleBrowser.ExportCncFab.CS
{
    /// <summary>
    /// Shared parameters to keep track of the CNC fabrication export history.
    /// </summary>
    internal class ExportParameters
    {
        const string _is_exported = "CncFabIsExported";
        const string _exported_first = "CncFabExportedFirst";
        const string _exported_last = "CncFabExportedLast";

        // Optional; when present, inserted into the export filename.
        const string _sort_mark = "CncFabSortMark";

        readonly Definition _definition_is_exported;
        readonly Definition _definition_exported_first;
        readonly Definition _definition_exported_last;
        readonly Definition _definition_sort_mark;

        readonly Document _doc;
        readonly List<ElementId> _ids;

        static Definition GetDefinition(Element e, string parameter_name)
        {
            var ps = e.GetParameters(parameter_name);
            var n = ps.Count;

            Debug.Assert(1 >= n,
                "expected maximum one shared parameters named " + parameter_name);

            return 0 == n ? null : ps[0].Definition;
        }

        public ExportParameters(Element e)
        {
            _definition_is_exported = GetDefinition(e, _is_exported);
            _definition_exported_first = GetDefinition(e, _exported_first);
            _definition_exported_last = GetDefinition(e, _exported_last);
            _definition_sort_mark = GetDefinition(e, _sort_mark);

            if (IsValid)
            {
                _doc = e.Document;
                _ids = [];
            }
        }

        public bool IsValid =>
            null != _definition_is_exported
            && null != _definition_exported_first
            && null != _definition_exported_last;

        public void Add(ElementId id)
        {
            _ids.Add(id);
        }

        void UpdateExportHistory(Element e)
        {
            var s = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            e.get_Parameter(_definition_is_exported).Set(1);

            var p = e.get_Parameter(_definition_exported_first);
            var s2 = p.AsString();

            if (null == s2 || 0 == s2.Length)
                p.Set(s);

            e.get_Parameter(_definition_exported_last).Set(s);
        }

        public void UpdateExportHistory()
        {
            foreach (var id in _ids)
                UpdateExportHistory(_doc.GetElement(id));
        }

        static Definition CreateNewDefinition(
            DefinitionGroup group,
            string parameter_name,
            ForgeTypeId parameter_type)
        {
            return group.Definitions.Create(
                new ExternalDefinitionCreationOptions(parameter_name, parameter_type));
        }

        public static void Create(Document doc)
        {
            const string _shared_parameters_filename = "export_cnc_fab_shared_parameters.txt";
            const string _definition_group_name = "CncFab";

            var app = doc.Application;

            var sharedParamsFileName = app.SharedParametersFilename;

            if (null == sharedParamsFileName || 0 == sharedParamsFileName.Length)
            {
                var path = Path.Combine(Path.GetTempPath(), _shared_parameters_filename);
                using (StreamWriter stream = new(path))
                {
                }

                app.SharedParametersFilename = path;
                sharedParamsFileName = app.SharedParametersFilename;
            }

            var f = app.OpenSharedParameterFile();

            using Transaction t = new(doc);
            t.Start("Create CNC Export Tracking Shared Parameters");

            var catSet = app.Create.NewCategorySet();
            var cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parts);
            catSet.Insert(cat);

            Binding binding = app.Create.NewInstanceBinding(catSet);

            var group = f.Groups.get_Item(_definition_group_name)
                        ?? f.Groups.Create(_definition_group_name);

            var definition = group.Definitions.get_Item(_is_exported)
                             ?? CreateNewDefinition(group, _is_exported, SpecTypeId.Boolean.YesNo);

            doc.ParameterBindings.Insert(definition, binding, GroupTypeId.General);

            definition = group.Definitions.get_Item(_exported_first)
                ?? CreateNewDefinition(group, _exported_first, SpecTypeId.String.Text);

            doc.ParameterBindings.Insert(definition, binding, GroupTypeId.General);

            definition = group.Definitions.get_Item(_exported_last)
                ?? CreateNewDefinition(group, _exported_last, SpecTypeId.String.Text);

            doc.ParameterBindings.Insert(definition, binding, GroupTypeId.General);

            t.Commit();
        }

        public string GetSortMarkFor(Element e)
        {
            return null != _definition_sort_mark
                ? e.get_Parameter(_definition_sort_mark).AsString()
                : null;
        }
    }
}
