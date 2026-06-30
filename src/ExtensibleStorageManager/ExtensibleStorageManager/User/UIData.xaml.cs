// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User
{
    public partial class UiData : Window
    {
        public UiData()
        {
            InitializeComponent();
        }

        public void SetData(string data)
        {
            m_tb_Data.Text = data;
        }
    }
}
