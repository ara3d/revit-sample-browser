// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.IO;
using System.Xml;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

namespace RevitMultiSample.RoutingPreferenceTools.CS
{
    /// <summary>
    ///     A command to read routing preference data from a document and write an XML file summarizing it that can later be
    ///     read by the
    ///     CommandReadPreferences command.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class CommandWritePreferences : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!Validation.ValidateMep(commandData.Application.Application))
            {
                Validation.MepWarning();
                return Result.Succeeded;
            }

            if (!Validation.ValidatePipesDefined(commandData.Application.ActiveUIDocument.Document))
            {
                Validation.PipesDefinedWarning();
                return Result.Succeeded;
            }

            var sfd = new SaveFileDialog
            {
                DefaultExt = ".xml",
                Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml",
                FileName = Path.GetFileNameWithoutExtension(commandData.Application.ActiveUIDocument.Document.PathName) +
                           ".routingPreferences.xml"
            };
            if (sfd.ShowDialog() == true)
            {
                var builder = new RoutingPreferenceBuilder(commandData.Application.ActiveUIDocument.Document);
                var pathsNotFound = false;
                var routingPreferenceBuilderDoc = builder.CreateXmlFromAllPipingPolicies(ref pathsNotFound);
                var xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = false
                };
                var writer = XmlWriter.Create(sfd.FileName, xmlWriterSettings);
                routingPreferenceBuilderDoc.WriteTo(writer);
                writer.Flush();
                writer.Close();
                var pathmessage = "";
                if (pathsNotFound)
                    pathmessage =
                        "One or more paths to .rfa files were not found.  You may need to add these paths in manually to the generated xml file.";
                TaskDialog.Show("RoutingPreferenceBuilder",
                    "Routing Preferences exported successfully.   " + pathmessage);
            }

            return Result.Succeeded;
        }
    }
}
