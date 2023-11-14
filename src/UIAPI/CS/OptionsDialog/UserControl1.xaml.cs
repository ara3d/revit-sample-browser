// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.UIAPI.CS
{
    /// <summary>
    ///     Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private readonly string _name;

        public UserControl1()
        {
            InitializeComponent();


            var memberData = new ObservableCollection<Member>
            {
                new Member
                    { Name = "Joe", Age = "23", Pass = true, Email = new Uri("mailto:Joe@school.com") },
                new Member
                    { Name = "Mike", Age = "20", Pass = false, Email = new Uri("mailto:Mike@school.com") },
                new Member
                    { Name = "Lucy", Age = "25", Pass = true, Email = new Uri("mailto:Lucy@school.com") }
            };
            dataGrid1.DataContext = memberData;
            _name = "WPF components";
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

    public enum SexOpt
    {
        Male,
        Female
    }

    public class Member
    {
        public string Name { get; set; }

        public string Age { get; set; }

        public bool Pass { get; set; }

        public Uri Email { get; set; }
    }
}
