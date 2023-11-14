//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.ComponentModel;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
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
        [CategoryAttribute("Vertical Grid Pattern")]
        [DefaultValueAttribute(CurtainGridAlign.Beginning)]
        [ReadOnlyAttribute(true)]
        public CurtainGridAlign VerticalJustification { get; set; }

        /// <summary>
        ///     stores the data of vertical angle
        /// </summary>
        [CategoryAttribute("Vertical Grid Pattern")]
        [DefaultValueAttribute(0.0)]
        [ReadOnlyAttribute(true)]
        public double VerticalAngle { get; set; }

        /// <summary>
        ///     stores the data of vertical offset
        /// </summary>
        [CategoryAttribute("Vertical Grid Pattern")]
        [DefaultValueAttribute(0.0)]
        [ReadOnlyAttribute(true)]
        public double VerticalOffset { get; set; }

        /// <summary>
        ///     stores how many U lines there are in the grid
        /// </summary>
        [CategoryAttribute("Vertical Grid Pattern")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int VerticalLinesNumber { get; set; }

        /// <summary>
        ///     stores the data of horizontal justification
        /// </summary>
        [CategoryAttribute("Horizontal Grid Pattern")]
        [DefaultValueAttribute(CurtainGridAlign.Beginning)]
        [ReadOnlyAttribute(true)]
        public CurtainGridAlign HorizontalJustification { get; set; }

        /// <summary>
        ///     stores the data of horizontal angle
        /// </summary>
        [CategoryAttribute("Horizontal Grid Pattern")]
        [DefaultValueAttribute(0.0)]
        [ReadOnlyAttribute(true)]
        public double HorizontalAngle { get; set; }

        /// <summary>
        ///     stores the data of horizontal offset
        /// </summary>
        [CategoryAttribute("Horizontal Grid Pattern")]
        [DefaultValueAttribute(0.0)]
        [ReadOnlyAttribute(true)]
        public double HorizontalOffset { get; set; }

        /// <summary>
        ///     stores how many V lines there are in the grid
        /// </summary>
        [CategoryAttribute("Horizontal Grid Pattern")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int HorizontalLinesNumber { get; set; }

        /// <summary>
        ///     stores how many panels there are in the grid
        /// </summary>
        [CategoryAttribute("Other Data")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int PanelNumber { get; set; }

        /// <summary>
        ///     stores how many curtain cells there are in the grid
        /// </summary>
        [CategoryAttribute("Other Data")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int CellNumber { get; set; }

        /// <summary>
        ///     stores how many unlocked panels there are in the grid
        /// </summary>
        [CategoryAttribute("Other Data")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int UnlockedPanelsNumber { get; set; }

        /// <summary>
        ///     stores how many mullions there are in the grid
        /// </summary>
        [CategoryAttribute("Other Data")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int MullionsNumber { get; set; }

        /// <summary>
        ///     stores how many unlocked mullions there are in the grid
        /// </summary>
        [CategoryAttribute("Other Data")]
        [DefaultValueAttribute(0)]
        [ReadOnlyAttribute(true)]
        public int UnlockedmullionsNumber { get; set; }
    } // end of class
}