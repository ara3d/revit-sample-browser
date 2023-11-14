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
using System.Collections;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.MoveLinear.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData cmdData, ref string msg, ElementSet eleSet)
        {
            var res = Result.Succeeded;
            var trans = new Transaction(cmdData.Application.ActiveUIDocument.Document, "Revit.SDK.Samples.MoveLinear");
            trans.Start();
            try
            {
                var sel = cmdData.Application.ActiveUIDocument.Selection;

                var elemSet = new ElementSet();
                foreach (var elementId in sel.GetElementIds())
                    elemSet.Insert(cmdData.Application.ActiveUIDocument.Document.GetElement(elementId));

                //Check whether user has selected only one element
                if (0 == elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select an element");
                    trans.Commit();
                    return res;
                }

                if (1 < elemSet.Size)
                {
                    TaskDialog.Show("MoveLinear", "Please select only one element");
                    trans.Commit();
                    return res;
                }

                IEnumerator iter = elemSet.ForwardIterator();

                iter.MoveNext();

                var element = (Element)iter.Current;

                if (element != null)
                {
                    var lineLoc = element.Location as LocationCurve;

                    if (null == lineLoc)
                    {
                        TaskDialog.Show("MoveLinear", "Please select an element which based on a Line");
                        trans.Commit();
                        return res;
                    }

                    //get start point via "get_EndPoint(0)"
                    var newStart = new XYZ(
                        lineLoc.Curve.GetEndPoint(0).X + 100,
                        lineLoc.Curve.GetEndPoint(0).Y,
                        lineLoc.Curve.GetEndPoint(0).Z);
                    //get end point via "get_EndPoint(1)"
                    var newEnd = new XYZ(
                        lineLoc.Curve.GetEndPoint(1).X,
                        lineLoc.Curve.GetEndPoint(1).Y + 100,
                        lineLoc.Curve.GetEndPoint(1).Z);


                    //get a new line and use it to move current element 
                    //with property "Autodesk.Revit.DB.LocationCurve.Curve"
                    var line = Line.CreateBound(newStart, newEnd);
                    lineLoc.Curve = line;
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MoveLinear", ex.Message);
                res = Result.Failed;
            }

            trans.Commit();
            return res;
        }
    }
}