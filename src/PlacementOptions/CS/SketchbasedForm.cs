// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.PlacementOptions.CS
{
    /// <summary>
    ///     The dialog for setting the SketchGalleryOptions option of the face based family instance.
    /// </summary>
    public partial class SketchbasedForm : Form
    {
        private readonly List<FamilySymbol> m_familySymbolList;

        /// <summary>
        ///     Constructor
        /// </summary>
        public SketchbasedForm(List<FamilySymbol> symbolList)
        {
            InitializeComponent();

            FiPlacementOptions = new PromptForFamilyInstancePlacementOptions();
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_Line;

            m_familySymbolList = symbolList;
            var nameList = new List<string>();
            foreach (var symbol in m_familySymbolList)
            {
                nameList.Add(symbol.Name);
            }

            comboBoxFamilySymbol.DataSource = nameList;
            comboBoxFamilySymbol.SelectedIndex = 0;
            SelectedFamilySymbol = m_familySymbolList[comboBoxFamilySymbol.SelectedIndex];
        }

        /// <summary>
        ///     The family instance placement options for placement.
        /// </summary>
        public PromptForFamilyInstancePlacementOptions FiPlacementOptions { get; }

        /// <summary>
        ///     The family symbol for placement.
        /// </summary>
        public FamilySymbol SelectedFamilySymbol { get; private set; }

        /// <summary>
        ///     Select the family symbol for family instance placement.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void comboBoxFamilySymbol_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFamilySymbol = m_familySymbolList[comboBoxFamilySymbol.SelectedIndex];
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_Line option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonLine_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_Line;
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_Arc3Point option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonArc3P_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_Arc3Point;
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_ArcCenterEnds option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonArcC_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_ArcCenterEnds;
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_Spline option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonSpline_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_Spline;
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_PartialEllipse option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonPEllipse_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_PartialEllipse;
        }

        /// <summary>
        ///     Use the SketchGalleryOptions.SGO_PickLines option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonPickLine_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.SketchGalleryOptions = SketchGalleryOptions.SGO_PickLines;
        }
    }
}
