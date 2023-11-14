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
using System.Text;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    /// A class to store Load Combination and it's properties.
    /// </summary>
    public class LoadCombinationMap
    {
        // Private Members

        /// <summary>
        /// Name property of LoadCombinationMap
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Formula property of LoadCombinationMap
        /// </summary>
        public string Formula { get; }

        /// <summary>
        /// Type property of LoadCombinationMap
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// State property of LoadCombinationMap
        /// </summary>
        public string State { get; }

        /// <summary>
        /// Usage property of LoadCombinationMap
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Default constructor of LoadCombinationMap
        /// </summary>
        /// <param name="combination">the reference of LoadCombination</param>
        public LoadCombinationMap(LoadCombination combination)
        {
            Name = combination.Name;
            Type = combination.Type.ToString();
            State = combination.State.ToString();
            var m_document = combination.Document;

            // Generate the formula field.
            var formulaString = new StringBuilder();
            var components = combination.GetComponents();
            foreach (var component in components)
            {
                formulaString.Append(component.Factor);
                formulaString.Append("*");
                formulaString.Append(m_document.GetElement(component.LoadCaseOrCombinationId).Name);

                if (components.IndexOf(component) < components.Count - 1)
                {
                    formulaString.Append(" + ");
                }
            }

            Formula = formulaString.ToString();
            
            // Generate the usage field.
            var usageString = new StringBuilder();
            var usageIds = combination.GetUsageIds();
            foreach (var id in usageIds)
            {
                m_document.GetElement(id);
                usageString.Append(m_document.GetElement(id).Name);

                if (usageIds.IndexOf(id) < usageIds.Count - 1)
                {
                    usageString.Append(";");
                }
            }

            Usage = usageString.ToString();
        }
    }
}
