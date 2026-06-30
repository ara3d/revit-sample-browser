// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ModelessDialog.ModelessForm_IdlingEvent.CS
{
    /// <summary>
    ///     The class of our modeless dialog.
    /// </summary>
    /// <remarks>
    ///     Besides other methods, it has one method per each command button.
    ///     In each of those methods nothing else is done but setting a request
    ///     to be later picked up by the Idling handler. All those commands
    ///     are performed on doors currently selected in the active document.
    /// </remarks>
    public partial class ModelessForm : Form
    {
        public ModelessForm()
        {
            InitializeComponent();
            Request = new Request();
        }

        public Request Request { get; private set; }

        private void EnableCommands(bool status)
        {
            foreach (Control ctrl in Controls)
            {
                ctrl.Enabled = status;
            }

            if (!status) btnExit.Enabled = true;
        }

        /// <summary>
        ///     A private helper method to make a request
        ///     and put the dialog to sleep at the same time.
        /// </summary>
        /// <remarks>
        ///     It is expected that the process which executes the request
        ///     (the Idling helper in this particular case) will also
        ///     wake the dialog up after finishing the execution.
        /// </remarks>
        private void MakeRequest(RequestId request)
        {
            Request.Make(request);
            DozeOff();
        }

        /// <summary>
        ///     DozeOff -> disable all controls (but the Exit button)
        /// </summary>
        private void DozeOff()
        {
            EnableCommands(false);
        }

        public void WakeUp()
        {
            EnableCommands(true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFlipLeft_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.MakeLeft);
        }

        private void btnFlipRight_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.MakeRight);
        }

        /// <summary>
        ///     Flipping a door between Right and Left
        /// </summary>
        private void btnFlipLeftRight_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.FlipLeftRight);
        }

        /// <summary>
        ///     Flipping a door between facing In and Out
        /// </summary>
        private void btnFlipInOut_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.FlipInOut);
        }

        private void btnFlipOut_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.TurnOut);
        }

        private void btnFlipIn_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.TurnIn);
        }

        /// <summary>
        ///     Turning a door around - flipping both hand and face
        /// </summary>
        private void btnRotate_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Rotate);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Delete);
        }
    } // class
}
