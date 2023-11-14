using RevitMultiSample;

namespace MultiSampleDevUtils
{
    public class Tests
    {
        [Test]
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
    }
}