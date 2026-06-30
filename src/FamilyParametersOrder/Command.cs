// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        // When false, suppresses the DocumentOpened handler while the file-sort dialog is open.
        public static bool SortDialogIsOpened = true;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                using SortFamilyFilesParamsForm form = new(commandData.Application);
                SortDialogIsOpened = false;
                form.ShowDialog();
                SortDialogIsOpened = true;
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
