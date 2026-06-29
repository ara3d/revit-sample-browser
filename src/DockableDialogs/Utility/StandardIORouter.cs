// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.Utility
{
    /// <summary>
    ///     A simple utility class to route calls from Console.WriteLine and other standard IO to
    ///     a TextBox.  Note that one side effect of this system is that any time a host application calls
    ///     Console.WriteLine, cout, printf, or something similar, the output will be funneled through here,
    ///     giving occasional output you may not have expected.
    /// </summary>
    public class StandardIoRouter : TextWriter
    {
        /// <summary>
        ///     A stored reference of a textbox to output to.
        /// </summary>
        private readonly TextBox m_outputTextBox;

        /// <summary>
        ///     Create a new router given a WPF Textbox to output to.
        /// </summary>
        public StandardIoRouter(TextBox output)
        {
            m_outputTextBox = output;
        }

        /// <summary>
        ///     A default override to use UTF8 text
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>
        ///     Write a character from standardIO to a Textbox.
        /// </summary>
        public override void Write(char oneCharacter)
        {
            m_outputTextBox.AppendText(oneCharacter.ToString());
            m_outputTextBox.ScrollToEnd();
        }
    }
}
