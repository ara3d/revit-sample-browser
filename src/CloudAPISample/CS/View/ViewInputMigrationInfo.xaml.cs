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

using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Windows;
using Ara3D.RevitSampleBrowser.CloudAPISample.CS.Samples.Migration;
using Microsoft.Win32;

namespace Ara3D.RevitSampleBrowser.CloudAPISample.CS.View
{
    /// <summary>
    ///     Interaction logic for ViewInputMigrationInfo.xaml
    /// </summary>
    public partial class ViewInputMigrationInfo : Window
    {
        /// <summary>
        ///     Constructor for ViewSamplePortal
        ///     ViewSamplePortal is a child window aggregating all sample case with tabs
        /// </summary>
        public ViewInputMigrationInfo(MigrationToBim360 sampleContext)
        {
            DataContext = sampleContext;
            InitializeComponent();
        }

        private void OnBtnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            ((MigrationToBim360)DataContext).Model.AvailableFolders.Add(new FolderLocation());
        }

        private void OnBtnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            var model = ((MigrationToBim360)DataContext).Model;
            if (lvFolders.SelectedIndex >= 0 && lvFolders.SelectedIndex < model.AvailableFolders.Count)
                model.AvailableFolders.RemoveAt(lvFolders.SelectedIndex);
        }

        private void OnBtnImport_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Json file (*.json)|*.json|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                var model = ((MigrationToBim360)DataContext).Model;
                var jsonString = File.ReadAllText(openFileDialog.FileName);
                var serializer = new JavaScriptSerializer();
                var info = serializer.Deserialize<SerializableProjectInfo>(jsonString);

                model.AccountGuid = info.AccountGuid;
                model.ProjectGuid = info.ProjectGuid;
                model.AvailableFolders.Clear();
                foreach (var folder in info.AvailableFolders)
                {
                    model.AvailableFolders.Add(folder);
                }
            }
        }

        private void OnBtnExport_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Json file (*.json)|*.json" };
            if (saveFileDialog.ShowDialog() == true)
            {
                var model = ((MigrationToBim360)DataContext).Model;
                var info = new SerializableProjectInfo
                {
                    AccountGuid = model.AccountGuid,
                    ProjectGuid = model.ProjectGuid,
                    AvailableFolders = model.AvailableFolders.ToArray()
                };
                var serializer = new JavaScriptSerializer();
                var jsonString = serializer.Serialize(info);
                File.WriteAllText(saveFileDialog.FileName, jsonString);
            }
        }

        [DataContract]
        public class SerializableProjectInfo
        {
            [DataMember]
            public string AccountGuid { get; set; }

            [DataMember]
            public string ProjectGuid { get; set; }

            [DataMember]
            public FolderLocation[] AvailableFolders { get; set; }
        }
    }
}
