#region Namespaces

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Windows;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        private const double SetTangentLock_mm_per_inch = 25.4;
        private const double SetTangentLock_inches_per_foot = 12.0;
        private const double SetTangentLock_mm = SetTangentLock_mm_per_inch * SetTangentLock_inches_per_foot;
        private const double SetTangentLock_r1 = 16 * SetTangentLock_mm;
        private const double SetTangentLock_r2 = 12 * SetTangentLock_mm;

        public static void SetTangentLockInProfileSketch1(
            Document famdoc,
            Form[] extrusions)
        {
            ICollection<ElementId> delIds = null;
            List<ElementId> enmIDs = [];

            using (SubTransaction delTrans = new(famdoc))
            {
                try
                {
                    delTrans.Start();
                    delIds = famdoc.Delete(extrusions[0].Id);
                    delTrans.RollBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            List<ModelArc> mArcsR1 = [];
            List<ModelArc> mArcsR2 = [];

            foreach (var id in delIds) enmIDs.Add(id);

            for (var i = 0; i < enmIDs.Count; i++)
            {
                var ele = famdoc.GetElement(enmIDs[i]);
                if (ele is ModelArc ma)
                {
                    var c = ma.GeometryCurve;
                    var a = c as Arc;

                    if (Math.Round(SetTangentLock_r1, 6) == Math.Round(a.Radius, 6)) mArcsR1.Add(ma);
                    if (Math.Round(SetTangentLock_r2, 6) == Math.Round(a.Radius, 6)) mArcsR2.Add(ma);
                }

                if (ele is ModelLine ml)
                {
                    Element before = null;
                    Element after = null;
                    ElementId beforeId = null;
                    ElementId afterId = null;

                    if (i > 0)
                    {
                        before = famdoc.GetElement(enmIDs[i - 1]);
                        beforeId = enmIDs[i - 1];
                    }
                    else
                    {
                        before = famdoc.GetElement(enmIDs[enmIDs.Count - 1]);
                        beforeId = enmIDs[enmIDs.Count - 1];
                    }

                    if (i == enmIDs.Count - 1)
                    {
                        after = famdoc.GetElement(enmIDs[0]);
                        afterId = enmIDs[0];
                    }
                    else
                    {
                        after = famdoc.GetElement(enmIDs[i + 1]);
                        afterId = enmIDs[i + 1];
                    }

                    if (before is ModelArc && after is ModelArc)
                    {
                        ml.SetTangentLock(0, beforeId, true);
                        ml.SetTangentLock(1, afterId, true);
                    }
                }
            }
        }

        public static void SetTangentLockInProfileSketch2(
            Document famdoc,
            Form[] extrusions)
        {
            ICollection<ElementId> delIds = null;
            List<ElementId> enmIDs = [];

            using (SubTransaction delTrans = new(famdoc))
            {
                try
                {
                    delTrans.Start();
                    delIds = famdoc.Delete(extrusions[0].Id);
                    delTrans.RollBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            List<ModelArc> mArcsR1 = [];
            List<ModelArc> mArcsR2 = [];

            foreach (var id in delIds) enmIDs.Add(id);

            for (var i = 0; i < enmIDs.Count; i++)
            {
                var ele = famdoc.GetElement(enmIDs[i]);
                if (ele is ModelArc ma)
                {
                    var c = ma.GeometryCurve;
                    var a = c as Arc;

                    if (Math.Round(SetTangentLock_r1, 6) == Math.Round(a.Radius, 6)) mArcsR1.Add(ma);
                    if (Math.Round(SetTangentLock_r2, 6) == Math.Round(a.Radius, 6)) mArcsR2.Add(ma);
                }

                if (ele is ModelLine ml)
                {
                    ElementId beforeId = null;
                    ElementId afterId = null;

                    var joinedBefore = ml.GetAdjoinedCurveElements(0);
                    foreach (var id in joinedBefore)
                    {
                        var joinedEle = famdoc.GetElement(id);

                        if (joinedEle is ModelArc)
                        {
                            beforeId = id;
                            break;
                        }
                    }

                    var joinedAfter = ml.GetAdjoinedCurveElements(1);
                    foreach (var id in joinedAfter)
                    {
                        var joinedEle = famdoc.GetElement(id);

                        if (joinedEle is ModelArc)
                        {
                            afterId = id;
                            break;
                        }
                    }

                    if (beforeId != null
                        && afterId != null
                        && ml.HasTangentJoin(0, beforeId)
                        && ml.HasTangentJoin(1, afterId))
                    {
                        ml.SetTangentLock(0, beforeId, true);
                        ml.SetTangentLock(1, afterId, true);
                    }
                }
            }
        }
    }
}
