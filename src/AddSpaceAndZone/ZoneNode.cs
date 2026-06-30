// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Mechanical;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public class ZoneNode : TreeNode
    {
        public ZoneNode(Zone zone)
            : base(zone.Name)
        {
            Zone = zone;
            Text = Zone.Name;
            ToolTipText = $"Phase: {Zone.Phase.Name}";
        }

        public Zone Zone { get; }
    }
}
