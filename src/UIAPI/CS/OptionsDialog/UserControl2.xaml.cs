// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.UIAPI.CS
{
    /// <summary>
    ///     Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        private readonly string _name;

        public UserControl2(string name)
        {
            InitializeComponent();
            _name = name;
        }

        private void onbtn_click(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("Hello", _name);
        }

        public void OnOK()
        {
            TaskDialog.Show("OK", _name);
        }

        public void OnCancel()
        {
            TaskDialog.Show("OnCancel", _name);
        }

        public void OnRestoreDefaults()
        {
            TaskDialog.Show("OnRestoreDefaults", _name);
        }
    }
}
