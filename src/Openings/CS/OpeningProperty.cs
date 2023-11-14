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

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     This class use to create a object can use by PropertyGrid control
    /// </summary>
    public class OpeningProperty
    {
        /// <summary>
        ///     The default constructor
        /// </summary>
        /// <param name="opening">Opening in Revit</param>
        public OpeningProperty(Opening opening)
        {
            if (null == opening) throw new ArgumentNullException();

            //get parameters which need to show
            Name = opening.Name;
            ElementID = opening.Id.ToString();

            if (null != opening.Host)
            {
                if (null != opening.Host.Category)
                    HostName = opening.Host.Category.Name;

                HostElementID = opening.Host.Id.ToString();
            }

            if (null != opening.Category)
                if ("Shaft Openings" == opening.Category.Name)
                    ShaftOpening = true;
        }

        /// <summary>
        ///     name
        /// </summary>
        [Description("Name of current diaplayed Opening")]
        [Category("Opening Name")]
        public string Name { get; } = "Opening";

        /// <summary>
        ///     element id
        /// </summary>
        [Description("ElementId of current diaplayed Opening")]
        [Category("Opening Property")]
        public string ElementID { get; } = "";

        /// <summary>
        ///     host name
        /// </summary>
        [Description("Name of the Host which contains Current displayed Opening")]
        [CategoryAttribute("Opening Property")]
        public string HostName { get; } = "Null";

        /// <summary>
        ///     host elements id
        /// </summary>
        [Description("ElementId of Host")]
        [CategoryAttribute("Opening Property")]
        public string HostElementID { get; } = "";

        /// <summary>
        ///     shaft opening
        /// </summary>
        [Description("whether displayed openging is Shaft Opening")]
        [CategoryAttribute("Opening Property")]
        public bool ShaftOpening { get; }
    }
}