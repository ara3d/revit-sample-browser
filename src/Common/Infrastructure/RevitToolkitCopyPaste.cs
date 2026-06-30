// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.Options;
using System;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class RevitToolkitCopyPaste
    {
        public static IDuplicateTypeNamesHandler UseDestinationTypes { get; } =
            new DuplicateTypeNamesHandler(DuplicateTypeAction.UseDestinationTypes);

        public static IDuplicateTypeNamesHandler Abort { get; } =
            new DuplicateTypeNamesHandler(DuplicateTypeAction.Abort);

        public static IDuplicateTypeNamesHandler Custom(
            Func<DuplicateTypeNamesHandlerArgs, DuplicateTypeAction> actionHandler)
        {
            return new DuplicateTypeNamesHandler(actionHandler);
        }

        public static CopyPasteOptions CreateOptions(IDuplicateTypeNamesHandler handler = null)
        {
            CopyPasteOptions options = new();
            options.SetDuplicateTypeNamesHandler(handler ?? UseDestinationTypes);
            return options;
        }
    }
}
