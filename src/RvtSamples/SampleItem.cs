// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.RvtSamples.CS
{
    public class SampleItem
    {
        public SampleItem()
        {
        }

        public SampleItem(string category, string displayName, string description, string largeImage, string image,
            string assembly, string className)
        {
            Category = category;
            DisplayName = displayName;
            Description = description;
            LargeImage = largeImage;
            Image = image;
            Assembly = assembly;
            ClassName = className;
        }

        /// <summary>
        ///     category
        /// </summary>
        public string Category { get; }

        /// <summary>
        ///     display name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        ///     path of large image
        /// </summary>
        public string LargeImage { get; }

        /// <summary>
        ///     path of image
        /// </summary>
        public string Image { get; }

        /// <summary>
        ///     description
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     path of assembly
        /// </summary>
        public string Assembly { get; }

        public string ClassName { get; }
    }
}
