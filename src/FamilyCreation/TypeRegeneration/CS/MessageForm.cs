// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Revit.SDK.Samples.TypeRegeneration.CS
{
    /// <summary>
    ///     The form is used to show the result
    /// </summary>
    public partial class MessageForm : Form
    {
        /// <summary>
        ///     new a Timer,set the interval 2 seconds
        /// </summary>
        private readonly Timer timer = new Timer(2000);

        /// <summary>
        ///     construction of MessageForm
        /// </summary>
        public MessageForm()
        {
            InitializeComponent();
            Text = "Type Regeneration Message Form";
            //set the timer elapsed event
            timer.Elapsed += onTimeOut; //Set the executed event when time is out          
            timer.Enabled = false;
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        ///     add text to the richtextbox and set time enable is true, then timer starts timing
        /// </summary>
        /// <param name="message">message from the regeneration</param>
        /// <param name="enableTimer">enable or disable the timer elapsed event</param>
        public void AddMessage(string message, bool enableTimer)
        {
            messageRichTextBox.AppendText(message);
            timer.Enabled = enableTimer;
        }

        /// <summary>
        ///     the method is executed when time is out, and set the timer enabled false,then timer stop timing
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">time elapsed event args</param>
        private void onTimeOut(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            Close();
        }
    }
}
