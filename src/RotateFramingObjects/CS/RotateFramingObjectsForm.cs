// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Forms.TextBox;

namespace Revit.SDK.Samples.RotateFramingObjects.CS
{
    /// <summary>
    ///     Summary description for PutDialog.
    /// </summary>
    public class RotateFramingObjectsForm : Form
    {
        public RadioButton absoluteRadio;
        private Button cancelButton;
        private readonly Container m_components = null;

        private readonly RotateFramingObjects m_instance;
        private Button okButton;
        private RadioButton relativeRadio;
        private Label rotationLabel;
        public TextBox rotationTextBox;

        /// <summary>
        ///     new form, retrieve relevant data from instance
        /// </summary>
        /// <param name="Inst">RotateFramingObjects instance</param>
        public RotateFramingObjectsForm(RotateFramingObjects Inst)
        {
            IsReset = false;
            m_instance = Inst;
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
            absoluteRadio = new RadioButton();
            relativeRadio = new RadioButton();
            cancelButton = new Button();
            okButton = new Button();
            rotationLabel = new Label();
            rotationTextBox = new TextBox();
            SuspendLayout();
            // 
            // absoluteRadio
            // 
            absoluteRadio.Location = new Point(85, 35);
            absoluteRadio.Name = "absoluteRadio";
            absoluteRadio.Size = new Size(72, 24);
            absoluteRadio.TabIndex = 0;
            absoluteRadio.Text = "Absolute";
            absoluteRadio.CheckedChanged += allRadio_CheckedChanged;
            // 
            // relativeRadio
            // 
            relativeRadio.Checked = true;
            relativeRadio.Location = new Point(15, 35);
            relativeRadio.Name = "relativeRadio";
            relativeRadio.Size = new Size(64, 24);
            relativeRadio.TabIndex = 1;
            relativeRadio.TabStop = true;
            relativeRadio.Text = "Relative";
            relativeRadio.CheckedChanged += singleRadio_CheckedChanged;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(93, 65);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "&Cancel";
            cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            okButton.Location = new Point(15, 65);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 8;
            okButton.Text = "&OK";
            okButton.Click += okButton_Click;
            // 
            // rotationLabel
            // 
            rotationLabel.Location = new Point(12, 12);
            rotationLabel.Name = "rotationLabel";
            rotationLabel.Size = new Size(50, 16);
            rotationLabel.TabIndex = 10;
            rotationLabel.Text = "Rotation";
            // 
            // rotationTextBox
            // 
            rotationTextBox.Location = new Point(68, 9);
            rotationTextBox.Name = "rotationTextBox";
            rotationTextBox.Size = new Size(100, 20);
            rotationTextBox.TabIndex = 1;
            rotationTextBox.KeyPress += rotationTextBox_KeyPress;
            rotationTextBox.TextChanged += rotationTextBox_TextChanged;
            // 
            // RotateFramingObjectsForm
            // 
            AcceptButton = okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = cancelButton;
            ClientSize = new Size(185, 101);
            Controls.Add(rotationTextBox);
            Controls.Add(relativeRadio);
            Controls.Add(rotationLabel);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(absoluteRadio);
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
            if ("" != rotationTextBox.Text)
                try
                {
                    m_instance.ReceiveRotationTextBox = Convert.ToDouble(rotationTextBox.Text);
                }
                catch (Exception)
                {
                    //this.DialogResult=DialogResult.Cancel;
                    TaskDialog.Show("Revit", "Please input number.");
                    rotationTextBox.Clear();
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
