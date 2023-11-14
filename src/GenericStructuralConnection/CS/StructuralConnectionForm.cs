// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.GenericStructuralConnection.CS
{
    /// <summary>
    /// </summary>
    public partial class StructuralConnectionForm : Form
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public StructuralConnectionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Property set with the user's choice.
        /// </summary>
        public CommandOption UserOption { get; set; }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (rbCreateGeneric.Checked)
                UserOption = CommandOption.CreateGeneric;
            else if (rbDeleteGeneric.Checked)
                UserOption = CommandOption.DeleteGeneric;
            else if (rbReadGeneric.Checked)
                UserOption = CommandOption.ReadGeneric;
            else if (rbDeleteGeneric.Checked)
                UserOption = CommandOption.DeleteGeneric;
            else if (rbUpdateGeneric.Checked)
                UserOption = CommandOption.UpdateGeneric;
            else if (rbCreateDetailed.Checked)
                UserOption = CommandOption.CreateDetailed;
            else if (rbChangedDetail.Checked)
                UserOption = CommandOption.ChangeDetailed;
            else if (rbCopyDetailed.Checked)
                UserOption = CommandOption.CopyDetailed;
            else if (rbMatchPropDetailed.Checked)
                UserOption = CommandOption.MatchPropDetailed;
            else if (rbResetDetailed.Checked)
                UserOption = CommandOption.ResetDetailed;
        }
    }
}
