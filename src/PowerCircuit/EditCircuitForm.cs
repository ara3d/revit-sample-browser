// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    /// <summary>
    ///     The dialog which provides the options of editing circuit
    /// </summary>
    public partial class EditCircuitForm : Form
    {
        /// <summary>
        ///     data class object
        /// </summary>
        private readonly CircuitOperationData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public EditCircuitForm(CircuitOperationData data)
        {
            m_data = data;

            InitializeComponent();
            // Add tool tips
            AddToolTips();
        }

        /// <summary>
        ///     Add tool tips
        /// </summary>
        private void AddToolTips()
        {
            var rsm = Resources.ResourceManager;
            toolTip.SetToolTip(buttonAdd, rsm.GetString("tipAddToCircuit"));
            toolTip.SetToolTip(buttonRemove, rsm.GetString("tipRemoveFromCircuit"));
            toolTip.SetToolTip(buttonSelectPanel, rsm.GetString("tipSelectPanel"));
            toolTip.SetToolTip(buttonCancel, "tipCancel");
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            m_data.EditOption = EditOption.Add;
            Close();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            m_data.EditOption = EditOption.Remove;
            Close();
        }

        private void buttonSelectPanel_Click(object sender, EventArgs e)
        {
            m_data.EditOption = EditOption.SelectPanel;
            Close();
        }
    }
}
