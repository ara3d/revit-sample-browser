//
// (C) Copyright 2003-2019 by Autodesk, Inc.
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