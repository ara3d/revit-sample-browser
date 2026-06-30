using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static void HideLightingFixtureHosts(View view)
        {
            var doc = view.Document;

            var categories = doc.Settings.Categories;

            var catLightingFixtures
                = categories.get_Item(
                    BuiltInCategory.OST_LightingFixtures);

            var subcats
                = catLightingFixtures.SubCategories;

            var catHosts = subcats.get_Item("Hosts");

            view.SetCategoryHidden(catHosts.Id, true);
        }

        internal static void ProblemAddingParameterBindingForCategory(
            Document doc)
        {
            var app = doc.Application;

            var sharedParametersFile
                = app.OpenSharedParameterFile();

            var group = sharedParametersFile
                .Groups.Create("Reinforcement");

            ExternalDefinitionCreationOptions opt
                = new(
                    "ReinforcementParameter", SpecTypeId.String.Text);

            var def = group.Definitions.Create(opt);

            List<BuiltInCategory> bics
                = new()
                {
                    BuiltInCategory
                .OST_IOSRebarSystemSpanSymbolCtrl
                };

            CategorySet catset = new();

            foreach (var bic in bics)
                catset.Insert(
                    doc.Settings.Categories.get_Item(bic));

            var binding
                = app.Create.NewInstanceBinding(catset);

            doc.ParameterBindings.Insert(def, binding,
                GroupTypeId.Construction);
        }
        internal static void BuiltInCategoryNames(Document doc)
        {
            var categories = doc.Settings.Categories;

            var bics = Enum.GetValues(
                typeof(BuiltInCategory));

            foreach (BuiltInCategory bic in bics)
                try
                {
                    var cat = categories.get_Item(bic);

                    Debug.Print(cat.Name);
                }
                catch (Exception)
                {
                }
        }
    }
}
