// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Mep;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using System.Xml.Linq;
namespace Ara3D.RevitSampleBrowser.RoutingPreferenceTools.CS.RoutingPreferenceBuilder
{
    /// <summary>
    ///     A command to read a routing preference builder xml file and add pipe types, schedules, segments, and sizes, and
    ///     routing preferences rules to the
    ///     document from the xml data.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CommandReadPreferences : IExternalCommand
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

            OpenFileDialog ofd = new()
            {
                DefaultExt = ".xml",
                Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml"
            };

            if (ofd.ShowDialog() == true)
            {
                StreamReader reader = new(ofd.FileName);
                var routingPreferenceBuilderDoc = XDocument.Load(new XmlTextReader(reader));
                reader.Close();

                //Distribute the .xsd file to routing preference builder xml authors as necessary.
                string xmlValidationMessage;
                if (!SchemaValidationHelper.ValidateRoutingPreferenceBuilderXml(routingPreferenceBuilderDoc,
                        out xmlValidationMessage))
                {
                    TaskDialog.Show("RoutingPreferenceBuilder",
                        $"Xml file is not a valid RoutingPreferenceBuilder xml document.  Please check RoutingPreferenceBuilderData.xsd.  {xmlValidationMessage}");
                    return Result.Succeeded;
                }

                try
                {
                    RoutingPreferenceBuilder builder = new(commandData.Application.ActiveUIDocument.Document);
                    builder.ParseAllPipingPoliciesFromXml(routingPreferenceBuilderDoc);
                    TaskDialog.Show("RoutingPreferenceBuilder", "Routing Preferences imported successfully.");
                }
                catch (RoutingPreferenceDataException ex)
                {
                    TaskDialog.Show("RoutingPreferenceBuilder error: ", ex.ToString());
                }
            }

            return Result.Succeeded;
        }
    }
}
