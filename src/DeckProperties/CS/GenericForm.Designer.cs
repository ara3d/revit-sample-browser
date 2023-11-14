// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.DeckProperties.CS
{
    partial class DeckPropertyForm
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

        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyListBox
            // 
            this.propertyListBox.FormattingEnabled = true;
            this.propertyListBox.Location = new System.Drawing.Point(12, 12);
            this.propertyListBox.Name = "propertyListBox";
            this.propertyListBox.ScrollAlwaysVisible = true;
            this.propertyListBox.Size = new System.Drawing.Size(431, 355);
            this.propertyListBox.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(287, 373);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(368, 373);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // DeckPropertyForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(456, 400);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.propertyListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeckPropertyForm";
            this.ShowInTaskbar = false;
            this.Text = "Deck Properties";
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.ListBox propertyListBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
