// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.InPlaceMembers.CS
{
    /// <summary>
    ///     This class is used as PropertyGrid.SelectedObject.
    /// </summary>
    public class Properties
    {
        /// <summary>
        ///     get this family instance's properties to display.
        /// </summary>
        /// <param name="f">a In-Place family instance</param>
        public Properties(FamilyInstance f)
        {
            ID = f.Id.Value;
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
        [CategoryAttribute("Identity")]
        public long ID { get; }

        /// <summary>
        ///     a human readable name for the Element.
        /// </summary>
        [CategoryAttribute("Identity")]
        public string Name { get; }

        /// <summary>
        ///     a human readable name for the family name.
        /// </summary>
        [CategoryAttribute("Category")]
        public string Family { get; }

        /// <summary>
        ///     a human readable name for the family type name.
        /// </summary>
        [CategoryAttribute("Category")]
        public string Type { get; }

        /// <summary>
        ///     the primary structural type of the instance, such as beam or column etc.
        /// </summary>
        [CategoryAttribute("Structural")]
        public string StructuralType { get; }

        /// <summary>
        ///     the primary structural usage of the instance, such as brace, girder etc.
        /// </summary>
        [CategoryAttribute("Structural")]
        public string StructuralUsage { get; }

        /// <summary>
        ///     the physical material from which the instance is made.
        /// </summary>
        [CategoryAttribute("Structural")]
        public string Material { get; }
    }
}
