// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

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
