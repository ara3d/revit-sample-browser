// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.DeckProperties.CS
{
    public partial class DeckPropertyForm : Form
    {
        public DeckPropertyForm()
        {
            InitializeComponent();
        }

        public void WriteLine(string message)
        {
            propertyListBox.Items.Add(message);
        }
    }
}
