// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS
{
    public class StorageUtility
    {
        /// <summary>
        ///     Returns true if any extensible storage exists in the document, false otherwise.
        /// </summary>
        public static bool DoesAnyStorageExist(Document doc)
        {
            var schemas = Schema.ListSchemas();
            if (schemas.Count == 0)
            {
                return false;
            }

            foreach (var schema in schemas)
            {
                var ids = ElementsWithStorage(doc, schema);
                if (ids.Count > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns a formatted string containing schema guids and element info for all elements
        ///     containing extensible storage.
        /// </summary>
        public static string GetElementsWithAllSchemas(Document doc)
        {
            var sBuilder = new StringBuilder();
            var schemas = Schema.ListSchemas();
            if (schemas.Count == 0)
            {
                return "No schemas or storage.";
            }

            foreach (var schema in schemas)
            {
                sBuilder.Append(GetElementsWithSchema(doc, schema));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        ///     Returns a formatted string containing a schema guid and element info for all elements
        ///     containing extensible storage of a given schema.
        /// </summary>
        private static string GetElementsWithSchema(Document doc, Schema schema)
        {
            var sBuilder = new StringBuilder();
            sBuilder.AppendLine($"Schema: {schema.GUID}, {schema.SchemaName}");
            var elementsofSchema = ElementsWithStorage(doc, schema);
            if (elementsofSchema.Count == 0)
                sBuilder.AppendLine("No elements.");
            else
                foreach (var id in elementsofSchema)
                {
                    sBuilder.AppendLine(PrintElementInfo(id, doc));
                }

            return sBuilder.ToString();
        }

        /// <summary>
        ///     Returns a list of ElementIds that contain extensible storage of a given schema using
        ///     the ExtensibleStorageFilter ElementQuickFilter.
        /// </summary>
        private static List<ElementId> ElementsWithStorage(Document doc, Schema schema)
        {
            var ids = new List<ElementId>();
            var collector = new FilteredElementCollector(doc);
            collector.WherePasses(new ExtensibleStorageFilter(schema.GUID));
            ids.AddRange(collector.ToElementIds());
            return ids;
        }

        /// <summary>
        ///     Writes basic element info to a string.
        /// </summary>
        private static string PrintElementInfo(ElementId id, Document document)
        {
            var element = document.GetElement(id);
            var retval = $"{element.Id}, {element.Name}, {element.GetType().FullName}";
            Debug.WriteLine(retval);
            return retval;
        }
    }
}
