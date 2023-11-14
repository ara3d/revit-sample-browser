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
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;

namespace Revit.SDK.Samples.RoutingPreferenceTools.CS
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

            var sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xml";
            sfd.Filter = "RoutingPreference Builder Xml files (*.xml)|*.xml";
            sfd.FileName =
                Path.GetFileNameWithoutExtension(commandData.Application.ActiveUIDocument.Document.PathName) +
                ".routingPreferences.xml";
            if (sfd.ShowDialog() == true)
            {
                var builder = new RoutingPreferenceBuilder(commandData.Application.ActiveUIDocument.Document);
                var pathsNotFound = false;
                var routingPreferenceBuilderDoc = builder.CreateXmlFromAllPipingPolicies(ref pathsNotFound);
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineOnAttributes = false;
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