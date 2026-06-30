// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Creators;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms
{
    public partial class MainForm : Form
    {
        private readonly BindingSource m_binding;

        private readonly CreationMgr m_creationMgr;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(CreationMgr mgr) : this()
        {
            m_creationMgr = mgr;
            m_binding = new BindingSource();
        }

        /// <summary>
        ///     Show a form to fetch edges for hosted-sweep creation, and then create
        ///     the hosted-sweep.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var creator =
                comboBoxHostedSweepType.SelectedItem as HostedSweepCreator;

            var creationData = new CreationData(creator);

            using (var createForm = new EdgeFetchForm(creationData))
            {
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    creator.Create(creationData);
                    RefreshListBox();
                }
            }
        }

        private void buttonModify_Click(object sender, EventArgs e)
        {
            var modificationData = listBoxCreatedHostedSweeps.SelectedItem as ModificationData;

            using (var modifyForm = new HostedSweepModifyForm(modificationData))
            {
                modifyForm.ShowDialog();
            }
        }

        private void RefreshListBox()
        {
            var creator =
                comboBoxHostedSweepType.SelectedItem as HostedSweepCreator;
            m_binding.DataSource = creator.CreatedHostedSweeps;
            listBoxCreatedHostedSweeps.DataSource = m_binding;
            listBoxCreatedHostedSweeps.DisplayMember = "Name";
            m_binding.ResetBindings(false);
        }

        private void InitializeComboBox()
        {
            comboBoxHostedSweepType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxHostedSweepType.Items.Add(m_creationMgr.FasciaCreator);
            comboBoxHostedSweepType.Items.Add(m_creationMgr.GutterCreator);
            comboBoxHostedSweepType.Items.Add(m_creationMgr.SlabEdgeCreator);
            comboBoxHostedSweepType.SelectedIndex = 0;
            comboBoxHostedSweepType.DisplayMember = "Name";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeComboBox();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listBoxHostedSweeps_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxCreatedHostedSweeps.SelectedItem != null)
                buttonModify.Enabled = true;
            else
                buttonModify.Enabled = false;
        }

        private void comboBoxHostedSweepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListBox();
        }
    }
}
