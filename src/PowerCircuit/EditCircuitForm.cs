// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;
using System;
using System.Resources;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.PowerCircuit.CS
{
    public partial class EditCircuitForm : Form
    {
        private readonly CircuitOperationData m_data;

        public EditCircuitForm(CircuitOperationData data)
        {
            m_data = data;

            InitializeComponent();
            // Add tool tips
            AddToolTips();
        }

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
