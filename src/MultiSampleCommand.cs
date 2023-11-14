using System.Linq;
using System.Reflection;
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
                .Where(t => typeof(IExternalCommand).IsAssignableFrom(t))
                .OrderBy(t => t.Name);
            foreach (var t in types)
                form.listView1.Items.Add(t.Name);
            form.Show();
            return Result.Succeeded;
        }
    }
}