#region Header

//
// CmdPressKeys.cs - press keys to launch 'Create Similar' and other Revit commands
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    #region Press class: encapsulates PostMessage and provides Keys method

    public class Press
    {
        public enum KEYBOARD_MSG : uint
        {
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101
        }

        [DllImport("USER32.DLL")]
        public static extern bool PostMessage(
            IntPtr hWnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(
            uint uCode, uint uMapType);
        public static void OneKey(IntPtr handle, char letter)
        {
            var scanCode = MapVirtualKey(letter,
                (uint) MVK_MAP_TYPE.VKEY_TO_SCANCODE);

            var keyDownCode = (uint)
                              WH_KEYBOARD_LPARAM.KEYDOWN
                              | (scanCode << 16);

            var keyUpCode = (uint)
                            WH_KEYBOARD_LPARAM.KEYUP
                            | (scanCode << 16);

            PostMessage(handle,
                (uint) KEYBOARD_MSG.WM_KEYDOWN,
                letter, keyDownCode);

            PostMessage(handle,
                (uint) KEYBOARD_MSG.WM_KEYUP,
                letter, keyUpCode);
        }
        public static void Keys(
            IntPtr revitHandle,
            string command)
        {
            foreach (var letter in command) OneKey(revitHandle, letter);
        }

        private enum WH_KEYBOARD_LPARAM : uint
        {
            KEYDOWN = 0x00000001,
            KEYUP = 0xC0000001
        }

        private enum MVK_MAP_TYPE : uint
        {
            VKEY_TO_SCANCODE = 0,
            SCANCODE_TO_VKEY = 1,
            VKEY_TO_CHAR = 2,
            SCANCODE_TO_LR_VKEY = 3
        }
    }

    #endregion //Press class: encapsulates PostMessage and provides Keys method

    [Transaction(TransactionMode.Manual)]
    internal class CmdPressKeys : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            var hRvt = uiapp.MainWindowHandle;

            var wallTypeName = "Generic - 203";

            var wallType = Util.GetFirstElementOfTypeNamed(
                doc, typeof(WallType), wallTypeName) as WallType;

            var wall = Util.GetFirstWallUsingType(
                doc, wallType);

            var ids = new List<ElementId>(1);
            ids.Add(wall.Id);
            uidoc.Selection.SetElementIds(ids);

            if (0 == uidoc.Selection.GetElementIds().Count)
            {
                var collector
                    = new FilteredElementCollector(doc);

                var ll = collector
                    .OfClass(typeof(Level))
                    .FirstElement() as Level;

                var geomLine = Line.CreateBound(XYZ.Zero, new XYZ(2, 0, 0));

                var t = new Transaction(
                    doc, "Create dummy wall");

                t.Start();

                var nw = Wall.Create(doc, geomLine,
                    wallType.Id, ll.Id, 1, 0, false, false);

                t.Commit();

                ids.Clear();
                ids.Add(nw.Id);
                uidoc.Selection.SetElementIds(ids);

                Press.Keys(hRvt, "CS");

                ids.Clear();
                ids.Add(nw.Id);
                uidoc.Selection.SetElementIds(ids);

                // Use keyboard delete; doc.Delete would not respect undo in this flow.

                Press.Keys(hRvt, "DE");

                Press.Keys(hRvt, "WA");
            }
            else
            {
                Press.Keys(hRvt, "CS");
            }

            return Result.Succeeded;
        }

    }
}