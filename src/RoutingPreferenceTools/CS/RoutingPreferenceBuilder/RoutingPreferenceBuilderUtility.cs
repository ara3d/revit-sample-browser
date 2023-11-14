// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Autodesk.Revit.ApplicationServices;

namespace Revit.SDK.Samples.RoutingPreferenceTools.CS
{
    /// <summary>
    ///     A helper class to find paths of .rfa family files in a document.
    /// </summary>
    internal class FindFolderUtility
    {
        private readonly Application m_application;
        private readonly string m_basePath;
        private readonly Dictionary<string, string> m_familyfiles;

        /// <summary>
        ///     Create an instance of the helper class
        /// </summary>
        public FindFolderUtility(Application application)
        {
            m_application = application;
            m_basePath = GetFamilyBasePath();
            var extraFamilyPaths = GetAdditionalFamilyPaths();
            m_familyfiles = new Dictionary<string, string>();
            GetAllFiles(m_basePath, m_familyfiles);
            //Get additional .rfa files in user-defined paths specified in familypaths.xml
            foreach (var extraPath in extraFamilyPaths) GetAllFiles(extraPath, m_familyfiles);
        }

        /// <summary>
        ///     Gets a list of paths defined in the familypaths.xml file in the RoutingPreferenceTools.dll folder.
        /// </summary>
        /// <returns>A list of paths containing .rfa files</returns>
        public static List<string> GetAdditionalFamilyPaths()
        {
            var pathsList = new List<string>();
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName +
                                          "\\familypaths.xml");
            }
            catch (Exception)
            {
                return pathsList;
            }

            var pathsDoc = XDocument.Load(new XmlTextReader(reader));
            var paths = pathsDoc.Root.Elements("FamilyPath");

            foreach (var xpath in paths)
                try
                {
                    var xaPath = xpath.Attribute(XName.Get("pathname"));
                    var familyPath = xaPath.Value;
                    pathsList.Add(familyPath);
                }
                catch (Exception)
                {
                }

            reader.Close();
            return pathsList;
        }

        /// <summary>
        ///     Returns the full path of an .rfa file in the "Data" directory of a Revit installation given an .rfa filename.
        /// </summary>
        /// <param name="filename">an .rfa filename without any path, e.g. "Generic elbow.rfa"</param>
        /// <returns>The full path to the .rfa file</returns>
        public string FindFileFolder(string filename)
        {
            var retval = "";
            try
            {
                retval = m_familyfiles[filename];
            }
            catch (KeyNotFoundException)
            {
            }

            return retval;
        }

        private static void GetAllFiles(string basePath, Dictionary<string, string> allPaths)
        {
            if (!Directory.Exists(basePath))
                return;
            var paths = Directory.GetFiles(basePath, "*.rfa");
            foreach (var path in paths)
            {
                var filename = Path.GetFileName(path);
                allPaths[filename] = path;
            }

            var dirs = Directory.GetDirectories(basePath);
            foreach (var dir in dirs) GetAllFiles(dir, allPaths);
        }

        private string GetFamilyBasePath()
        {
            var basePath = "";
            try
            {
                basePath = m_application.GetLibraryPaths()["Imperial Library"];
                Debug.WriteLine(basePath);
                if (basePath.EndsWith(@"\")) basePath = basePath.Substring(0, basePath.Length - 1);
                basePath = Directory.GetParent(basePath).ToString();
                if (!string.IsNullOrEmpty(basePath))
                    return basePath;
            }
            catch (Exception)
            {
            }

            try
            {
                basePath = m_application.GetLibraryPaths().First().Value;
                if (basePath.EndsWith(@"\")) basePath = basePath.Substring(0, basePath.Length - 1);
                basePath = Directory.GetParent(Directory.GetParent(basePath).ToString()).ToString();
                if (!string.IsNullOrEmpty(basePath))
                    return basePath;
            }
            catch (Exception)
            {
            }

            try
            {
                var exe = Process.GetCurrentProcess().MainModule.FileName;
                basePath = Directory.GetParent(Path.GetDirectoryName(exe)).ToString();
            }
            catch (Exception)
            {
            }

            return basePath;
        }
    }

    /// <summary>
    ///     A simple exception class to help identify errors related to this application.
    /// </summary>
    internal class RoutingPreferenceDataException : Exception
    {
        private readonly string m_message;

        public RoutingPreferenceDataException(string message)
        {
            m_message = message;
        }

        public override string ToString()
        {
            return m_message;
        }
    }

    /// <summary>
    ///     A helper class to validate RoutingPreferenceBuilder xml documents against
    ///     the embedded resource "RoutingPreferenceBuilderData.xsd"
    /// </summary>
    internal class SchemaValidationHelper
    {
        /// <summary>
        ///     Returns true if a document is a valid RoutingPreferenceBuilder xml document, false otherwise.
        /// </summary>
        public static bool ValidateRoutingPreferenceBuilderXml(XDocument doc, out string message)
        {
            message = "";
            var schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(GetXmlSchema())));

            try
            {
                doc.Validate(schemas, null);
                return true;
            }
            catch (XmlSchemaValidationException ex)
            {
                message = ex.Message + ", " + ex.LineNumber + ", " + ex.LinePosition;
                return false;
            }
        }

        private static string GetXmlSchema()
        {
            //string[] names = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
            Stream schemaStream = null;
            try
            {
                schemaStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "Revit.SDK.Samples.RoutingPreferenceTools.CS.RoutingPreferenceBuilder.RoutingPreferenceBuilderData.xsd");
            }
            catch (Exception)
            {
                throw new Exception("Could not find embedded xml RotingPreferenceBuilder schema resource.");
            }

            if (schemaStream == null)
                throw new Exception("Could not find embedded xml RotingPreferenceBuilder schema resource.");
            var stReader = new StreamReader(schemaStream);
            return stReader.ReadToEnd();
        }
    }
}
