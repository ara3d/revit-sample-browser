// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System.Linq;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Common.Units
{
    public static class DocumentUnits
    {
        public static double ConvertValueDocumentUnits(double decimalFeet, Document document)
                {
                    var formatOption = document.GetUnits().GetFormatOptions(SpecTypeId.PipeSize);
                    return UnitUtils.ConvertFromInternalUnits(decimalFeet, formatOption.GetUnitTypeId());
                }

        public static double ConvertValueToFeet(double unitValue, Document document)
                {
                    var tempVal = ConvertValueDocumentUnits(unitValue, document);
                    var ratio = unitValue / tempVal;
                    return unitValue * ratio;
                }

    }
}