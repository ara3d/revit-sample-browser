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

using System.Reflection;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.StringSearch.CS
{
    partial class AboutBox : Form
    {
        public const string AssemblyProduct = "Revit String Search";
        public const string AssemblyTitle = "Revit String Search";
        public const string AssemblyDescription =
            "Search Revit project elements and their parameter values for a given string.";
        public const string AssemblyCopyright = "(C) Copyright 2011-2014 by Autodesk, Inc.";
        public const string AssemblyCompany = "Autodesk, Inc.";
        public const string AssemblyVersion = "2016.0.0.0";

        public AboutBox()
        {
            InitializeComponent();

            Text = "About " + AssemblyTitle;
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = "Version " + AssemblyVersion;
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AssemblyDescription;
        }

        public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    }
}
