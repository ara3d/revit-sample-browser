// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.EnergyAnalysisModel.CS
{
    public partial class OptionsAndAnalysisForm : Form
    {
        private readonly EnergyAnalysisModel m_model;

        public OptionsAndAnalysisForm(EnergyAnalysisModel analysisModel)
        {
            m_model = analysisModel;
            InitializeComponent();
            InitializeOptionsUi();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            m_model.SetTier(comboBoxTier.SelectedText);
            m_model.Options.ExportMullions = checkBoxExportMullions.Checked;
            m_model.Options.IncludeShadingSurfaces = checkBoxIncludeShadingSurfaces.Checked;
            m_model.Options.SimplifyCurtainSystems = checkBoxSimplifyCurtainSystems.Checked;

            m_model.Initialize();
            m_model.RefreshAnalysisData(treeViewAnalyticalData);

            treeViewAnalyticalData.ExpandAll();
            Refresh();
        }

        // Index 3 corresponds to SecondLevelBoundaries in comboBoxTier.
        private void InitializeOptionsUi()
        {
            comboBoxTier.SelectedIndex = 3;
        }
    }
}
