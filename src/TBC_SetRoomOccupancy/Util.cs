#region Namespaces

using System.Diagnostics;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SetRoomOccupancy sample.</summary>
    internal static partial class Util
    {
        private static char[] _digits;

        /// <summary>
        ///     Analyse the given string.
        ///     If it ends in a sequence of digits representing a number,
        ///     return a string with the number oincremented by one.
        ///     Otherwise, return a string with a suffix "1" appended.
        /// </summary>
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

        /// <summary>
        ///     Read the value of the element ROOM_OCCUPANCY parameter.
        ///     If it ends in a number, increment the number, else append "1".
        /// </summary>
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
