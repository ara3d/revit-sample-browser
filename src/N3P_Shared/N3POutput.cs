using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.N3P_Shared.CS
{
    internal static class N3POutput
    {
        public static void Header(string title)
            => Debug.WriteLine($"=== {title} ===");

        public static void Line(string message)
            => Debug.WriteLine(message);

        public static void Line(string label, object value)
            => Debug.WriteLine($"{label}: {value}");
    }
}
