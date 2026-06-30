// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

#region Namespaces

using System;
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace BuildingCoder
{
    public class JtWindowHandle : IWin32Window
    {
        public JtWindowHandle(IntPtr h)
        {
            Debug.Assert(IntPtr.Zero != h,
                "expected non-null window handle");

            Handle = h;
        }

        public IntPtr Handle { get; }
    }
}