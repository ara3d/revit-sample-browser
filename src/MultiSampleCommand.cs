// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample
{
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MultiSampleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var form = new MultiSampleMainForm();

            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IExternalCommand).IsAssignableFrom(t)
                || typeof(IExternalApplication).IsAssignableFrom(t))
                .OrderBy(t => t.Name);

            var header1 = new ColumnHeader
            {
                Text = "Name",
                Width = 400
            };

            var header2 = new ColumnHeader
            {
                Text = "Type",
                Width = 200
            };

            form.listView1.Columns.Add(header1);
            form.listView1.Columns.Add(header2);

            foreach (var t in types)
            {
                var item = form.listView1.Items.Add(t.FullName);
                var isCommand = typeof(IExternalCommand).IsAssignableFrom(t);
                item.SubItems.Add(isCommand ? "IExternalCommand" : "IExternalApplication");
            }

            form.Show();
            return Result.Succeeded;
        }
    }
}
