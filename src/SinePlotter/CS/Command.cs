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
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SinePlotter.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                //instantiate a finder to locate the FamilySymbol of the class instance we want to array
                var familySymbolFinder = new FilteredElementCollector(document);
                familySymbolFinder.OfClass(typeof(FamilySymbol));

                //the name of the family symbol we are looking for
                var fsName = Application.GetFamilySymbolName();
                FamilySymbol fs = null;
                try
                {
                    fs = familySymbolFinder.ToElements().Single(s => s.Name == fsName) as FamilySymbol;
                }
                catch (InvalidOperationException)
                {
                    TaskDialog.Show("FamilySymbol Loading Error",
                        "The family symbol is not loaded in the project file.");
                    return Result.Failed;
                }

                //instantiate an instance plotter object
                var plotter = new FamilyInstancePlotter(fs, document);

                //reference the necessary inputs for the placeInstancesOnCurve method
                var partitions = (int)Application.GetNumberOfPartitions();
                var period = Application.GetPeriod();
                var amplitude = Application.GetAplitude();
                var numOfCircles = Application.GetNumberOfCycles();
                //place the instances of the family objects along a curve         
                plotter.PlaceInstancesOnCurve(partitions, period, amplitude, numOfCircles);

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