using System.Windows.Controls;

namespace Revit.SDK.Samples.UIAPI.CS.OptionsDialog
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