// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.PlacementOptions.CS
{
    /// <summary>
    ///     The dialog for setting the FaceBasedPlacementType option of the face based family instance.
    /// </summary>
    public partial class FacebasedForm : Form
    {
        private readonly List<FamilySymbol> m_familySymbolList;

        /// <summary>
        ///     Constructor
        /// </summary>
        public FacebasedForm(List<FamilySymbol> symbolList)
        {
            InitializeComponent();

            radioButtonFace.Checked = true;
            FiPlacementOptions = new PromptForFamilyInstancePlacementOptions();
            FiPlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnFace;

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
        ///     Use the FaceBasedPlacementType.Default option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonDefault_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.Default;
        }

        /// <summary>
        ///     Use the FaceBasedPlacementType.PlaceOnFace option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonFace_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnFace;
        }

        /// <summary>
        ///     Use the FaceBasedPlacementType.PlaceOnVerticalFace option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonVF_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnVerticalFace;
        }

        /// <summary>
        ///     Use the FaceBasedPlacementType.PlaceOnWorkPlane option or not.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void radioButtonWP_CheckedChanged(object sender, EventArgs e)
        {
            FiPlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnWorkPlane;
        }

        /// <summary>
        ///     Select the family symbol for family instance placement.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arg.</param>
        private void comboBoxFamilySymbol_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedFamilySymbol = m_familySymbolList[comboBoxFamilySymbol.SelectedIndex];
        }
    }
}
