#region Header

//
// CmdSetTangentLock.cs - set tangent lock on adjoining curve elements
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     Written by Christian @chhadidg73 in the
    ///     Revit API discussion forum thread
    ///     http://forums.autodesk.com/t5/revit-api-forum/settangentlock-in-profilesketch/m-p/6587402
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdSetTangentLock : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            message = "Sorry, no sample model available. "
                      + "Please refer to the Revit API discussion "
                      + "forum thread and blog post instead.";

            return Result.Failed;
        }
    }
}
