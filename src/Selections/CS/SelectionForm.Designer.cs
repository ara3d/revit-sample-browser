// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace RevitMultiSample.Selections.CS
{
    partial class SelectionForm
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
            this.PickElementButton = new System.Windows.Forms.Button();
            this.MoveToButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PickElementButton
            // 
            this.PickElementButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.PickElementButton.Location = new System.Drawing.Point(21, 27);
            this.PickElementButton.Name = "PickElementButton";
            this.PickElementButton.Size = new System.Drawing.Size(104, 23);
            this.PickElementButton.TabIndex = 0;
            this.PickElementButton.Text = "Pick Element";
            this.PickElementButton.UseVisualStyleBackColor = true;
            this.PickElementButton.Click += new System.EventHandler(this.PickElementButton_Click);
            // 
            // MoveToButton
            // 
            this.MoveToButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.MoveToButton.Location = new System.Drawing.Point(21, 67);
            this.MoveToButton.Name = "MoveToButton";
            this.MoveToButton.Size = new System.Drawing.Size(104, 23);
            this.MoveToButton.TabIndex = 0;
            this.MoveToButton.Text = "Move To";
            this.MoveToButton.UseVisualStyleBackColor = true;
            this.MoveToButton.Click += new System.EventHandler(this.MoveToButton_Click);
            // 
            // SelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 117);
            this.Controls.Add(this.MoveToButton);
            this.Controls.Add(this.PickElementButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select";
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.Button PickElementButton;
        private System.Windows.Forms.Button MoveToButton;
    }
}
