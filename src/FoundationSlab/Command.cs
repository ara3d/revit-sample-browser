// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.FoundationSlab.CS
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
                if (null == commandData) return Result.Failed;

                SlabData revitDatas = null;
                try
                {
                    revitDatas = new SlabData(commandData.Application);
                }
                catch (NullReferenceException e)
                {
                    message = e.Message;
                    return Result.Cancelled;
                }

                using FoundationSlabForm displayForm = new(revitDatas);
                return displayForm.ShowDialog() == DialogResult.OK ? Result.Succeeded : Result.Cancelled;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }
    }
}
