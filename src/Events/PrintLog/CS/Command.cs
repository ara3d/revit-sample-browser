//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PrintLog.CS
{
    /// <summary>
    /// Class used to call API to raise ViewPrint and DocumentPrint events
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command: IExternalCommand
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
                {
                    // skip view templates because they're invalid for print
                    if (!view.IsTemplate && view.CanBePrinted)
                    {
                        printableViews.Insert(view);
                    }
                }
                // 
                // Print to file to folder of assembly
                var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var pm = document.PrintManager;
                pm.PrintToFile = true;
                pm.PrintToFileName = assemblyPath + "\\PrintOut.prn";
                pm.Apply();
                // 
                // Print views now to raise events:
                document.Print(printableViews);
            }
            catch(Exception ex)
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
