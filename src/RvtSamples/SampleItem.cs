// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.RvtSamples.CS
{
    public class SampleItem
    {
        private readonly string m_assembly;

        private readonly string m_category;

        private readonly string m_className;

        private readonly string m_description;

        private readonly string m_displayName;

        private readonly string m_image;

        private readonly string m_largeImage;

        public SampleItem()
        {
        }

        public SampleItem(string category, string displayName, string description, string largeImage, string image,
            string assembly, string className)
        {
            m_category = category;
            m_displayName = displayName;
            m_description = description;
            m_largeImage = largeImage;
            m_image = image;
            m_assembly = assembly;
            m_className = className;
        }

        /// <summary>
        ///     category
        /// </summary>
        public string Category => m_category;

        /// <summary>
        ///     display name
        /// </summary>
        public string DisplayName => m_displayName;

        /// <summary>
        ///     path of large image
        /// </summary>
        public string LargeImage => m_largeImage;

        /// <summary>
        ///     path of image
        /// </summary>
        public string Image => m_image;

        /// <summary>
        ///     description
        /// </summary>
        public string Description => m_description;

        /// <summary>
        ///     path of assembly
        /// </summary>
        public string Assembly => m_assembly;

        public string ClassName => m_className;
    }
}
