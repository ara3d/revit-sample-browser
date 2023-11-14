// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PrintLog.CS
{
    /// <summary>
    ///     Class used to call API to raise ViewPrint and DocumentPrint events
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // Filter all printable views in current document and print them,
            // the print will raise events registered in controlled application.
            // After run this external command please refer to log files under folder of this assembly.
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                var viewElems = new List<Element>();
                var collector = new FilteredElementCollector(document);
                viewElems.AddRange(collector.OfClass(typeof(View)).ToElements());
                //
                // Filter all printable views 
                var printableViews = new ViewSet();
                foreach (View view in viewElems)
                    // skip view templates because they're invalid for print
                    if (!view.IsTemplate && view.CanBePrinted)
                        printableViews.Insert(view);
                // 
                // Print to file to folder of assembly
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var pm = document.PrintManager;
                pm.PrintToFile = true;
                pm.PrintToFileName = assemblyPath + "\\PrintOut.prn";
                pm.Apply();
                // 
                // Print views now to raise events:
                document.Print(printableViews);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            //
            // return succeed by default
            return Result.Succeeded;
        }
    }
}
