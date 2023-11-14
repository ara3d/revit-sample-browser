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

namespace Revit.SDK.Samples.DeleteObject.CS
{
    /// <summary>
    /// Delete the elements that were selected
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var trans = new Transaction(revit.ActiveUIDocument.Document, "Revit.SDK.Samples.DeleteObject");
            trans.Start();
            var collection = new ElementSet();
            foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
            {
               collection.Insert(revit.ActiveUIDocument.Document.GetElement(elementId));
            }
            // check user selection
            if (collection.Size < 1)
            {
                message = "Please select object(s) before delete.";
                trans.RollBack();
                return Result.Cancelled;
            }

            var error = true;
            try
            {
                error = true;

                // delete selection
                var e = collection.GetEnumerator();
                var MoreValue = e.MoveNext();
                while (MoreValue)
                {
                    var component = e.Current as Element;
                    revit.ActiveUIDocument.Document.Delete(component.Id);
                    MoreValue = e.MoveNext();
                }

                error = false;
            }
            catch
            {
                // if revit threw an exception, try to catch it
                foreach (Element c in collection)
                {
                    elements.Insert(c);
                }
                message = "object(s) can't be deleted.";
                trans.RollBack();
                return Result.Failed;
            }
            finally
            {
                // if revit threw an exception, display error and return failed
                if (error)
                {
                    TaskDialog.Show("Error", "Delete failed.");
                }
            }
            trans.Commit();
            return Result.Succeeded;
        }
    }
}
