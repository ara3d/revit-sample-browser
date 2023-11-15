// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Ara3D.RevitSampleBrowser.InPlaceMembers.CS
{
    /// <summary>
    ///     generate GraphicsData by given geometry object
    /// </summary>
    public class GraphicsDataFactory
    {
        /// <summary>
        ///     create GraphicsData of given AnalyticalModel3D
        /// </summary>
        /// <param name="model">AnalyticalModel3D contains geometry data</param>
        /// <returns></returns>
        public static GraphicsData CreateGraphicsData(AnalyticalElement model)
        {
            IList<Curve> curveList = new List<Curve>();
            switch (model)
            {
                case AnalyticalMember _:
                    curveList.Add(model.GetCurve());
                    break;
                case AnalyticalPanel panel:
                    curveList = panel.GetOuterContour().ToList();
                    break;
            }

            if (curveList.Count > 0)
            {
                var data = new GraphicsData();

                var curves = curveList.GetEnumerator();
                curves.Reset();
                while (curves.MoveNext())
                {
                    var curve = curves.Current;

                    try
                    {
                        var points = curve.Tessellate() as List<XYZ>;
                        data.InsertCurve(points);
                    }
                    catch
                    {
                    }
                }

                data.UpdataData();

                return data;
            }

            throw new Exception("Can't get curves.");
        }
    }
}
