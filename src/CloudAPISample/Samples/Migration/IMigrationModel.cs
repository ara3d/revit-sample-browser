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

using System.Collections.ObjectModel;

namespace Ara3D.RevitSampleBrowser.CloudAPISample.CS.Samples.Migration
{
    public class FolderLocation
    {
        public string Name { get; set; }

        public string Urn { get; set; }
    }

    public class MigrationRule
    {
        public string Pattern { get; set; }

        public FolderLocation Target { get; set; }
    }

    public interface IMigrationModel
    {
        string AccountGuid { get; set; }

        string ProjectGuid { get; set; }

        ObservableCollection<FolderLocation> AvailableFolders { get; set; }

        ObservableCollection<MigrationRule> Rules { get; set; }
    }
}
