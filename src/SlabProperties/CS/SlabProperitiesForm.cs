// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.SlabProperties.CS
{
    /// <summary>
    ///     Show some properties of a slab in Revit Structure 5, including Level, Type name, Span direction,
    ///     Material name, Thickness, and Young Modulus for each layer of the slab's material.
    /// </summary>
    public class SlabPropertiesForm : Form
    {
        private Button closeButton;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private Label degreeLabel;
        private GroupBox layerGroupBox;
        private RichTextBox layerRichTextBox;
        private Label levelLabel;
        private TextBox levelTextBox;

        // To store the data
        private readonly Command m_dataBuffer;
        private Label spanDirectionLabel;
        private TextBox spanDirectionTextBox;
        private Label typeNameLabel;
        private TextBox typeNameTextBox;


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
            layerGroupBox = new GroupBox();
            layerRichTextBox = new RichTextBox();
            levelLabel = new Label();
            levelTextBox = new TextBox();
            typeNameTextBox = new TextBox();
            spanDirectionTextBox = new TextBox();
            typeNameLabel = new Label();
            spanDirectionLabel = new Label();
            closeButton = new Button();
            degreeLabel = new Label();
            layerGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // layerGroupBox
            // 
            layerGroupBox.Controls.Add(layerRichTextBox);
            layerGroupBox.Location = new Point(22, 86);
            layerGroupBox.Name = "layerGroupBox";
            layerGroupBox.Size = new Size(375, 265);
            layerGroupBox.TabIndex = 29;
            layerGroupBox.TabStop = false;
            layerGroupBox.Text = "Layers:";
            // 
            // layerRichTextBox
            // 
            layerRichTextBox.Location = new Point(6, 19);
            layerRichTextBox.Name = "layerRichTextBox";
            layerRichTextBox.ReadOnly = true;
            layerRichTextBox.Size = new Size(359, 232);
            layerRichTextBox.TabIndex = 2;
            layerRichTextBox.Text = "";
            // 
            // levelLabel
            // 
            levelLabel.Location = new Point(13, 7);
            levelLabel.Name = "levelLabel";
            levelLabel.Size = new Size(98, 23);
            levelLabel.TabIndex = 27;
            levelLabel.Text = "Level:";
            levelLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // levelTextBox
            // 
            levelTextBox.Location = new Point(117, 8);
            levelTextBox.Name = "levelTextBox";
            levelTextBox.ReadOnly = true;
            levelTextBox.Size = new Size(280, 20);
            levelTextBox.TabIndex = 24;
            // 
            // typeNameTextBox
            // 
            typeNameTextBox.Location = new Point(117, 34);
            typeNameTextBox.Name = "typeNameTextBox";
            typeNameTextBox.ReadOnly = true;
            typeNameTextBox.Size = new Size(280, 20);
            typeNameTextBox.TabIndex = 22;
            // 
            // spanDirectionTextBox
            // 
            spanDirectionTextBox.Location = new Point(117, 60);
            spanDirectionTextBox.Name = "spanDirectionTextBox";
            spanDirectionTextBox.ReadOnly = true;
            spanDirectionTextBox.Size = new Size(224, 20);
            spanDirectionTextBox.TabIndex = 23;
            // 
            // typeNameLabel
            // 
            typeNameLabel.Location = new Point(13, 34);
            typeNameLabel.Name = "typeNameLabel";
            typeNameLabel.Size = new Size(98, 23);
            typeNameLabel.TabIndex = 25;
            typeNameLabel.Text = "Type Name:";
            typeNameLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // spanDirectionLabel
            // 
            spanDirectionLabel.Location = new Point(13, 60);
            spanDirectionLabel.Name = "spanDirectionLabel";
            spanDirectionLabel.Size = new Size(98, 23);
            spanDirectionLabel.TabIndex = 26;
            spanDirectionLabel.Text = "Span Direction:";
            spanDirectionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // closeButton
            // 
            closeButton.DialogResult = DialogResult.Cancel;
            closeButton.Location = new Point(322, 367);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 0;
            closeButton.Text = "Close";
            closeButton.Click += closeButton_Click;
            // 
            // degreeLabel
            // 
            degreeLabel.Location = new Point(347, 59);
            degreeLabel.Name = "degreeLabel";
            degreeLabel.Size = new Size(50, 23);
            degreeLabel.TabIndex = 26;
            degreeLabel.Text = "Degree";
            degreeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SlabPropertiesForm
            // 
            AcceptButton = closeButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = closeButton;
            ClientSize = new Size(411, 402);
            Controls.Add(layerGroupBox);
            Controls.Add(levelLabel);
            Controls.Add(levelTextBox);
            Controls.Add(typeNameTextBox);
            Controls.Add(spanDirectionTextBox);
            Controls.Add(typeNameLabel);
            Controls.Add(degreeLabel);
            Controls.Add(spanDirectionLabel);
            Controls.Add(closeButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SlabPropertiesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Slab Properties";
            Load += SlabPropertiesForm_Load;
            layerGroupBox.ResumeLayout(false);
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
            levelTextBox.Text = m_dataBuffer.Level;
            typeNameTextBox.Text = m_dataBuffer.TypeName;
            spanDirectionTextBox.Text = m_dataBuffer.SpanDirection;

            var numberOfLayers = m_dataBuffer.NumberOfLayers;

            layerRichTextBox.Text = "";

            for (var i = 0; i < numberOfLayers; i++)
            {
                // Get each layer's Material name and Young Modulus properties
                m_dataBuffer.SetLayer(i);

                layerRichTextBox.Text += "Layer " + (i + 1) + "\r\n";
                layerRichTextBox.Text += "Material name:  " + m_dataBuffer.LayerMaterialName + "\r\n";
                layerRichTextBox.Text += "Thickness: " + m_dataBuffer.LayerThickness + "\r\n";
                layerRichTextBox.Text += "YoungModulus X:  " + m_dataBuffer.LayerYoungModulusX + "\r\n";
                layerRichTextBox.Text += "YoungModulus Y:  " + m_dataBuffer.LayerYoungModulusY + "\r\n";
                layerRichTextBox.Text += "YoungModulus Z:  " + m_dataBuffer.LayerYoungModulusZ + "\r\n";
                layerRichTextBox.Text += "-----------------------------------------------------------" + "\r\n";
            }
        }
    }
}
