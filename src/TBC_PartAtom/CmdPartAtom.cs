#region Header

//
// CmdPartAtom.cs - extract part atom from family file
//
// Copyright (C) 2010-2020 by By Hеvard Dagsvik, Symetri 
// and Jeremy Tammik, Autodesk Inc. All rights reserved.
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
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdPartAtom : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var app = uiapp.Application;

            var familyFilePath
                = "C:/Documents and Settings/All Users"
                  + "/Application Data/Autodesk/RAC 2011"
                  + "/Metric Library/Doors/M_Double-Flush.rfa";

            familyFilePath = "C:/Users/All Users/Autodesk"
                             + "/RVT 2017/Libraries/US Metric/Doors"
                             + "/M_Door-Double-Flush_Panel.rfa";

            var xmlPath = "C:/tmp/ExtractPartAtom.xml";

            // Using Revit API:

            app.ExtractPartAtomFromFamilyFile(
                familyFilePath, xmlPath);

            // Revit API independent:

            var xml_data = Util.GetFamilyXmlData(xmlPath);

            return Result.Succeeded;
        }
    }
}