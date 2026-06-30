// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Nice3point.Revit.Toolkit.Options;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class RevitToolkitFamilies
    {
        public static IFamilyLoadOptions OverwriteExisting { get; } = new FamilyLoadOptions(true);

        public static IFamilyLoadOptions KeepExisting { get; } = new FamilyLoadOptions(false);

        public static IFamilyLoadOptions FromProject { get; } =
            new FamilyLoadOptions(true, FamilySource.Project);

        public static IFamilyLoadOptions FromFamily { get; } =
            new FamilyLoadOptions(true, FamilySource.Family);

        public static IFamilyLoadOptions Default { get; } = new FamilyLoadOptions();

        public static bool LoadFamily(
            Document document,
            string path,
            out Family family,
            IFamilyLoadOptions options = null) =>
            document.LoadFamily(path, options ?? Default, out family);
    }
}
