// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.IO;
using System.Reflection;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.MultiplanarRebar.CS
{
    /// <summary>
    ///     This is an utility class used to create shared parameter in Revit Document.
    ///     It simplifies the process of shared parameters creation.
    /// </summary>
    internal class SharedParameterUtil
    {
        /// <summary>
        ///     Get existed or create a new shared parameters with the given name and Revit DB Document.
        /// </summary>
        /// <param name="name">Shared parameter name</param>
        /// <param name="revitDoc">Revit DB Document</param>
        /// <returns>ElementId of get or created shared parameter</returns>
        public static ElementId GetOrCreateDef(string name, Document revitDoc)
        {
            var ed = GetOrCreateDef(name, revitDoc.Application);
            return RebarShapeParameters.GetOrCreateElementIdForExternalDefinition(revitDoc, ed);
        }

        /// <summary>
        ///     Get existed or create a new shared parameters with the given name and Revit DB Application.
        /// </summary>
        /// <param name="name">Shared parameter name</param>
        /// <param name="revitApp">Revit DB Application</param>
        /// <returns>ExternalDefinition of get or created shared parameter</returns>
        public static ExternalDefinition GetOrCreateDef(string name, Application revitApp)
        {
            return GetOrCreateDef(name, "Rebar Shape", revitApp);
        }

        /// <summary>
        ///     Get existed or create a new shared parameters with the given name, group and Revit DB Application.
        /// </summary>
        /// <param name="name">Shared parameter name</param>
        /// <param name="groupName">Shared parameter group name</param>
        /// <param name="revitApp">Revit DB Application</param>
        /// <returns>ExternalDefinition of get or created shared parameter</returns>
        public static ExternalDefinition GetOrCreateDef(string name, string groupName, Application revitApp)
        {
            var parameterFile = GetSharedParameterFile(revitApp);

            var group = parameterFile.Groups.get_Item(groupName) ?? parameterFile.Groups.Create(groupName);

            if (!(group.Definitions.get_Item(name) is ExternalDefinition bdef))
            {
                var externalDefinitionCreationOptions =
                    new ExternalDefinitionCreationOptions(name, SpecTypeId.ReinforcementLength);
                bdef = group.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
            }

            return bdef;
        }

        /// <summary>
        ///     Get shared parameter DefinitionFile of given Revit DB Application.
        /// </summary>
        /// <param name="revitApp">Revit DB Application</param>
        /// <returns>DefinitionFile of Revit DB Application</returns>
        public static DefinitionFile GetSharedParameterFile(Application revitApp)
        {
            DefinitionFile file = null;

            var count = 0;
            // A count is to avoid infinite loop
            while (null == file && count < 100)
            {
                file = revitApp.OpenSharedParameterFile();
                if (file == null)
                {
                    // If Shared parameter file does not exist, then create a new one.
                    var shapeFile =
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                        + "\\MultiplanarParameterFiles.txt";

                    // Fill Schema data of Revit shared parameter file.
                    // If no this schema data, OpenSharedParameterFile may alway return null.
                    var contents = new StringBuilder();
                    contents.AppendLine("# This is a Revit shared parameter file.");
                    contents.AppendLine("# Do not edit manually.");
                    contents.AppendLine("*META	VERSION	MINVERSION");
                    contents.AppendLine("META	2	1");
                    contents.AppendLine("*GROUP	ID	NAME");
                    contents.AppendLine("*PARAM	GUID	NAME	DATATYPE	DATACATEGORY	GROUP	VISIBLE");

                    // Write Schema data of Revit shared parameter file.
                    File.WriteAllText(shapeFile, contents.ToString());

                    // Set Revit shared parameter file
                    revitApp.SharedParametersFilename = shapeFile;
                }

                // To avoid infinite loop.
                ++count;
            }

            return file;
        }
    }
}
