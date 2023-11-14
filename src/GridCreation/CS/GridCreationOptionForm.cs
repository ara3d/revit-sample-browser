// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    ///     The dialog which lets user choose the way to create grids
    /// </summary>
    public partial class GridCreationOptionForm : Form
    {
        // data class object
        private readonly GridCreationOptionData m_gridCreationOption;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="opt">Data class object</param>
        public GridCreationOptionForm(GridCreationOptionData opt)
        {
            m_gridCreationOption = opt;

            InitializeComponent();
            // Set state of controls
            InitializeControls();
        }

        /// <summary>
        ///     Set state of controls
        /// </summary>
        private void InitializeControls()
        {
            if (!m_gridCreationOption.HasSelectedLinesOrArcs)
            {
                radioButtonSelect.Enabled = false;
                radioButtonOrthogonalGrids.Checked = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Transfer data back into data class
            SetData();
        }

        /// <summary>
        ///     Transfer data back into data class
        /// </summary>
        private void SetData()
        {
            m_gridCreationOption.CreateGridsMode = radioButtonSelect.Checked ? CreateMode.Select :
                radioButtonOrthogonalGrids.Checked ? CreateMode.Orthogonal : CreateMode.RadialAndArc;
        }
    }
}
