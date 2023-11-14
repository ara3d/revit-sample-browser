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
        private Button applyButton;
        private Button cancelButton;
        private Button changeButton;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private readonly MaterialProperties m_dataBuffer;
        private Button okButton;
        private DataGrid parameterDataGrid;
        private ComboBox subTypeComboBox;
        private ComboBox typeComboBox;
        private Label typeLable;

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
            parameterDataGrid.PreferredColumnWidth = parameterDataGrid.Width / 2 - 2;

            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            typeLable = new Label();
            typeComboBox = new ComboBox();
            subTypeComboBox = new ComboBox();
            parameterDataGrid = new DataGrid();
            okButton = new Button();
            cancelButton = new Button();
            applyButton = new Button();
            changeButton = new Button();
            ((ISupportInitialize)parameterDataGrid).BeginInit();
            SuspendLayout();
            // 
            // typeLable
            // 
            typeLable.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            typeLable.Location = new Point(24, 16);
            typeLable.Name = "typeLable";
            typeLable.Size = new Size(80, 23);
            typeLable.TabIndex = 0;
            typeLable.Text = "Material Type:";
            // 
            // typeComboBox
            // 
            typeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            typeComboBox.Location = new Point(112, 16);
            typeComboBox.Name = "typeComboBox";
            typeComboBox.Size = new Size(264, 21);
            typeComboBox.TabIndex = 2;
            typeComboBox.SelectedIndexChanged += typeComboBox_SelectedIndexChanged;
            // 
            // subTypeComboBox
            // 
            subTypeComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            subTypeComboBox.Location = new Point(112, 48);
            subTypeComboBox.MaxDropDownItems = 30;
            subTypeComboBox.Name = "subTypeComboBox";
            subTypeComboBox.Size = new Size(264, 21);
            subTypeComboBox.TabIndex = 3;
            subTypeComboBox.SelectedIndexChanged += subTypeComboBox_SelectedIndexChanged;
            // 
            // parameterDataGrid
            // 
            parameterDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            parameterDataGrid.CaptionVisible = false;
            parameterDataGrid.DataMember = "";
            parameterDataGrid.HeaderForeColor = SystemColors.ControlText;
            parameterDataGrid.Location = new Point(16, 88);
            parameterDataGrid.Name = "parameterDataGrid";
            parameterDataGrid.ReadOnly = true;
            parameterDataGrid.RowHeadersVisible = false;
            parameterDataGrid.Size = new Size(480, 380);
            parameterDataGrid.TabIndex = 4;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.Location = new Point(104, 480);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 5;
            okButton.Text = "&OK";
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Location = new Point(192, 480);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "&Cancel";
            cancelButton.Click += cancelButton_Click;
            // 
            // applyButton
            // 
            applyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            applyButton.Location = new Point(280, 480);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(75, 23);
            applyButton.TabIndex = 7;
            applyButton.Text = "&Apply";
            applyButton.Click += applyButton_Click;
            // 
            // changeButton
            // 
            changeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            changeButton.Location = new Point(368, 480);
            changeButton.Name = "changeButton";
            changeButton.Size = new Size(128, 23);
            changeButton.TabIndex = 8;
            changeButton.Text = "Change &Unit Weight";
            changeButton.Click += changeButton_Click;
            // 
            // MaterialPropertiesForm
            // 
            AcceptButton = okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = cancelButton;
            ClientSize = new Size(512, 512);
            Controls.Add(changeButton);
            Controls.Add(applyButton);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(parameterDataGrid);
            Controls.Add(subTypeComboBox);
            Controls.Add(typeComboBox);
            Controls.Add(typeLable);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MaterialPropertiesForm";
            ShowInTaskbar = false;
            Text = "Material Properties";
            Load += MaterialPropFrm_Load;
            ((ISupportInitialize)parameterDataGrid).EndInit();
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
            switch ((StructuralAssetClass)typeComboBox.SelectedIndex)
            {
                case StructuralAssetClass.Metal:
                    applyButton.Enabled = true;
                    changeButton.Enabled = true;
                    subTypeComboBox.Enabled = true;
                    subTypeComboBox.DataSource = m_dataBuffer.SteelCollection;
                    subTypeComboBox.DisplayMember = "MaterialName";
                    subTypeComboBox.ValueMember = "Material";
                    parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(subTypeComboBox.SelectedValue,
                        (StructuralAssetClass)typeComboBox.SelectedIndex);
                    break;
                case StructuralAssetClass.Concrete:
                    applyButton.Enabled = true;
                    changeButton.Enabled = true;
                    subTypeComboBox.Enabled = true;
                    subTypeComboBox.DataSource = m_dataBuffer.ConcreteCollection;
                    subTypeComboBox.DisplayMember = "MaterialName";
                    subTypeComboBox.ValueMember = "Material";
                    parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(subTypeComboBox.SelectedValue,
                        (StructuralAssetClass)typeComboBox.SelectedIndex);
                    break;
                default:
                    applyButton.Enabled = false;
                    changeButton.Enabled = false;
                    subTypeComboBox.DataSource = new ArrayList();
                    subTypeComboBox.Enabled = false;
                    parameterDataGrid.DataSource = new DataTable();
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
            if (null != subTypeComboBox.SelectedValue) m_dataBuffer.UpdateMaterial(subTypeComboBox.SelectedValue);

            parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(subTypeComboBox.SelectedValue,
                (StructuralAssetClass)typeComboBox.SelectedIndex);
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
            if (null != subTypeComboBox.SelectedValue)
            {
                m_dataBuffer.UpdateMaterial(subTypeComboBox.SelectedValue);
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
            if (null != subTypeComboBox.SelectedValue)
            {
                m_dataBuffer.UpdateMaterial(subTypeComboBox.SelectedValue);
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
            typeComboBox.DataSource = m_dataBuffer.MaterialTypes;

            typeComboBox.SelectedIndex = (int)m_dataBuffer.CurrentType;

            if (null == m_dataBuffer.CurrentMaterial || (m_dataBuffer.CurrentType != StructuralAssetClass.Metal
                                                         && m_dataBuffer.CurrentType != StructuralAssetClass.Concrete))
                return;
            if (!(m_dataBuffer.CurrentMaterial is Material tmp))
                return;

            subTypeComboBox.SelectedValue = tmp;
            parameterDataGrid.DataSource = m_dataBuffer.GetParameterTable(subTypeComboBox.SelectedValue,
                (StructuralAssetClass)typeComboBox.SelectedIndex);
        }
    }
}
