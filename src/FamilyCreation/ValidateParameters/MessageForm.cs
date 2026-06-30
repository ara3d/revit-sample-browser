// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.ValidateParameters.CS
{
    public partial class MessageForm : Form
    {
        private readonly string m_logFileName;

        public MessageForm()
        {
            InitializeComponent();
        }

        public MessageForm(string[] messages)
            : this()
        {
            var msgText = "";
            Text = "Validate Parameters Message Form";
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_logFileName = $"{assemblyPath}\\ValidateParametersLog.txt";
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
                {
                    if (row == null)
                    {
                    }
                    else
                    {
                        WriteLog(row);
                        msgText += $"{row}\n";
                    }
                }
            }

            msgText +=
                $"\n\nIf you want to know the validating parameters result, please get the log file at \n{m_logFileName}";
            messageRichTextBox.Text = msgText;
            StartPosition = FormStartPosition.CenterParent;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void WriteLog(string logStr)
        {
            var writer = File.AppendText(m_logFileName);
            writer.WriteLine(logStr);
            writer.Close();
        }
    }
}
