// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using System.Drawing;
using System.Xml.Linq;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class SerializationHelper
    {
        public static XYZ GetXyz(XElement element) =>
                    new XYZ(
                        double.Parse(element.Attribute("X").Value),
                        double.Parse(element.Attribute("Y").Value),
                        double.Parse(element.Attribute("Z").Value));

        public static bool GetBoolean(XElement element) =>
                    bool.Parse(element.Attribute("value").Value);

        public static double GetDouble(XElement element) =>
                    double.Parse(element.Attribute("value").Value);

        public static int GetInteger(XElement element) =>
                    int.Parse(element.Attribute("value").Value);

        public static int GetColor(XElement element) =>
                    ColorTranslator.ToWin32(ColorTranslator.FromHtml(element.Attribute("value").Value));

        public static XElement GetXElement(double value, string name)
                {
                    var ret = new XElement(name);
                    ret.Add(new XAttribute("value", value));
                    return ret;
                }

        public static XElement GetXElement(bool value, string name)
                {
                    var ret = new XElement(name);
                    ret.Add(new XAttribute("value", value));
                    return ret;
                }

        public static XElement GetXElement(XYZ point, string name)
                {
                    var ret = new XElement(name);
                    ret.Add(new XAttribute("X", point.X));
                    ret.Add(new XAttribute("Y", point.Y));
                    ret.Add(new XAttribute("Z", point.Z));
                    return ret;
                }

        public static XElement GetColorXElement(int color, string name)
                {
                    var ret = new XElement(name);
                    ret.Add(new XAttribute("value", ColorTranslator.ToHtml(ColorTranslator.FromWin32(color))));
                    return ret;
                }

    }
}