// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Colour Conversion

        /// <summary>
        ///     Revit text colour parameter value stored as an integer
        ///     in text note type BuiltInParameter.LINE_COLOR.
        /// </summary>
        public static int ToColorParameterValue(
            int red,
            int green,
            int blue)
        {
            // from https://forums.autodesk.com/t5/revit-api-forum/how-to-change-text-color/td-p/2567672

            var c = red + (green << 8) + (blue << 16);

#if DEBUG
            var c2 = red + 256 * green + 65536 * blue;
            Debug.Assert(c == c2, "expected shift result to equal multiplication");
#endif // DEBUG

            return c;
        }

        /// <summary>
        ///     Revit text colour parameter value stored as an integer
        ///     in text note type BuiltInParameter.LINE_COLOR.
        /// </summary>
        public static int GetRevitTextColorFromSystemColor(
            Color color)
        {
            // from https://forums.autodesk.com/t5/revit-api-forum/how-to-change-text-color/td-p/2567672

            return ToColorParameterValue(color.R, color.G, color.B);
        }

        #endregion
    }
}
