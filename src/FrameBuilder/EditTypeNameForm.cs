// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    /// <summary>
    ///     form to edit Type's name
    /// </summary>
    public partial class EditTypeNameForm : Form
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="typeName">original Type name</param>
        public EditTypeNameForm(string typeName)
        {
            InitializeComponent();

            typeNameTextBox.Text = typeName;
        }

        /// <summary>
        ///     constructor without constructor is forbidden
        /// </summary>
        private EditTypeNameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Type name to be edited
        /// </summary>
        public string TypeName => typeNameTextBox.Text;

        /// <summary>
        ///     return with OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     return with Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
