// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ReadonlySharedParameters.CS
{
    public class SharedParameterBindingManager
    {
        private readonly List<BuiltInCategory> m_categories = new List<BuiltInCategory>();

        public SharedParameterBindingManager()
        {
            Name = "Invalid";
            Type = new ForgeTypeId();
            UserModifiable = true;
            Description = "";
            Instance = true;
            Definition = null;
            ParameterGroup = GroupTypeId.IdentityData;
        }

        public string Name { get; set; }

        public ForgeTypeId Type { get; set; }

        public bool UserModifiable { get; set; }

        public string Description { get; set; }

        public bool Instance { get; set; }

        public Definition Definition { get; set; }

        public ForgeTypeId ParameterGroup { get; set; }

        public ExternalDefinitionCreationOptions GetCreationOptions()
        {
            var options = new ExternalDefinitionCreationOptions(Name, Type);
            options.UserModifiable = UserModifiable;
            options.Description = Description;
            return options;
        }

        public void AddCategory(BuiltInCategory category)
        {
            m_categories.Add(category);
        }

        private CategorySet GetCategories(Document doc)
        {
            var categories = doc.Settings.Categories;

            var categorySet = new CategorySet();

            foreach (var bic in m_categories)
            {
                categorySet.Insert(categories.get_Item(bic));
            }

            return categorySet;
        }

        public void AddBindings(Document doc)
        {
            Binding binding;
            if (Instance)
                binding = new InstanceBinding(GetCategories(doc));
            else
                binding = new TypeBinding(GetCategories(doc));
            // assumes transaction open
            doc.ParameterBindings.Insert(Definition, binding, ParameterGroup);
        }
    }

    public class Utils
    {
    }
}
