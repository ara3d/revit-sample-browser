// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.ApplicationServices;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS
{
    public sealed class RevitVersionAttribute : Attribute
    {
        private readonly List<ProductType> m_products = new List<ProductType>();

        public RevitVersionAttribute(params ProductType[] names)
        {
            m_products.AddRange(names);
        }

        public ReadOnlyCollection<ProductType> Names => m_products.AsReadOnly();
    }
}
