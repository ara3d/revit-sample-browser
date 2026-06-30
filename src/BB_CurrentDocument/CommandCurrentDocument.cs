using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples
{
    /// <summary>
    /// Displays the current active document in a window
    /// </summary>
    public class CommandCurrentDocument : NamedCommand
    {
        public override string Name => "Current Open Document";

        public override void Execute(object arg)
        {
            var app = (UIApplication)arg;
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
            {
                MessageBox.Show("No document open");
            }
            else
            {
                MessageBox.Show($"Open document: {doc.PathName}");
            }
        }
    }
}