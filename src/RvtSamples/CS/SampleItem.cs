// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace RvtSamples
{
    /// <summary>
    ///     The class contains information of a sample item to be added into samples menu
    /// </summary>
    public class SampleItem
    {
        /// <summary>
        ///     path of assembly
        /// </summary>
        private readonly string m_assembly;

        /// <summary>
        ///     category
        /// </summary>
        private readonly string m_category;

        /// <summary>
        ///     class name
        /// </summary>
        private readonly string m_className;

        /// <summary>
        ///     description
        /// </summary>
        private readonly string m_description;

        /// <summary>
        ///     display name
        /// </summary>
        private readonly string m_displayName;

        /// <summary>
        ///     path of image
        /// </summary>
        private readonly string m_image;

        /// <summary>
        ///     path of large image
        /// </summary>
        private readonly string m_largeImage;

        /// <summary>
        ///     Constructor
        /// </summary>
        public SampleItem()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="category">category</param>
        /// <param name="displayName">display name</param>
        /// <param name="description">description</param>
        /// <param name="largeImage">path of large image</param>
        /// <param name="image">path of image</param>
        /// <param name="assembly">path of assembly</param>
        /// <param name="className">class name</param>
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

        /// <summary>
        ///     class name
        /// </summary>
        public string ClassName => m_className;
    }
}
