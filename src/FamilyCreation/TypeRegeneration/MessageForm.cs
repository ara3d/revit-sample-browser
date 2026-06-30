// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.TypeRegeneration.CS
{
    public partial class MessageForm : Form
    {
        private readonly Timer m_timer = new(2000);

        public MessageForm()
        {
            InitializeComponent();
            Text = "Type Regeneration Message Form";
            m_timer.Elapsed += OnTimeOut;
            m_timer.Enabled = false;
            CheckForIllegalCrossThreadCalls = false;
        }

        public void AddMessage(string message, bool enableTimer)
        {
            messageRichTextBox.AppendText(message);
            m_timer.Enabled = enableTimer;
        }

        private void OnTimeOut(object source, ElapsedEventArgs e)
        {
            m_timer.Enabled = false;
            Close();
        }
    }
}
