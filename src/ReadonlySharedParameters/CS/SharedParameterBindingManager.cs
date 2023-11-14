//
// (C) Copyright 2003-2015 by Autodesk, Inc.
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

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ReadonlySharedParameters.CS
{
    internal class SharedParameterBindingManager
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

            foreach (var bic in m_categories) categorySet.Insert(categories.get_Item(bic));

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

    internal class Utils
    {
    }
}