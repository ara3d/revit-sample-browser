// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

namespace Revit.SDK.Samples.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The ZoneNode class inherit TreeNode Class, it is used
    ///     to display the Zones is a TreeView, each ZoneNode contains
    ///     a Zone element.
    /// </summary>
    public class ZoneNode : TreeNode
    {
        /// <summary>
        ///     The constructor of ZoneNode class
        /// </summary>
        /// <param name="zone"></param>
        public ZoneNode(Zone zone)
            : base(zone.Name)
        {
            Zone = zone;
            Text = Zone.Name;
            ToolTipText = "Phase: " + Zone.Phase.Name;
        }

        /// <summary>
        ///     Get the Zone element in the ZoneNode.
        /// </summary>
        public Zone Zone { get; }
    }
}
