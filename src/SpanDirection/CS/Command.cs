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
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SpanDirection.CS
{
    /// <summary>
    /// Get Span direction of Floor and all the SpanDirection Symbols
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Document m_docment;
        #region Interface implementation
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var application = commandData.Application;
            m_docment = application.ActiveUIDocument.Document;
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
                {
                   elementSet.Insert(application.ActiveUIDocument.Document.GetElement(elementId));
                }
                var elemIter = elementSet.ForwardIterator();
                elemIter.Reset();
                while (elemIter.MoveNext())
                {
                    var floor = elemIter.Current as Floor;
                    if (floor != null)
                    {
                        GetSpanDirectionAndSymobls(floor);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        #endregion


        /// <summary>
        /// Get SpanDirection and SpanDirectionSymobols of Floor
        /// </summary>
        /// <param name="floor"></param>
        void GetSpanDirectionAndSymobls(Floor floor)
        {
            if (null != floor)
            {
                // get SpanDirection angle of Floor(Slab)
                // The angle returned is in radians. An exception will be thrown if the floor
                // is non structural.
                var spanDirAngle = "Span direction angle: " + floor.SpanDirectionAngle.ToString() + "\r\n";

                // get span direction symbols of Floor(Slab)
                var symbols = "Span direction symbols: \r\n\t";
                var symbolArray = floor.GetSpanDirectionSymbolIds();
                //ElementArrayIterator symbolIter = symbolArray.ForwardIterator();
                //symbolIter.Reset();
                //while (symbolIter.MoveNext())
                foreach (var eid in symbolArray)
                {
                    var elem = m_docment.GetElement(eid);
                    if (elem != null)
                    {
                        symbols += (m_docment.GetElement(elem.GetTypeId()) as ElementType).Name + "\r\n";
                    }
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
