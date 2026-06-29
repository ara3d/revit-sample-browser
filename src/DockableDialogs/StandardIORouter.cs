// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS
{
    /// <summary>
    ///     Routes Console.WriteLine and other standard IO to a TextBox.
    /// </summary>
    public class StandardIoRouter : TextWriter
    {
        private readonly TextBox m_outputTextBox;

        public StandardIoRouter(TextBox output) => m_outputTextBox = output;

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char oneCharacter)
        {
            m_outputTextBox.AppendText(oneCharacter.ToString());
            m_outputTextBox.ScrollToEnd();
        }
    }
}
