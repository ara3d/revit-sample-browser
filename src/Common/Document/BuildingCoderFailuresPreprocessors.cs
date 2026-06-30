// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    internal class ContinueFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(
            FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }

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
