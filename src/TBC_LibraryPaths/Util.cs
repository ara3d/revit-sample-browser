using System.Collections.Generic;
using System.Diagnostics;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_LibraryPaths sample.</summary>
    internal static partial class Util
    {
        internal static void PrintLibraryPathMap(
            IDictionary<string, string> map,
            string description)
        {
            Debug.Print("\n{0}:\n", description);

            foreach (var pair in map)
                Debug.Print("{0} -> {1}", pair.Key, pair.Value);
        }
    }
}
