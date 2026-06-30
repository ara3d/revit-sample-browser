// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    public partial class EditTypeNameForm : Form
    {
        public EditTypeNameForm(string typeName)
        {
            InitializeComponent();

            typeNameTextBox.Text = typeName;
        }

        private EditTypeNameForm()
        {
            InitializeComponent();
        }

        public string TypeName => typeNameTextBox.Text;

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
