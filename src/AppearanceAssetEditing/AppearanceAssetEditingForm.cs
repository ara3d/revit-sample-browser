// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AppearanceAssetEditing.CS
{
    public partial class AppearanceAssetEditingForm : Form
    {
        private Request m_request;

        public AppearanceAssetEditingForm()
        {
            InitializeComponent();

            m_request = new Request();
        }

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

        public void EnableButtons(bool bLighterStatus, bool bDarkerStatus)
        {
            buttonLighter.Enabled = bLighterStatus;
            buttonDarker.Enabled = bDarkerStatus;
            buttonSelect.Enabled = true;
        }

        // A private helper method to make a request
        // and put the dialog to sleep at the same time.
        // It is expected that the process which executes the request
        // (the Idling helper in this particular case) will also
        // wake the dialog up after finishing the execution.
        private void MakeRequest(RequestId request)
        {
            Request.Make(request);
            EnableButtons(false, false);
        }
    }
}
