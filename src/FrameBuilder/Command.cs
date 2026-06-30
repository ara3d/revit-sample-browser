// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FrameBuilder.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var data = FrameData.CreateInstance(commandData);
            using (var framingForm = new CreateFrameForm(data))
            {
                if (framingForm.ShowDialog() == DialogResult.OK)
                {
                    var builder = new FrameBuilder(data);
                    builder.CreateFraming();
                }
                else
                {
                    return Result.Cancelled;
                }
            }

            return Result.Succeeded;
        }
    }
}
