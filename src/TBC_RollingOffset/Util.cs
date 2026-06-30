#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_RollingOffset sample.</summary>
    internal static partial class Util
    {
        private const BuiltInParameter bipDiameterInner
            = BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM;

        private const BuiltInParameter bipDiameterOuter
            = BuiltInParameter.RBS_PIPE_OUTER_DIAMETER;

        public static double GetWallThickness(Pipe pipe)
        {
            var dinner = pipe.get_Parameter(
                bipDiameterInner).AsDouble();

            var douter = pipe.get_Parameter(
                bipDiameterOuter).AsDouble();

            return 0.5 * (douter - dinner);
        }
    }
}
