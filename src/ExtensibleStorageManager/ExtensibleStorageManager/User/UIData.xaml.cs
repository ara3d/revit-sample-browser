// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User
{
    /// <summary>
    ///     A class for a Window with a TextBox for displaying many lines of text data.
    /// </summary>
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
