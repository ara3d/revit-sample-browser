// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class RevitToolkitScopes
    {
        public static IDisposable BeginFailureSuppression(bool resolveErrors = true) =>
            RevitApiContext.BeginFailureSuppressionScope(resolveErrors);

        public static IDisposable BeginDialogSuppression(int resultCode = 1) =>
            RevitContext.BeginDialogSuppressionScope(resultCode);

        public static IDisposable BeginDialogSuppression(MessageBoxResult resultCode) =>
            RevitContext.BeginDialogSuppressionScope(resultCode);

        public static IDisposable BeginDialogSuppression(TaskDialogResult resultCode) =>
            RevitContext.BeginDialogSuppressionScope(resultCode);
    }
}