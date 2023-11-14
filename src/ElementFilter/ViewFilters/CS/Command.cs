// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ViewFilters.CS
{
    /// <summary>
    ///     To add an external command to Autodesk Revit,
    ///     the developer must define an class which implement the IExternalCommand interface.
    ///     This class is used as the connection of revit and external program
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                // create a form to display the information of view filters
                using (var infoForm = new ViewFiltersForm(commandData))
                {
                    infoForm.ShowDialog();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If there is something wrong, give error information and return failed
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
