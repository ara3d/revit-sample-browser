// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.InvisibleParam.CS
{
    /// <summary>
    ///     This command show how to use parameter file to store invisible and
    ///     visible parameters.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "External Tool");
            try
            {
                transaction.Start();

                //Create a clear file as parameter file.
                var path = Assembly.GetExecutingAssembly().Location;
                var index = path.LastIndexOf("\\");
                var newPath = path.Substring(0, index);
                newPath += "\\RevitParameters.txt";
                if (File.Exists(newPath)) File.Delete(newPath);
                var fs = File.Create(newPath);
                fs.Close();

                //cache application handle
                var revitApp = commandData.Application.Application;
                //prepare shared parameter file
                commandData.Application.Application.SharedParametersFilename = newPath;

                //Open shared parameter file
                var parafile = revitApp.OpenSharedParameterFile();

                //get walls category
                var wallCat =
                    commandData.Application.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory
                        .OST_Walls);
                var categories = revitApp.Create.NewCategorySet();
                categories.Insert(wallCat);

                var binding = revitApp.Create.NewInstanceBinding(categories);

                //Create a group
                var apiGroup = parafile.Groups.Create("APIGroup");

                //Create a visible "VisibleParam" of text type.
                var ExternalDefinitionCreationOptions1 =
                    new ExternalDefinitionCreationOptions("VisibleParam", SpecTypeId.String.Text);
                var visibleParamDef = apiGroup.Definitions.Create
                    (ExternalDefinitionCreationOptions1);
                ;
                var bindingMap = commandData.Application.ActiveUIDocument.Document.ParameterBindings;
                bindingMap.Insert(visibleParamDef, binding);

                //Create a invisible "InvisibleParam" of text type.
                var ExternalDefinitionCreationOptions2 =
                    new ExternalDefinitionCreationOptions("InvisibleParam", SpecTypeId.String.Text);
                var invisibleParamDef = apiGroup.Definitions.Create
                    (ExternalDefinitionCreationOptions2);
                bindingMap.Insert(invisibleParamDef, binding);
            }
            catch (Exception e)
            {
                transaction.RollBack();
                message = e.ToString();
                return Result.Cancelled;
            }
            finally
            {
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
