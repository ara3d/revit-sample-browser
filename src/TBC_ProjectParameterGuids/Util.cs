#region Namespaces

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ProjectParameterGuids sample.</summary>
    internal static partial class Util
    {
        // RevitLookup #57
        public static void DeleteSharedParameters(Document doc)
        {
            doc.Delete(new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .ToElementIds());
        }
        public static ElementId GetProjectParameterId(
            Document doc,
            string name)
        {
            var pElem
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ParameterElement))
                    .Cast<ParameterElement>()
                    .Where(e => e.Name.Equals(name))
                    .FirstOrDefault();

            return pElem?.Id;
        }
        public static void DeleteNonSharedProjectParam(
            Document doc,
            string parametername)
        {
            var projectparameter
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(ParameterElement))
                    .Cast<ParameterElement>()
                    .Where(e => e.GetDefinition()
                        .Name.Equals(parametername))
                    .FirstOrDefault();

            if (projectparameter != null) doc.Delete(projectparameter.Id);
        }
        public static List<ProjectParameterData>
            GetProjectParameterData(
                Document doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");

            if (doc.IsFamilyDocument) throw new Exception("doc can not be a family document.");

            List<ProjectParameterData> result
                = new();

            var map = doc.ParameterBindings;
            var it
                = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                ProjectParameterData newProjectParameterData
                    = new()
                    {
                        Definition = it.Key,
                        Name = it.Key.Name,
                        Binding = it.Current
                        as ElementBinding
                    };

                result.Add(newProjectParameterData);
            }

            return result;
        }
        public static bool AddProjectParameterBinding(
            Document doc,
            ProjectParameterData projectParameterData,
            Category category)
        {
            if (doc == null) throw new ArgumentNullException("doc");

            if (doc.IsFamilyDocument)
                throw new Exception(
                    "doc can not be a family document.");

            if (projectParameterData == null)
                throw new ArgumentNullException(
                    "projectParameterData");

            if (category == null) throw new ArgumentNullException("category");

            var result = false;

            var cats = projectParameterData.Binding
                .Categories;

            if (cats.Contains(category))
            {
                var errorMessage = $"The project parameter '{projectParameterData.Definition.Name}' is already bound to the '{category.Name}' category.";

                throw new Exception(errorMessage);
            }

            cats.Insert(category);

            if (projectParameterData.Binding is InstanceBinding)
            {
                var newInstanceBinding
                    = doc.Application.Create
                        .NewInstanceBinding(cats);

                if (doc.ParameterBindings.ReInsert(
                    projectParameterData.Definition,
                    newInstanceBinding))
                    result = true;
            }
            else
            {
                var typeBinding
                    = doc.Application.Create
                        .NewTypeBinding(cats);

                if (doc.ParameterBindings.ReInsert(
                    projectParameterData.Definition, typeBinding))
                    result = true;
            }

            return result;
        }
        public static void PopulateProjectParameterData(
            Parameter parameter,
            ProjectParameterData projectParameterDataToFill)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");

            if (projectParameterDataToFill == null)
                throw new ArgumentNullException(
                    "projectParameterDataToFill");

            projectParameterDataToFill.IsSharedStatusKnown = true;
            projectParameterDataToFill.IsShared = parameter.IsShared;
            if (parameter.IsShared)
                projectParameterDataToFill.GUID = parameter.GUID.ToString();
        }
        public class ProjectParameterData
        {
            public ElementBinding Binding;
            public Definition Definition;
            public string GUID;
            public bool IsShared;
            public bool IsSharedStatusKnown;
            public string Name;
        }
    }
}
