// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    public partial class SelectCircuitForm : Form
    {
        private readonly CircuitOperationData m_optionData;

        public SelectCircuitForm(CircuitOperationData optionData)
        {
            m_optionData = optionData;

            InitializeComponent();
            InitializeElectricalSystems();
        }

        private void InitializeElectricalSystems()
        {
            listBoxElectricalSystem.DataSource = m_optionData.ElectricalSystemItems;
            listBoxElectricalSystem.DisplayMember = "Name";
        }

        private void listBoxElectricalSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBoxElectricalSystem.SelectedIndex;
            m_optionData.ShowCircuit(index);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var index = listBoxElectricalSystem.SelectedIndex;
            m_optionData.SelectCircuit(index);
        }
    }
}
