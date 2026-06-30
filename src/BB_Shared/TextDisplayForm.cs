using Ara3D.Logging;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples
{
    public class TextDisplayForm : System.Windows.Forms.Form
    {
        private readonly System.Windows.Forms.TextBox textBox;

        public TextDisplayForm(string text)
        {
            Text = "Multi-line Text";
            Size = new System.Drawing.Size(400, 300);

            textBox = new System.Windows.Forms.TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Text = text
            };

            Controls.Add(textBox);
        }

        public void AddLine(string s)
        {
            textBox.AppendText(s + Environment.NewLine);
        }

        public ILogger CreateLogger()
        {
            return new Logger(LogWriter.Create(AddLine), "");
        }

        public static TextDisplayForm DisplayText(IEnumerable<string> lines)
        {
            return DisplayText(string.Join("\r\n", lines));
        }

        public static TextDisplayForm DisplayText(string text)
        {
            TextDisplayForm form = new(text);
            form.Show();
            return form;
        }

        public void SetText(string s)
        {
            textBox.Text = s;
        }
    }
}