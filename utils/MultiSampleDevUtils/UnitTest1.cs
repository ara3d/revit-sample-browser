using RevitMultiSample;

namespace MultiSampleDevUtils
{
    public class Tools
    {
        [Test, Explicit]
        public static void TestUI()
        {
            var path = @"C:\Users\cdigg\git\revit-samples\src";
            foreach (var f in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                var contents = File.ReadAllLines(f);

                File.WriteAllLines(f, contents
                    .SkipWhile(s => s.StartsWith("//"))
                    .Prepend("// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt"));
            }
        }

        [Test, Explicit]
        public static void FindReadmeFiles()
        {
            var path = @"C:\Users\cdigg\git\revit-samples\src";
            foreach (var f in Directory.GetFiles(path, "*.rtf", SearchOption.AllDirectories))
            {
                Console.WriteLine(f);
            }
        }
    }
}