// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DoorSwing.CS
{
    /// <summary>
    ///     Static class used to add project shared parameters.
    /// </summary>
    public static class DoorSharedParameters
    {
        /// <summary>
        ///     Add shared parameters needed in this sample.
        ///     parameter 1: one string parameter named as "BasalOpening" which  is used for customization of door opening for each
        ///     country.
        ///     parameter 2: one string parameter named as "InstanceOpening" to indicate the door's opening value.
        ///     parameter 3: one YESNO parameter named as "Internal Door" to flag the door is internal door or not.
        /// </summary>
        /// <param name="app">Revit application.</param>
        public static void AddSharedParameters(UIApplication app)
        {
            // Create a new Binding object with the categories to which the parameter will be bound.
            var categories = app.Application.Create.NewCategorySet();

            // get door category and insert into the CategorySet.
            var doorCategory = app.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Doors);
            categories.Insert(doorCategory);

            // create one instance binding for "Internal Door" and "InstanceOpening" parameters;
            // and one type binding for "BasalOpening" parameters
            var instanceBinding = app.Application.Create.NewInstanceBinding(categories);
            var typeBinding = app.Application.Create.NewTypeBinding(categories);
            var bindingMap = app.ActiveUIDocument.Document.ParameterBindings;

            // Open the shared parameters file 
            // via the private method AccessOrCreateSharedParameterFile
            var defFile = AccessOrCreateSharedParameterFile(app.Application);
            if (null == defFile) return;

            // Access an existing or create a new group in the shared parameters file
            var defGroups = defFile.Groups;
            var defGroup = defGroups.get_Item("DoorProjectSharedParameters") ?? defGroups.Create("DoorProjectSharedParameters");

            // Access an existing or create a new external parameter definition belongs to a specific group.
            // for "BasalOpening"
            if (!AlreadyAddedSharedParameter(app.ActiveUIDocument.Document, "BasalOpening", BuiltInCategory.OST_Doors))
            {
                var basalOpening = defGroup.Definitions.get_Item("BasalOpening");

                if (null == basalOpening)
                {
                    var externalDefinitionCreationOptions1 =
                        new ExternalDefinitionCreationOptions("BasalOpening", SpecTypeId.String.Text);
                    basalOpening = defGroup.Definitions.Create(externalDefinitionCreationOptions1);
                }

                // Add the binding and definition to the document.
                bindingMap.Insert(basalOpening, typeBinding, GroupTypeId.Geometry);
            }

            // for "InstanceOpening"
            if (!AlreadyAddedSharedParameter(app.ActiveUIDocument.Document, "InstanceOpening",
                    BuiltInCategory.OST_Doors))
            {
                var instanceOpening = defGroup.Definitions.get_Item("InstanceOpening");

                if (null == instanceOpening)
                {
                    var externalDefinitionCreationOptions2 =
                        new ExternalDefinitionCreationOptions("InstanceOpening", SpecTypeId.String.Text);
                    instanceOpening = defGroup.Definitions.Create(externalDefinitionCreationOptions2);
                }

                // Add the binding and definition to the document.
                bindingMap.Insert(instanceOpening, instanceBinding, GroupTypeId.Geometry);
            }

            // for "Internal Door"
            if (!AlreadyAddedSharedParameter(app.ActiveUIDocument.Document, "Internal Door", BuiltInCategory.OST_Doors))
            {
                var internalDoorFlag = defGroup.Definitions.get_Item("Internal Door");

                if (null == internalDoorFlag)
                {
                    var externalDefinitionCreationOptions3 =
                        new ExternalDefinitionCreationOptions("Internal Door", SpecTypeId.Boolean.YesNo);
                    internalDoorFlag = defGroup.Definitions.Create(externalDefinitionCreationOptions3);
                }

                // Add the binding and definition to the document.
                bindingMap.Insert(internalDoorFlag, instanceBinding);
            }
        }

        /// <summary>
        ///     Access an existing or create a new shared parameters file.
        /// </summary>
        /// <param name="app">Revit Application.</param>
        /// <returns>the shared parameters file.</returns>
        private static DefinitionFile AccessOrCreateSharedParameterFile(Application app)
        {
            // The location of this command assembly
            var currentCommandAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // The path of ourselves shared parameters file
            var sharedParameterFilePath = Path.GetDirectoryName(currentCommandAssemblyPath);
            sharedParameterFilePath += "\\MySharedParameterFile.txt";

            //Method's return

            // Check if the file exits
            var documentMessage = new FileInfo(sharedParameterFilePath);
            var fileExist = documentMessage.Exists;

            // Create file for external shared parameter since it does not exist
            if (!fileExist)
            {
                var fileFlow = File.Create(sharedParameterFilePath);
                fileFlow.Close();
            }

            // Set ourselves file to the externalSharedParameterFile 
            app.SharedParametersFilename = sharedParameterFilePath;
            var sharedParameterFile = app.OpenSharedParameterFile();

            return sharedParameterFile;
        }

        /// <summary>
        ///     Has the specific document shared parameter already been added ago?
        /// </summary>
        /// <param name="doc">Revit project in which the shared parameter will be added.</param>
        /// <param name="paraName">the name of the shared parameter.</param>
        /// <param name="boundCategory">Which category the parameter will bind to</param>
        /// <returns>Returns true if already added ago else returns false.</returns>
        private static bool AlreadyAddedSharedParameter(Document doc, string paraName, BuiltInCategory boundCategory)
        {
            try
            {
                var bindingMap = doc.ParameterBindings;
                var bindingMapIter = bindingMap.ForwardIterator();

                while (bindingMapIter.MoveNext())
                    if (bindingMapIter.Key.Name.Equals(paraName))
                    {
                        var binding = bindingMapIter.Current as ElementBinding;
                        var categories = binding.Categories;

                        foreach (Category category in categories)
                            if (category.BuiltInCategory == boundCategory)
                                return true;
                    }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
