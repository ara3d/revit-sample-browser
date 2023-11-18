// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using System.Xml;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

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

            var ofd = new OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml"
            };

            if (ofd.ShowDialog() == true)
            {
                var reader = new StreamReader(ofd.FileName);
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
                    var builder = new RoutingPreferenceBuilder(commandData.Application.ActiveUIDocument.Document);
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
