// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows;
using System.Windows.Controls;
using SelectionChangedEventArgs = Autodesk.Revit.UI.Events.SelectionChangedEventArgs;

namespace Ara3D.RevitSampleBrowser.SelectionChanged.CS
{
    /// <summary>
    ///     Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="application"></param>
        public InfoWindow()
        {
            InitializeComponent();
        }

        internal void RevitUIApp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox_log.Text += e.GetInfo(true);
        }

        private void textBox_log_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_log.ScrollToEnd();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textBox_log.Text = "";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SelectionChanged.InfoWindow = null;
        }
    }
}
