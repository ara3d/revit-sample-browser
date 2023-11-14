// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FrameBuilder.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // try to initialize necessary data to create framing
            var data = FrameData.CreateInstance(commandData);
            // display UI for user's input
            using (var framingForm = new CreateFrameForm(data))
            {
                if (framingForm.ShowDialog() == DialogResult.OK)
                {
                    // create framing
                    var builder = new FrameBuilder(data);
                    builder.CreateFraming();
                }
                else
                {
                    // cancel the command
                    return Result.Cancelled;
                }
            }

            return Result.Succeeded;
        }
    }
}
