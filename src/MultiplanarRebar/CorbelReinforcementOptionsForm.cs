// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.MultiplanarRebar.CS
{
    public partial class CorbelReinforcementOptionsForm : Form
    {
        private readonly CorbelReinforcementOptions m_corbelReinforcementOptions;

        public CorbelReinforcementOptionsForm(CorbelReinforcementOptions options)
        {
            m_corbelReinforcementOptions = options;

            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            var bartypes4 = new List<RebarBarType>(m_corbelReinforcementOptions.RebarBarTypes);
            columnBarTypeComboBox.DataSource = bartypes4;
            columnBarTypeComboBox.ValueMember = "Name";

            var bartypes1 = new List<RebarBarType>(m_corbelReinforcementOptions.RebarBarTypes);
            topBarTypeComboBox.DataSource = bartypes1;
            topBarTypeComboBox.ValueMember = "Name";

            var bartypes2 = new List<RebarBarType>(m_corbelReinforcementOptions.RebarBarTypes);
            stirrupBarTypeComboBox.DataSource = bartypes2;
            stirrupBarTypeComboBox.ValueMember = "Name";

            var bartypes3 = new List<RebarBarType>(m_corbelReinforcementOptions.RebarBarTypes);
            multiplanarBarTypeComboBox.DataSource = bartypes3;
            multiplanarBarTypeComboBox.ValueMember = "Name";

            topBarCountTextBox.Text = "3";
            stirrupBarCountTextBox.Text = "3";
        }

        private void topBarTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_corbelReinforcementOptions.TopBarType = topBarTypeComboBox.SelectedItem as RebarBarType;
        }

        private void stirrupBarTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_corbelReinforcementOptions.StirrupBarType = stirrupBarTypeComboBox.SelectedItem as RebarBarType;
        }

        private void multiplanarBarTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_corbelReinforcementOptions.MultiplanarBarType = multiplanarBarTypeComboBox.SelectedItem as RebarBarType;
        }

        private void columnBarTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_corbelReinforcementOptions.HostStraightBarType = columnBarTypeComboBox.SelectedItem as RebarBarType;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_corbelReinforcementOptions.TopBarCount = int.Parse(topBarCountTextBox.Text);
            m_corbelReinforcementOptions.StirrupBarCount = int.Parse(stirrupBarCountTextBox.Text);

            DialogResult = DialogResult.OK;
        }

        private void topBarCountTextBox_Validating(object sender, CancelEventArgs e)
        {
            var count = 0;
            if (!int.TryParse(topBarCountTextBox.Text, out count) || count < 2) e.Cancel = true;
        }

        private void stirrupBarCountTextBox_Validating(object sender, CancelEventArgs e)
        {
            var count = 0;
            if (!int.TryParse(stirrupBarCountTextBox.Text, out count) || count < 2) e.Cancel = true;
        }
    }
}
