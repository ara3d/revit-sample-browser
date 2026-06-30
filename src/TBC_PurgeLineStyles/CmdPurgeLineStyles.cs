#region Header

//
// CmdPurgeLineStyles.cs - purge specific line styles
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
    [Transaction(TransactionMode.Manual)]
    internal class CmdPurgeLineStyles : IExternalCommand
    {
        private const string _line_style_name = "_Solid-Red-1";

        /// <summary>
        ///     External command Execute method.
        /// </summary>
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var doc = uiapp.ActiveUIDocument.Document;

            Util.PurgeGraphicStyles(doc, _line_style_name);

            return Result.Succeeded;
        }

        /// <summary>
        ///     Revit macro mainline.
        ///     Uncomment the line referencing 'this'.
        /// </summary>
        public void PurgeLineStyles_macro_mainline()
        {
            Document doc = null; // in a macro, use this.Document
            var name = "_Solid-Red-1";
            Util.PurgeGraphicStyles(doc, name);
        }
    }
}