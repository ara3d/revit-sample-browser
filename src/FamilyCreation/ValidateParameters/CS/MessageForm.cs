// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RevitMultiSample.ValidateParameters.CS
{
    /// <summary>
    ///     The form is used to show the result
    /// </summary>
    public partial class MessageForm : Form
    {
        /// <summary>
        ///     store the log file name
        /// </summary>
        private readonly string m_logFileName;

        /// <summary>
        ///     construction of form
        /// </summary>
        public MessageForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     construction method with parameter
        /// </summary>
        /// <param name="messages">messages</param>
        public MessageForm(string[] messages)
            : this()
        {
            var msgText = "";
            //If the size of error messages is 0, means the validate parameters is successful
            Text = "Validate Parameters Message Form";
            //create regeneration log file
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_logFileName = assemblyPath + "\\ValidateParametersLog.txt";
            var writer = File.CreateText(m_logFileName);
            writer.Close();
            if (messages.Length == 0)
            {
                msgText = "All types and parameters passed the validation for API";
                WriteLog(msgText);
                messageRichTextBox.Text = msgText;
            }
            else
            {
                foreach (var row in messages)
                    if (row == null)
                    {
                    }
                    else
                    {
                        WriteLog(row);
                        msgText += row + "\n";
                    }
            }

            msgText += "\n\nIf you want to know the validating parameters result, please get the log file at \n" +
                       m_logFileName;
            messageRichTextBox.Text = msgText;
            StartPosition = FormStartPosition.CenterParent;
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        ///     The method is used to write line to log file
        /// </summary>
        /// <param name="logStr">the log string</param>
        private void WriteLog(string logStr)
        {
            var writer = File.AppendText(m_logFileName);
            writer.WriteLine(logStr);
            writer.Close();
        }
    }
}
