// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

 
namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    partial class EventsInfoWindows
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
            this.appEventsLogDataGridView = new System.Windows.Forms.DataGridView();
            this.timeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.appEventsLogDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // appEventsLogDataGridView
            // 
            this.appEventsLogDataGridView.AllowUserToAddRows = false;
            this.appEventsLogDataGridView.AllowUserToDeleteRows = false;
            this.appEventsLogDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.appEventsLogDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.appEventsLogDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.timeColumn,
            this.eventColumn,
            this.typeColumn});
            this.appEventsLogDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appEventsLogDataGridView.Location = new System.Drawing.Point(0, 0);
            this.appEventsLogDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.appEventsLogDataGridView.Name = "appEventsLogDataGridView";
            this.appEventsLogDataGridView.ReadOnly = true;
            this.appEventsLogDataGridView.RowHeadersVisible = false;
            this.appEventsLogDataGridView.RowTemplate.Height = 24;
            this.appEventsLogDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.appEventsLogDataGridView.Size = new System.Drawing.Size(475, 116);
            this.appEventsLogDataGridView.TabIndex = 0;
            this.appEventsLogDataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.appEventsLogDataGridView_RowsAdded);
            // 
            // timeColumn
            // 
            this.timeColumn.HeaderText = "Time";
            this.timeColumn.Name = "timeColumn";
            this.timeColumn.ReadOnly = true;
            this.timeColumn.Width = 120;
            // 
            // eventColumn
            // 
            this.eventColumn.HeaderText = "Event";
            this.eventColumn.Name = "eventColumn";
            this.eventColumn.ReadOnly = true;
            this.eventColumn.Width = 150;
            // 
            // typeColumn
            // 
            this.typeColumn.HeaderText = "Type";
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.ReadOnly = true;
            this.typeColumn.Width = 200;
            // 
            // EventsInfoWindows
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 116);
            this.Controls.Add(this.appEventsLogDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EventsInfoWindows";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Events History";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.applicationEventsInfoWindows_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InformationWindows_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.appEventsLogDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        
        private System.Windows.Forms.DataGridView appEventsLogDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eventColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeColumn;
    }
}
