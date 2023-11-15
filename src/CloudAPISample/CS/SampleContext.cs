// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System.Windows.Controls;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CloudAPISample.CS
{
    /// <summary>
    ///     Base class for each sample context in samples portal.
    /// </summary>
    public abstract class SampleContext
    {
        /// <summary>
        ///     The root node for this sample
        /// </summary>
        public UserControl View { get; set; }

        /// <summary>
        ///     Gives each sample the ability to access Revit application
        /// </summary>
        public UIApplication Application { get; set; }

        /// <summary>
        ///     Terminate this sample context, resource allocated should be released here
        /// </summary>
        public abstract void Terminate();
    }
}
