// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    /// <summary>
    ///     The dialog which lets user operate selected elements
    /// </summary>
    public partial class CircuitOperationForm : Form
    {
        /// <summary>
        ///     Object of data class
        /// </summary>
        private readonly CircuitOperationData m_optionData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="optionData"></param>
        public CircuitOperationForm(CircuitOperationData optionData)
        {
            m_optionData = optionData;
            InitializeComponent();
            InitializeButtons();
            // Add tool tips
            AddToolTips();
        }

        /// <summary>
        ///     Initialize buttons
        /// </summary>
        private void InitializeButtons()
        {
            // Set enabled status
            buttonCreate.Enabled = m_optionData.CanCreateCircuit;
            buttonEdit.Enabled = m_optionData.HasCircuit;
            buttonSelectPanel.Enabled = m_optionData.HasCircuit;
            buttonDisconnectPanel.Enabled = m_optionData.HasPanel;
        }

        /// <summary>
        ///     Add tool tips
        /// </summary>
        private void AddToolTips()
        {
            var rsm = Resources.ResourceManager;
            toolTip.SetToolTip(buttonCreate, rsm.GetString("tipCreateCircuit"));
            toolTip.SetToolTip(buttonEdit, rsm.GetString("tipEditCircuit"));
            toolTip.SetToolTip(buttonSelectPanel, rsm.GetString("tipSelectPanel"));
            toolTip.SetToolTip(buttonDisconnectPanel, rsm.GetString("tipDisconnectPanel"));
            toolTip.SetToolTip(buttonCancel, rsm.GetString("tipCancel"));
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            m_optionData.Operation = Operation.CreateCircuit;
            Close();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            m_optionData.Operation = Operation.EditCircuit;
            Close();
        }

        private void buttonSelectPanel_Click(object sender, EventArgs e)
        {
            m_optionData.Operation = Operation.SelectPanel;
            Close();
        }

        private void buttonDisconnectPanel_Click(object sender, EventArgs e)
        {
            m_optionData.Operation = Operation.DisconnectPanel;
            Close();
        }
    }
}
