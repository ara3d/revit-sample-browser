// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    /// <summary>
    /// Create a sheet with four views (right, front, top, and isometric 3D)
    /// for a selected element, then print the sheet to PDF and JPG.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdCreateAndPrintSheetAndViews : IExternalCommand
    {
        const string _title_block_name = "AIR CRO - Plano DIN A4";
        const int _view_scale = 20;
        const int _view_scale_3d = 25;

        static TextNote CreateTextNote(
            View view,
            string text,
            XYZ p,
            double rotation = 0)
        {
            Document doc = view.Document;

            var options = new TextNoteOptions
            {
                Rotation = rotation,
                HorizontalAlignment = HorizontalTextAlignment.Center,
                VerticalAlignment = VerticalTextAlignment.Middle,
                TypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType)
            };

            return TextNote.Create(doc, view.Id, p, text, options);
        }

        const double angleHorizD = 135;
        const double angleVertD = -30;

        static XYZ VectorFromHorizVertAngles(
            double angleHorizD,
            double angleVertD)
        {
            double degToRadian = Math.PI * 2 / 360;
            double angleHorizR = angleHorizD * degToRadian;
            double angleVertR = angleVertD * degToRadian;

            double a = Math.Cos(angleVertR);
            double b = Math.Cos(angleHorizR);
            double c = Math.Sin(angleHorizR);
            double d = Math.Sin(angleVertR);

            return new XYZ(a * b, a * c, d);
        }

        static View3D CreateView3d(Document doc)
        {
            ViewFamilyType viewFamilyType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .Where(v => ViewFamily.ThreeDimensional == v.ViewFamily)
                    .First();

            View3D view = View3D.CreateIsometric(doc, viewFamilyType.Id);

            XYZ eye = XYZ.Zero;

            XYZ forward = VectorFromHorizVertAngles(
                angleHorizD, angleVertD);

            XYZ up = VectorFromHorizVertAngles(
                angleHorizD, angleVertD + 90);

            ViewOrientation3D viewOrientation3D
                = new ViewOrientation3D(eye, up, forward);

            view.SetOrientation(viewOrientation3D);
            view.SaveOrientation();
            view.DetailLevel = ViewDetailLevel.Fine;
            view.Scale = _view_scale_3d;

            return view;
        }

        static ViewSection CreateViewSection(
            Document doc,
            XYZ pOrigin,
            double halfsize,
            XYZ vRight,
            XYZ vUp,
            ref List<ElementId> idsToShow)
        {
            ViewFamilyType viewFamilyType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .Where(v => ViewFamily.Section == v.ViewFamily)
                    .First();

            XYZ vSize = new XYZ(halfsize, halfsize, halfsize);

            Transform transform = Transform.Identity;

            transform.Origin = pOrigin;
            transform.BasisX = vUp.CrossProduct(vRight);
            transform.BasisY = vUp;
            transform.BasisZ = vRight;
            Debug.Assert(Util.IsEqual(1, transform.Determinant), "expected 1 determinant");

            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
            sectionBox.Transform = transform;
            sectionBox.Min = -vSize;
            sectionBox.Max = vSize;

            ViewSection viewSection = ViewSection.CreateSection(
                doc, viewFamilyType.Id, sectionBox);

            viewSection.DetailLevel = ViewDetailLevel.Fine;
            viewSection.Scale = _view_scale;

            double f = 0.2 * halfsize;
            idsToShow.Add(CreateTextNote(viewSection, "O", pOrigin).Id);
            idsToShow.Add(CreateTextNote(viewSection, "R", pOrigin + f * vRight).Id);
            idsToShow.Add(CreateTextNote(viewSection, "U", pOrigin + f * vUp).Id);

            return viewSection;
        }

        static Connector GetPrimaryConnector(ConnectorSet conset)
        {
            Connector primary_connector = null;
            foreach (Connector c in conset)
            {
                MEPConnectorInfo info = c.GetMEPConnectorInfo();
                if (info.IsPrimary)
                {
                    primary_connector = c;
                    break;
                }
            }
            return primary_connector;
        }

        static Transform GetDuctLcs(FabricationPart part)
        {
            ConnectorManager conmgr = part.ConnectorManager;
            ConnectorSet conset = conmgr.Connectors;
            Connector start = GetPrimaryConnector(conset);
            Transform twcs = start.CoordinateSystem;
            Debug.Assert(Util.IsEqual(1, twcs.Determinant), "expected 1 twcs determinant");
            twcs.BasisY = -(twcs.BasisY);
            twcs.BasisZ = -(twcs.BasisZ);
            Debug.Assert(Util.IsEqual(1, twcs.Determinant), "expected 1 flipped twcs determinant");
            return twcs;
        }

        static XYZ MidPoint(XYZ a, XYZ b)
        {
            return a + 0.5 * (b - a);
        }

        static void IsolateElementInView(IList<ElementId> idsToShow, View v)
        {
            v.IsolateElementsTemporary(idsToShow);
        }

        /// <summary>
        /// Create a sheet and four views for the given element: right, front,
        /// top and 3D, isolated and zoomed. For fabrication parts, orient
        /// the views according to the duct LCS.
        /// </summary>
        public static void CreateSheetAndViewsFor(Element e)
        {
            Document doc = e.Document;
            BoundingBoxXYZ bb = e.get_BoundingBox(null);
            XYZ p = MidPoint(bb.Min, bb.Max);
            double halfsize = 0.5 * (bb.Max - bb.Min).GetLength();

            FabricationPart part = e as FabricationPart;
            Transform twcs = (null != part)
                ? GetDuctLcs(part)
                : Transform.Identity;

            List<ElementId> idsToShowFront = new List<ElementId>() { e.Id };
            List<ElementId> idsToShowRight = new List<ElementId>() { e.Id };
            List<ElementId> idsToShowTop = new List<ElementId>() { e.Id };

            View view3d = CreateView3d(doc);
            View viewFront = CreateViewSection(doc, p, halfsize, twcs.BasisY, twcs.BasisZ, ref idsToShowFront);
            View viewRight = CreateViewSection(doc, p, halfsize, twcs.BasisX, twcs.BasisZ, ref idsToShowRight);
            View viewTop = CreateViewSection(doc, p, halfsize, twcs.BasisX, twcs.BasisY, ref idsToShowTop);

            view3d.Name = "3D";
            viewFront.Name = "Front";
            viewRight.Name = "Right";
            viewTop.Name = "Top";

            IsolateElementInView(new List<ElementId>(1) { e.Id }, view3d);
            IsolateElementInView(idsToShowFront, viewFront);
            IsolateElementInView(idsToShowRight, viewRight);
            IsolateElementInView(idsToShowTop, viewTop);

            FamilySymbol titleBlock
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .Where<Element>(x => x.Name.Equals(_title_block_name))
                    .First() as FamilySymbol;

            if (null != titleBlock)
            {
                ViewSheet viewSheet = ViewSheet.Create(doc, titleBlock.Id);
                if (null != viewSheet)
                {
                    string s = Util.GetProductCode(e);
                    if (null == s)
                    {
                        s = string.Empty;
                    }
                    else
                    {
                        s += "-";
                    }

                    viewSheet.SheetNumber = "00";
                    viewSheet.Name = $"Part CAM Dimension Sheet for {s}{e.Id.Value}";

                    UV pmin = viewSheet.Outline.Min;
                    UV pmax = viewSheet.Outline.Max;
                    UV v = pmax - pmin;
                    double w = v.U;
                    double h = v.V;
                    double left = pmin.U + 0.1 * w;
                    double bottom = pmin.V + 0.15 * h;
                    w *= 0.9;
                    h *= 0.8;
                    XYZ pul = new XYZ(left + 0.25 * w, bottom + 0.75 * h, 0);
                    XYZ pur = new XYZ(left + 0.75 * w, bottom + 0.75 * h, 0);
                    XYZ pll = new XYZ(left + 0.25 * w, bottom + 0.3 * h, 0);
                    XYZ plr = new XYZ(left + 0.7 * w, bottom + 0.3 * h, 0);

                    Viewport.Create(doc, viewSheet.Id, viewRight.Id, pul);
                    Viewport.Create(doc, viewSheet.Id, viewFront.Id, pur);
                    Viewport.Create(doc, viewSheet.Id, viewTop.Id, pll);
                    Viewport.Create(doc, viewSheet.Id, view3d.Id, plr);

                    if (viewSheet.CanBePrinted)
                    {
                        IList<ElementId> viewIds = new List<ElementId>(1) {
                            viewSheet.Id };

                        string dir = "C:/tmp";
                        string project_name = doc.Title;
                        string path = dir + "/" + project_name;

                        PDFExportOptions opt = new PDFExportOptions();
                        opt.FileName = viewSheet.Name;
                        doc.Export(dir, viewIds, opt);

                        ImageExportOptions imgopt = new ImageExportOptions();
                        imgopt.ExportRange = ExportRange.SetOfViews;
                        imgopt.SetViewsAndSheets(viewIds);
                        imgopt.FilePath = path;

                        doc.ExportImage(imgopt);
                    }
                }
            }
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            ElementId id_sample_element;

            try
            {
                Reference r = sel.PickObject(
                    ObjectType.Element,
                    "Please select an element");

                id_sample_element = r.ElementId;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            uiapp.DialogBoxShowing
                += new EventHandler<DialogBoxShowingEventArgs>(
                    Command.OnDialogBoxShowing);

            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create sheet and four views");
                Element e = doc.GetElement(id_sample_element);
                CreateSheetAndViewsFor(e);
                bool save = Util.AskYesNoQuestion("Save the sheet?");
                if (save)
                {
                    t.Commit();
                }
                else
                {
                    t.RollBack();
                }
            }
            return Result.Succeeded;
        }
    }
}
