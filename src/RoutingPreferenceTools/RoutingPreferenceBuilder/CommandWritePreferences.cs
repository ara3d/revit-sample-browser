// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System.IO;
using System.Xml;
namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceBuilder
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
            if (!RoutingPreferenceHelper.ValidateMep(commandData.Application.Application))
            {
                RoutingPreferenceHelper.MepWarning();
                return Result.Succeeded;
            }

            if (!RoutingPreferenceHelper.ValidatePipesDefined(commandData.Application.ActiveUIDocument.Document))
            {
                RoutingPreferenceHelper.PipesDefinedWarning();
                return Result.Succeeded;
            }

            SaveFileDialog sfd = new()
            {
                DefaultExt = ".xml",
                Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml",
                FileName =
                    $"{Path.GetFileNameWithoutExtension(commandData.Application.ActiveUIDocument.Document.PathName)}.routingPreferences.xml"
            };
            if (sfd.ShowDialog() == true)
            {
                RoutingPreferenceBuilder builder = new(commandData.Application.ActiveUIDocument.Document);
                var pathsNotFound = false;
                var routingPreferenceBuilderDoc = builder.CreateXmlFromAllPipingPolicies(ref pathsNotFound);
                XmlWriterSettings xmlWriterSettings = new()
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
                    $"Routing Preferences exported successfully.   {pathmessage}");
            }

            return Result.Succeeded;
        }
    }
}
