// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System.Windows.Controls;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS.OptionsDialog
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options
    {
        public Options()
        {
            InitializeComponent();
            var options = ApplicationOptions.Get();
            ButtonAccessibility.SelectedIndex = (int)options.Availability;
        }

        public void OnOK()
        {
            ApplicationOptions.Get().Availability = (ApplicationAvailablity)ButtonAccessibility.SelectedIndex;
        }

        public void OnRestoreDefaults()
        {
            ButtonAccessibility.SelectedIndex = 0;
        }

        private bool GetCheckbuttonChecked(CheckBox checkBox)
        {
            return checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
        }
    }
}
