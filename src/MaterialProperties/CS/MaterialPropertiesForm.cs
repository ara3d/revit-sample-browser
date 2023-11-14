// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace Revit.SDK.Samples.MaterialProperties.CS
{
    /// <summary>
    ///     Summary description for MaterialPropFrm.
    /// </summary>
    public class MaterialPropertiesForm : Form
    {
        private Button m_applyButton;
        private Button m_cancelButton;
        private Button m_changeButton;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container m_components = null;

        private readonly MaterialProperties m_dataBuffer;
        private Button m_okButton;
        private DataGrid m_parameterDataGrid;
        private ComboBox m_subTypeComboBox;
        private ComboBox m_typeComboBox;
        private Label m_typeLable;

        private MaterialPropertiesForm()
        {
        }

        /// <summary>
        ///     material properties from
        /// </summary>
        /// <param name="dataBuffer">material properties from Revit</param>
        public MaterialPropertiesForm(MaterialProperties dataBuffer)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            m_parameterDataGrid.PreferredColumnWidth = m_parameterDataGrid.Width / 2 - 2;

            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            m_typeLable = new Label();
            m_typeComboBox = new ComboBox();
            m_subTypeComboBox = new ComboBox();
            m_parameterDataGrid = new DataGrid();
            m_okButton = new Button();
            m_cancelButton = new Button();
            m_applyButton = new Button();
            m_changeButton = new Button();
            ((ISupportInitialize)m_parameterDataGrid).BeginInit();
            SuspendLayout();
            // 
            // typeLable
            // 
            m_typeLable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_typeLable.Location = new Point(24, 16);
            m_typeLable.Name = "m_typeLable";
            m_typeLable.Size = new Size(80, 23);
            m_typeLable.TabIndex = 0;
            m_typeLable.Text = "Material Type:";
            // 
            // typeComboBox
            // 
            m_typeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_typeComboBox.Location = new Point(112, 16);
            m_typeComboBox.Name = "m_typeComboBox";
            m_typeComboBox.Size = new Size(264, 21);
            m_typeComboBox.TabIndex = 2;
            m_typeComboBox.SelectedIndexChanged += typeComboBox_SelectedIndexChanged;
            // 
            // subTypeComboBox
            // 
            m_subTypeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_subTypeComboBox.Location = new Point(112, 48);
            m_subTypeComboBox.MaxDropDownItems = 30;
            m_subTypeComboBox.Name = "m_subTypeComboBox";
            m_subTypeComboBox.Size = new Size(264, 21);
            m_subTypeComboBox.TabIndex = 3;
            m_subTypeComboBox.SelectedIndexChanged += subTypeComboBox_SelectedIndexChanged;
            // 
            // parameterDataGrid
            // 
            m_parameterDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            m_parameterDataGrid.CaptionVisible = false;
            m_parameterDataGrid.DataMember = "";
            m_parameterDataGrid.HeaderForeColor = SystemColors.ControlText;
            m_parameterDataGrid.Location = new Point(16, 88);
            m_parameterDataGrid.Name = "m_parameterDataGrid";
            m_parameterDataGrid.ReadOnly = true;
            m_parameterDataGrid.RowHeadersVisible = false;
            m_parameterDataGrid.Size = new Size(480, 380);
            m_parameterDataGrid.TabIndex = 4;
            // 
            // okButton
            // 
            m_okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_okButton.Location = new Point(104, 480);
            m_okButton.Name = "m_okButton";
            m_okButton.Size = new Size(75, 23);
            m_okButton.TabIndex = 5;
            m_okButton.Text = "&OK";
            m_okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            m_cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_cancelButton.Location = new Point(192, 480);
            m_cancelButton.Name = "m_cancelButton";
            m_cancelButton.Size = new Size(75, 23);
            m_cancelButton.TabIndex = 6;
            m_cancelButton.Text = "&Cancel";
            m_cancelButton.Click += cancelButton_Click;
            // 
            // applyButton
            // 
            m_applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_applyButton.Location = new Point(280, 480);
            m_applyButton.Name = "m_applyButton";
            m_applyButton.Size = new Size(75, 23);
            m_applyButton.TabIndex = 7;
            m_applyButton.Text = "&Apply";
            m_applyButton.Click += applyButton_Click;
            // 
            // changeButton
            // 
            m_changeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            m_changeButton.Location = new Point(368, 480);
            m_changeButton.Name = "m_changeButton";
            m_changeButton.Size = new Size(128, 23);
            m_changeButton.TabIndex = 8;
            m_changeButton.Text = "Change &Unit Weight";
            m_changeButton.Click += changeButton_Click;
            // 
            // MaterialPropertiesForm
            // 
            AcceptButton = m_okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = m_cancelButton;
            ClientSize = new Size(512, 512);
            Controls.Add(m_changeButton);
            Controls.Add(m_applyButton);
            Controls.Add(m_cancelButton);
            Controls.Add(m_okButton);
            Controls.Add(m_parameterDataGrid);
            Controls.Add(m_subTypeComboBox);
            Controls.Add(m_typeComboBox);
            Controls.Add(m_typeLable);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MaterialPropertiesForm";
            ShowInTaskbar = false;
            Text = "Material Properties";
            Load += MaterialPropFrm_Load;
            ((ISupportInitialize)m_parameterDataGrid).EndInit();
            ResumeLayout(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialPropFrm_Load(object sender, EventArgs e)
        {
            LoadCurrentMaterial();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((StructuralAssetClass)m_typeComboBox.SelectedIndex)
            {
                case StructuralAssetClass.Metal:
                    m_applyButton.Enabled = true;
                    m_changeButton.Enabled = true;
                    m_subTypeComboBox.Enabled = true;
                    m_subTypeComboBox.DataSource = m_dataBuffer.SteelCollection;
                    m_subTypeComboBox.DisplayMember = "MaterialName";
                    m_subTypeComboBox.ValueMember = "Material";
                    m_parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(m_subTypeComboBox.SelectedValue,
                        (StructuralAssetClass)m_typeComboBox.SelectedIndex);
                    break;
                case StructuralAssetClass.Concrete:
                    m_applyButton.Enabled = true;
                    m_changeButton.Enabled = true;
                    m_subTypeComboBox.Enabled = true;
                    m_subTypeComboBox.DataSource = m_dataBuffer.ConcreteCollection;
                    m_subTypeComboBox.DisplayMember = "MaterialName";
                    m_subTypeComboBox.ValueMember = "Material";
                    m_parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(m_subTypeComboBox.SelectedValue,
                        (StructuralAssetClass)m_typeComboBox.SelectedIndex);
                    break;
                default:
                    m_applyButton.Enabled = false;
                    m_changeButton.Enabled = false;
                    m_subTypeComboBox.DataSource = new ArrayList();
                    m_subTypeComboBox.Enabled = false;
                    m_parameterDataGrid.DataSource = new DataTable();
                    break;
            }
        }

        /// <summary>
        ///     change the content in datagrid according to selected material type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != m_subTypeComboBox.SelectedValue) m_dataBuffer.UpdateMaterial(m_subTypeComboBox.SelectedValue);

            m_parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(m_subTypeComboBox.SelectedValue,
                (StructuralAssetClass)m_typeComboBox.SelectedIndex);
        }

        /// <summary>
        ///     close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     set selected element's material to current selection and close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (null != m_subTypeComboBox.SelectedValue)
            {
                m_dataBuffer.UpdateMaterial(m_subTypeComboBox.SelectedValue);
                m_dataBuffer.SetMaterial();
            }

            Close();
        }

        /// <summary>
        ///     set selected element's material to current selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyButton_Click(object sender, EventArgs e)
        {
            if (null != m_subTypeComboBox.SelectedValue)
            {
                m_dataBuffer.UpdateMaterial(m_subTypeComboBox.SelectedValue);
                m_dataBuffer.SetMaterial();
            }
        }

        /// <summary>
        ///     change unit weight all instances of the elements that use this material
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeButton_Click(object sender, EventArgs e)
        {
            TaskDialog.Show("Revit",
                "This will change the unit weight of all instances that use this material in current document.");

            if (!m_dataBuffer.ChangeUnitWeight())
            {
                TaskDialog.Show("Revit", "Failed to change the unit weight.");
                return;
            }

            LoadCurrentMaterial();
        }

        /// <summary>
        ///     update display data to selected element's material
        /// </summary>
        private void LoadCurrentMaterial()
        {
            m_typeComboBox.DataSource = m_dataBuffer.MaterialTypes;

            m_typeComboBox.SelectedIndex = (int)m_dataBuffer.CurrentType;

            if (null == m_dataBuffer.CurrentMaterial || (m_dataBuffer.CurrentType != StructuralAssetClass.Metal
                                                         && m_dataBuffer.CurrentType != StructuralAssetClass.Concrete))
                return;
            if (!(m_dataBuffer.CurrentMaterial is Material tmp))
                return;

            m_subTypeComboBox.SelectedValue = tmp;
            m_parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(m_subTypeComboBox.SelectedValue,
                (StructuralAssetClass)m_typeComboBox.SelectedIndex);
        }
    }
}
