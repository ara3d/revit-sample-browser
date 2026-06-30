using System;
using System.IO;
using System.Reflection;

namespace Ara3D.Bowerbird.RevitSamples.AecAgent;

public static class RepoPaths
{
    public static string FindRepoRoot()
    {
        var env = Environment.GetEnvironmentVariable("REVIT_SAMPLE_BROWSER_ROOT");
        if (!string.IsNullOrWhiteSpace(env) && File.Exists(Path.Combine(env, "COMMANDS.md")))
            return Path.GetFullPath(env);

        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        for (var i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
        {
            if (File.Exists(Path.Combine(dir, "COMMANDS.md")))
                return dir;
            dir = Directory.GetParent(dir)?.FullName;
        }

        return null;
    }

    public static string AgentRoot(string repoRoot)
        => string.IsNullOrEmpty(repoRoot) ? null : Path.Combine(repoRoot, ".agent");

    public static string StandardsRoot(string repoRoot)
        => string.IsNullOrEmpty(repoRoot) ? null : Path.Combine(repoRoot, ".agent", "standards");
}
