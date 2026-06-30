// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Mechanical;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AddSpaceAndZone.CS
{
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

        public Space Space { get; }
    }
}
