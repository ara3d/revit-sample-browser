// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
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
            var doc = view.Document;

            TextNoteOptions options = new()
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
            var degToRadian = Math.PI * 2 / 360;
            var angleHorizR = angleHorizD * degToRadian;
            var angleVertR = angleVertD * degToRadian;

            var a = Math.Cos(angleVertR);
            var b = Math.Cos(angleHorizR);
            var c = Math.Sin(angleHorizR);
            var d = Math.Sin(angleVertR);

            return new XYZ(a * b, a * c, d);
        }

        static View3D CreateView3d(Document doc)
        {
            var viewFamilyType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .Where(v => ViewFamily.ThreeDimensional == v.ViewFamily)
                    .First();

            var view = View3D.CreateIsometric(doc, viewFamilyType.Id);

            var eye = XYZ.Zero;

            var forward = VectorFromHorizVertAngles(
                angleHorizD, angleVertD);

            var up = VectorFromHorizVertAngles(
                angleHorizD, angleVertD + 90);

            ViewOrientation3D viewOrientation3D
                = new(eye, up, forward);

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
            var viewFamilyType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewFamilyType))
                    .Cast<ViewFamilyType>()
                    .Where(v => ViewFamily.Section == v.ViewFamily)
                    .First();

            XYZ vSize = new(halfsize, halfsize, halfsize);

            var transform = Transform.Identity;

            transform.Origin = pOrigin;
            transform.BasisX = vUp.CrossProduct(vRight);
            transform.BasisY = vUp;
            transform.BasisZ = vRight;
            Debug.Assert(Util.IsEqual(1, transform.Determinant), "expected 1 determinant");

            BoundingBoxXYZ sectionBox = new()
            {
                Transform = transform,
                Min = -vSize,
                Max = vSize
            };

            var viewSection = ViewSection.CreateSection(
                doc, viewFamilyType.Id, sectionBox);

            viewSection.DetailLevel = ViewDetailLevel.Fine;
            viewSection.Scale = _view_scale;

            var f = 0.2 * halfsize;
            idsToShow.Add(CreateTextNote(viewSection, "O", pOrigin).Id);
            idsToShow.Add(CreateTextNote(viewSection, "R", pOrigin + (f * vRight)).Id);
            idsToShow.Add(CreateTextNote(viewSection, "U", pOrigin + (f * vUp)).Id);

            return viewSection;
        }

        static Connector GetPrimaryConnector(ConnectorSet conset)
        {
            Connector primary_connector = null;
            foreach (Connector c in conset)
            {
                var info = c.GetMEPConnectorInfo();
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
            var conmgr = part.ConnectorManager;
            var conset = conmgr.Connectors;
            var start = GetPrimaryConnector(conset);
            var twcs = start.CoordinateSystem;
            Debug.Assert(Util.IsEqual(1, twcs.Determinant), "expected 1 twcs determinant");
            twcs.BasisY = -twcs.BasisY;
            twcs.BasisZ = -twcs.BasisZ;
            Debug.Assert(Util.IsEqual(1, twcs.Determinant), "expected 1 flipped twcs determinant");
            return twcs;
        }

        static XYZ MidPoint(XYZ a, XYZ b)
        {
            return a + (0.5 * (b - a));
        }

        static void IsolateElementInView(IList<ElementId> idsToShow, View v)
        {
            v.IsolateElementsTemporary(idsToShow);
        }

        public static void CreateSheetAndViewsFor(Element e)
        {
            var doc = e.Document;
            var bb = e.get_BoundingBox(null);
            var p = MidPoint(bb.Min, bb.Max);
            var halfsize = 0.5 * (bb.Max - bb.Min).GetLength();

            var twcs = (e is FabricationPart part)
                ? GetDuctLcs(part)
                : Transform.Identity;

            List<ElementId> idsToShowFront = [e.Id];
            List<ElementId> idsToShowRight = [e.Id];
            List<ElementId> idsToShowTop = [e.Id];

            View view3d = CreateView3d(doc);
            View viewFront = CreateViewSection(doc, p, halfsize, twcs.BasisY, twcs.BasisZ, ref idsToShowFront);
            View viewRight = CreateViewSection(doc, p, halfsize, twcs.BasisX, twcs.BasisZ, ref idsToShowRight);
            View viewTop = CreateViewSection(doc, p, halfsize, twcs.BasisX, twcs.BasisY, ref idsToShowTop);

            view3d.Name = "3D";
            viewFront.Name = "Front";
            viewRight.Name = "Right";
            viewTop.Name = "Top";

            IsolateElementInView([e.Id], view3d);
            IsolateElementInView(idsToShowFront, viewFront);
            IsolateElementInView(idsToShowRight, viewRight);
            IsolateElementInView(idsToShowTop, viewTop);


            if (new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilySymbol))
                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                    .Where<Element>(x => x.Name.Equals(_title_block_name))
                    .First() is FamilySymbol titleBlock)
            {
                var viewSheet = ViewSheet.Create(doc, titleBlock.Id);
                if (null != viewSheet)
                {
                    var s = Util.GetProductCode(e);
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

                    var pmin = viewSheet.Outline.Min;
                    var pmax = viewSheet.Outline.Max;
                    var v = pmax - pmin;
                    var w = v.U;
                    var h = v.V;
                    var left = pmin.U + (0.1 * w);
                    var bottom = pmin.V + (0.15 * h);
                    w *= 0.9;
                    h *= 0.8;
                    XYZ pul = new(left + (0.25 * w), bottom + (0.75 * h), 0);
                    XYZ pur = new(left + (0.75 * w), bottom + (0.75 * h), 0);
                    XYZ pll = new(left + (0.25 * w), bottom + (0.3 * h), 0);
                    XYZ plr = new(left + (0.7 * w), bottom + (0.3 * h), 0);

                    Viewport.Create(doc, viewSheet.Id, viewRight.Id, pul);
                    Viewport.Create(doc, viewSheet.Id, viewFront.Id, pur);
                    Viewport.Create(doc, viewSheet.Id, viewTop.Id, pll);
                    Viewport.Create(doc, viewSheet.Id, view3d.Id, plr);

                    if (viewSheet.CanBePrinted)
                    {
                        IList<ElementId> viewIds = [
                            viewSheet.Id ];

                        var dir = "C:/tmp";
                        var project_name = doc.Title;
                        var path = dir + "/" + project_name;

                        PDFExportOptions opt = new()
                        {
                            FileName = viewSheet.Name
                        };
                        doc.Export(dir, viewIds, opt);

                        ImageExportOptions imgopt = new()
                        {
                            ExportRange = ExportRange.SetOfViews
                        };
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
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            ElementId id_sample_element;

            try
            {
                var r = sel.PickObject(
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

            using Transaction t = new(doc);
            t.Start("Create sheet and four views");
            var e = doc.GetElement(id_sample_element);
            CreateSheetAndViewsFor(e);
            var save = Util.AskYesNoQuestion("Save the sheet?");
            if (save)
            {
                t.Commit();
            }
            else
            {
                t.RollBack();
            }
            return Result.Succeeded;
        }
    }
}
