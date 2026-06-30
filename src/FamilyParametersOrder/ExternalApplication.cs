// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.FamilyParametersOrder.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class ExternalApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                application.ControlledApplication.DocumentOpened += SortLoadedFamiliesParams;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public void SortLoadedFamiliesParams(object obj, DocumentOpenedEventArgs args)
        {
            if (!Command.SortDialogIsOpened)
                return;

            using SortLoadedFamiliesParamsForm sortForm = new(args.Document);
            sortForm.ShowDialog();
        }
    }
}
