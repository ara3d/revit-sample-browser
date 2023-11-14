// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.SpotDimension.CS
{
    /// <summary>
    ///     this class is used to get some information
    ///     of SpotDimension's Parameters
    /// </summary>
    public class SpotDimensionParams
    {
        private const double ToFractionalInches = 0.08333333; //const number to convert number to FractionalInches

        private static readonly List<string>
            SElevationOrigin = new List<string>(); //list store information about Elevation origin

        private static readonly List<string>
            STextOrientation = new List<string>(); //list store information about Text Orientation

        private static readonly List<string>
            SIndicator = new List<string>(); //list store information about s_indicator

        private static readonly List<string>
            STopBottomValue = new List<string>(); //list store information about Top and Bottom Value

        private static readonly List<string>
            STextBackground = new List<string>(); //list store information about Text background

        private readonly Document m_document; //a reference to Revit's document

        /// <summary>
        ///     static constructor used to initialize lists
        /// </summary>
        static SpotDimensionParams()
        {
            //add string elements to lists
            SElevationOrigin.Add("Project");
            SElevationOrigin.Add("Shared");
            SElevationOrigin.Add("Relative");

            STextOrientation.Add("Horizontal Above");
            STextOrientation.Add("Horizontal Below");

            SIndicator.Add("Prefix");
            SIndicator.Add("Suffix");

            STopBottomValue.Add("None");
            STopBottomValue.Add("North / South");
            STopBottomValue.Add("East / West");

            STextBackground.Add("Opaque");
            STextBackground.Add("Transparent");
        }

        /// <summary>
        ///     a constructor of class SpotDimensionParams
        /// </summary>
        /// <param name="document">external command document of Revit</param>
        public SpotDimensionParams(Document document)
        {
            m_document = document;
        }

        /// <summary>
        ///     get a datatable contains parameters'information of SpotDimension
        ///     here, almost only get Type Parameters of SpotDimension
        /// </summary>
        /// <param name="spotDimension">the SpotDimension need to be dealt with</param>
        /// <returns>a DataTable store Parameter information</returns>
        public DataTable GetParameterTable(Autodesk.Revit.DB.SpotDimension spotDimension)
        {
            try
            {
                //check whether is null
                if (null == spotDimension) return null;

                //create an empty datatable
                var parameterTable = CreateTable();

                //get DimensionType
                var dimensionType = spotDimension.DimensionType;

                //begin to get Parameters and add them to a DataTable
                var temporaryValue = "";

                //string formatter
                var formatter = "#0.000";

                //Property Category of SpotDimension
                AddDataRow("Category", spotDimension.Category.Name, parameterTable);

                //Leader Arrowhead
                var temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_LEADER_ARROWHEAD);
                var elementId = temporaryParam.AsElementId();
                //if not found that element, add string "None" to DataTable
                if (ElementId.InvalidElementId == elementId)
                    temporaryValue = "None";
                else
                    temporaryValue = m_document.GetElement(elementId).Name;
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Leader Line Weight
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_LINE_PEN);
                temporaryValue = temporaryParam.AsInteger().ToString();
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Leader Arrowhead Line Weight
                temporaryParam =
                    dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TICK_MARK_PEN);
                temporaryValue = temporaryParam.AsInteger().ToString();
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Symbol
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_SYMBOL);
                elementId = temporaryParam.AsElementId();
                //if not found that element, add string "None" to DataTable
                if (ElementId.InvalidElementId == elementId)
                    temporaryValue = "None";
                else
                    temporaryValue = m_document.GetElement(elementId).Name;
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Text Size
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.TEXT_SIZE);
                temporaryValue =
                    (temporaryParam.AsDouble() / ToFractionalInches).ToString(formatter) + "''";
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Text Offset from Leader
                temporaryParam =
                    dimensionType.get_Parameter(BuiltInParameter.SPOT_TEXT_FROM_LEADER);
                temporaryValue =
                    (temporaryParam.AsDouble() / ToFractionalInches).ToString(formatter) + "''";
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Text Offset from Symbol
                temporaryParam =
                    dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TEXT_HORIZ_OFFSET);
                temporaryValue =
                    (temporaryParam.AsDouble() / ToFractionalInches).ToString(formatter) + "''";
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //for Spot Coordinates, add some other Parameters 
                if ("Spot Coordinates" == spotDimension.Category.Name)
                {
                    //Coordinate Origin
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_COORDINATE_BASE);
                    temporaryValue = temporaryParam.AsInteger().ToString();
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //Top Value
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TOP_VALUE);
                    temporaryValue = STopBottomValue[temporaryParam.AsInteger()];
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //Bottom Value
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_BOT_VALUE);
                    temporaryValue = STopBottomValue[temporaryParam.AsInteger()];
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //North / South s_indicator
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_NS);
                    temporaryValue = temporaryParam.AsString();
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //East / West s_indicator
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_EW);
                    temporaryValue = temporaryParam.AsString();
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);
                }
                //for Spot Elevation, add some other Parameters 
                else
                {
                    //Instance Parameter----Value
                    temporaryParam =
                        spotDimension.get_Parameter(BuiltInParameter.DIM_VALUE_LENGTH);
                    temporaryValue = temporaryParam.AsDouble().ToString(formatter) + "'";
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //Elevation Origin
                    temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_BASE);
                    temporaryValue = SElevationOrigin[temporaryParam.AsInteger()];
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                    //Elevation s_indicator
                    temporaryParam =
                        dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_ELEVATION);
                    temporaryValue = temporaryParam.AsString();
                    AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);
                }

                //Text Orientation
                temporaryParam =
                    dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_TEXT_ORIENTATION);
                temporaryValue = STextOrientation[temporaryParam.AsInteger()];
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //s_indicator as Prefix / Suffix
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.SPOT_ELEV_IND_TYPE);
                temporaryValue = SIndicator[temporaryParam.AsInteger()];
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Text Font
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.TEXT_FONT);
                temporaryValue = temporaryParam.AsString();
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                //Text Background
                temporaryParam = dimensionType.get_Parameter(BuiltInParameter.DIM_TEXT_BACKGROUND);
                temporaryValue = STextBackground[temporaryParam.AsInteger()];
                AddDataRow(temporaryParam.Definition.Name, temporaryValue, parameterTable);

                return parameterTable;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "A error in Function 'GetParameterTable':");
                return null;
            }
        }

        /// <summary>
        ///     Create an empty table with parameter's name column and value column
        /// </summary>
        /// <returns>a DataTable be initialized</returns>
        private DataTable CreateTable()
        {
            // Create a new DataTable.
            var propDataTable = new DataTable("ParameterTable");

            // Create parameter column and add to the DataTable.
            var paraDataColumn = new DataColumn();
            paraDataColumn.DataType = Type.GetType("System.String");
            paraDataColumn.ColumnName = "Parameter";
            paraDataColumn.Caption = "Parameter";
            paraDataColumn.ReadOnly = true;
            // Add the column to the DataColumnCollection.
            propDataTable.Columns.Add(paraDataColumn);

            // Create value column and add to the DataTable.
            var valueDataColumn = new DataColumn();
            valueDataColumn.DataType = Type.GetType("System.String");
            valueDataColumn.ColumnName = "Value";
            valueDataColumn.Caption = "Value";
            valueDataColumn.ReadOnly = true;
            propDataTable.Columns.Add(valueDataColumn);

            return propDataTable;
        }

        /// <summary>
        ///     add one row to datatable
        /// </summary>
        /// <param name="parameterName">name of parameter</param>
        /// <param name="value">value of parameter</param>
        /// <param name="parameterTable">datatable to be added row</param>
        private void AddDataRow(string parameterName, string value, DataTable parameterTable)
        {
            var newRow = parameterTable.NewRow();
            newRow["Parameter"] = parameterName;
            newRow["Value"] = value;
            parameterTable.Rows.Add(newRow);
        }
    }
}
