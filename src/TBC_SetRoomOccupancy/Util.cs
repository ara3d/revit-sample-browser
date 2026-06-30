#region Namespaces

using Autodesk.Revit.DB;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        private static char[] _digits;

        public static string BumpStringSuffix(string s)
        {
            if (null == s || 0 == s.Length) return "1";
            if (null == _digits)
                _digits = new[]
                {
                    '0', '1', '2', '3', '4',
                    '5', '6', '7', '8', '9'
                };
            var n = s.Length;
            var t = s.TrimEnd(_digits);
            if (t.Length == n)
            {
                t += "1";
            }
            else
            {
                n = t.Length;
                n = int.Parse(s.Substring(n));
                ++n;
                t += n.ToString();
            }

            return t;
        }

        public static void BumpOccupancy(Element e)
        {
            var p = e.get_Parameter(
                BuiltInParameter.ROOM_OCCUPANCY);

            if (null == p)
            {
                Debug.Print(
                    "{0} has no room occupancy parameter.",
                    ElementDescription(e));
            }
            else
            {
                var occupancy = p.AsString();

                var newOccupancy = BumpStringSuffix(
                    occupancy);

                p.Set(newOccupancy);
            }
        }
    }
}
