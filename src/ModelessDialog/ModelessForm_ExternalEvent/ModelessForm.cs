// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ModelessDialog.ModelessForm_ExternalEvent.CS
{
    /// <summary>
    ///     The class of our modeless dialog.
    /// </summary>
    /// <remarks>
    ///     Besides other methods, it has one method per each command button.
    ///     In each of those methods nothing else is done but raising an external
    ///     event with a specific request set in the request handler.
    /// </remarks>
    public partial class ModelessForm : Form
    {
        private ExternalEvent m_exEvent;
        // In this sample, the dialog owns the handler and the event objects,
        // but it is not a requirement. They may as well be static properties
        // of the application.

        private RequestHandler m_handler;

        public ModelessForm(ExternalEvent exEvent, RequestHandler handler)
        {
            InitializeComponent();
            m_handler = handler;
            m_exEvent = exEvent;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // we own both the event and the handler
            // we should dispose it before we are closed
            m_exEvent.Dispose();
            m_exEvent = null;
            m_handler = null;

            // do not forget to call the base class
            base.OnFormClosed(e);
        }

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
            m_handler.Request.Make(request);
            m_exEvent.Raise();
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
