// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.BeamAndSlabNewParameter.CS
{
    /// <summary>
    ///     User Interface.
    /// </summary>
    public class BeamAndSlabParametersForm : Form
    {
        private Button addParameterButton;
        private Label attributeValueLabel;
        private ListBox attributeValueListBox;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private Button displayValueButton;
        private Button exitButton;
        private Button findButton;

        // an instance of Command class
        private readonly Command m_dataBuffer;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer"></param>
        public BeamAndSlabParametersForm(Command dataBuffer)
        {
            InitializeComponent();
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
            addParameterButton = new Button();
            displayValueButton = new Button();
            exitButton = new Button();
            attributeValueListBox = new ListBox();
            attributeValueLabel = new Label();
            findButton = new Button();
            SuspendLayout();
            // 
            // addParameterButton
            // 
            addParameterButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            addParameterButton.Location = new Point(311, 65);
            addParameterButton.Name = "addParameterButton";
            addParameterButton.Size = new Size(105, 26);
            addParameterButton.TabIndex = 1;
            addParameterButton.Text = "&Add";
            addParameterButton.TextAlign = ContentAlignment.TopCenter;
            addParameterButton.Click += addParameterButton_Click;
            // 
            // displayValueButton
            // 
            displayValueButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            displayValueButton.Location = new Point(311, 111);
            displayValueButton.Name = "displayValueButton";
            displayValueButton.Size = new Size(105, 26);
            displayValueButton.TabIndex = 2;
            displayValueButton.Text = "&Display Value";
            displayValueButton.Click += displayValueButton_Click;
            // 
            // exitButton
            // 
            exitButton.DialogResult = DialogResult.Cancel;
            exitButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            exitButton.Location = new Point(311, 203);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(105, 26);
            exitButton.TabIndex = 4;
            exitButton.Text = "&Exit";
            exitButton.Click += exitButton_Click;
            // 
            // attributeValueListBox
            // 
            attributeValueListBox.ItemHeight = 16;
            attributeValueListBox.Location = new Point(19, 46);
            attributeValueListBox.Name = "attributeValueListBox";
            attributeValueListBox.Size = new Size(269, 228);
            attributeValueListBox.TabIndex = 18;
            attributeValueListBox.TabStop = false;
            // 
            // attributeValueLabel
            // 
            attributeValueLabel.Location = new Point(19, 9);
            attributeValueLabel.Name = "attributeValueLabel";
            attributeValueLabel.Size = new Size(279, 37);
            attributeValueLabel.TabIndex = 19;
            attributeValueLabel.Text = "Display the value of the Unique ID if present for all the selected elements";
            // 
            // findButton
            // 
            findButton.DialogResult = DialogResult.OK;
            findButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            findButton.Location = new Point(311, 157);
            findButton.Name = "findButton";
            findButton.Size = new Size(105, 26);
            findButton.TabIndex = 3;
            findButton.Text = "&Find";
            findButton.Click += findButton_Click;
            // 
            // BeamAndSlabParametersForm
            // 
            CancelButton = exitButton;
            ClientSize = new Size(438, 292);
            Controls.Add(attributeValueLabel);
            Controls.Add(attributeValueListBox);
            Controls.Add(addParameterButton);
            Controls.Add(findButton);
            Controls.Add(displayValueButton);
            Controls.Add(exitButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BeamAndSlabParametersForm";
            ShowInTaskbar = false;
            Text = "Beam and Slab New Parameters";
            ResumeLayout(false);
        }

        /// <summary>
        ///     Call SetNewParameterToBeamsAndSlabs function
        ///     which is belongs to BeamAndSlabParameters class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addParameterButton_Click(object sender, EventArgs e)
        {
            var successAddParameter = m_dataBuffer.SetNewParameterToBeamsAndSlabs();

            if (successAddParameter)
            {
                DialogResult = DialogResult.OK;
                m_dataBuffer.SetValueToUniqueIDParameter();
                TaskDialog.Show("Revit", "Done");
            }
            else
            {
                DialogResult = DialogResult.None;
                m_dataBuffer.SetValueToUniqueIDParameter();
                TaskDialog.Show("Revit", "Unique ID parameter exist");
            }
        }

        /// <summary>
        ///     Call SetValueToUniqueIDParameter function
        ///     which is belongs to BeamAndSlabNewParameters class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findButton_Click(object sender, EventArgs e)
        {
            if (null != attributeValueListBox.SelectedItem)
                m_dataBuffer.FindElement(attributeValueListBox.SelectedItem.ToString());
        }

        /// <summary>
        ///     Call SendValueToListBox function which is belongs to BeamAndSlabNewParameters class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayValueButton_Click(object sender, EventArgs e)
        {
            attributeValueListBox.DataSource = m_dataBuffer.SendValueToListBox();

            //If we displayed nothing, give possible reasons
            if (0 == attributeValueListBox.Items.Count)
            {
                var message = "There was an error executing the command.\r\n";
                message = message + "Possible reasons for this are:\r\n\r\n";
                message = message + "1. No parameter was added.\r\n";
                message = message + "2. No beam or slab was selected.\r\n";
                message = message + "3. The value was blank.\r\n";
                TaskDialog.Show("Revit", message);
            }
        }

        /// <summary>
        ///     Close this form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
