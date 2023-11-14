// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Drawing;
using System.Xml.Linq;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.CS.PointCloudEngine
{
    /// <summary>
    ///     Utilities used by the sample to process XML entries in file-based point clouds.
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        ///     Gets an XYZ point from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The XYZ.</returns>
        public static XYZ GetXyz(XElement element)
        {
            var x = element.Attribute("X");
            var y = element.Attribute("Y");
            var z = element.Attribute("Z");

            return new XYZ(double.Parse(x.Value), double.Parse(y.Value), double.Parse(z.Value));
        }

        /// <summary>
        ///     Gets a boolean value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static bool GetBoolean(XElement element)
        {
            return bool.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        ///     Gets a double value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static double GetDouble(XElement element)
        {
            return double.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        ///     Gets an integer value from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static int GetInteger(XElement element)
        {
            return int.Parse(element.Attribute("value").Value);
        }

        /// <summary>
        ///     Gets a color value (in the form needed for inclusion in a CloudPoint) from an XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The value.</returns>
        public static int GetColor(XElement element)
        {
            return ColorTranslator.ToWin32(ColorTranslator.FromHtml(element.Attribute("value").Value));
        }

        /// <summary>
        ///     Gets the XML element representing a point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="name">The name of the XML element.</param>
        /// <returns>The element.</returns>
        public static XElement GetXElement(XYZ point, string name)
        {
            var ret = new XElement(name);
            ret.Add(new XAttribute("X", point.X));
            ret.Add(new XAttribute("Y", point.Y));
            ret.Add(new XAttribute("Z", point.Z));

            return ret;
        }

        /// <summary>
        ///     Gets the XML element representing a CloudPoint color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="name">The name.</param>
        /// <returns>The element.</returns>
        public static XElement GetColorXElement(int color, string name)
        {
            var ret = new XElement(name);
            var htmlRep = ColorTranslator.ToHtml(ColorTranslator.FromWin32(color));
            ret.Add(new XAttribute("value", htmlRep));

            return ret;
        }

        /// <summary>
        ///     Gets the XML element representing an object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns>The element.</returns>
        public static XElement GetXElement(object obj, string name)
        {
            var ret = new XElement(name);
            ret.Add(new XAttribute("value", obj.ToString()));

            return ret;
        }
    }
}
