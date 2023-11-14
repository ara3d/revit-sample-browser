// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.NewHostedSweep.CS
{
    /// <summary>
    ///     This is the main form. It is the entry to create a new hosted sweep or to modify
    ///     a created hosted sweep.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///     Encapsulates the data source for a form.
        /// </summary>
        private readonly BindingSource m_binding;

        /// <summary>
        ///     Creation manager, which collects all the creators.
        /// </summary>
        private readonly CreationMgr m_creationMgr;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Customize constructor.
        /// </summary>
        /// <param name="mgr"></param>
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

        /// <summary>
        ///     Show a form to modify the created hosted-sweep.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonModify_Click(object sender, EventArgs e)
        {
            var modificationData = listBoxCreatedHostedSweeps.SelectedItem as ModificationData;

            using (var modifyForm = new HostedSweepModifyForm(modificationData))
            {
                modifyForm.ShowDialog();
            }
        }

        /// <summary>
        ///     Refresh list box data source.
        /// </summary>
        private void RefreshListBox()
        {
            var creator =
                comboBoxHostedSweepType.SelectedItem as HostedSweepCreator;
            m_binding.DataSource = creator.CreatedHostedSweeps;
            listBoxCreatedHostedSweeps.DataSource = m_binding;
            listBoxCreatedHostedSweeps.DisplayMember = "Name";
            m_binding.ResetBindings(false);
        }

        /// <summary>
        ///     Initialize combobox data source.
        /// </summary>
        private void InitializeComboBox()
        {
            comboBoxHostedSweepType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxHostedSweepType.Items.Add(m_creationMgr.FasciaCreator);
            comboBoxHostedSweepType.Items.Add(m_creationMgr.GutterCreator);
            comboBoxHostedSweepType.Items.Add(m_creationMgr.SlabEdgeCreator);
            comboBoxHostedSweepType.SelectedIndex = 0;
            comboBoxHostedSweepType.DisplayMember = "Name";
        }

        /// <summary>
        ///     Initialize combo-box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeComboBox();
        }

        /// <summary>
        ///     Close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Update "Modify" button status according to the list-box selection item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxHostedSweeps_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxCreatedHostedSweeps.SelectedItem != null)
                buttonModify.Enabled = true;
            else
                buttonModify.Enabled = false;
        }

        /// <summary>
        ///     Update the list-box data source according to the combobox selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxHostedSweepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshListBox();
        }
    }
}
