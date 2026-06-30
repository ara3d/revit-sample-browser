// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Mechanical;
using System.Windows.Forms;

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
