// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Ara3D.RevitSampleBrowser.Events.PrintLog.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                List<Element> viewElems = new();
                FilteredElementCollector collector = new(document);
                viewElems.AddRange(collector.OfClass(typeof(View)).ToElements());

                ViewSet printableViews = new();
                foreach (View view in viewElems)
                {
                    // View templates cannot be printed.
                    if (!view.IsTemplate && view.CanBePrinted)
                        printableViews.Insert(view);
                }

                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var pm = document.PrintManager;
                pm.PrintToFile = true;
                pm.PrintToFileName = $"{assemblyPath}\\PrintOut.prn";
                pm.Apply();
                document.Print(printableViews);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
