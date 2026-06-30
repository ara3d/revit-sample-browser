// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.PowerCircuit.CS.Properties;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsForm = System.Windows.Forms.Form;
using WinFormsPoint = System.Drawing.Point;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public delegate void DoorOperation(FamilyInstance door);

    public class HidePasteDuplicateTypesPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (var failure in failuresAccessor.GetFailureMessages())
            {
                if (failure.GetFailureDefinitionId() == BuiltInFailures.CopyPasteFailures.CannotCopyDuplicates)
                    failuresAccessor.DeleteWarning(failure);
            }

            return FailureProcessingResult.Continue;
        }
    }

    public static class DialogHelper
    {
        public static void ShowMessage(string message)
        {
            TaskDialog.Show("Revit", message);
        }

        public const string ShowPage = "Show Page";

        public static void Message(string message, int level = 0)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
            if (level > 0)
                TaskDialog.Show("Revit", message);
        }

        public static Matrix GetTransformMatrix(RectangleF rclip, RectangleF rBox)
        {
            try
            {
                var rdraw = rclip;
                const float shrink = 0.15f;
                var shrinked = 1.0f - (2 * shrink);
                if (rBox.Width * rclip.Height > rBox.Height * rclip.Width)
                    rdraw.Inflate(-rclip.Width * shrink,
                        ((rclip.Width * shrinked * rBox.Height / rBox.Width) - rclip.Height) / 2);
                else
                    rdraw.Inflate(((rclip.Height * shrinked * rBox.Width / rBox.Height) - rclip.Width) / 2,
                        -rclip.Height * shrink);

                var plgpts = new[]
                        {
                            new PointF(rdraw.Left, rdraw.Bottom),
                            new PointF(rdraw.Right, rdraw.Bottom),
                            new PointF(rdraw.Left, rdraw.Top)
                        };
                return new Matrix(rBox, plgpts);
            }
            catch (ArithmeticException)
            {
                return null;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
        }

        public static void ShowWarningMessage(string message, string caption)
        {
            TaskDialog.Show(caption, message, TaskDialogCommonButtons.Ok);
        }

        public static void ModifySelectedDoors(UIApplication uiapp, string text, DoorOperation operation)
        {
            var uidoc = uiapp.ActiveUIDocument;
            if (uidoc?.Selection == null) return;

            var selElements = uidoc.Selection.GetElementIds();
            if (selElements.Count == 0) return;

            var doorset = new FilteredElementCollector(uidoc.Document, selElements)
                        .OfCategory(BuiltInCategory.OST_Doors).ToElements();
            if (doorset == null) return;

            using var trans = new Transaction(uidoc.Document);
            if (trans.Start(text) != TransactionStatus.Started) return;
            foreach (FamilyInstance door in doorset)
                operation(door);
            trans.Commit();
        }

        public static void FlipHandAndFace(FamilyInstance door)
        {
            door.flipFacing();
            door.flipHand();
        }

        public static void MakeLeft(FamilyInstance door)
        {
            if (door.FacingFlipped ^ door.HandFlipped) door.flipHand();
        }

        public static void MakeRight(FamilyInstance door)
        {
            if (!(door.FacingFlipped ^ door.HandFlipped)) door.flipHand();
        }

        public static void TurnIn(FamilyInstance door)
        {
            if (!door.FacingFlipped) door.flipFacing();
        }

        public static void TurnOut(FamilyInstance door)
        {
            if (door.FacingFlipped) door.flipFacing();
        }

        public static void ShowErrorMessage(string message)
        {
            TaskDialog.Show(Resources.ResourceManager.GetString("OperationFailed"),
                        Resources.ResourceManager.GetString(message), TaskDialogCommonButtons.Ok);
        }

        public static void CenterOnScreen(WinFormsForm form)
        {
            var left = (Screen.PrimaryScreen.WorkingArea.Right - form.Width) / 2;
            var top = (Screen.PrimaryScreen.WorkingArea.Bottom - form.Height) / 2;
            form.Location = new WinFormsPoint(left, top);
        }

    }
}