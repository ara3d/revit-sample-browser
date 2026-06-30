// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit;
using System;
using System.Windows;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class RevitToolkitScopes
    {
        public static IDisposable BeginFailureSuppression(bool resolveErrors = true)
        {
            return RevitApiContext.BeginFailureSuppressionScope(resolveErrors);
        }

        public static IDisposable BeginDialogSuppression(int resultCode = 1)
        {
            return RevitContext.BeginDialogSuppressionScope(resultCode);
        }

        public static IDisposable BeginDialogSuppression(MessageBoxResult resultCode)
        {
            return RevitContext.BeginDialogSuppressionScope(resultCode);
        }

        public static IDisposable BeginDialogSuppression(TaskDialogResult resultCode)
        {
            return RevitContext.BeginDialogSuppressionScope(resultCode);
        }
    }
}