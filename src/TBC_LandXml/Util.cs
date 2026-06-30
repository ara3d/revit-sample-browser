using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Xml;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_LandXml sample.</summary>
    internal static partial class Util
    {
        internal static List<XYZ> ParseLandXmlPoints(XmlDocument xmlDoc)
        {
            var pnts = xmlDoc.GetElementsByTagName("Pnts");
            var separator = new[] { ' ' };
            double x = 0, y = 0, z = 0;
            var pts = new List<XYZ>();

            for (var k = 0; k < pnts.Count; ++k)
                for (var i = 0; i < pnts[k].ChildNodes.Count; ++i)
                {
                    var j = 1;

                    var text = pnts[k].ChildNodes[i].InnerText;
                    var coords = text.Split(separator);

                    foreach (var coord in coords)
                    {
                        switch (j)
                        {
                            case 1:
                                x = double.Parse(coord);
                                break;
                            case 2:
                                y = double.Parse(coord);
                                break;
                            case 3:
                                z = double.Parse(coord);
                                break;
                        }

                        j++;
                    }

                    pts.Add(new XYZ(x, y, z));
                }

            return pts;
        }
    }
}
