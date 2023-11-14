// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Windows;

namespace ExtensibleStorageManager
{
    /// <summary>
    ///     A class for a Window with a TextBox for displaying many lines of text data.
    /// </summary>
    public partial class UIData : Window
    {
        public UIData()
        {
            InitializeComponent();
        }

        public void SetData(string data)
        {
            m_tb_Data.Text = data;
        }
    }
}
