// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.Common.Documents;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class ExtensibleStorageHelper
    {
        private static int sCounter = DateTime.Now.Second;

        public static Guid NewGuid()
                {
                    var guidBytes = new byte[16];
                    new Random(sCounter).NextBytes(guidBytes);
                    sCounter++;
                    return new Guid(guidBytes);
                }

        public static bool DoesAnyStorageExist(Document doc) =>
                    Schema.ListSchemas().Any(schema => ElementQuery.ElementsWithStorage(doc, schema).Count > 0);

        public static string GetElementsWithAllSchemas(Document doc)
                {
                    var schemas = Schema.ListSchemas();
                    if (schemas.Count == 0)
                        return "No schemas or storage.";

                    var sBuilder = new StringBuilder();
                    foreach (var schema in schemas)
                        sBuilder.Append(ElementQuery.GetElementsWithSchema(doc, schema));
                    return sBuilder.ToString();
                }

    }
}