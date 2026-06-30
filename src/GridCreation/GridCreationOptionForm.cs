// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public partial class GridCreationOptionForm : Form
    {
        private readonly GridCreationOptionData m_gridCreationOption;

        public GridCreationOptionForm(GridCreationOptionData opt)
        {
            m_gridCreationOption = opt;

            InitializeComponent();
            InitializeControls();
        }

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
            SetData();
        }

        private void SetData()
        {
            m_gridCreationOption.CreateGridsMode = radioButtonSelect.Checked ? CreateMode.Select :
                radioButtonOrthogonalGrids.Checked ? CreateMode.Orthogonal : CreateMode.RadialAndArc;
        }
    }
}
