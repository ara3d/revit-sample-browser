// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.BeamAndSlabNewParameter.CS
{
    public class BeamAndSlabParametersForm : Form
    {
        private Button m_addParameterButton;
        private Label m_attributeValueLabel;
        private ListBox m_attributeValueListBox;

        private readonly Container m_components = null;

        private Button m_displayValueButton;
        private Button m_exitButton;
        private Button m_findButton;

        private readonly Command m_dataBuffer;

        public BeamAndSlabParametersForm(Command dataBuffer)
        {
            InitializeComponent();
            m_dataBuffer = dataBuffer;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            m_addParameterButton = new Button();
            m_displayValueButton = new Button();
            m_exitButton = new Button();
            m_attributeValueListBox = new ListBox();
            m_attributeValueLabel = new Label();
            m_findButton = new Button();
            SuspendLayout();
            // 
            // 
            m_addParameterButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_addParameterButton.Location = new Point(311, 65);
            m_addParameterButton.Name = "m_addParameterButton";
            m_addParameterButton.Size = new Size(105, 26);
            m_addParameterButton.TabIndex = 1;
            m_addParameterButton.Text = "&Add";
            m_addParameterButton.TextAlign = ContentAlignment.TopCenter;
            m_addParameterButton.Click += addParameterButton_Click;
            // 
            // 
            m_displayValueButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_displayValueButton.Location = new Point(311, 111);
            m_displayValueButton.Name = "m_displayValueButton";
            m_displayValueButton.Size = new Size(105, 26);
            m_displayValueButton.TabIndex = 2;
            m_displayValueButton.Text = "&Display Value";
            m_displayValueButton.Click += displayValueButton_Click;
            // 
            // 
            m_exitButton.DialogResult = DialogResult.Cancel;
            m_exitButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_exitButton.Location = new Point(311, 203);
            m_exitButton.Name = "m_exitButton";
            m_exitButton.Size = new Size(105, 26);
            m_exitButton.TabIndex = 4;
            m_exitButton.Text = "&Exit";
            m_exitButton.Click += exitButton_Click;
            // 
            // 
            m_attributeValueListBox.ItemHeight = 16;
            m_attributeValueListBox.Location = new Point(19, 46);
            m_attributeValueListBox.Name = "m_attributeValueListBox";
            m_attributeValueListBox.Size = new Size(269, 228);
            m_attributeValueListBox.TabIndex = 18;
            m_attributeValueListBox.TabStop = false;
            // 
            // 
            m_attributeValueLabel.Location = new Point(19, 9);
            m_attributeValueLabel.Name = "m_attributeValueLabel";
            m_attributeValueLabel.Size = new Size(279, 37);
            m_attributeValueLabel.TabIndex = 19;
            m_attributeValueLabel.Text = "Display the value of the Unique ID if present for all the selected elements";
            // 
            // 
            m_findButton.DialogResult = DialogResult.OK;
            m_findButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            m_findButton.Location = new Point(311, 157);
            m_findButton.Name = "m_findButton";
            m_findButton.Size = new Size(105, 26);
            m_findButton.TabIndex = 3;
            m_findButton.Text = "&Find";
            m_findButton.Click += findButton_Click;
            // 
            // 
            CancelButton = m_exitButton;
            ClientSize = new Size(438, 292);
            Controls.Add(m_attributeValueLabel);
            Controls.Add(m_attributeValueListBox);
            Controls.Add(m_addParameterButton);
            Controls.Add(m_findButton);
            Controls.Add(m_displayValueButton);
            Controls.Add(m_exitButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BeamAndSlabParametersForm";
            ShowInTaskbar = false;
            Text = "Beam and Slab New Parameters";
            ResumeLayout(false);
        }

        private void addParameterButton_Click(object sender, EventArgs e)
        {
            var successAddParameter = m_dataBuffer.SetNewParameterToBeamsAndSlabs();

            if (successAddParameter)
            {
                DialogResult = DialogResult.OK;
                m_dataBuffer.SetValueToUniqueIdParameter();
                TaskDialog.Show("Revit", "Done");
            }
            else
            {
                DialogResult = DialogResult.None;
                m_dataBuffer.SetValueToUniqueIdParameter();
                TaskDialog.Show("Revit", "Unique ID parameter exist");
            }
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            if (null != m_attributeValueListBox.SelectedItem)
                m_dataBuffer.FindElement(m_attributeValueListBox.SelectedItem.ToString());
        }

        private void displayValueButton_Click(object sender, EventArgs e)
        {
            m_attributeValueListBox.DataSource = m_dataBuffer.SendValueToListBox();

            //If we displayed nothing, give possible reasons
            if (0 == m_attributeValueListBox.Items.Count)
            {
                var message = "There was an error executing the command.\r\n";
                message += "Possible reasons for this are:\r\n\r\n";
                message += "1. No parameter was added.\r\n";
                message += "2. No beam or slab was selected.\r\n";
                message += "3. The value was blank.\r\n";
                TaskDialog.Show("Revit", message);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
