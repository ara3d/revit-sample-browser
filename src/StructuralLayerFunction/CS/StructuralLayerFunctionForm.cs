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


using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.StructuralLayerFunction.CS
{
    /// <summary>
    ///     display the function of each of a select floor's structural layers
    /// </summary>
    public class StructuralLayerFunctionForm : Form
    {
        private readonly Container components = null;
        private GroupBox functionGroupBox;
        private ListBox functionListBox;
        private Button okButton;


        /// <summary>
        ///     Constructor of StructuralLayerFunctionForm
        /// </summary>
        /// <param name="dataBuffer">A reference of StructuralLayerFunction class</param>
        public StructuralLayerFunctionForm(Command dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            // Set the data source of the ListBox control
            functionListBox.DataSource = dataBuffer.Functions;
        }


        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (null != components)
                    components.Dispose();
            base.Dispose(disposing);
        }


        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            functionListBox = new ListBox();
            functionGroupBox = new GroupBox();
            okButton = new Button();
            functionGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // functionListBox
            // 
            functionListBox.Location = new Point(6, 24);
            functionListBox.Name = "functionListBox";
            functionListBox.Size = new Size(189, 147);
            functionListBox.TabIndex = 0;
            // 
            // functionGroupBox
            // 
            functionGroupBox.Controls.Add(functionListBox);
            functionGroupBox.Location = new Point(12, 12);
            functionGroupBox.Name = "functionGroupBox";
            functionGroupBox.Size = new Size(201, 184);
            functionGroupBox.TabIndex = 1;
            functionGroupBox.TabStop = false;
            functionGroupBox.Text = "Layers Functions List";
            // 
            // okButton
            // 
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(138, 202);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            // 
            // StructuralLayerFunctionForm
            // 
            AcceptButton = okButton;
            AutoScaleBaseSize = new Size(5, 13);
            CancelButton = okButton;
            ClientSize = new Size(225, 236);
            Controls.Add(okButton);
            Controls.Add(functionGroupBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "StructuralLayerFunctionForm";
            ShowInTaskbar = false;
            Text = "Structure Layers Function";
            functionGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}