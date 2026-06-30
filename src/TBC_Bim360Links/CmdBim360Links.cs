#region Header

//
// CmdBim360Links.cs - retrieve and list BIM360 linked models
//
// Copyright (C) 2010-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;


#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdBim360Links : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var xrefs = ExternalResourceUtils
                .GetAllExternalResourceReferences(doc);

            var caption = "BIM360 Links";

            try
            {
                var n = 0;
                var msg = string.Empty;

                foreach (var eid in xrefs)
                {
                    var elem = doc.GetElement(eid);
                    if (elem == null) continue;

                    if (elem is not RevitLinkType link) continue;

                    try
                    {
                        var result = link.Load();
                        var mdPath = result.GetModelName();
                        link.Unload(null);

                        var path = ModelPathUtils
                            .ConvertModelPathToUserVisiblePath(mdPath);

                        msg += $"{link.AttachmentType} {path}\r\n";

                        ++n;
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show(caption, ex.Message);
                    }
                }

                caption = $"{n} BIM360 Link{Util.PluralSuffix(n)}";

                TaskDialog.Show(caption, msg);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(caption, ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
