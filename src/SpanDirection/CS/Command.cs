// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.SpanDirection.CS
{
    /// <summary>
    ///     Get Span direction of Floor and all the SpanDirection Symbols
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Document Docment;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var application = commandData.Application;
            Docment = application.ActiveUIDocument.Document;
            try
            {
                // user should select one slab firstly. 
                if (application.ActiveUIDocument.Selection.GetElementIds().Count == 0)
                {
                    TaskDialog.Show("Revit", "Please select one slab firstly.", TaskDialogCommonButtons.Ok);
                    return Result.Cancelled;
                }

                // get the selected slab and show its span direction
                var elementSet = new ElementSet();
                foreach (var elementId in application.ActiveUIDocument.Selection.GetElementIds())
                    elementSet.Insert(application.ActiveUIDocument.Document.GetElement(elementId));
                var elemIter = elementSet.ForwardIterator();
                elemIter.Reset();
                while (elemIter.MoveNext())
                {
                    if (elemIter.Current is Floor floor) GetSpanDirectionAndSymobls(floor);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Get SpanDirection and SpanDirectionSymobols of Floor
        /// </summary>
        /// <param name="floor"></param>
        private void GetSpanDirectionAndSymobls(Floor floor)
        {
            if (null != floor)
            {
                // get SpanDirection angle of Floor(Slab)
                // The angle returned is in radians. An exception will be thrown if the floor
                // is non structural.
                var spanDirAngle = "Span direction angle: " + floor.SpanDirectionAngle + "\r\n";

                // get span direction symbols of Floor(Slab)
                var symbols = "Span direction symbols: \r\n\t";
                var symbolArray = floor.GetSpanDirectionSymbolIds();
                //ElementArrayIterator symbolIter = symbolArray.ForwardIterator();
                //symbolIter.Reset();
                //while (symbolIter.MoveNext())
                foreach (var eid in symbolArray)
                {
                    var elem = Docment.GetElement(eid);
                    if (elem != null) symbols += (Docment.GetElement(elem.GetTypeId()) as ElementType).Name + "\r\n";
                }

                TaskDialog.Show("Revit Direction", spanDirAngle + symbols, TaskDialogCommonButtons.Ok);
            }
            else
            {
                new Exception("Get Floor and SpanDirectionAngle and Symbols failed!");
            }
        }
    }
}
