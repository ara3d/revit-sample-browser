// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;

namespace RevitMultiSample.CreateBeamsColumnsBraces.CS
{
    /// <summary>
    ///     UI
    /// </summary>
    public class CreateBeamsColumnsBracesForm : Form
    {
        private ComboBox m_beamComboBox;
        private Label m_beamLabel;
        private ComboBox m_braceComboBox;
        private Label m_braceLabel;
        private Button m_cancelButton;
        private ComboBox m_columnComboBox;
        private Label m_columnLabel;

        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container m_components = null;

        private Label m_distanceLabel;
        private TextBox m_distanceTextBox;
        private Label m_floornumberLabel;
        private TextBox m_floornumberTextBox;

        // To store the datas
        private readonly Command m_dataBuffer;
        private Button m_okButton;
        private Label m_unitLabel;
        private Label m_xLabel;
        private TextBox m_xTextBox;
        private Label m_yLabel;
        private TextBox m_yTextBox;

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
            m_okButton = new Button();
            m_xTextBox = new TextBox();
            m_yTextBox = new TextBox();
            m_distanceTextBox = new TextBox();
            m_columnComboBox = new ComboBox();
            m_beamComboBox = new ComboBox();
            m_braceComboBox = new ComboBox();
            m_columnLabel = new Label();
            m_beamLabel = new Label();
            m_braceLabel = new Label();
            m_floornumberTextBox = new TextBox();
            m_distanceLabel = new Label();
            m_yLabel = new Label();
            m_xLabel = new Label();
            m_floornumberLabel = new Label();
            m_cancelButton = new Button();
            m_unitLabel = new Label();
            SuspendLayout();
            // 
            // OKButton
            // 
            m_okButton.Location = new Point(296, 208);
            m_okButton.Name = "m_okButton";
            m_okButton.TabIndex = 8;
            m_okButton.Text = "&OK";
            m_okButton.Click += OKButton_Click;
            // 
            // XTextBox
            // 
            m_xTextBox.Location = new Point(16, 96);
            m_xTextBox.Name = "m_xTextBox";
            m_xTextBox.Size = new Size(136, 20);
            m_xTextBox.TabIndex = 2;
            m_xTextBox.Text = "";
            m_xTextBox.Validating += XTextBox_Validating;
            // 
            // YTextBox
            // 
            m_yTextBox.Location = new Point(16, 152);
            m_yTextBox.Name = "m_yTextBox";
            m_yTextBox.Size = new Size(136, 20);
            m_yTextBox.TabIndex = 3;
            m_yTextBox.Text = "";
            m_yTextBox.Validating += YTextBox_Validating;
            // 
            // DistanceTextBox
            // 
            m_distanceTextBox.Location = new Point(16, 40);
            m_distanceTextBox.Name = "m_distanceTextBox";
            m_distanceTextBox.Size = new Size(112, 20);
            m_distanceTextBox.TabIndex = 1;
            m_distanceTextBox.Text = "";
            m_distanceTextBox.Validating += DistanceTextBox_Validating;
            // 
            // columnComboBox
            // 
            m_columnComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            m_columnComboBox.Location = new Point(240, 40);
            m_columnComboBox.Name = "m_columnComboBox";
            m_columnComboBox.Size = new Size(288, 21);
            m_columnComboBox.TabIndex = 5;
            // 
            // beamComboBox
            // 
            m_beamComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            m_beamComboBox.Location = new Point(240, 96);
            m_beamComboBox.Name = "m_beamComboBox";
            m_beamComboBox.Size = new Size(288, 21);
            m_beamComboBox.TabIndex = 6;
            // 
            // braceComboBox
            // 
            m_braceComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            m_braceComboBox.Location = new Point(240, 152);
            m_braceComboBox.Name = "m_braceComboBox";
            m_braceComboBox.Size = new Size(288, 21);
            m_braceComboBox.TabIndex = 7;
            // 
            // columnLabel
            // 
            m_columnLabel.Location = new Point(240, 16);
            m_columnLabel.Name = "m_columnLabel";
            m_columnLabel.Size = new Size(120, 23);
            m_columnLabel.TabIndex = 10;
            m_columnLabel.Text = "Type of Columns:";
            // 
            // beamLabel
            // 
            m_beamLabel.Location = new Point(240, 72);
            m_beamLabel.Name = "m_beamLabel";
            m_beamLabel.Size = new Size(120, 23);
            m_beamLabel.TabIndex = 11;
            m_beamLabel.Text = "Type of Beams:";
            // 
            // braceLabel
            // 
            m_braceLabel.Location = new Point(240, 128);
            m_braceLabel.Name = "m_braceLabel";
            m_braceLabel.Size = new Size(120, 23);
            m_braceLabel.TabIndex = 12;
            m_braceLabel.Text = "Type of Braces:";
            // 
            // floornumberTextBox
            // 
            m_floornumberTextBox.Location = new Point(16, 208);
            m_floornumberTextBox.Name = "m_floornumberTextBox";
            m_floornumberTextBox.Size = new Size(112, 20);
            m_floornumberTextBox.TabIndex = 4;
            m_floornumberTextBox.Text = "";
            m_floornumberTextBox.Validating += floornumberTextBox_Validating;
            // 
            // DistanceLabel
            // 
            m_distanceLabel.Location = new Point(16, 16);
            m_distanceLabel.Name = "m_distanceLabel";
            m_distanceLabel.Size = new Size(152, 23);
            m_distanceLabel.TabIndex = 14;
            m_distanceLabel.Text = "Distance between Columns:";
            // 
            // YLabel
            // 
            m_yLabel.Location = new Point(16, 128);
            m_yLabel.Name = "m_yLabel";
            m_yLabel.Size = new Size(200, 23);
            m_yLabel.TabIndex = 15;
            m_yLabel.Text = "Number of Columns in the Y Direction:";
            // 
            // XLabel
            // 
            m_xLabel.Location = new Point(16, 72);
            m_xLabel.Name = "m_xLabel";
            m_xLabel.Size = new Size(200, 23);
            m_xLabel.TabIndex = 16;
            m_xLabel.Text = "Number of Columns in the X Direction:";
            // 
            // floornumberLabel
            // 
            m_floornumberLabel.Location = new Point(16, 184);
            m_floornumberLabel.Name = "m_floornumberLabel";
            m_floornumberLabel.Size = new Size(144, 23);
            m_floornumberLabel.TabIndex = 17;
            m_floornumberLabel.Text = "Number of Floors:";
            // 
            // cancelButton
            // 
            m_cancelButton.DialogResult = DialogResult.Cancel;
            m_cancelButton.Location = new Point(392, 208);
            m_cancelButton.Name = "m_cancelButton";
            m_cancelButton.TabIndex = 9;
            m_cancelButton.Text = "&Cancel";
            m_cancelButton.Click += cancelButton_Click;
            // 
            // unitLabel
            // 
            m_unitLabel.Location = new Point(136, 42);
            m_unitLabel.Name = "m_unitLabel";
            m_unitLabel.Size = new Size(32, 23);
            m_unitLabel.TabIndex = 18;
            m_unitLabel.Text = "feet";
            // 
            // CreateBeamsColumnsBracesForm
            // 
            AcceptButton = m_okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = m_cancelButton;
            ClientSize = new Size(546, 246);
            Controls.Add(m_unitLabel);
            Controls.Add(m_cancelButton);
            Controls.Add(m_floornumberLabel);
            Controls.Add(m_xLabel);
            Controls.Add(m_yLabel);
            Controls.Add(m_distanceLabel);
            Controls.Add(m_floornumberTextBox);
            Controls.Add(m_distanceTextBox);
            Controls.Add(m_yTextBox);
            Controls.Add(m_xTextBox);
            Controls.Add(m_braceLabel);
            Controls.Add(m_beamLabel);
            Controls.Add(m_columnLabel);
            Controls.Add(m_braceComboBox);
            Controls.Add(m_beamComboBox);
            Controls.Add(m_columnComboBox);
            Controls.Add(m_okButton);
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
            m_xTextBox.Text = "2";
            m_yTextBox.Text = "2";
            m_distanceTextBox.Text = 20.0.ToString("0.0");
            m_floornumberTextBox.Text = "1";
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

            m_columnComboBox.DataSource = m_dataBuffer.ColumnMaps;
            m_columnComboBox.DisplayMember = "SymbolName";
            m_columnComboBox.ValueMember = "ElementType";

            m_beamComboBox.DataSource = m_dataBuffer.BeamMaps;
            m_beamComboBox.DisplayMember = "SymbolName";
            m_beamComboBox.ValueMember = "ElementType";

            m_braceComboBox.DataSource = m_dataBuffer.BraceMaps;
            m_braceComboBox.DisplayMember = "SymbolName";
            m_braceComboBox.ValueMember = "ElementType";
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
                var xNumber = int.Parse(m_xTextBox.Text);
                var yNumber = int.Parse(m_yTextBox.Text);
                var distance = double.Parse(m_distanceTextBox.Text);
                var columnType = m_columnComboBox.SelectedValue;
                var beamType = m_beamComboBox.SelectedValue;
                var braceType = m_braceComboBox.SelectedValue;
                var floorNumber = int.Parse(m_floornumberTextBox.Text);

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
                distance = double.Parse(m_distanceTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please enter a value larger than 5 and less than 30000.");
                m_distanceTextBox.Text = "";
                m_distanceTextBox.Focus();
                return;
            }

            if (distance <= 5)
            {
                TaskDialog.Show("Revit", "Please enter a value larger than 5.");
                m_distanceTextBox.Text = "";
                m_distanceTextBox.Focus();
                return;
            }

            if (distance > 30000)
            {
                TaskDialog.Show("Revit", "Please enter a value less than 30000.");
                m_distanceTextBox.Text = "";
                m_distanceTextBox.Focus();
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
                xNumber = int.Parse(m_xTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for X direction between 1 to 20.");
                m_xTextBox.Text = "";
            }

            if (xNumber < 1 || xNumber > 20)
            {
                TaskDialog.Show("Revit", "Please input an integer for X direction between 1 to 20.");
                m_xTextBox.Text = "";
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
                yNumber = int.Parse(m_yTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for Y direction between 1 to 20.");
                m_yTextBox.Text = "";
            }

            if (yNumber < 1 || yNumber > 20)
            {
                TaskDialog.Show("Revit", "Please input an integer for Y direction between 1 to 20.");
                m_yTextBox.Text = "";
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
                floorNumber = int.Parse(m_floornumberTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", "Please input an integer for the number of floors between 1 to 10.");
                m_floornumberTextBox.Text = "";
            }

            if (floorNumber < 1 || floorNumber > 10)
            {
                TaskDialog.Show("Revit", "Please input an integer for the number of floors between 1 to 10.");
                m_floornumberTextBox.Text = "";
            }
        }
    }
}
