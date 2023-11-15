// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.StructuralLayerFunction.CS
{
    /// <summary>
    ///     display the function of each of a select floor's structural layers
    /// </summary>
    public class StructuralLayerFunctionForm : Form
    {
        private readonly Container m_components = null;
        private GroupBox m_functionGroupBox;
        private ListBox m_functionListBox;
        private Button m_okButton;

        /// <summary>
        ///     Constructor of StructuralLayerFunctionForm
        /// </summary>
        /// <param name="dataBuffer">A reference of StructuralLayerFunction class</param>
        public StructuralLayerFunctionForm(Command dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // Set the data source of the ListBox control
            m_functionListBox.DataSource = dataBuffer.Functions;
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
            m_functionListBox = new ListBox();
            m_functionGroupBox = new GroupBox();
            m_okButton = new Button();
            m_functionGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // functionListBox
            // 
            m_functionListBox.Location = new Point(6, 24);
            m_functionListBox.Name = "m_functionListBox";
            m_functionListBox.Size = new Size(189, 147);
            m_functionListBox.TabIndex = 0;
            // 
            // functionGroupBox
            // 
            m_functionGroupBox.Controls.Add(m_functionListBox);
            m_functionGroupBox.Location = new Point(12, 12);
            m_functionGroupBox.Name = "m_functionGroupBox";
            m_functionGroupBox.Size = new Size(201, 184);
            m_functionGroupBox.TabIndex = 1;
            m_functionGroupBox.TabStop = false;
            m_functionGroupBox.Text = "Layers Functions List";
            // 
            // okButton
            // 
            m_okButton.DialogResult = DialogResult.OK;
            m_okButton.Location = new Point(138, 202);
            m_okButton.Name = "m_okButton";
            m_okButton.Size = new Size(75, 23);
            m_okButton.TabIndex = 2;
            m_okButton.Text = "OK";
            // 
            // StructuralLayerFunctionForm
            // 
            AcceptButton = m_okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = m_okButton;
            ClientSize = new Size(225, 236);
            Controls.Add(m_okButton);
            Controls.Add(m_functionGroupBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StructuralLayerFunctionForm";
            ShowInTaskbar = false;
            Text = "Structure Layers Function";
            m_functionGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
