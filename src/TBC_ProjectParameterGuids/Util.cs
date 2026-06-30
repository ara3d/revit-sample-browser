#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ProjectParameterGuids sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Delete all shared parameters from document, for
        ///     https://github.com/jeremytammik/RevitLookup/issues/57
        /// </summary>
        public static void DeleteSharedParameters(Document doc)
        {
            doc.Delete(new FilteredElementCollector(doc)
                .OfClass(typeof(SharedParameterElement))
                .ToElementIds());
        }

        /// <summary>
        ///     Return project parameter id for given name
        ///     for https://forums.autodesk.com/t5/revit-api-forum/create-view-filters-for-project-parameter/m-p/9051132
        /// </summary>
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

        /// <summary>
        ///     Delete non-shared project parameter by name
        ///     for https://forums.autodesk.com/t5/revit-api-forum/deleting-a-non-shared-project-parameter/td-p/5975020
        /// </summary>
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

        /// <summary>
        ///     Returns a list of the objects containing
        ///     references to the project parameter definitions
        /// </summary>
        public static List<ProjectParameterData>
            GetProjectParameterData(
                Document doc)
        {
            if (doc == null) throw new ArgumentNullException("doc");

            if (doc.IsFamilyDocument) throw new Exception("doc can not be a family document.");

            var result
                = new List<ProjectParameterData>();

            var map = doc.ParameterBindings;
            var it
                = map.ForwardIterator();
            it.Reset();
            while (it.MoveNext())
            {
                var newProjectParameterData
                    = new ProjectParameterData();

                newProjectParameterData.Definition = it.Key;
                newProjectParameterData.Name = it.Key.Name;
                newProjectParameterData.Binding = it.Current
                    as ElementBinding;

                result.Add(newProjectParameterData);
            }

            return result;
        }

        /// <summary>
        ///     This method takes a category and information
        ///     about a project parameter and adds a binding
        ///     to the category for the parameter.
        /// </summary>
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

        /// <summary>
        ///     This method populates the appropriate values
        ///     on a ProjectParameterData object with
        ///     information from the given Parameter object.
        /// </summary>
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

        /// <summary>
        ///     This class contains information discovered
        ///     about a (shared or non-shared) project parameter
        /// </summary>
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
