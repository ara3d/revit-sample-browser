using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_Categories sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Hide the LightingFixtures category
        ///     Hosts subcategory in the given view.
        /// </summary>
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

            var opt
                = new ExternalDefinitionCreationOptions(
                    "ReinforcementParameter", SpecTypeId.String.Text);

            var def = group.Definitions.Create(opt);

            var bics
                = new List<BuiltInCategory>();

            bics.Add(BuiltInCategory
                .OST_IOSRebarSystemSpanSymbolCtrl);

            var catset = new CategorySet();

            foreach (var bic in bics)
                catset.Insert(
                    doc.Settings.Categories.get_Item(bic));

            var binding
                = app.Create.NewInstanceBinding(catset);

            doc.ParameterBindings.Insert(def, binding,
                GroupTypeId.Construction);
        }

        /// <summary>
        ///     List names of built-in categories in document.
        /// </summary>
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
