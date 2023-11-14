// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FamilyParametersOrder.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     Used to disable SortLoadedFamiliesParamsForm opened when document is opened.
        /// </summary>
        public static bool m_SortDialogIsOpened = true;

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // Show UI
                using (var form = new SortFamilyFilesParamsForm(commandData.Application))
                {
                    m_SortDialogIsOpened = false;
                    form.ShowDialog();
                    m_SortDialogIsOpened = true;
                    return Result.Succeeded;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
