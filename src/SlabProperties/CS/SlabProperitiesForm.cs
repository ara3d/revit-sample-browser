// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.SlabProperties.CS
{
    /// <summary>
    ///     Show some properties of a slab in Revit Structure 5, including Level, Type name, Span direction,
    ///     Material name, Thickness, and Young Modulus for each layer of the slab's material.
    /// </summary>
    public class SlabPropertiesForm : Form
    {
        private Button m_closeButton;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container m_components = null;

        private Label m_degreeLabel;
        private GroupBox m_layerGroupBox;
        private RichTextBox m_layerRichTextBox;
        private Label m_levelLabel;
        private TextBox m_levelTextBox;

        // To store the data
        private readonly Command m_dataBuffer;
        private Label m_spanDirectionLabel;
        private TextBox m_spanDirectionTextBox;
        private Label m_typeNameLabel;
        private TextBox m_typeNameTextBox;

        private SlabPropertiesForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        ///     overload the constructor
        /// </summary>
        /// <param name="dataBuffer">To store the data of a slab</param>
        public SlabPropertiesForm(Command dataBuffer)
        {
            InitializeComponent();

            // get all the data
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
            m_layerGroupBox = new GroupBox();
            m_layerRichTextBox = new RichTextBox();
            m_levelLabel = new Label();
            m_levelTextBox = new TextBox();
            m_typeNameTextBox = new TextBox();
            m_spanDirectionTextBox = new TextBox();
            m_typeNameLabel = new Label();
            m_spanDirectionLabel = new Label();
            m_closeButton = new Button();
            m_degreeLabel = new Label();
            m_layerGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // layerGroupBox
            // 
            m_layerGroupBox.Controls.Add(m_layerRichTextBox);
            m_layerGroupBox.Location = new Point(22, 86);
            m_layerGroupBox.Name = "m_layerGroupBox";
            m_layerGroupBox.Size = new Size(375, 265);
            m_layerGroupBox.TabIndex = 29;
            m_layerGroupBox.TabStop = false;
            m_layerGroupBox.Text = "Layers:";
            // 
            // layerRichTextBox
            // 
            m_layerRichTextBox.Location = new Point(6, 19);
            m_layerRichTextBox.Name = "m_layerRichTextBox";
            m_layerRichTextBox.ReadOnly = true;
            m_layerRichTextBox.Size = new Size(359, 232);
            m_layerRichTextBox.TabIndex = 2;
            m_layerRichTextBox.Text = "";
            // 
            // levelLabel
            // 
            m_levelLabel.Location = new Point(13, 7);
            m_levelLabel.Name = "m_levelLabel";
            m_levelLabel.Size = new Size(98, 23);
            m_levelLabel.TabIndex = 27;
            m_levelLabel.Text = "Level:";
            m_levelLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // levelTextBox
            // 
            m_levelTextBox.Location = new Point(117, 8);
            m_levelTextBox.Name = "m_levelTextBox";
            m_levelTextBox.ReadOnly = true;
            m_levelTextBox.Size = new Size(280, 20);
            m_levelTextBox.TabIndex = 24;
            // 
            // typeNameTextBox
            // 
            m_typeNameTextBox.Location = new Point(117, 34);
            m_typeNameTextBox.Name = "m_typeNameTextBox";
            m_typeNameTextBox.ReadOnly = true;
            m_typeNameTextBox.Size = new Size(280, 20);
            m_typeNameTextBox.TabIndex = 22;
            // 
            // spanDirectionTextBox
            // 
            m_spanDirectionTextBox.Location = new Point(117, 60);
            m_spanDirectionTextBox.Name = "m_spanDirectionTextBox";
            m_spanDirectionTextBox.ReadOnly = true;
            m_spanDirectionTextBox.Size = new Size(224, 20);
            m_spanDirectionTextBox.TabIndex = 23;
            // 
            // typeNameLabel
            // 
            m_typeNameLabel.Location = new Point(13, 34);
            m_typeNameLabel.Name = "m_typeNameLabel";
            m_typeNameLabel.Size = new Size(98, 23);
            m_typeNameLabel.TabIndex = 25;
            m_typeNameLabel.Text = "Type Name:";
            m_typeNameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // spanDirectionLabel
            // 
            m_spanDirectionLabel.Location = new Point(13, 60);
            m_spanDirectionLabel.Name = "m_spanDirectionLabel";
            m_spanDirectionLabel.Size = new Size(98, 23);
            m_spanDirectionLabel.TabIndex = 26;
            m_spanDirectionLabel.Text = "Span Direction:";
            m_spanDirectionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // closeButton
            // 
            m_closeButton.DialogResult = DialogResult.Cancel;
            m_closeButton.Location = new Point(322, 367);
            m_closeButton.Name = "m_closeButton";
            m_closeButton.Size = new Size(75, 23);
            m_closeButton.TabIndex = 0;
            m_closeButton.Text = "Close";
            m_closeButton.Click += closeButton_Click;
            // 
            // degreeLabel
            // 
            m_degreeLabel.Location = new Point(347, 59);
            m_degreeLabel.Name = "m_degreeLabel";
            m_degreeLabel.Size = new Size(50, 23);
            m_degreeLabel.TabIndex = 26;
            m_degreeLabel.Text = "Degree";
            m_degreeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SlabPropertiesForm
            // 
            AcceptButton = m_closeButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = m_closeButton;
            ClientSize = new Size(411, 402);
            Controls.Add(m_layerGroupBox);
            Controls.Add(m_levelLabel);
            Controls.Add(m_levelTextBox);
            Controls.Add(m_typeNameTextBox);
            Controls.Add(m_spanDirectionTextBox);
            Controls.Add(m_typeNameLabel);
            Controls.Add(m_degreeLabel);
            Controls.Add(m_spanDirectionLabel);
            Controls.Add(m_closeButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SlabPropertiesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Slab Properties";
            Load += SlabPropertiesForm_Load;
            m_layerGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        ///     Close the Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     Display the properties on the form when the form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlabPropertiesForm_Load(object sender, EventArgs e)
        {
            m_levelTextBox.Text = m_dataBuffer.Level;
            m_typeNameTextBox.Text = m_dataBuffer.TypeName;
            m_spanDirectionTextBox.Text = m_dataBuffer.SpanDirection;

            var numberOfLayers = m_dataBuffer.NumberOfLayers;

            m_layerRichTextBox.Text = "";

            for (var i = 0; i < numberOfLayers; i++)
            {
                // Get each layer's Material name and Young Modulus properties
                m_dataBuffer.SetLayer(i);

                m_layerRichTextBox.Text += "Layer " + (i + 1) + "\r\n";
                m_layerRichTextBox.Text += "Material name:  " + m_dataBuffer.LayerMaterialName + "\r\n";
                m_layerRichTextBox.Text += "Thickness: " + m_dataBuffer.LayerThickness + "\r\n";
                m_layerRichTextBox.Text += "YoungModulus X:  " + m_dataBuffer.LayerYoungModulusX + "\r\n";
                m_layerRichTextBox.Text += "YoungModulus Y:  " + m_dataBuffer.LayerYoungModulusY + "\r\n";
                m_layerRichTextBox.Text += "YoungModulus Z:  " + m_dataBuffer.LayerYoungModulusZ + "\r\n";
                m_layerRichTextBox.Text += "-----------------------------------------------------------" + "\r\n";
            }
        }
    }
}
