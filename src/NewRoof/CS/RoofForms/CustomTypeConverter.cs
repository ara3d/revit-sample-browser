// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewRoof.CS.RoofForms
{
    /// <summary>
    ///     The LevelConverter class is inherited from the TypeConverter class which is used to
    ///     show the property which returns Level type as like a combo box in the PropertyGrid control.
    /// </summary>
    public class LevelConverter : TypeConverter
    {
        /// <summary>
        ///     To store the levels element
        /// </summary>
        private static readonly Dictionary<string, Level> Levels = new Dictionary<string, Level>();

        /// <summary>
        ///     Initialize the levels data.
        /// </summary>
        /// <param name="levels"></param>
        public static void SetStandardValues(ReadOnlyCollection<Level> levels)
        {
            Levels.Clear();
            foreach (var level in levels) Levels.Add(level.Id.ToString(), level);
        }

        /// <summary>
        ///     Get a level by a level id.
        /// </summary>
        /// <param name="id">The id of the level</param>
        /// <returns>Returns a level which id equals the specified id.</returns>
        public static Level GetLevelById(ElementId id)
        {
            return Levels[id.ToString()];
        }

        /// <summary>
        ///     Override the CanConvertTo method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Level) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Override the ConvertTo method, convert a level type value to a string type value for displaying in the
        ///     PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string) && value is Level level)
            {
                return level.Name + "[" + level.Id + "]";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///     Override the CanConvertFrom method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Override the ConvertFrom method, convert a string type value to a level type value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string levelString)
                try
                {
                    var leftBracket = levelString.IndexOf('[');
                    var rightBracket = levelString.IndexOf(']');

                    var idString = levelString.Substring(leftBracket + 1, rightBracket - leftBracket - 1);

                    return Levels[idString];
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Revit", ex.Message);
                }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Override the GetStandardValuesSupported method for displaying a level list in the PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Override the StandardValuesCollection method for supplying a level list in the PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Levels.Values);
        }
    }

    /// <summary>
    ///     The FootPrintRoofLineConverter class is inherited from the ExpandableObjectConverter class which is used to
    ///     expand the property which returns FootPrintRoofLine type as like a tree view in the PropertyGrid control.
    /// </summary>
    public class FootPrintRoofLineConverter : ExpandableObjectConverter
    {
        // To store the FootPrintRoofLines data.
        private static readonly Dictionary<string, FootPrintRoofLine> FootPrintLines =
            new Dictionary<string, FootPrintRoofLine>();

        /// <summary>
        ///     Initialize the FootPrintRoofLines data.
        /// </summary>
        /// <param name="footPrintRoofLines"></param>
        public static void SetStandardValues(List<FootPrintRoofLine> footPrintRoofLines)
        {
            FootPrintLines.Clear();
            foreach (var footPrintLine in footPrintRoofLines)
            {
                if (FootPrintLines.ContainsKey(footPrintLine.Id.ToString()))
                    continue;
                FootPrintLines.Add(footPrintLine.Id.ToString(), footPrintLine);
            }
        }

        /// <summary>
        ///     Override the CanConvertTo method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(FootPrintRoofLine) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Override the ConvertTo method, convert a FootPrintRoofLine type value to a string type value for displaying in the
        ///     PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (destinationType == typeof(string) && value is FootPrintRoofLine footPrintLine)
            {
                return footPrintLine.Name + "[" + footPrintLine.Id + "]";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///     Override the CanConvertFrom method.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Override the ConvertFrom method, convert a string type value to a FootPrintRoofLine type value.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string footPrintLineString)
                try
                {
                    var leftBracket = footPrintLineString.IndexOf('[');
                    var rightBracket = footPrintLineString.IndexOf(']');

                    var idString = footPrintLineString.Substring(leftBracket + 1, rightBracket - leftBracket - 1);

                    return FootPrintLines[idString];
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Revit", ex.Message);
                }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Override the GetStandardValuesSupported method for displaying a FootPrintRoofLine list in the PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Override the StandardValuesCollection method for supplying a FootPrintRoofLine list in the PropertyGrid.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(FootPrintLines.Values);
        }
    }
}
