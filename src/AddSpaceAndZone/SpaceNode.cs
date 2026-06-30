// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    public class SpaceNode : TreeNode
    {
        public SpaceNode(Space space)
            : base(space.Name)
        {
            Space = space;
            Text = space.Name;
        }

        public Space Space { get; }
    }
}
