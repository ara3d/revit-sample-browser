// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.RebarFreeForm.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AddSharedParams : IExternalCommand
    {
        /// <summary>
        ///     Updated: is used to start the regeneration
        /// </summary>
        public static readonly string m_paramName = "Updated";

        /// <summary>
        ///     CurveElementId: is used to store the id of a model curve
        /// </summary>
        public static readonly string m_CurveIdName = "CurveElementId";

        /// <summary>
        ///     Add two shared parameters to the rebar category instance elements:
        ///     Updated: is used to start the regeneration
        ///     CurveElementId: is used to store the id of a model curve
        /// </summary>
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;
                if (doc == null)
                    return Result.Failed;
                using (var tran = new Transaction(doc, "Add shared param"))
                {
                    tran.Start();

                    // Add Shared parameters:
                    //   Update is a simple boolean.
                    //   CurveElementId is an ElementId, which is a 64-bit Entity, so stringify it to keep data intact. 
                    var paramsAdded = AddSharedTestParameter(commandData, m_paramName, SpecTypeId.Boolean.YesNo, false);
                    paramsAdded &= AddSharedTestParameter(commandData, m_CurveIdName, SpecTypeId.String.Text, true);
                    if (paramsAdded)
                    {
                        tran.Commit();
                        return Result.Succeeded;
                    }

                    tran.RollBack();
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }

        private bool AddSharedTestParameter(ExternalCommandData commandData, string paramName, ForgeTypeId paramType,
            bool userModifiable)
        {
            try
            {
                // check whether shared parameter exists
                if (ShareParameterExists(commandData.Application.ActiveUIDocument.Document, paramName)) return true;

                // create shared parameter file
                var modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var paramFile = modulePath + "\\RebarTestParameters.txt";
                if (File.Exists(paramFile)) File.Delete(paramFile);
                var fs = File.Create(paramFile);
                fs.Close();

                // cache application handle
                var revitApp = commandData.Application.Application;

                // prepare shared parameter file
                commandData.Application.Application.SharedParametersFilename = paramFile;

                // open shared parameter file
                var parafile = revitApp.OpenSharedParameterFile();

                // create a group
                var apiGroup = parafile.Groups.Create("RebarTestParamGroup");

                // create a visible param 
                var ExtDefinitionCreationOptions = new ExternalDefinitionCreationOptions(paramName, paramType);
                ExtDefinitionCreationOptions.HideWhenNoValue =
                    true; //used this to show the parameter only in some rebar instances that will use it
                ExtDefinitionCreationOptions.UserModifiable = userModifiable; //  set if users need to modify this
                var rebarSharedParamDef = apiGroup.Definitions.Create(ExtDefinitionCreationOptions);

                // get rebar category
                var rebarCat =
                    commandData.Application.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory
                        .OST_Rebar);
                var categories = revitApp.Create.NewCategorySet();
                categories.Insert(rebarCat);

                // insert the new parameter
                var binding = revitApp.Create.NewInstanceBinding(categories);
                commandData.Application.ActiveUIDocument.Document.ParameterBindings.Insert(rebarSharedParamDef,
                    binding);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create shared parameter: " + ex.Message);
            }
        }

        /// <summary>
        ///     Checks if a parameter exists based of a name
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private bool ShareParameterExists(Document doc, string paramName)
        {
            var bindingMap = doc.ParameterBindings;
            var iter = bindingMap.ForwardIterator();
            iter.Reset();

            while (iter.MoveNext())
            {
                var tempDefinition = iter.Key;

                // find the definition of which the name is the appointed one
                if (string.Compare(tempDefinition.Name, paramName) != 0) continue;

                // get the category which is bound
                var binding = bindingMap.get_Item(tempDefinition) as ElementBinding;
                var bindCategories = binding.Categories;
                foreach (Category category in bindCategories)
                    if (category.Name
                        == doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rebar).Name)
                        return true;
            }

            return false;
        }
    }
}
