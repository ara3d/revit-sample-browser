// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.ApplicationServices;

namespace RevitMultiSample.ProjectInfo.CS
{
    /// <summary>
    ///     Attribute which designates Revit version names
    /// </summary>
    public sealed class RevitVersionAttribute : Attribute
    {
        /// <summary>
        ///     Revit version name array
        /// </summary>
        private readonly List<ProductType> m_products = new List<ProductType>();

        /// <summary>
        ///     Initializes Revit version name array
        /// </summary>
        /// <param name="names"></param>
        public RevitVersionAttribute(params ProductType[] names)
        {
            m_products.AddRange(names);
        }

        /// <summary>
        ///     Gets Revit version names
        /// </summary>
        public ReadOnlyCollection<ProductType> Names => m_products.AsReadOnly();
    }
}
