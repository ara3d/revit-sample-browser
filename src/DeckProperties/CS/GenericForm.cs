// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Windows.Forms;

namespace Revit.SDK.Samples.DeckProperties.CS
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
