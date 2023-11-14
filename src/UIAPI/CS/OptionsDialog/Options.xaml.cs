using System.Windows.Controls;

namespace Revit.SDK.Samples.UIAPI.CS.OptionsDialog
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        public Options()
        {
            InitializeComponent();

            var options = ApplicationOptions.Get();


            ButtonAccessibility.SelectedIndex = (int)options.Availability;
        }

        public void OnOK()
        {
            var options = ApplicationOptions.Get();

            options.Availability = (ApplicationAvailablity)ButtonAccessibility.SelectedIndex;

        }

        public void OnRestoreDefaults()
        {
            ButtonAccessibility.SelectedIndex = 0;
        }

        private bool GetCheckbuttonChecked(CheckBox checkBox)
        {
            if (checkBox.IsChecked.HasValue)
                return checkBox.IsChecked.Value;
            return false;
        }

    }
}
