// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.UIAPI.CS.OptionsDialog
{
    public partial class UserControl3 : UserControl
    {
        private readonly string m_name;

        public UserControl3(string name)
        {
            InitializeComponent();

            m_name = name;

            image1.Source = BitmapHelper.GetBitmapAsImageSource(Properties.Resources.autodesk);
        }

        public void OnOK() => TaskDialog.Show("OK", m_name);

        public void OnCancel() => TaskDialog.Show("OnCancel", m_name);

        public void OnRestoreDefaults() => TaskDialog.Show("OnRestoreDefaults", m_name);
    }
}
