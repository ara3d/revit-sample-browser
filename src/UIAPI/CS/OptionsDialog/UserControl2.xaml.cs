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
        private readonly string m_name;

        public UserControl2(string name)
        {
            InitializeComponent();
            m_name = name;
        }

        private void onbtn_click(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("Hello", m_name);
        }

        public void OnOK()
        {
            TaskDialog.Show("OK", m_name);
        }

        public void OnCancel()
        {
            TaskDialog.Show("OnCancel", m_name);
        }

        public void OnRestoreDefaults()
        {
            TaskDialog.Show("OnRestoreDefaults", m_name);
        }
    }
}
