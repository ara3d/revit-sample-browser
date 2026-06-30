// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from ExportCncFab by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/ExportCncFab

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

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

        /// <summary>
        /// An optional sort mark can be added to elements.
        /// The sort mark is inserted into the generated output filename.
        /// </summary>
        const string _sort_mark = "CncFabSortMark";

        Definition _definition_is_exported;
        Definition _definition_exported_first;
        Definition _definition_exported_last;
        Definition _definition_sort_mark;

        Document _doc;
        List<ElementId> _ids;

        static Definition GetDefinition(Element e, string parameter_name)
        {
            IList<Parameter> ps = e.GetParameters(parameter_name);
            int n = ps.Count;

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
                _ids = new List<ElementId>();
            }
        }

        public bool IsValid =>
            null != _definition_is_exported
            && null != _definition_exported_first
            && null != _definition_exported_last;

        public void Add(ElementId id) => _ids.Add(id);

        void UpdateExportHistory(Element e)
        {
            string s = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");

            e.get_Parameter(_definition_is_exported).Set(1);

            Parameter p = e.get_Parameter(_definition_exported_first);
            string s2 = p.AsString();

            if (null == s2 || 0 == s2.Length)
                p.Set(s);

            e.get_Parameter(_definition_exported_last).Set(s);
        }

        public void UpdateExportHistory()
        {
            foreach (ElementId id in _ids)
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

            Application app = doc.Application;

            string sharedParamsFileName = app.SharedParametersFilename;

            if (null == sharedParamsFileName || 0 == sharedParamsFileName.Length)
            {
                string path = Path.Combine(Path.GetTempPath(), _shared_parameters_filename);
                using (var stream = new StreamWriter(path))
                {
                }

                app.SharedParametersFilename = path;
                sharedParamsFileName = app.SharedParametersFilename;
            }

            DefinitionFile f = app.OpenSharedParameterFile();

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create CNC Export Tracking Shared Parameters");

                CategorySet catSet = app.Create.NewCategorySet();
                Category cat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parts);
                catSet.Insert(cat);

                Binding binding = app.Create.NewInstanceBinding(catSet);

                DefinitionGroup group = f.Groups.get_Item(_definition_group_name)
                    ?? f.Groups.Create(_definition_group_name);

                Definition definition = group.Definitions.get_Item(_is_exported)
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
        }

        public string GetSortMarkFor(Element e)
        {
            return null != _definition_sort_mark
                ? e.get_Parameter(_definition_sort_mark).AsString()
                : null;
        }
    }
}
