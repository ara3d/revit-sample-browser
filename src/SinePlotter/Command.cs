// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SinePlotter.CS
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
