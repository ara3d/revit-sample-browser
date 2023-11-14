// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.AppearanceAssetEditing.CS
{
    /// <summary>
    ///     The class of our modeless dialog.
    /// </summary>
    public partial class AppearanceAssetEditingForm : Form
    {
        /// <summary>
        ///     In this sample, the dialog owns the value of the request but it is not necessary. It may as
        ///     well be a static property of the application.
        /// </summary>
        private Request m_request;

        /// <summary>
        ///     Constructor
        /// </summary>
        public AppearanceAssetEditingForm()
        {
            InitializeComponent();

            m_request = new Request();
        }

        /// <summary>
        ///     Request property
        /// </summary>
        public Request Request
        {
            get => m_request;
            private set => m_request = value;
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Select);
        }

        private void buttonLighter_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Lighter);
        }

        private void buttonDarker_Click(object sender, EventArgs e)
        {
            MakeRequest(RequestId.Darker);
        }

        /// <summary>
        ///     Enable buttons or not.
        /// </summary>
        public void EnableButtons(bool bLighterStatus, bool bDarkerStatus)
        {
            buttonLighter.Enabled = bLighterStatus;
            buttonDarker.Enabled = bDarkerStatus;
            buttonSelect.Enabled = true;
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
            EnableButtons(false, false);
        }
    }
}
