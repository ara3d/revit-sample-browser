// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.AllViews.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            if (null == commandData) throw new ArgumentNullException(nameof(commandData));

            var doc = commandData.Application.ActiveUIDocument.Document;
            var view = new ViewsMgr(doc);

            var dlg = new AllViewsForm(view);

            try
            {
                if (dlg.ShowDialog() == DialogResult.OK) return view.GenerateSheet(doc);
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }

    public class ViewsMgr
    {
        private readonly double m_goldensection = 0.618;
        private IList<Element> m_allTitleBlocks = new List<Element>();
        private readonly ViewSet m_allViews = new ViewSet();

        private readonly Document m_doc;
        private double m_rows;
        private readonly ViewSet m_selectedViews = new ViewSet();
        private FamilySymbol m_titleBlock;

        private Viewport m_vp;

        private readonly double m_titlebar = 0.2;

        public ViewsMgr(Document doc)
        {
            m_doc = doc;
            GetAllViews(doc);
            GetTitleBlocks(doc);
        }

        public TreeNode AllViewsNames { get; } = new TreeNode("Views (all)");

        public ArrayList AllTitleBlocksNames { get; } = new ArrayList();

        public string SheetName { get; set; }

        public void UpdateViewportProperties(AllViewsForm form)
        {
            form.GetMinBoxOutline = m_vp.GetBoxOutline().MinimumPoint;
            form.GetMaxBoxOutline = m_vp.GetBoxOutline().MaximumPoint;

            form.GetMinLabelOutline = m_vp.GetLabelOutline().MinimumPoint;
            form.GetMaxLabelOutline = m_vp.GetLabelOutline().MaximumPoint;

            form.GetLabelLineOffset = m_vp.LabelOffset;
            form.GetLabelLineLength = m_vp.LabelLineLength;

            form.GetBoxCenter = m_vp.GetBoxCenter();
            form.GetOrientation = m_vp.Rotation;
        }

        public bool SelectViewport(AllViewsForm form, string selectSheetName, string selectAssociatedViewName)
        {
            m_vp = null;
            form.InvalidViewport = true;

            var viewSheet = m_doc.GetElements<ViewSheet>()
                .FirstOrDefault(vp => !vp.IsTemplate 
                                      && vp.ViewType == ViewType.DrawingSheet 
                                      && vp.Name == selectSheetName);

            if (viewSheet != null)
            {
                m_vp = m_doc
                    .GetElements<Viewport>(viewSheet.GetAllViewports())
                    .First(vp => m_doc.GetElement<View>(vp.ViewId).Name == selectAssociatedViewName);
            }

            if (m_vp == null)
            {
                throw new InvalidOperationException("Viewport not found.");
            }

            form.InvalidViewport = false;
            UpdateViewportProperties(form);
            return true;
        }

        public void SetLabelOffset(AllViewsForm form,
            double labelOffsetX, double labelOffsetY)
        {
            using (var t = new Transaction(m_doc, "Change label offset"))
            {
                t.Start();

                m_vp.LabelOffset = new XYZ(labelOffsetX, labelOffsetY, 0.0);

                t.Commit();

                UpdateViewportProperties(form);
            }
        }

        public void SetLabelLength(AllViewsForm form, double labelLineLength)
        {
            using (var t = new Transaction(m_doc, "Change label length"))
            {
                t.Start();

                m_vp.LabelLineLength = labelLineLength;

                t.Commit();

                UpdateViewportProperties(form);
            }
        }

        public void SetRotation(AllViewsForm form, ViewportRotation rotation)
        {
            using (var t = new Transaction(m_doc, "Change label orientation"))
            {
                t.Start();

                m_vp.Rotation = rotation;

                t.Commit();

                UpdateViewportProperties(form);
            }
        }

        private void GetAllViews(Document doc)
        {
            foreach (var view in doc.GetElements<View>())
            {
                // skip view templates because they're invisible in project browser
                if (!(doc.GetElement(view.GetTypeId()) is ElementType objType) 
                    || objType.Name.Equals("Schedule")
                    || objType.Name.Equals("Drawing Sheet"))
                    continue;

                m_allViews.Insert(view);
                AssortViews(view.Name, objType.Name);
            }
        }

        private void AssortViews(string view, string type)
        {
            foreach (TreeNode t in AllViewsNames.Nodes)
            {
                if (t.Tag.Equals(type))
                {
                    t.Nodes.Add(new TreeNode(view));
                    return;
                }
            }

            var categoryNode = new TreeNode(type)
            {
                Tag = type
            };
            if (type.Equals("Building Elevation"))
                categoryNode.Text = $"Elevations [{type}]";
            else
                categoryNode.Text = $"{type}s";
            categoryNode.Nodes.Add(new TreeNode(view));
            AllViewsNames.Nodes.Add(categoryNode);
        }

        public void SelectViews()
        {
            var names = new ArrayList();
            foreach (TreeNode t in AllViewsNames.Nodes)
            {
                foreach (TreeNode n in t.Nodes)
                {
                    if (n.Checked && 0 == n.Nodes.Count)
                        names.Add(n.Text);
                }
            }

            foreach (View v in m_allViews)
            {
                foreach (string s in names)
                {
                    if (s.Equals(v.Name))
                    {
                        m_selectedViews.Insert(v);
                        break;
                    }
                }
            }
        }

        public Result GenerateSheet(Document doc)
        {
            if (null == doc) throw new ArgumentNullException(nameof(doc));

            if (m_selectedViews.IsEmpty)
                throw new InvalidOperationException("No view be selected, generate sheet be canceled.");

            var result = Result.Succeeded;

            using (var newTran = new Transaction(doc, "AllViews_Sample"))
            {
                newTran.Start();

                try
                {
                    var sheet = ViewSheet.Create(doc, m_titleBlock.Id);
                    sheet.Name = SheetName;
                    PlaceViews(m_selectedViews, sheet);
                }
                catch (Exception)
                {
                    result = Result.Failed;
                }

                if (result == Result.Succeeded)
                {
                    newTran.Commit();
                }
                else
                {
                    newTran.RollBack();
                    throw new InvalidOperationException("Failed to generate sheet view and/or its viewports.");
                }
            }

            return result;
        }

        public void ChooseTitleBlock(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            foreach (FamilySymbol f in m_allTitleBlocks)
            {
                if (name.Equals($"{f.Family.Name}:{f.Name}"))
                {
                    m_titleBlock = f;
                    return;
                }
            }
        }

        private void GetTitleBlocks(Document doc)
        {
            var filteredElementCollector = new FilteredElementCollector(doc);
            filteredElementCollector.OfClass(typeof(FamilySymbol));
            filteredElementCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            m_allTitleBlocks = filteredElementCollector.ToElements();
            if (0 == m_allTitleBlocks.Count)
                throw new InvalidOperationException("There is no title block to generate sheet.");

            foreach (var element in m_allTitleBlocks)
            {
                var f = element as FamilySymbol;
                AllTitleBlocksNames.Add($"{f.Family.Name}:{f.Name}");
                if (null == m_titleBlock) m_titleBlock = f;
            }
        }

        private void PlaceViews(ViewSet views, View sheet)
        {
            double xDistance = 0;
            double yDistance = 0;
            CalculateDistance(sheet.Outline, views.Size, ref xDistance, ref yDistance);

            var origin = GetOffSet(sheet.Outline, xDistance, yDistance);
            var tempU = origin.U;
            var tempV = origin.V;
            var n = 1;
            foreach (View v in views)
            {
                var location = new UV(tempU, tempV);
                var view = v;
                SampleBrowserUtils.Rescale(view, xDistance, yDistance);
                try
                {
                    //sheet.AddView(view, location);
                    Viewport.Create(view.Document, sheet.Id, view.Id, new XYZ(location.U, location.V, 0));
                }
                catch (ArgumentException /*ae*/)
                {
                    throw new InvalidOperationException(
                        $"The view '{view.Name}' can't be added, it may have already been placed in another sheet.");
                }

                if (0 != n++ % m_rows)
                {
                    tempU += xDistance * (1 - m_titlebar);
                }
                else
                {
                    tempU = origin.U;
                    tempV += yDistance;
                }
            }
        }

        private UV GetOffSet(BoundingBoxUV bBox, double x, double y)
        {
            return new UV(bBox.Min.U + x * m_goldensection, bBox.Min.V + y * m_goldensection);
        }

        private void CalculateDistance(BoundingBoxUV bBox, int amount, ref double x, ref double y)
        {
            var xLength = (bBox.Max.U - bBox.Min.U) * (1 - m_titlebar);
            var yLength = bBox.Max.V - bBox.Min.V;

            //calculate appropriate rows numbers.
            var result = Math.Sqrt(amount);

            while (0 < result - (int)result)
            {
                amount++;
                result = Math.Sqrt(amount);
            }

            m_rows = result;
            var area = xLength * yLength / amount;

            //calculate appropriate distance between the views.
            if (bBox.Max.U > bBox.Max.V)
            {
                x = Math.Sqrt(area / m_goldensection);
                y = m_goldensection * x;
            }
            else
            {
                y = Math.Sqrt(area / m_goldensection);
                x = m_goldensection * y;
            }
        }
    }
}
