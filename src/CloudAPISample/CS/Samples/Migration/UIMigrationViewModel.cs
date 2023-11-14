// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

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
using System.ComponentModel;

namespace Revit.SDK.Samples.CloudAPISample.CS.Migration
{
    /// <summary>
    ///     Provide user BIM360 project information and configuration for this sample.
    ///     Can be modified via ViewInputMigrationInfo dialog
    /// </summary>
    public class UIMigrationViewModel : IMigrationModel, INotifyPropertyChanged
    {
        private string accountGuid;

        private string projectGuid;

        /// <inheritdoc />
        public ObservableCollection<FolderLocation> AvailableFolders { get; set; } = new ObservableCollection<FolderLocation>();

        /// <inheritdoc />
        public ObservableCollection<MigrationRule> Rules { get; set; } = new ObservableCollection<MigrationRule>();

        /// <inheritdoc />
        public string AccountGuid
        {
            get => accountGuid;
            set
            {
                if (accountGuid != value)
                {
                    accountGuid = value;
                    NotifyPropertyChanged("AccountGuid");
                }
            }
        }

        /// <inheritdoc />
        public string ProjectGuid
        {
            get => projectGuid;
            set
            {
                if (projectGuid != value)
                {
                    projectGuid = value;
                    NotifyPropertyChanged("ProjectGuid");
                }
            }
        }

        /// <summary>
        ///     Indicates target folder has been changed in this case.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
