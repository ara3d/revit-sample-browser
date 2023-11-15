// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The SpaceNode class inherit TreeNode Class, it is used
    ///     to display the Spaces is a TreeView, each SpaceNode contains
    ///     a Space element.
    /// </summary>
    internal class SpaceNode : TreeNode
    {
        /// <summary>
        ///     The constructor of SpaceNode class
        /// </summary>
        /// <param name="space"></param>
        public SpaceNode(Space space)
            : base(space.Name)
        {
            Space = space;
            Text = space.Name;
        }

        /// <summary>
        ///     Get the Zone element in the ZoneNode.
        /// </summary>
        public Space Space { get; }
    }
}
