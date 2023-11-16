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

    /// <summary>
    ///     Generating a new sheet that has all the selected views placed in.
    ///     Updating and retrieving properties of a selected viewport.
    /// </summary>
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

        /// <summary>
        ///     Constructor of views object.
        /// </summary>
        /// <param name="doc">the active document</param>
        public ViewsMgr(Document doc)
        {
            m_doc = doc;
            GetAllViews(doc);
            GetTitleBlocks(doc);
        }

        /// <summary>
        ///     Tree node store all views' names.
        /// </summary>
        public TreeNode AllViewsNames { get; } = new TreeNode("Views (all)");

        /// <summary>
        ///     List of all title blocks' names.
        /// </summary>
        public ArrayList AllTitleBlocksNames { get; } = new ArrayList();

        /// <summary>
        ///     The selected sheet's name.
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        ///     Update Form data members bonded to UI controls.
        /// </summary>
        /// <param name="form">The Form to be updated.</param>
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

        /// <summary>
        ///     Select a viewport by its associated view name and sheet name.
        /// </summary>
        /// <param name="form">The Form to be updated.</param>
        /// <param name="selectSheetName"> Sheet name.</param>
        /// <param name="selectAssociatedViewName">Associated view name.</param>
        public bool SelectViewport(AllViewsForm form, string selectSheetName, string selectAssociatedViewName)
        {
            m_vp = null;
            form.InvalidViewport = true;

            var fec = new FilteredElementCollector(m_doc);
            fec.OfClass(typeof(View));
            var viewSheets = fec.Cast<View>().Where(vp => !vp.IsTemplate && vp.ViewType == ViewType.DrawingSheet);

            foreach (var view in viewSheets)
                if (view.Name.Equals(selectSheetName))
                {
                    var viewSheet = (ViewSheet)view;
                    foreach (var vpId in viewSheet.GetAllViewports())
                    {
                        var vp = (Viewport)m_doc.GetElement(vpId);

                        var associatedView = m_doc.GetElement(vp.ViewId) as View;

                        if (associatedView.Name.Equals(selectAssociatedViewName))
                        {
                            m_vp = vp;
                            break;
                        }
                    }
                }

            if (m_vp == null) throw new InvalidOperationException("Viewport not found.");

            form.InvalidViewport = false;
            UpdateViewportProperties(form);
            return true;
        }

        /// <summary>
        ///     Change viewport label offset.
        /// </summary>
        /// <param name="form">The Form to be updated.</param>
        /// <param name="labelOffsetX">Label offset X component.</param>
        /// <param name="labelOffsetY">Label offset Y component.</param>
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

        /// <summary>
        ///     Change viewport label length.
        /// </summary>
        /// <param name="form">The Form to be updated.</param>
        /// <param name="labelLineLength">Label line length.</param>
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

        /// <summary>
        ///     Change viewport orientation.
        /// </summary>
        /// <param name="form">The Form to be updated.</param>
        /// <param name="rotation">Label line rotation.</param>
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

        /// <summary>
        ///     Finds all the views in the active document.
        /// </summary>
        /// <param name="doc">the active document</param>
        private void GetAllViews(Document doc)
        {
            var collector = new FilteredElementCollector(doc);
            var itor = collector.OfClass(typeof(View)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                // skip view templates because they're invisible in project browser
                if (!(itor.Current is View view) || view.IsTemplate) continue;

                if (!(doc.GetElement(view.GetTypeId()) is ElementType objType) || objType.Name.Equals("Schedule")
                                                                               || objType.Name.Equals("Drawing Sheet"))
                    continue;

                m_allViews.Insert(view);
                AssortViews(view.Name, objType.Name);
            }
        }

        /// <summary>
        ///     Assort all views for tree view displaying.
        /// </summary>
        /// <param name="view">The view assorting</param>
        /// <param name="type">The type of view</param>
        private void AssortViews(string view, string type)
        {
            foreach (TreeNode t in AllViewsNames.Nodes)
                if (t.Tag.Equals(type))
                {
                    t.Nodes.Add(new TreeNode(view));
                    return;
                }

            var categoryNode = new TreeNode(type)
            {
                Tag = type
            };
            if (type.Equals("Building Elevation"))
                categoryNode.Text = "Elevations [" + type + "]";
            else
                categoryNode.Text = type + "s";
            categoryNode.Nodes.Add(new TreeNode(view));
            AllViewsNames.Nodes.Add(categoryNode);
        }

        /// <summary>
        ///     Retrieve the checked view from tree view.
        /// </summary>
        public void SelectViews()
        {
            var names = new ArrayList();
            foreach (TreeNode t in AllViewsNames.Nodes)
            foreach (TreeNode n in t.Nodes)
                if (n.Checked && 0 == n.Nodes.Count)
                    names.Add(n.Text);

            foreach (View v in m_allViews)
            foreach (string s in names)
                if (s.Equals(v.Name))
                {
                    m_selectedViews.Insert(v);
                    break;
                }
        }

        /// <summary>
        ///     Generate sheet in active document.
        /// </summary>
        /// <param name="doc">the currently active document</param>
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

        /// <summary>
        ///     Retrieve the title block to be generate by its name.
        /// </summary>
        /// <param name="name">The title block's name</param>
        public void ChooseTitleBlock(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            foreach (FamilySymbol f in m_allTitleBlocks)
                if (name.Equals(f.Family.Name + ":" + f.Name))
                {
                    m_titleBlock = f;
                    return;
                }
        }

        /// <summary>
        ///     Retrieve all available title blocks in the currently active document.
        /// </summary>
        /// <param name="doc">the currently active document</param>
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
                AllTitleBlocksNames.Add(f.Family.Name + ":" + f.Name);
                if (null == m_titleBlock) m_titleBlock = f;
            }
        }

        /// <summary>
        ///     Place all selected views on this sheet's appropriate location.
        /// </summary>
        /// <param name="views">all selected views</param>
        /// <param name="sheet">all views located sheet</param>
        private void PlaceViews(ViewSet views, ViewSheet sheet)
        {
            double xDistance = 0;
            double yDistance = 0;
            CalculateDistance(sheet.Outline, views.Size, ref xDistance, ref yDistance);

            var origin = GetOffSet(sheet.Outline, xDistance, yDistance);
            //Autodesk.Revit.DB.UV temp = new Autodesk.Revit.DB.UV (origin.U, origin.V);
            var tempU = origin.U;
            var tempV = origin.V;
            var n = 1;
            foreach (View v in views)
            {
                var location = new UV(tempU, tempV);
                var view = v;
                Rescale(view, xDistance, yDistance);
                try
                {
                    //sheet.AddView(view, location);
                    Viewport.Create(view.Document, sheet.Id, view.Id, new XYZ(location.U, location.V, 0));
                }
                catch (ArgumentException /*ae*/)
                {
                    throw new InvalidOperationException("The view '" + view.Name +
                                                        "' can't be added, it may have already been placed in another sheet.");
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

        /// <summary>
        ///     Retrieve the appropriate origin.
        /// </summary>
        /// <param name="bBox"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private UV GetOffSet(BoundingBoxUV bBox, double x, double y)
        {
            return new UV(bBox.Min.U + x * m_goldensection, bBox.Min.V + y * m_goldensection);
        }

        /// <summary>
        ///     Calculate the appropriate distance between the views lay on the sheet.
        /// </summary>
        /// <param name="bBox">The outline of sheet.</param>
        /// <param name="amount">Amount of views.</param>
        /// <param name="x">Distance in x axis between each view</param>
        /// <param name="y">Distance in y axis between each view</param>
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

        /// <summary>
        ///     Rescale the view's Scale value for suitable.
        /// </summary>
        /// <param name="view">The view to be located on sheet.</param>
        /// <param name="x">Distance in x axis between each view</param>
        /// <param name="y">Distance in y axis between each view</param>
        private static void Rescale(View view, double x, double y)
        {
            double rescale = 2;
            var outline = new UV(view.Outline.Max.U - view.Outline.Min.U,
                view.Outline.Max.V - view.Outline.Min.V);

            if (outline.U > outline.V)
                rescale = outline.U / x * rescale;
            else
                rescale = outline.V / y * rescale;

            if (1 != view.Scale && 0 != rescale) view.Scale = (int)(view.Scale * rescale);
        }
    }
}
