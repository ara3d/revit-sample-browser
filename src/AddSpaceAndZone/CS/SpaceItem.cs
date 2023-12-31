// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
    /// <summary>
    ///     The SpaceItem class inherit ListViewItem Class, it is used
    ///     to display the Spaces is a ListView, each SpaceItem contains
    ///     a Space element.
    /// </summary>
    public class SpaceItem : ListViewItem
    {
        public SpaceItem(Space space) : base(space.Name)
        {
            Space = space;
            Text = space.Name;
            if (space.Zone != null)
                SubItems.Add(space.Zone.Name);
            else
                SubItems.Add("Default");
        }

        /// <summary>
        ///     Get the Space element in the SpaceItem.
        /// </summary>
        public Space Space { get; }
    }
}
