// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.PowerCircuit.CS
{
    /// <summary>
    ///     The dialog which provides the options of selecting and showing circuit
    /// </summary>
    public partial class SelectCircuitForm : Form
    {
        /// <summary>
        ///     Data class object
        /// </summary>
        private readonly CircuitOperationData m_optionData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="optionData">Data class object</param>
        public SelectCircuitForm(CircuitOperationData optionData)
        {
            m_optionData = optionData;

            InitializeComponent();
            InitializeElectricalSystems();
        }

        /// <summary>
        ///     Initialize the list of circuits to display
        /// </summary>
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
