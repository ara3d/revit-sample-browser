// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    public partial class HostedSweepModifyForm : Form
    {
        private readonly ModificationData m_modificationData;

        public HostedSweepModifyForm()
        {
            InitializeComponent();
        }

        public HostedSweepModifyForm(ModificationData modificationData)
            : this()
        {
            m_modificationData = modificationData;
            Text = $"Modify {m_modificationData.CreatorName}";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void HostedSweepModify_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = m_modificationData;
            m_modificationData.ShowElement();
        }
    }
}
