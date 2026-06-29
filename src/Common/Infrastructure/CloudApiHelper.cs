// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ara3D.RevitSampleBrowser.CloudAPISample.CS.Samples.Migration;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class CloudApiHelper
    {
        public static FolderLocation GetTargetFolderUrn(IList<MigrationRule> rules, string directory, string model,
                    IList<FolderLocation> availableFolders)
                {
                    foreach (var rule in rules)
                    {
                        var models = Directory.GetFiles(directory, rule.Pattern, SearchOption.TopDirectoryOnly);
                        if (models.Contains(model))
                            return rule.Target;
                    }

                    return availableFolders.Last();
                }

    }
}