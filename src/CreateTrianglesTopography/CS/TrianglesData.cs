// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using Autodesk.Revit.DB;

namespace RevitMultiSample.CreateTrianglesTopography.CS
{
    /// <summary>
    ///     Triangles points and facets data which can be used to create topography surface.
    /// </summary>
    public class TrianglesData
    {
        /// The points represent an enclosed area in the XY plane.
        public IList<XYZ> Points { get; set; }

        ///Triangle faces composing a polygon mesh.
        public IList<IList<int>> Facets { get; set; }

        /// <summary>
        ///     parse all points and facets stored in the TrianglesData.json
        /// </summary>
        /// <returns>an instance of TrianglesData</returns>
        public static TrianglesData Load()
        {
            var assemblyFileFolder = Path.GetDirectoryName(typeof(TrianglesData).Assembly.Location);
            var emmfilePath = Path.Combine(assemblyFileFolder, "TrianglesData.json");
            var emmfileContent = File.ReadAllText(emmfilePath);
            return JsonParse(emmfileContent);
        }

        private static TrianglesData JsonParse(string jsonString)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[] { new XyzConverter() });

            return serializer.Deserialize(jsonString, typeof(TrianglesData)) as TrianglesData;
        }
    }

    /// <summary>
    ///     The converter for Revit XYZ.
    /// </summary>
    public class XyzConverter : JavaScriptConverter
    {
        /// <summary>
        ///     gets a collection of the supported types
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new[] { typeof(XYZ) }; }
        }

        /// <summary>
        ///     Converts the provided dictionary into an object of Revit XYZ
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            return new XYZ(Convert.ToDouble(dictionary["X"]), Convert.ToDouble(dictionary["Y"]),
                Convert.ToDouble(dictionary["Z"]));
        }

        /// <summary>
        ///     Converts the provided Revit XYZ object to a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var dic = new Dictionary<string, object>();
            if (!(obj is XYZ node))
                return null;
            dic.Add("X", node.X);
            dic.Add("Y", node.Y);
            dic.Add("Z", node.Z);

            return dic;
        }
    }
}
