// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Units;
using Ara3D.RevitSampleBrowser.Common.Views;
namespace Ara3D.RevitSampleBrowser.SpotDimension.CS
{
    public class SpotDimensionParams
    {
        private readonly Document m_document;

        public SpotDimensionParams(Document document) => m_document = document;

        public DataTable GetParameterTable(Autodesk.Revit.DB.SpotDimension spotDimension)
        {
            try
            {
                if (spotDimension == null)
                    return null;

                var parameterTable = ScheduleHelper.CreateTable();
                var dimensionType = spotDimension.DimensionType;
                var formatter = "#0.000";

                ScheduleHelper.AddDataRow("Category", spotDimension.Category.Name, parameterTable);

                var temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_LEADER_ARROWHEAD);
                var elementId = temporaryParam.AsElementId();
                var temporaryValue = ElementId.InvalidElementId == elementId
                    ? "None"
                    : m_document.GetElement(elementId).Name;
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_LINE_PEN);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsInteger().ToString(), parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TICK_MARK_PEN);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsInteger().ToString(), parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_SYMBOL);
                elementId = temporaryParam.AsElementId();
                temporaryValue = ElementId.InvalidElementId == elementId
                    ? "None"
                    : m_document.GetElement(elementId).Name;
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.TEXT_SIZE);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    ValueFormatting.FormatFractionalInches(temporaryParam.AsDouble(), formatter), parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_TEXT_FROM_LEADER);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    ValueFormatting.FormatFractionalInches(temporaryParam.AsDouble(), formatter), parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TEXT_HORIZ_OFFSET);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    ValueFormatting.FormatFractionalInches(temporaryParam.AsDouble(), formatter), parameterTable);

                if ("Spot Coordinates" == spotDimension.Category.Name)
                {
                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_COORDINATE_BASE);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsInteger().ToString(),
                        parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TOP_VALUE);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                        SampleBrowserUtils.STopBottomValue[temporaryParam.AsInteger()], parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_BOT_VALUE);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                        SampleBrowserUtils.STopBottomValue[temporaryParam.AsInteger()], parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_NS);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsString(), parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_EW);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsString(), parameterTable);
                }
                else
                {
                    temporaryParam = spotDimension.get_Parameter(BuiltInParameter.DIM_VALUE_LENGTH);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                        $"{temporaryParam.AsDouble().ToString(formatter)}'", parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_BASE);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                        SampleBrowserUtils.SElevationOrigin[temporaryParam.AsInteger()], parameterTable);

                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_ELEVATION);
                    ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsString(), parameterTable);
                }

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TEXT_ORIENTATION);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    SampleBrowserUtils.STextOrientation[temporaryParam.AsInteger()], parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_TYPE);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    SampleBrowserUtils.SIndicator[temporaryParam.AsInteger()], parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.TEXT_FONT);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name, temporaryParam.AsString(), parameterTable);

                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.DIM_TEXT_BACKGROUND);
                ScheduleHelper.AddDataRow(temporaryParam.Definition.Name,
                    SampleBrowserUtils.STextBackground[temporaryParam.AsInteger()], parameterTable);

                return parameterTable;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "A error in Function 'GetParameterTable':");
                return null;
            }
        }
    }
}
