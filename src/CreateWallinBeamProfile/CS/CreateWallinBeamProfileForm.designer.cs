//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

namespace Revit.SDK.Samples.CreateWallinBeamProfile.CS
{
    partial class CreateWallinBeamProfileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.optionGroupBox = new System.Windows.Forms.GroupBox();
            this.wallTypeComboBox = new System.Windows.Forms.ComboBox();
            this.structualCheckBox = new System.Windows.Forms.CheckBox();
            this.wallTypeLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.optionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionGroupBox
            // 
            this.optionGroupBox.Controls.Add(this.wallTypeComboBox);
            this.optionGroupBox.Controls.Add(this.structualCheckBox);
            this.optionGroupBox.Controls.Add(this.wallTypeLabel);
            this.optionGroupBox.Location = new System.Drawing.Point(12, 25);
            this.optionGroupBox.Name = "optionGroupBox";
            this.optionGroupBox.Size = new System.Drawing.Size(268, 100);
            this.optionGroupBox.TabIndex = 0;
            this.optionGroupBox.TabStop = false;
            this.optionGroupBox.Text = "Create Option";
            // 
            // wallTypeComboBox
            // 
            this.wallTypeComboBox.FormattingEnabled = true;
            this.wallTypeComboBox.Location = new System.Drawing.Point(78, 29);
            this.wallTypeComboBox.Name = "wallTypeComboBox";
            this.wallTypeComboBox.Size = new System.Drawing.Size(184, 21);
            this.wallTypeComboBox.TabIndex = 1;
            this.wallTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            // 
            // structualCheckBox
            // 
            this.structualCheckBox.AutoSize = true;
            this.structualCheckBox.Location = new System.Drawing.Point(17, 65);
            this.structualCheckBox.Name = "structualCheckBox";
            this.structualCheckBox.Size = new System.Drawing.Size(103, 17);
            this.structualCheckBox.TabIndex = 2;
            this.structualCheckBox.Text = "Is Structual Wall";
            this.structualCheckBox.UseVisualStyleBackColor = true;
            // 
            // wallTypeLabel
            // 
            this.wallTypeLabel.AutoSize = true;
            this.wallTypeLabel.Location = new System.Drawing.Point(14, 32);
            this.wallTypeLabel.Name = "wallTypeLabel";
            this.wallTypeLabel.Size = new System.Drawing.Size(58, 13);
            this.wallTypeLabel.TabIndex = 0;
            this.wallTypeLabel.Text = "Wall Type:";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(50, 159);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(57, 25);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(179, 159);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(57, 25);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // CreateWallinBeamProfileForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 212);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.optionGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateWallinBeamProfileForm";
            this.ShowInTaskbar = false;
            this.Text = "Create Wall in Beam Profile";
            this.Load += new System.EventHandler(this.CreateWallinBeamProfileForm_Load);
            this.optionGroupBox.ResumeLayout(false);
            this.optionGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox optionGroupBox;
        private System.Windows.Forms.CheckBox structualCheckBox;
        private System.Windows.Forms.Label wallTypeLabel;
        private System.Windows.Forms.ComboBox wallTypeComboBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;

    }
}