#region Header

//
// CmdNewTextNote.cs - Create a new text note and determine its exact width
//
// Copyright (C) 2014-2020 by Scott Wilson and Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewTextNote : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var view = doc.ActiveView;

            XYZ p;

            try
            {
                p = uidoc.Selection.PickPoint(
                    "Please pick text insertion point");
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            var textType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(TextNoteType))
                    .FirstElement() as TextNoteType;

            Debug.Print($"TextNoteType.Name = {textType.Name}");

            // Windows font sizing uses 96 points per inch, not 72.

            float text_type_height_mm = 6;

            var mm_per_inch = 25.4f;

            float points_per_inch = 96;

            var em_size = points_per_inch
                          * (text_type_height_mm / mm_per_inch);

            em_size += 2.5f;

            Font font = new("Arial", em_size,
                FontStyle.Regular);

            TextNote txNote = null;

            using (Transaction t = new(doc))
            {
                t.Start("Create TextNote");


                var s = "The quick brown fox jumps over the lazy dog";

                var txtBox = TextRenderer
                    .MeasureText(s, font);

                double w_inch = txtBox.Width / Util.DpiX;
                double v_scale = view.Scale; // ratio of true model size to paper size

                Debug.Print(
                    "Text box width in pixels {0} = {1} inch, "
                    + "view scale = {2}",
                    txtBox.Width, w_inch, v_scale);

                var newWidth = w_inch / 12;

                txNote = TextNote.Create(doc,
                    doc.ActiveView.Id, p, s, textType.Id);

                Debug.Print(
                    "NewTextNote lineWidth {0} times view scale "
                    + "{1} = {2} generated TextNote.Width {3}",
                    Util.RealString(newWidth),
                    Util.RealString(v_scale),
                    Util.RealString(newWidth * v_scale),
                    Util.RealString(txNote.Width));

                var wmin = txNote.GetMinimumAllowedWidth();
                var wmax = txNote.GetMaximumAllowedWidth();

                var wnew = newWidth;

                txNote.Width = wnew;

                t.Commit();
            }

            using (Transaction t = new(doc))
            {
                t.Start("Change Text Colour");

                var color = Util.ToColorParameterValue(
                    255, 0, 0);

                var textNoteType = doc.GetElement(
                    txNote.GetTypeId());

                var param = textNoteType.get_Parameter(
                    BuiltInParameter.LINE_COLOR);

                // Modifies the text note type for all instances; Duplicate() first if needed.

                param.Set(color);

                t.Commit();
            }

            return Result.Succeeded;
        }

        #region Solution 2 using Graphics.MeasureString

        public Result Execute_2(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var commandResult = Result.Succeeded;

            try
            {
                var uiApp = commandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;
                var dbDoc = uiDoc.Document;
                var view = uiDoc.ActiveGraphicalView;

                var pLoc = XYZ.Zero;

                try
                {
                    pLoc = uiDoc.Selection.PickPoint(
                        "Please pick text insertion point");
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("Operation cancelled.");
                    message = "Operation cancelled.";

                    return Result.Succeeded;
                }

                var noteTypeList
                    = new FilteredElementCollector(dbDoc)
                        .OfClass(typeof(TextNoteType))
                        .Cast<TextNoteType>()
                        .ToList();


                var bipTextSize
                    = BuiltInParameter.TEXT_SIZE;

                noteTypeList.Sort((a, b)
                    => a.get_Parameter(bipTextSize).AsDouble()
                        .CompareTo(
                            b.get_Parameter(bipTextSize).AsDouble()));

                foreach (var textType in noteTypeList)
                {
                    Debug.WriteLine(textType.Name);

                    var paramTextFont
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_FONT);

                    var paramTextSize
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_SIZE);

                    var paramBorderSize
                        = textType.get_Parameter(
                            BuiltInParameter.LEADER_OFFSET_SHEET);

                    var paramTextBold
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_STYLE_BOLD);

                    var paramTextItalic
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_STYLE_ITALIC);

                    var paramTextUnderline
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_STYLE_UNDERLINE);

                    var paramTextWidthScale
                        = textType.get_Parameter(
                            BuiltInParameter.TEXT_WIDTH_SCALE);

                    var fontName = paramTextFont.AsString();

                    var textHeight = paramTextSize.AsDouble();

                    var textBold = paramTextBold.AsInteger() == 1;

                    var textItalic = paramTextItalic.AsInteger() == 1;

                    var textUnderline = paramTextUnderline.AsInteger() == 1;

                    var textBorder = paramBorderSize.AsDouble();

                    var textWidthScale = paramTextWidthScale.AsDouble();

                    var textStyle = FontStyle.Regular;

                    if (textBold) textStyle |= FontStyle.Bold;

                    if (textItalic) textStyle |= FontStyle.Italic;

                    if (textUnderline) textStyle |= FontStyle.Underline;

                    var fontHeightInch = (float)textHeight * 12.0f;
                    var displayDpiX = Util.GetDpiX();

                    var fontDpi = 96.0f;
                    var pointSize = (float)(textHeight * 12.0 * fontDpi);

                    Font font = new(fontName, pointSize, textStyle);

                    var viewScale = view.Scale;

                    using (Transaction t = new(dbDoc))
                    {
                        t.Start("Test TextNote lineWidth calculation");

                        var textString =
                            $"{textType.Name} ({fontName} {textHeight * 304.8:0.##}mm, {textStyle}, {textWidthScale * 100.0:0.##}%): The quick brown fox jumps over the lazy dog.";

                        var stringWidthPx = Util.GetStringWidth(textString, font);

                        var stringWidthIn = stringWidthPx / displayDpiX;

                        Debug.WriteLine($"String Width in pixels: {stringWidthPx:F3}");
                        Debug.WriteLine($"{stringWidthIn * 25.4 * viewScale:F3} mm at 1:{viewScale}");

                        var stringWidthFt = stringWidthIn / 12.0;

                        var lineWidth = ((stringWidthFt * textWidthScale)
                                         + (textBorder * 2.0)) * viewScale;

                        var textNote = TextNote.Create(dbDoc,
                            view.Id, pLoc, textString, textType.Id);

                        textNote.Width = lineWidth;

                        t.Commit();
                    }


                    pLoc += view.UpDirection.Multiply(
                        (textHeight + (5.0 / 304.8))
                        * viewScale).Negate();
                }
            }
            catch (ExternalApplicationException e)
            {
                message = e.Message;
                Debug.WriteLine($"Exception Encountered (Application)\n{e.Message}\nStack Trace: {e.StackTrace}");

                commandResult = Result.Failed;
            }
            catch (Exception e)
            {
                message = e.Message;
                Debug.WriteLine($"Exception Encountered (General)\n{e.Message}\nStack Trace: {e.StackTrace}");

                commandResult = Result.Failed;
            }

            return commandResult;
        }

        #endregion // Solution 2 using Graphics.MeasureString
    }
}