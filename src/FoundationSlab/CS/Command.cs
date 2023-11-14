// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.FoundationSlab.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Check commandData parameter.
                if (null == commandData) return Result.Failed;

                SlabData revitDatas = null;
                try
                {
                    // The Datas for UI.
                    revitDatas = new SlabData(commandData.Application);
                }
                catch (NullReferenceException e)
                {
                    message = e.Message;
                    return Result.Cancelled; // Data error.
                }

                // Display form.
                using (var displayForm = new FoundationSlabForm(revitDatas))
                {
                    if (displayForm.ShowDialog() == DialogResult.OK)
                        return Result.Succeeded; // Create foundation slabs successfully.
                }

                return Result.Cancelled; // Cancel creation.
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed; // Unknow error.
            }
        }
    }
}
