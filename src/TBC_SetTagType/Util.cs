#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static FamilySymbol GetFirstFamilySymbol(
            Document doc,
            BuiltInCategory bic)
        {
            var s = GetElementsOfType(doc,
                    typeof(FamilySymbol), bic)
                .FirstElement() as FamilySymbol;

            Debug.Assert(null != s, $"expected at least one {bic.ToString()} symbol in project");

            return s;
        }

        // Default empty RAC project returns Level 1 and Level 2.
        public static bool GetBottomAndTopLevels(
            Document doc,
            ref Level levelBottom,
            ref Level levelTop)
        {
            var levels
                = GetElementsOfType(doc, typeof(Level),
                    BuiltInCategory.OST_Levels);

            foreach (var e in levels)
                if (null == levelBottom)
                    levelBottom = e as Level;
                else if (null == levelTop)
                    levelTop = e as Level;
                else
                    break;

            if (levelTop.Elevation < levelBottom.Elevation)
            {
                var tmp = levelTop;
                levelTop = levelBottom;
                levelBottom = tmp;
            }

            return null != levelBottom && null != levelTop;
        }

        // https://forums.autodesk.com/t5/revit-api-forum/macro-doesnt-work-properly-on-big-projects/m-p/10186076
        public static void SetTagColorToElementColor(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            var sel = uiDoc.Selection.PickObject(ObjectType.Element);
            var ele = doc.GetElement(sel.ElementId);
            var cat = ele.Category;
            var builtInCat = (BuiltInCategory)cat.Id.Value;

            var views = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(v => !v.IsTemplate)
                .Where(v
                    => 0 < new FilteredElementCollector(doc, v.Id)
                        .OfCategory(builtInCat)
                        .GetElementCount());

            foreach (var view in views)
            {
                var tags = new FilteredElementCollector(doc, view.Id)
                    .OfCategory(builtInCat)
                    .WhereElementIsNotElementType();

                List<Element> taggedElements = [];

                foreach (var e in tags)
                {
                    var tag = doc.GetElement(e.Id) as IndependentTag;
                    var taggedElems = tag.GetTaggedLocalElements();
                    taggedElements.AddRange(taggedElems);
                }

                List<Color> tagColor = [];

                var builtInParam
                    = (BuiltInParameter)Enum.Parse(
                        typeof(BuiltInParameter),
                        taggedElements[0]
                            .GetParameters("System Type")[0]
                            .Id.ToString());

                foreach (var e in taggedElements)
                {
                    var elem = doc.GetElement(e.Id);
                    var param = elem.get_Parameter(builtInParam);
                    if (param == null)
                        return;
                    var systemType = doc.GetElement(param.AsElementId())
                        as MEPSystemType;

                    var c = systemType.LineColor;

                    var systemColorRed = c.Red;
                    var systemColorGreen = c.Green;
                    var systemColorblue = c.Blue;

                    Color color = new(systemColorRed,
                        systemColorGreen, systemColorblue);
                    tagColor.Add(color);
                }

                OverrideGraphicSettings overRide = new();

                Transaction trans1 = new(doc);

                trans1.Start("change color tag");

                var index = 0;

                foreach (var e in tags)
                {
                    doc.ActiveView.SetElementOverrides(e.Id,
                        overRide.SetProjectionLineColor(
                            tagColor[index]));
                    index += 1;
                }

                trans1.Commit();

                index = 0;
            }
        }

        // Tag extents via temporary leader move; see https://forums.autodesk.com/t5/revit-api-forum/tag-width-height-or-accurate-boundingbox-of-independenttag/m-p/11274095
        public static Tuple<double, double> GetTagExtents(
            IndependentTag tag)
        {
            var doc = tag.Document;

            double tagWidth;
            double tagHeight;

            var sec = doc.GetElement(tag.OwnerViewId) as View;
            var rightDirection = sec.RightDirection;
            var upDirection = sec.UpDirection;
            var pipeReference = tag.GetTaggedReferences().First();

            using (TransactionGroup transG = new(doc))
            {
                transG.Start("Determine Tag Dimension");

                using (Transaction trans = new(doc))
                {
                    trans.Start("Determine Tag Dimension");

                    tag.LeaderEndCondition = LeaderEndCondition.Free;
                    var leaderEndPoint = tag.GetLeaderEnd(pipeReference);
                    tag.TagHeadPosition = leaderEndPoint;
                    tag.SetLeaderElbow(pipeReference, leaderEndPoint);

                    trans.Commit();
                }

                var tagBox = tag.get_BoundingBox(sec);
                tagWidth = (tagBox.Max - tagBox.Min).DotProduct(rightDirection);
                tagHeight = (tagBox.Max - tagBox.Min).DotProduct(upDirection);

                transG.RollBack();
            }
            return Tuple.Create(tagWidth, tagHeight);
        }
    }
}
