#region Header

//
// CmdMepElementShape.cs - determine element shape, i.e. MEP element cross section
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdMepElementShape : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            var n = uidoc.Selection.GetElementIds().Count;

            var preselected = 0 < n;

            Element e = null;

            while (true)
            {
                try
                {
                    e = Util.SelectSingleElementOfType(
                        uidoc, typeof(Element), "an element", true);
                }
                catch (OperationCanceledException)
                {
                    message = "No element selected";
                    break;
                }

                if (null == e) break;

                var s = "Not a duct.";

                if (e is Duct duct)
                {
                    var profileTypes
                        = Util.GetDuctProfileTypes(duct);

                    n = profileTypes.GetLength(0);

                    s = $"{n} connectors:\r\n{string.Join("\r\n", profileTypes.Select(a => a.ToString()))}";
                }

                var msg = $"{Util.ElementDescription(e)} is {Util.GetElementShape4(e)}-{Util.MepElementShapeVersion2.GetElementShape(e)} ({Util.MepElementShapeV1.GetElementShape(e)})";

                Util.InfoMsg2(msg, s);

                if (preselected) break;
            }

            return Result.Succeeded;
        }
    }
}

// Z:\a\doc\revit\blog\zip\tmp\example_revit.rvt
// Z:\j\tmp\rvt\rac_2014_empty.rvt
