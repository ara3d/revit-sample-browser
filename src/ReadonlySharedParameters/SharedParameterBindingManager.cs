// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.ReadonlySharedParameters.CS
{
    public class SharedParameterBindingManager
    {
        private readonly List<BuiltInCategory> m_categories = [];

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
            ExternalDefinitionCreationOptions options = new(Name, Type)
            {
                UserModifiable = UserModifiable,
                Description = Description
            };
            return options;
        }

        public void AddCategory(BuiltInCategory category)
        {
            m_categories.Add(category);
        }

        private CategorySet GetCategories(Document doc)
        {
            var categories = doc.Settings.Categories;

            CategorySet categorySet = new();

            foreach (var bic in m_categories)
            {
                categorySet.Insert(categories.get_Item(bic));
            }

            return categorySet;
        }

        public void AddBindings(Document doc)
        {
            Binding binding = Instance ? new InstanceBinding(GetCategories(doc)) : new TypeBinding(GetCategories(doc));
            // assumes transaction open
            doc.ParameterBindings.Insert(Definition, binding, ParameterGroup);
        }
    }

    public class Utils
    {
    }
}
