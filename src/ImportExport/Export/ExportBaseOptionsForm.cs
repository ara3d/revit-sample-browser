// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public partial class ExportBaseOptionsForm : Form
    {
        private readonly bool m_contain3DView;

        private readonly ExportBaseOptionsData m_exportOptionsData;

        public ExportBaseOptionsForm(ExportBaseOptionsData exportOptionsData, bool contain3DView, string exportFormat)
        {
            InitializeComponent();
            m_exportOptionsData = exportOptionsData;
            m_contain3DView = contain3DView;
            Text = $"Export {exportFormat} Options";
            InitializeControl();
        }

        private void InitializeControl()
        {
            comboBoxLayersAndProperties.DataSource = m_exportOptionsData.LayersAndProperties;
            comboBoxLayersAndProperties.SelectedIndex = 0;
            comboBoxLayerSettings.DataSource = m_exportOptionsData.LayerMapping;
            comboBoxLayerSettings.SelectedIndex = 0;
            comboBoxLinetypeScaling.DataSource = m_exportOptionsData.LineScaling;
            comboBoxLinetypeScaling.SelectedIndex = 2;
            comboBoxCoorSystem.DataSource = m_exportOptionsData.CoorSystem;
            comboBoxCoorSystem.SelectedIndex = 0;
            comboBoxDWGUnit.DataSource = m_exportOptionsData.Units;
            comboBoxDWGUnit.SelectedIndex = 1;
            comboBoxSolids.DataSource = m_exportOptionsData.Solids;
            comboBoxSolids.SelectedIndex = 0;
            checkBoxMergeViews.Checked = m_exportOptionsData.ExportMergeFiles;
            checkBoxMergeViews.Text = "Merge all views in one file (via XRefs).";
            if (m_contain3DView)
                comboBoxSolids.Enabled = true;
            else
                comboBoxSolids.Enabled = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_exportOptionsData.ExportLayersAndProperties =
                m_exportOptionsData.EnumLayersAndProperties[comboBoxLayersAndProperties.SelectedIndex];
            m_exportOptionsData.ExportLayerMapping =
                m_exportOptionsData.EnumLayerMapping[comboBoxLayerSettings.SelectedIndex];
            m_exportOptionsData.ExportLineScaling =
                m_exportOptionsData.EnumLineScaling[comboBoxLinetypeScaling.SelectedIndex];
            m_exportOptionsData.ExportCoorSystem =
                m_exportOptionsData.EnumCoorSystem[comboBoxCoorSystem.SelectedIndex];
            m_exportOptionsData.ExportUnit = m_exportOptionsData.EnumUnits[comboBoxDWGUnit.SelectedIndex];
            m_exportOptionsData.ExportSolid = m_exportOptionsData.EnumSolids[comboBoxSolids.SelectedIndex];
            m_exportOptionsData.ExportAreas = checkBoxExportingAreas.Checked;
            m_exportOptionsData.ExportMergeFiles = checkBoxMergeViews.Checked;

            Close();
        }
    }
}
