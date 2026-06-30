// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ara3D.RevitSampleBrowser.InPlaceMembers.CS
{
    public delegate void UpdateViewDelegate();

    /// <summary>
    ///     interface as the datasource of view, include nececessary members
    /// </summary>
    public interface IGraphicsData
    {
        RectangleF Region { get; }

        List<PointF[]> PointCurves();

        event UpdateViewDelegate UpdateViewEvent;
    }

    public abstract class GraphicsDataBase : IGraphicsData
    {
        /// <summary>
        ///     minimum value of the region box's edge
        /// </summary>
        public const float MinedgElEngth = 1.0f;

        /// <summary>
        ///     default angle when rotate around X,Y,Z axis
        /// </summary>
        public const double Rotateangle = Math.PI / 90;

        protected double[,] Origin = { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 } };

        protected XYZ OriginMax = new(double.MinValue, double.MinValue, double.MinValue);

        protected XYZ OriginMin = new(double.MaxValue, double.MaxValue, double.MaxValue);

        protected XYZ TransferedMax;

        protected XYZ TransferedMin;

        public GraphicsDataBase()
        {
            Initialize();
        }

        public virtual event UpdateViewDelegate UpdateViewEvent;

        public abstract List<PointF[]> PointCurves();

        public RectangleF Region
        {
            get
            {
                var width = (float)(TransferedMax.X - TransferedMin.X);
                var height = (float)(TransferedMax.Y - TransferedMin.Y);

                var minX = -(width / 2);
                var minY = -(height / 2);

                if (width < 1) width = 1;
                if (height < 1) height = 1;

                RectangleF rec = new(minX, minY, width, height);
                return rec;
            }
        }

        protected void Initialize()
        {
            var initangle = Math.PI / 4;

            RotateX(ref Origin, initangle);
            RotateY(ref Origin, initangle);
            TransferedMax = new XYZ(double.MinValue, double.MinValue, double.MinValue);
            TransferedMin = new XYZ(double.MaxValue, double.MaxValue, double.MaxValue);
        }

        public virtual void UpdataData()
        {
            UpdateViewEvent();
        }

        /// <summary>
        ///     rotate around Z axis with default angle
        /// </summary>
        /// <param name="direction">minus or positive angle</param>
        public void RotateZ(bool direction)
        {
            var angle = Rotateangle;
            if (!direction) angle = -Rotateangle;

            RotateZ(ref Origin, angle);
            UpdataData();
        }

        private void RotateZ(ref double[,] origin, double angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            double[,] rotate = { { cos, sin, 0.0 }, { -sin, cos, 0.0 }, { 0.0, 0.0, 1.0 } };
            origin = MatrixArith.MultiCross(Origin, rotate);
        }

        /// <summary>
        ///     rotate around Y axis with default angle
        /// </summary>
        /// <param name="direction">minus or positive angle</param>
        public void RotateY(bool direction)
        {
            var angle = Rotateangle;
            if (!direction) angle = -Rotateangle;

            RotateY(ref Origin, angle);
            UpdataData();
        }

        private void RotateY(ref double[,] origin, double angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            double[,] rotate = { { cos, 0.0, -sin }, { 0.0, 1.0, 0.0 }, { sin, 0.0, cos } };
            origin = MatrixArith.MultiCross(Origin, rotate);
        }

        /// <summary>
        ///     rotate around X axis with default angle
        /// </summary>
        /// <param name="direction">minus or positive angle</param>
        public void RotateX(bool direction)
        {
            var angle = Rotateangle;
            if (!direction) angle = -Rotateangle;

            RotateX(ref Origin, angle);
            UpdataData();
        }

        private void RotateX(ref double[,] origin, double angle)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            double[,] rotate = { { 1.0, 0.0, 0.0 }, { 0.0, cos, sin }, { 0.0, -sin, cos } };
            origin = MatrixArith.MultiCross(Origin, rotate);
        }
    }

    public class GraphicsData : GraphicsDataBase
    {
        private readonly List<PointF[]> m_curves2D;
        private readonly List<List<XYZ>> m_originCurves;
        private readonly List<List<XYZ>> m_transferedCurves;

        public GraphicsData()
        {
            m_originCurves = [];
            m_transferedCurves = [];
            m_curves2D = [];
        }

        public override event UpdateViewDelegate UpdateViewEvent;

        public override List<PointF[]> PointCurves()
        {
            return m_curves2D;
        }

        public void InsertCurve(List<XYZ> points)
        {
            m_originCurves.Add(points);
            foreach (var point in points)
            {
                OriginMin = UpdateMinRange(point);
                OriginMax = UpdateMaxRange(point);
            }
        }

        public override void UpdataData()
        {
            TransferedMin = TransferRotate(OriginMin);
            TransferedMax = TransferRotate(OriginMax);

            m_transferedCurves.Clear();
            m_curves2D.Clear();

            foreach (var points in m_originCurves)
            {
                SynChroData(points);
            }

            UpdateViewEvent?.Invoke();
        }

        /// <summary>
        ///     update 3D view curve data with origin data and transfer matrix
        /// </summary>
        /// <param name="points"></param>
        private void SynChroData(List<XYZ> points)
        {
            var size = points.Count;
            var points2D = new PointF[size];
            List<XYZ> transferedPoints = new();
            for (var i = 0; i < size; i++)
            {
                var point = points[i];
                var temp = TransferRotate(point);
                var transferedPoint = TransferMove(temp);
                points2D[i] = new PointF((float)transferedPoint.X, (float)transferedPoint.Y);
            }

            m_transferedCurves.Add(transferedPoints);
            m_curves2D.Add(points2D);
        }

        private XYZ UpdateMaxRange(XYZ pnt)
        {
            return new XYZ(
                pnt.X > OriginMax.X ? pnt.X : OriginMax.X,
                pnt.Y > OriginMax.Y ? pnt.Y : OriginMax.Y,
                pnt.Z > OriginMax.Z ? pnt.Z : OriginMax.Z);
        }

        private XYZ UpdateMinRange(XYZ pnt)
        {
            return new XYZ(
                pnt.X < OriginMin.X ? pnt.X : OriginMin.X,
                pnt.Y < OriginMin.Y ? pnt.Y : OriginMin.Y,
                pnt.Z < OriginMin.Z ? pnt.Z : OriginMin.Z);
        }

        private XYZ TransferRotate(XYZ point)
        {
            var x = point.X;
            var y = point.Y;
            var z = point.Z;

            return new XYZ(
                (x * Origin[0, 0]) + (y * Origin[0, 1]) + (z * Origin[0, 2]),
                (x * Origin[1, 0]) + (y * Origin[1, 1]) + (z * Origin[1, 2]),
                (x * Origin[2, 0]) + (y * Origin[2, 1]) + (z * Origin[2, 2]));
        }

        /// <summary>
        ///     move the point so that the center of the curves in 3D view is origin
        /// </summary>
        /// <param name="point">points to be moved</param>
        /// <returns>moved result</returns>
        private XYZ TransferMove(XYZ point)
        {
            //transform the origin of the old coordinate system in the new coordinate system

            return new XYZ(
                point.X - ((TransferedMax.X + TransferedMin.X) / 2),
                point.Y - ((TransferedMax.Y + TransferedMin.Y) / 2),
                point.Z - ((TransferedMax.Z + TransferedMin.Z) / 2));
        }
    }

    public class MatrixArith
    {
        public static double[,] MultiCross(double[,] m1, double[,] m2)
        {
            var result = new double[3, 3];

            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    for (var k = 0; k < 3; k++)
                        result[i, j] += m1[i, k] * m2[k, j];

            return result;
        }
    }
}
