// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
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

                var uidoc = commandData.Application.ActiveUIDocument;
                var collection = uidoc.Selection.GetElementIds();
                if (collection.Count > 0)
                {
                    ISet<ElementId> selIds = new HashSet<ElementId>();
                    foreach (var id in collection)
                    {
                        selIds.Add(id);
                    }

                    using (var tr = new Transaction(doc, "Optimize Straights"))
                    {
                        tr.Start();

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
