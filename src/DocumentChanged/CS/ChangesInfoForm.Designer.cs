// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace Revit.SDK.Samples.ChangesMonitor.CS
{
    partial class ChangesInformationForm
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
            this.changesdataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.changesdataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // changesdataGridView
            // 
            this.changesdataGridView.AllowUserToAddRows = false;
            this.changesdataGridView.AllowUserToDeleteRows = false;
            this.changesdataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.changesdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.changesdataGridView.Location = new System.Drawing.Point(-1, -2);
            this.changesdataGridView.Name = "changesdataGridView";
            this.changesdataGridView.ReadOnly = true;
            this.changesdataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.changesdataGridView.Size = new System.Drawing.Size(665, 81);
            this.changesdataGridView.TabIndex = 0;
            this.changesdataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.changesdataGridView_RowsAdded);
            // 
            // ChangesInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 77);
            this.Controls.Add(this.changesdataGridView);
            this.MaximizeBox = false;
            this.Name = "ChangesInformationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Changes Information";
            this.Shown += new System.EventHandler(this.ChangesInfoForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.changesdataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.DataGridView changesdataGridView;
    }
}
