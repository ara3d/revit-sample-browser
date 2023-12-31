// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    /// <summary>
    ///     all the supported Align type for the curtain grid
    /// </summary>
    public enum CurtainGridAlign
    {
        Beginning,
        Center,
        End
    }

    /// <summary>
    ///     stores all the properties of the curtain grid and manages the behaviors and operations of the curtain grid
    /// </summary>
    public class GridProperties
    {
        // stores the data of vertical justification

        // stores the data of vertical angle

        // stores the data of vertical offset

        // stores how many vertical lines there are in the grid

        // stores the data of horizontal justification

        // stores the data of horizontal angle

        // stores the data of horizontal offset

        // stores how many horizontal lines there are in the grid

        // stores how many panels there are in the grid

        // stores how many curtain cells there are in the grid

        // stores how many unlocked panels there are in the grid

        // stores how many mullions there are in the grid

        // stores how many unlocked mullions there are in the grid

        /// <summary>
        ///     stores the data of vertical justification
        /// </summary>
        [Category("Vertical Grid Pattern")]
        [DefaultValue(CurtainGridAlign.Beginning)]
        [ReadOnly(true)]
        public CurtainGridAlign VerticalJustification { get; set; }

        /// <summary>
        ///     stores the data of vertical angle
        /// </summary>
        [Category("Vertical Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double VerticalAngle { get; set; }

        /// <summary>
        ///     stores the data of vertical offset
        /// </summary>
        [Category("Vertical Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double VerticalOffset { get; set; }

        /// <summary>
        ///     stores how many U lines there are in the grid
        /// </summary>
        [Category("Vertical Grid Pattern")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int VerticalLinesNumber { get; set; }

        /// <summary>
        ///     stores the data of horizontal justification
        /// </summary>
        [Category("Horizontal Grid Pattern")]
        [DefaultValue(CurtainGridAlign.Beginning)]
        [ReadOnly(true)]
        public CurtainGridAlign HorizontalJustification { get; set; }

        /// <summary>
        ///     stores the data of horizontal angle
        /// </summary>
        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double HorizontalAngle { get; set; }

        /// <summary>
        ///     stores the data of horizontal offset
        /// </summary>
        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0.0)]
        [ReadOnly(true)]
        public double HorizontalOffset { get; set; }

        /// <summary>
        ///     stores how many V lines there are in the grid
        /// </summary>
        [Category("Horizontal Grid Pattern")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int HorizontalLinesNumber { get; set; }

        /// <summary>
        ///     stores how many panels there are in the grid
        /// </summary>
        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int PanelNumber { get; set; }

        /// <summary>
        ///     stores how many curtain cells there are in the grid
        /// </summary>
        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int CellNumber { get; set; }

        /// <summary>
        ///     stores how many unlocked panels there are in the grid
        /// </summary>
        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int UnlockedPanelsNumber { get; set; }

        /// <summary>
        ///     stores how many mullions there are in the grid
        /// </summary>
        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int MullionsNumber { get; set; }

        /// <summary>
        ///     stores how many unlocked mullions there are in the grid
        /// </summary>
        [Category("Other Data")]
        [DefaultValue(0)]
        [ReadOnly(true)]
        public int UnlockedmullionsNumber { get; set; }
    } // end of class
}
