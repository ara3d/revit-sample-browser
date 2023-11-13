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


using System;
using System.IO;
using System.Reflection;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;


namespace Revit.SDK.Samples.InvisibleParam.CS
{
    /// <summary>
    /// This command show how to use parameter file to store invisible and 
    /// visible parameters.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {

        #region IExternalCommand Members
        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="commandData">An object that is passed to the external application
        /// which contains data related to the command,
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application
        /// which will be displayed if a failure or cancellation is returned by
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command.
        /// A result of Succeeded means that the API external method functioned as expected.
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(
            ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();

                //Create a clear file as parameter file.
                var path = Assembly.GetExecutingAssembly().Location;
                var index = path.LastIndexOf("\\");
                var newPath = path.Substring(0,index);
                newPath += "\\RevitParameters.txt";
                if(File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
                var fs = File.Create(newPath);
                fs.Close();               

                //cache application handle
                var revitApp = commandData.Application.Application;
                //prepare shared parameter file
                commandData.Application.Application.SharedParametersFilename = newPath;

                //Open shared parameter file
                var parafile = revitApp.OpenSharedParameterFile();

                //get walls category
                var wallCat = commandData.Application.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
                var categories = revitApp.Create.NewCategorySet();
                categories.Insert(wallCat);

                var binding = revitApp.Create.NewInstanceBinding(categories);

                //Create a group
                var apiGroup = parafile.Groups.Create("APIGroup");

                //Create a visible "VisibleParam" of text type.
                var ExternalDefinitionCreationOptions1 = new ExternalDefinitionCreationOptions("VisibleParam", SpecTypeId.String.Text);
                var visibleParamDef = apiGroup.Definitions.Create
                    (ExternalDefinitionCreationOptions1);
                                        ;
                                        var bindingMap = commandData.Application.ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(visibleParamDef, binding);

                //Create a invisible "InvisibleParam" of text type.
                var ExternalDefinitionCreationOptions2 = new ExternalDefinitionCreationOptions("InvisibleParam", SpecTypeId.String.Text);
                var invisibleParamDef = apiGroup.Definitions.Create
                    (ExternalDefinitionCreationOptions2);
                bindingMap.Insert(invisibleParamDef, binding);
            }
            catch(Exception e)
            {
                transaction.RollBack();
                message = e.ToString();
                return Autodesk.Revit.UI.Result.Cancelled;
            }
            finally
            {
                transaction.Commit();
            }
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        #endregion
    }
}
