// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Openings.CS
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
            ElementId = opening.Id.ToString();

            if (null != opening.Host)
            {
                if (null != opening.Host.Category)
                    HostName = opening.Host.Category.Name;

                HostElementId = opening.Host.Id.ToString();
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
        public string ElementId { get; } = "";

        /// <summary>
        ///     host name
        /// </summary>
        [Description("Name of the Host which contains Current displayed Opening")]
        [Category("Opening Property")]
        public string HostName { get; } = "Null";

        /// <summary>
        ///     host elements id
        /// </summary>
        [Description("ElementId of Host")]
        [Category("Opening Property")]
        public string HostElementId { get; } = "";

        /// <summary>
        ///     shaft opening
        /// </summary>
        [Description("whether displayed openging is Shaft Opening")]
        [Category("Opening Property")]
        public bool ShaftOpening { get; }
    }
}
