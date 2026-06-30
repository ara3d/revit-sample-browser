using System;
using System.Diagnostics;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_Idling sample.</summary>
    internal static partial class Util
    {
        internal static void LogIdlingMessage(string msg)
        {
            var dt = DateTime.Now.ToString("u");
            Debug.Print($"{dt} {msg}");
        }
    }
}
