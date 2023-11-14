//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.IO;
using System.Xml;
using System.Xml.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

namespace Revit.SDK.Samples.RoutingPreferenceTools.CS
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

            var ofd = new OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml";

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
                        "Xml file is not a valid RoutingPreferenceBuilder xml document.  Please check RoutingPreferenceBuilderData.xsd.  " +
                        xmlValidationMessage);
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