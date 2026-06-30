// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.InPlaceMembers.CS
{
    public class Properties
    {
        public Properties(FamilyInstance f)
        {
            Id = f.Id.Value;
            Name = f.Name;
            Family = f.Symbol.Family.Name;
            Type = f.Symbol.Name;
            StructuralType = f.StructuralType.ToString();
            try
            {
                StructuralUsage = f.StructuralUsage.ToString();
            }
            catch (Exception)
            {
                StructuralUsage = null;
            }

            Material = f.StructuralMaterialType.ToString();
        }

        /// <summary>
        ///     the value of the element id as an integer
        /// </summary>
        [Category("Identity")]
        public long Id { get; }

        /// <summary>
        ///     a human readable name for the Element.
        /// </summary>
        [Category("Identity")]
        public string Name { get; }

        /// <summary>
        ///     a human readable name for the family name.
        /// </summary>
        [Category("Category")]
        public string Family { get; }

        /// <summary>
        ///     a human readable name for the family type name.
        /// </summary>
        [Category("Category")]
        public string Type { get; }

        /// <summary>
        ///     the primary structural type of the instance, such as beam or column etc.
        /// </summary>
        [Category("Structural")]
        public string StructuralType { get; }

        /// <summary>
        ///     the primary structural usage of the instance, such as brace, girder etc.
        /// </summary>
        [Category("Structural")]
        public string StructuralUsage { get; }

        /// <summary>
        ///     the physical material from which the instance is made.
        /// </summary>
        [Category("Structural")]
        public string Material { get; }
    }
}
