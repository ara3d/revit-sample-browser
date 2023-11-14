// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Forms.TextBox;

namespace RevitMultiSample.RotateFramingObjects.CS
{
    /// <summary>
    ///     Summary description for PutDialog.
    /// </summary>
    public class RotateFramingObjectsForm : Form
    {
        public RadioButton AbsoluteRadio;
        private Button m_cancelButton;
        private readonly Container m_components = null;

        private readonly RotateFramingObjects m_instance;
        private Button m_okButton;
        private RadioButton m_relativeRadio;
        private Label m_rotationLabel;
        public TextBox RotationTextBox;

        /// <summary>
        ///     new form, retrieve relevant data from instance
        /// </summary>
        /// <param name="inst">RotateFramingObjects instance</param>
        public RotateFramingObjectsForm(RotateFramingObjects inst)
        {
            IsReset = false;
            m_instance = inst;
            if (null == m_instance) TaskDialog.Show("Revit", "Load Application Failed");
            InitializeComponent();
            //this.rotationTextBox.Text = "Value";
        }

        /// <summary>
        /// </summary>
        public bool IsReset { get; set; }

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
            AbsoluteRadio = new RadioButton();
            m_relativeRadio = new RadioButton();
            m_cancelButton = new Button();
            m_okButton = new Button();
            m_rotationLabel = new Label();
            RotationTextBox = new TextBox();
            SuspendLayout();
            // 
            // absoluteRadio
            // 
            AbsoluteRadio.Location = new Point(85, 35);
            AbsoluteRadio.Name = "AbsoluteRadio";
            AbsoluteRadio.Size = new Size(72, 24);
            AbsoluteRadio.TabIndex = 0;
            AbsoluteRadio.Text = "Absolute";
            AbsoluteRadio.CheckedChanged += allRadio_CheckedChanged;
            // 
            // relativeRadio
            // 
            m_relativeRadio.Checked = true;
            m_relativeRadio.Location = new Point(15, 35);
            m_relativeRadio.Name = "m_relativeRadio";
            m_relativeRadio.Size = new Size(64, 24);
            m_relativeRadio.TabIndex = 1;
            m_relativeRadio.TabStop = true;
            m_relativeRadio.Text = "Relative";
            m_relativeRadio.CheckedChanged += singleRadio_CheckedChanged;
            // 
            // cancelButton
            // 
            m_cancelButton.DialogResult = DialogResult.Cancel;
            m_cancelButton.Location = new Point(93, 65);
            m_cancelButton.Name = "m_cancelButton";
            m_cancelButton.Size = new Size(75, 23);
            m_cancelButton.TabIndex = 6;
            m_cancelButton.Text = "&Cancel";
            m_cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            m_okButton.Location = new Point(15, 65);
            m_okButton.Name = "m_okButton";
            m_okButton.Size = new Size(75, 23);
            m_okButton.TabIndex = 8;
            m_okButton.Text = "&OK";
            m_okButton.Click += okButton_Click;
            // 
            // rotationLabel
            // 
            m_rotationLabel.Location = new Point(12, 12);
            m_rotationLabel.Name = "m_rotationLabel";
            m_rotationLabel.Size = new Size(50, 16);
            m_rotationLabel.TabIndex = 10;
            m_rotationLabel.Text = "Rotation";
            // 
            // rotationTextBox
            // 
            RotationTextBox.Location = new Point(68, 9);
            RotationTextBox.Name = "RotationTextBox";
            RotationTextBox.Size = new Size(100, 20);
            RotationTextBox.TabIndex = 1;
            RotationTextBox.KeyPress += rotationTextBox_KeyPress;
            RotationTextBox.TextChanged += rotationTextBox_TextChanged;
            // 
            // RotateFramingObjectsForm
            // 
            AcceptButton = m_okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = m_cancelButton;
            ClientSize = new Size(185, 101);
            Controls.Add(RotationTextBox);
            Controls.Add(m_relativeRadio);
            Controls.Add(m_rotationLabel);
            Controls.Add(m_okButton);
            Controls.Add(m_cancelButton);
            Controls.Add(AbsoluteRadio);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RotateFramingObjectsForm";
            ShowInTaskbar = false;
            Text = "Rotate Framing Objects";
            ResumeLayout(false);
            PerformLayout();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (IsReset) m_instance.RotateElement();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void singleRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsReset = true;
            m_instance.IsAbsoluteChecked = false;
        }

        private void allRadio_CheckedChanged(object sender, EventArgs e)
        {
            IsReset = true;
            m_instance.IsAbsoluteChecked = true;
        }

        private void rotationTextBox_TextChanged(object sender, EventArgs e)
        {
            if ("" != RotationTextBox.Text)
                try
                {
                    m_instance.ReceiveRotationTextBox = Convert.ToDouble(RotationTextBox.Text);
                }
                catch (Exception)
                {
                    //this.DialogResult=DialogResult.Cancel;
                    TaskDialog.Show("Revit", "Please input number.");
                    RotationTextBox.Clear();
                }
            else
                m_instance.ReceiveRotationTextBox = 0;

            IsReset = true;
        }

        private void rotationTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (13 == e.KeyChar)
                okButton_Click(sender, e);
            else
                rotationTextBox_TextChanged(sender, e);
        }
    }
}
