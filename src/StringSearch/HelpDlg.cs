#region Copyright
// (C) Copyright 2011-2014 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
#endregion // Copyright

#region Namespaces
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
#endregion

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    public partial class HelpDlg : Form
    {
        static readonly string HelpTextResourceName =
          "Ara3D.RevitSampleBrowser.StringSearch.help_text.rtf";

        public HelpDlg()
        {
            InitializeComponent();
        }

        private void HelpDlg_Load(object sender, EventArgs e)
        {
            var exe = AboutBox.ExecutingAssembly;

            var s = exe.GetManifestResourceStream(HelpTextResourceName);

            richTextBox1.LoadFile(s, RichTextBoxStreamType.RichText);
        }
    }
}
