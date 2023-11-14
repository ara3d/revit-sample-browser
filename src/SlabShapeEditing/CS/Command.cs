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

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SlabShapeEditing.CS
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
                public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var selectFloor = GetSelectFloor(commandData);
            if (null == selectFloor)
            {
                message = "Make sure selected only one floor (Slab) in Revit.";
                return Result.Failed;
            }

            var slabShapeEditingForm = 
                new SlabShapeEditingForm(selectFloor, commandData);
            slabShapeEditingForm.ShowDialog();

            return Result.Succeeded;
        }

        /// <summary>
        /// get selected floor (slab)
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application.</param>
        /// <returns>selected floor (slab)</returns>
        private Floor GetSelectFloor(ExternalCommandData commandData)
        {
           var eleSet = new ElementSet();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
            {
               eleSet.Insert(commandData.Application.ActiveUIDocument.Document.GetElement(elementId));
            }
            if (eleSet.Size != 1) { return null; }

            var iter = eleSet.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
            {
                return iter.Current as Floor;
            }
            return null;
        }
            }
}
