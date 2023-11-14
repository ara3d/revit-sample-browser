//
// (C) Copyright 2003-2011 by Autodesk, Inc.
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
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OptimizeStraights : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                // check user selection
                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();
                if (collection.Count > 0)
                {
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                        selIds.Add(id);

                    using (var tr = new Transaction(doc, "Optimize Straights"))
                    {
                        tr.Start();

                        // optimize lengths method will take a set of elements and any fabrication straight parts
                        // within this set that have been optimized will be returned.
                        var affectedPartIds = FabricationPart.OptimizeLengths(doc, selIds);
                        if (affectedPartIds.Count == 0)
                        {
                            message = "No fabrication straight parts were optimized.";
                            return Result.Cancelled;
                        }

                        doc.Regenerate();

                        tr.Commit();
                    }

                    return Result.Succeeded;
                }

                // inform user they need to select at least one element
                message = "Please select at least one element.";

                return Result.Failed;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}