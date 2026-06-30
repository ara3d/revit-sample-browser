#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PreprocessFailure sample.</summary>
    internal static partial class Util
    {
    }

    /// <summary>
    ///     Suppresses the "Room is not in a properly enclosed region"
    ///     warning when creating unbounded rooms.
    /// </summary>
    internal class RoomWarningSwallower : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(
            FailuresAccessor a)
        {
            var failures
                = a.GetFailureMessages();

            foreach (var f in failures)
            {
                var id
                    = f.GetFailureDefinitionId();

                if (BuiltInFailures.RoomFailures.RoomNotEnclosed
                    == id)
                    a.DeleteWarning(f);
            }
            return FailureProcessingResult.Continue;
        }
    }
}
