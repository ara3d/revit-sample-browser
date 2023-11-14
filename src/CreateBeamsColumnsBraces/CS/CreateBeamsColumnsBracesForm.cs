// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;

namespace Revit.SDK.Samples.CreateBeamsColumnsBraces.CS
{
    /// <summary>
    ///     UI
    /// </summary>
    public class CreateBeamsColumnsBracesForm : Form
    {
        private ComboBox beamComboBox;
        private Label beamLabel;
        private ComboBox braceComboBox;
        private Label braceLabel;
        private Button cancelButton;
        private ComboBox columnComboBox;
        private Label columnLabel;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private Label DistanceLabel;
        private TextBox DistanceTextBox;
        private Label floornumberLabel;
        private TextBox floornumberTextBox;

        // To store the datas
        private readonly Command m_dataBuffer;
        private Button OKButton;
        private Label unitLabel;
        private Label XLabel;
        private TextBox XTextBox;
        private Label YLabel;
        private TextBox YTextBox;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="dataBuffer">the revit datas</param>
        public CreateBeamsColumnsBracesForm(Command dataBuffer)
        {
            //
            // Required for Windows Form Designer support
            //
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
            OKButton = new Button();
            XTextBox = new TextBox();
            YTextBox = new TextBox();
            DistanceTextBox = new TextBox();
            columnComboBox = new ComboBox();
            beamComboBox = new ComboBox();
            braceComboBox = new ComboBox();
            columnLabel = new Label();
            beamLabel = new Label();
            braceLabel = new Label();
            floornumberTextBox = new TextBox();
            DistanceLabel = new Label();
            YLabel = new Label();
            XLabel = new Label();
            floornumberLabel = new Label();
            cancelButton = new Button();
            unitLabel = new Label();
            SuspendLayout();
            // 
            // OKButton
            // 
            OKButton.Location = new Point(296, 208);
            OKButton.Name = "OKButton";
            OKButton.TabIndex = 8;
            OKButton.Text = "&OK";
            OKButton.Click += OKButton_Click;
            // 
            // XTextBox
            // 
            XTextBox.Location = new Point(16, 96);
            XTextBox.Name = "XTextBox";
            XTextBox.Size = new Size(136, 20);
            XTextBox.TabIndex = 2;
            XTextBox.Text = "";
            XTextBox.Validating += XTextBox_Validating;
            // 
            // YTextBox
            // 
            YTextBox.Location = new Point(16, 152);
            YTextBox.Name = "YTextBox";
            YTextBox.Size = new Size(136, 20);
            YTextBox.TabIndex = 3;
            YTextBox.Text = "";
            YTextBox.Validating += YTextBox_Validating;
            // 
            // DistanceTextBox
            // 
            DistanceTextBox.Location = new Point(16, 40);
            DistanceTextBox.Name = "DistanceTextBox";
            DistanceTextBox.Size = new Size(112, 20);
            DistanceTextBox.TabIndex = 1;
            DistanceTextBox.Text = "";
            DistanceTextBox.Validating += DistanceTextBox_Validating;
            // 
            // columnComboBox
            // 
            columnComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            columnComboBox.Location = new Point(240, 40);
            columnComboBox.Name = "columnComboBox";
            columnComboBox.Size = new Size(288, 21);
            columnComboBox.TabIndex = 5;
            // 
            // beamComboBox
            // 
            beamComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            beamComboBox.Location = new Point(240, 96);
            beamComboBox.Name = "beamComboBox";
            beamComboBox.Size = new Size(288, 21);
            beamComboBox.TabIndex = 6;
            // 
            // braceComboBox
            // 
            braceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            braceComboBox.Location = new Point(240, 152);
            braceComboBox.Name = "braceComboBox";
            braceComboBox.Size = new Size(288, 21);
            braceComboBox.TabIndex = 7;
            // 
            // columnLabel
            // 
            columnLabel.Location = new Point(240, 16);
            columnLabel.Name = "columnLabel";
            columnLabel.Size = new Size(120, 23);
            columnLabel.TabIndex = 10;
            columnLabel.Text = "Type of Columns:";
            // 
            // beamLabel
            // 
            beamLabel.Location = new Point(240, 72);
            beamLabel.Name = "beamLabel";
            beamLabel.Size = new Size(120, 23);
            beamLabel.TabIndex = 11;
            beamLabel.Text = "Type of Beams:";
            // 
            // braceLabel
            // 
            braceLabel.Location = new Point(240, 128);
            braceLabel.Name = "braceLabel";
            braceLabel.Size = new Size(120, 23);
            braceLabel.TabIndex = 12;
            braceLabel.Text = "Type of Braces:";
            // 
            // floornumberTextBox
            // 
            floornumberTextBox.Location = new Point(16, 208);
            floornumberTextBox.Name = "floornumberTextBox";
            floornumberTextBox.Size = new Size(112, 20);
            floornumberTextBox.TabIndex = 4;
            floornumberTextBox.Text = "";
            floornumberTextBox.Validating += floornumberTextBox_Validating;
            // 
            // DistanceLabel
            // 
            DistanceLabel.Location = new Point(16, 16);
            DistanceLabel.Name = "DistanceLabel";
            DistanceLabel.Size = new Size(152, 23);
            DistanceLabel.TabIndex = 14;
            DistanceLabel.Text = "Distance between Columns:";
            // 
            // YLabel
            // 
            YLabel.Location = new Point(16, 128);
            YLabel.Name = "YLabel";
            YLabel.Size = new Size(200, 23);
            YLabel.TabIndex = 15;
            YLabel.Text = "Number of Columns in the Y Direction:";
            // 
            // XLabel
            // 
            XLabel.Location = new Point(16, 72);
            XLabel.Name = "XLabel";
            XLabel.Size = new Size(200, 23);
            XLabel.TabIndex = 16;
            XLabel.Text = "Number of Columns in the X Direction:";
            // 
            // floornumberLabel
            // 
            floornumberLabel.Location = new Point(16, 184);
            floornumberLabel.Name = "floornumberLabel";
            floornumberLabel.Size = new Size(144, 23);
            floornumberLabel.TabIndex = 17;
            floornumberLabel.Text = "Number of Floors:";
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(392, 208);
            cancelButton.Name = "cancelButton";
            cancelButton.TabIndex = 9;
            cancelButton.Text = "&Cancel";
            cancelButton.Click += cancelButton_Click;
            // 
            // unitLabel
            // 
            unitLabel.Location = new Point(136, 42);
            unitLabel.Name = "unitLabel";
            unitLabel.Size = new Size(32, 23);
            unitLabel.TabIndex = 18;
            unitLabel.Text = "feet";
            // 
            // CreateBeamsColumnsBracesForm
            // 
            AcceptButton = OKButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = cancelButton;
            ClientSize = new Size(546, 246);
            Controls.Add(unitLabel);
            Controls.Add(cancelButton);
            Controls.Add(floornumberLabel);
            Controls.Add(XLabel);
            Controls.Add(YLabel);
            Controls.Add(DistanceLabel);
            Controls.Add(floornumberTextBox);
            Controls.Add(DistanceTextBox);
            Controls.Add(YTextBox);
            Controls.Add(XTextBox);
            Controls.Add(braceLabel);
            Controls.Add(beamLabel);
            Controls.Add(columnLabel);
            Controls.Add(braceComboBox);
            Controls.Add(beamComboBox);
            Controls.Add(columnComboBox);
            Controls.Add(OKButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateBeamsColumnsBracesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create Beams Columns and Braces";
            Load += CreateBeamsColumnsBracesForm_Load;
            ResumeLayout(false);
        }

        /// <summary>
        ///     Refresh the text box for the default data
        /// </summary>
        private void TextBoxRefresh()
        {
            XTextBox.Text = "2";
            YTextBox.Text = "2";
            DistanceTextBox.Text = 20.0.ToString("0.0");
            floornumberTextBox.Text = "1";
        }

        private void CreateBeamsColumnsBracesForm_Load(object sender, EventArgs e)
        {
            TextBoxRefresh();

            var notLoadSymbol = false;
            if (0 == m_dataBuffer.ColumnMaps.Count)
            {
                TaskDialog.Show("Revit",
                    "No Structural Columns family is loaded in the project, please load one firstly.");
                notLoadSymbol = true;
            }

            if (0 == m_dataBuffer.BeamMaps.Count)
            {
                TaskDialog.Show("Revit",
                    "No Structural Framing family is loaded in the project, please load one firstly.");
                notLoadSymbol = true;
            }

            if (notLoadSymbol)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            columnComboBox.DataSource = m_dataBuffer.ColumnMaps;
            columnComboBox.DisplayMember = "SymbolName";
            columnComboBox.ValueMember = "ElementType";

            beamComboBox.DataSource = m_dataBuffer.BeamMaps;
            beamComboBox.DisplayMember = "SymbolName";
            beamComboBox.ValueMember = "ElementType";

            braceComboBox.DataSource = m_dataBuffer.BraceMaps;
            braceComboBox.DisplayMember = "SymbolName";
            braceComboBox.ValueMember = "ElementType";
        }

        /// <summary>
        ///     accept use's input and create columns, beams and braces
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            //check whether the input is correct and create elements
            try
            {
                var xNumber = int.Parse(XTextBox.Text);
                var yNumber = int.Parse(YTextBox.Text);
                var distance = double.Parse(DistanceTextBox.Text);
                var columnType = columnComboBox.SelectedValue;
                var beamType = beamComboBox.SelectedValue;
                var braceType = braceComboBox.SelectedValue;
                var floorNumber = int.Parse(floornumberTextBox.Text);

                m_dataBuffer.CreateMatrix(xNumber, yNumber, distance);
                m_dataBuffer.AddInstance(columnType, beamType, braceType, floorNumber);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input datas correctly.");
            }
        }

        /// <summary>
        ///     cancel the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        ///     Verify the distance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DistanceTextBox_Validating(object sender, CancelEventArgs e)
        {
            var distance = 0.1;
            try
            {
                distance = double.Parse(DistanceTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please enter a value larger than 5 and less than 30000.");
                DistanceTextBox.Text = "";
                DistanceTextBox.Focus();
                return;
            }

            if (distance <= 5)
            {
                TaskDialog.Show("Revit", "Please enter a value larger than 5.");
                DistanceTextBox.Text = "";
                DistanceTextBox.Focus();
                return;
            }

            if (distance > 30000)
            {
                TaskDialog.Show("Revit", "Please enter a value less than 30000.");
                DistanceTextBox.Text = "";
                DistanceTextBox.Focus();
            }
        }

        /// <summary>
        ///     Verify the number of X direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void XTextBox_Validating(object sender, CancelEventArgs e)
        {
            var xNumber = 1;
            try
            {
                xNumber = int.Parse(XTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for X direction between 1 to 20.");
                XTextBox.Text = "";
            }

            if (xNumber < 1 || xNumber > 20)
            {
                TaskDialog.Show("Revit", "Please input an integer for X direction between 1 to 20.");
                XTextBox.Text = "";
            }
        }

        /// <summary>
        ///     Verify the number of Y direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YTextBox_Validating(object sender, CancelEventArgs e)
        {
            var yNumber = 1;
            try
            {
                yNumber = int.Parse(YTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for Y direction between 1 to 20.");
                YTextBox.Text = "";
            }

            if (yNumber < 1 || yNumber > 20)
            {
                TaskDialog.Show("Revit", "Please input an integer for Y direction between 1 to 20.");
                YTextBox.Text = "";
            }
        }

        /// <summary>
        ///     Verify the number of floors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floornumberTextBox_Validating(object sender, CancelEventArgs e)
        {
            var floorNumber = 1;
            try
            {
                floorNumber = int.Parse(floornumberTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for the number of floors between 1 to 10.");
                floornumberTextBox.Text = "";
            }

            if (floorNumber < 1 || floorNumber > 10)
            {
                TaskDialog.Show("Revit", "Please input an integer for the number of floors between 1 to 10.");
                floornumberTextBox.Text = "";
            }
        }
    }
}
