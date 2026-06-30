using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_WallLayerVolumes sample.</summary>
    internal static partial class Util
    {
        private const BuiltInParameter _bipArea
            = BuiltInParameter.HOST_AREA_COMPUTED;

        private const BuiltInParameter _bipVolume
            = BuiltInParameter.HOST_VOLUME_COMPUTED;

        /// <summary>
        ///     Return the specified double parameter
        ///     value for the given wall.
        /// </summary>
        internal static double GetWallParameter(
            Wall wall,
            BuiltInParameter bip)
        {
            var p = wall.get_Parameter(bip);

            Debug.Assert(null != p,
                "expected wall to have "
                + "HOST_AREA_COMPUTED and "
                + "HOST_VOLUME_COMPUTED parameters");

            return p.AsDouble();
        }

        /// <summary>
        ///     Cumulate the compound wall layer volumes for the given wall.
        /// </summary>
        internal static void GetWallLayerVolumes(
            Wall wall,
            ref MapLayerToVolume totalVolumes)
        {
            var wt = wall.WallType;

            var structure = wt.GetCompoundStructure();

            var layers = structure.GetLayers();

            int i, n = layers.Count;

            var area = GetWallParameter(wall, _bipArea);
            var volume = GetWallParameter(wall, _bipVolume);
            var thickness = wt.Width;

            var desc = ElementDescription(wall);

            Debug.Print(
                "{0} with thickness {1}"
                + " and volume {2}"
                + " has {3} layer{4}{5}",
                desc,
                MmString(thickness),
                RealString(volume),
                n, PluralSuffix(n),
                DotOrColon(n));

            var key = wall.WallType.Name;
            totalVolumes.Cumulate(key, volume);

            if (0 < n)
            {
                i = 0;
                var total = 0.0;
                double layerVolume;
                foreach (var
                    layer in layers)
                {
                    key = $"{wall.WallType.Name} : {layer.Function}";

                    layerVolume = area * layer.Width;

                    totalVolumes.Cumulate(key, layerVolume);
                    total += layerVolume;

                    Debug.Print(
                        "  Layer {0}: function {1}, "
                        + "thickness {2}, volume {3}",
                        ++i, layer.Function,
                        MmString(layer.Width),
                        RealString(layerVolume));
                }

                Debug.Print("Wall volume = {0},"
                            + " total layer volume = {1}",
                    RealString(volume),
                    RealString(total));

                if (!IsEqual(volume, total))
                    Debug.Print("Wall host volume parameter"
                                + " value differs from sum of all layer"
                                + " volumes: {0}",
                        volume - total);
            }
        }

        /// <summary>
        ///     Enhance the standard Dictionary
        ///     class to have a Cumulate method.
        /// </summary>
        internal class MapLayerToVolume
            : Dictionary<string, double>
        {
            /// <summary>
            ///     Add cumulated value.
            ///     If seen for the first time for
            ///     this key, initialise with zero.
            /// </summary>
            public void Cumulate(
                string key,
                double value)
            {
                if (!ContainsKey(key)) this[key] = 0.0;
                this[key] += value;
            }
        }
    }
}
