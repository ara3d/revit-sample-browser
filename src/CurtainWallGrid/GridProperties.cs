// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    public enum CurtainGridAlign
    {
        Beginning,
        Center,
        End
    }

    public class GridProperties
    {


        // stores how many vertical lines there are in the grid


        // stores how many horizontal lines there are in the grid

        // stores how many panels there are in the grid

        // stores how many curtain cells there are in the grid

        // stores how many unlocked panels there are in the grid

        // stores how many mullions there are in the grid

        // stores how many unlocked mullions there are in the grid

        [Category("Vertical Grid Pattern")]
        [DefaultValue(CurtainGridAlign.Beginning)]
        [ReadOnly(true)]
        public CurtainGridAlign VerticalJustification { get; set; }

        [Category("Vertical Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double VerticalAngle { get; set; }

        [Category("Vertical Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double VerticalOffset { get; set; }

        [Category("Vertical Grid Pattern")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int VerticalLinesNumber { get; set; }

        [Category("Horizontal Grid Pattern")]
        [DefaultValue(CurtainGridAlign.Beginning)]
        [ReadOnly(true)]
        public CurtainGridAlign HorizontalJustification { get; set; }

        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double HorizontalAngle { get; set; }

        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double HorizontalOffset { get; set; }

        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int HorizontalLinesNumber { get; set; }

        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int PanelNumber { get; set; }

        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int CellNumber { get; set; }

        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int UnlockedPanelsNumber { get; set; }

        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int MullionsNumber { get; set; }

        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int UnlockedmullionsNumber { get; set; }
    } // end of class
}
