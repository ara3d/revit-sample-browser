// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     No-op failures preprocessor that allows failure handling to continue.
    /// </summary>
    internal class ContinueFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(
            FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
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
