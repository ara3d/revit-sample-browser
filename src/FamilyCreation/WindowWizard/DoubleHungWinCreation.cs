// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.FamilyCreation.WindowWizard.CS
{
    public class DoubleHungWinCreation : WindowCreation
    {
        private readonly UIApplication m_application;

        private Autodesk.Revit.DB.ReferencePlane m_centerPlane;

        private CreateDimension m_dimensionCreator;

        private readonly Document m_document;

        private Autodesk.Revit.DB.ReferencePlane m_exteriorPlane;

        private CreateExtrusion m_extrusionCreator;

        private readonly FamilyManager m_familyManager;

        private Category m_frameCat;

        private Category m_glassCat;

        private ElementId m_glassMatId;

        private double m_height;

        private View m_rightView;

        private ElementId m_sashMatId;

        private Autodesk.Revit.DB.ReferencePlane m_sashPlane;

        private double m_sillHeight;

        private Autodesk.Revit.DB.ReferencePlane m_sillPlane;

        private Autodesk.Revit.DB.ReferencePlane m_topPlane;

        private double m_wallHeight;

        private double m_wallThickness;

        private double m_wallWidth;

        private double m_width;

        private double m_windowInset;

        public DoubleHungWinCreation(WizardParameter para, ExternalCommandData commandData)
            : base(para)
        {
            m_application = commandData.Application;
            m_document = commandData.Application.ActiveUIDocument.Document;
            m_familyManager = m_document.FamilyManager;

            using (var tran = new Transaction(m_document, "InitializeWindowWizard"))
            {
                tran.Start();

                CollectTemplateInfo();
                para.Validator = new ValidateWindowParameter(m_wallHeight, m_wallWidth);
                switch (m_document.DisplayUnitSystem)
                {
                    case DisplayUnit.METRIC:
                        para.Validator.IsMetric = true;
                        break;
                    case DisplayUnit.IMPERIAL:
                        para.Validator.IsMetric = false;
                        break;
                }

                para.PathName = $"{Path.GetDirectoryName(para.PathName)}Double Hung.rfa";

                CreateCommon();

                tran.Commit();
            }
        }

        public override void CreateFrame()
        {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();

            var refPlaneCreator = new CreateRefPlane();
            if (m_sashPlane == null)
                m_sashPlane = refPlaneCreator.Create(m_document, m_centerPlane, m_rightView,
                    new XYZ(0, m_wallThickness / 2 - m_windowInset, 0), new XYZ(0, 0, 1), "Sash");
            if (m_exteriorPlane == null)
                m_exteriorPlane = refPlaneCreator.Create(m_document, m_centerPlane, m_rightView,
                    new XYZ(0, m_wallThickness / 2, 0), new XYZ(0, 0, 1), "MyExterior");
            m_document.Regenerate();

            var walls = m_document.GetElements<Wall>().ToList();
            var exteriorWallFace = SampleBrowserUtils.GetWallFace(walls[0], m_rightView, true);
            if (exteriorWallFace == null)
                return;

            // Label the sash-to-wall dimension with the Window Inset family parameter.
            var windowInsetDimension = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, exteriorWallFace);
            var windowInsetPara =
                m_familyManager.AddParameter("Window Inset", new ForgeTypeId(), SpecTypeId.Length, false);
            m_familyManager.Set(windowInsetPara, m_windowInset);
            windowInsetDimension.FamilyLabel = windowInsetPara;

            var frameCurveOffset1 = 0.075;
            var curveArr1 =
                m_extrusionCreator.CreateRectangle(m_width / 2, -m_width / 2, m_sillHeight + m_height, m_sillHeight, 0);
            var curveArr2 = m_extrusionCreator.CreateCurveArrayByOffset(curveArr1, frameCurveOffset1);
            var curveArrArray1 = new CurveArrArray();
            curveArrArray1.Append(curveArr1);
            curveArrArray1.Append(curveArr2);
            var extFrame = m_extrusionCreator.NewExtrusion(curveArrArray1, m_sashPlane,
                m_wallThickness / 2 + m_wallThickness / 12, -m_windowInset);
            extFrame.SetVisibility(CreateVisibility());
            m_document.Regenerate();

            exteriorWallFace =
                SampleBrowserUtils.GetWallFace(walls[0], m_rightView,
                    true); // Get the face again as the document is regenerated.
            var exteriorExtrusionFace1 = SampleBrowserUtils.GetExtrusionFace(extFrame, m_rightView, true);
            var interiorExtrusionFace1 = SampleBrowserUtils.GetExtrusionFace(extFrame, m_rightView, false);
            var alignmentCreator = new CreateAlignment(m_document);
            alignmentCreator.AddAlignment(m_rightView, exteriorWallFace, exteriorExtrusionFace1);

            var extFrameWithSashPlane =
                m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, interiorExtrusionFace1);
            extFrameWithSashPlane.IsLocked = true;
            m_document.Regenerate();

            var frameCurveOffset2 = 0.125;
            var curveArr3 =
                m_extrusionCreator.CreateRectangle(m_width / 2, -m_width / 2, m_sillHeight + m_height, m_sillHeight, 0);
            var curveArr4 = m_extrusionCreator.CreateCurveArrayByOffset(curveArr3, frameCurveOffset2);
            m_document.Regenerate();

            var curveArrArray2 = new CurveArrArray();
            curveArrArray2.Append(curveArr3);
            curveArrArray2.Append(curveArr4);
            var intFrame = m_extrusionCreator.NewExtrusion(curveArrArray2, m_sashPlane, m_wallThickness - m_windowInset,
                m_wallThickness / 2 + m_wallThickness / 12);
            intFrame.SetVisibility(CreateVisibility());
            m_document.Regenerate();

            var interiorWallFace = SampleBrowserUtils.GetWallFace(walls[0], m_rightView, false);
            var interiorExtrusionFace2 = SampleBrowserUtils.GetExtrusionFace(intFrame, m_rightView, false);
            var exteriorExtrusionFace2 = SampleBrowserUtils.GetExtrusionFace(intFrame, m_rightView, true);
            alignmentCreator.AddAlignment(m_rightView, interiorWallFace, interiorExtrusionFace2);

            var intFrameWithSashPlane =
                m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, exteriorExtrusionFace2);
            intFrameWithSashPlane.IsLocked = true;

            var sillCurs = m_extrusionCreator.CreateRectangle(m_width / 2, -m_width / 2,
                m_sillHeight + frameCurveOffset1, m_sillHeight, 0);
            var sillCurveArray = new CurveArrArray();
            sillCurveArray.Append(sillCurs);
            var sillFrame =
                m_extrusionCreator.NewExtrusion(sillCurveArray, m_sashPlane, -m_windowInset, -m_windowInset - 0.1);
            m_document.Regenerate();

            exteriorWallFace =
                SampleBrowserUtils.GetWallFace(walls[0], m_rightView,
                    true); // Get the face again as the document is regenerated.
            var sillExtFace = SampleBrowserUtils.GetExtrusionFace(sillFrame, m_rightView, false);
            alignmentCreator.AddAlignment(m_rightView, sillExtFace, exteriorWallFace);
            m_document.Regenerate();

            if (m_frameCat != null)
            {
                extFrame.Subcategory = m_frameCat;
                intFrame.Subcategory = m_frameCat;
                sillFrame.Subcategory = m_frameCat;
            }

            subTransaction.Commit();
        }

        public override void CreateSash()
        {
            var frameCurveOffset1 = 0.075;
            var frameDepth = 7 * m_wallThickness / 12 + m_windowInset;
            var sashCurveOffset = 0.075;
            var sashDepth = (frameDepth - m_windowInset) / 2;

            var exteriorView = m_document.GetNamedView("Exterior");
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();

            var refPlaneCreator = new CreateRefPlane();
            var middlePlane = refPlaneCreator.Create(m_document, m_topPlane, exteriorView, new XYZ(0, 0, -m_height / 2),
                new XYZ(0, -1, 0), "tempmiddle");
            m_document.Regenerate();

            var dim = m_dimensionCreator.AddDimension(exteriorView, m_topPlane, m_sillPlane, middlePlane);
            dim.AreSegmentsEqual = true;

            var curveArr5 = m_extrusionCreator.CreateRectangle(m_width / 2 - frameCurveOffset1,
                -m_width / 2 + frameCurveOffset1, m_sillHeight + m_height / 2 + sashCurveOffset / 2,
                m_sillHeight + frameCurveOffset1, 0);
            var curveArr6 = m_extrusionCreator.CreateCurveArrayByOffset(curveArr5, sashCurveOffset);
            m_document.Regenerate();

            var curveArrArray3 = new CurveArrArray();
            curveArrArray3.Append(curveArr5);
            curveArrArray3.Append(curveArr6);
            var sash1 = m_extrusionCreator.NewExtrusion(curveArrArray3, m_sashPlane, 2 * sashDepth, sashDepth);
            m_document.Regenerate();

            var esashFace1 = SampleBrowserUtils.GetExtrusionFace(sash1, m_rightView, true);
            var isashFace1 = SampleBrowserUtils.GetExtrusionFace(sash1, m_rightView, false);
            var sashDim1 = m_dimensionCreator.AddDimension(m_rightView, esashFace1, isashFace1);
            sashDim1.IsLocked = true;
            var sashWithPlane1 = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, isashFace1);
            sashWithPlane1.IsLocked = true;
            sash1.SetVisibility(CreateVisibility());

            var curveArr7 = m_extrusionCreator.CreateRectangle(m_width / 2 - frameCurveOffset1,
                -m_width / 2 + frameCurveOffset1, m_sillHeight + m_height - frameCurveOffset1,
                m_sillHeight + m_height / 2 - sashCurveOffset / 2, 0);
            var curveArr8 = m_extrusionCreator.CreateCurveArrayByOffset(curveArr7, sashCurveOffset);
            m_document.Regenerate();

            var curveArrArray4 = new CurveArrArray();
            curveArrArray4.Append(curveArr7);
            curveArrArray4.Append(curveArr8);
            var sash2 = m_extrusionCreator.NewExtrusion(curveArrArray4, m_sashPlane, sashDepth, 0);
            sash2.SetVisibility(CreateVisibility());
            m_document.Regenerate();

            var esashFace2 = SampleBrowserUtils.GetExtrusionFace(sash2, m_rightView, true);
            var isashFace2 = SampleBrowserUtils.GetExtrusionFace(sash2, m_rightView, false);
            var sashDim2 = m_dimensionCreator.AddDimension(m_rightView, esashFace2, isashFace2);
            sashDim2.IsLocked = true;
            var sashWithPlane2 = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, isashFace2);
            m_document.Regenerate();
            sashWithPlane2.IsLocked = true;

            if (m_frameCat != null)
            {
                sash1.Subcategory = m_frameCat;
                sash2.Subcategory = m_frameCat;
            }

            var id = m_sashMatId;
            sash1.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            sash2.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            subTransaction.Commit();
        }

        public override void CreateGlass()
        {
            var frameCurveOffset1 = 0.075;
            var frameDepth = m_wallThickness - 0.15;
            var sashCurveOffset = 0.075;
            var sashDepth = (frameDepth - m_windowInset) / 2;
            var glassDepth = 0.05;
            var glassOffsetSash = 0.05; // Offset from the exterior face of the sash.

            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            var curveArr9 = m_extrusionCreator.CreateRectangle(m_width / 2 - frameCurveOffset1 - sashCurveOffset,
                -m_width / 2 + frameCurveOffset1 + sashCurveOffset, m_sillHeight + m_height / 2 - sashCurveOffset / 2,
                m_sillHeight + frameCurveOffset1 + sashCurveOffset, 0);
            m_document.Regenerate();

            var curveArrArray5 = new CurveArrArray();
            curveArrArray5.Append(curveArr9);
            var glass1 = m_extrusionCreator.NewExtrusion(curveArrArray5, m_sashPlane,
                sashDepth + glassOffsetSash + glassDepth, sashDepth + glassOffsetSash);
            m_document.Regenerate();
            glass1.SetVisibility(CreateVisibility());
            m_document.Regenerate();
            var eglassFace1 = SampleBrowserUtils.GetExtrusionFace(glass1, m_rightView, true);
            var iglassFace1 = SampleBrowserUtils.GetExtrusionFace(glass1, m_rightView, false);
            var glassDim1 = m_dimensionCreator.AddDimension(m_rightView, eglassFace1, iglassFace1);
            glassDim1.IsLocked = true;
            var glass1WithSashPlane = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, eglassFace1);
            glass1WithSashPlane.IsLocked = true;

            var curveArr10 = m_extrusionCreator.CreateRectangle(m_width / 2 - frameCurveOffset1 - sashCurveOffset,
                -m_width / 2 + frameCurveOffset1 + sashCurveOffset,
                m_sillHeight + m_height - frameCurveOffset1 - sashCurveOffset,
                m_sillHeight + m_height / 2 + sashCurveOffset / 2, 0);
            var curveArrArray6 = new CurveArrArray();
            curveArrArray6.Append(curveArr10);
            var glass2 = m_extrusionCreator.NewExtrusion(curveArrArray6, m_sashPlane, glassOffsetSash + glassDepth,
                glassOffsetSash);
            m_document.Regenerate();
            glass2.SetVisibility(CreateVisibility());
            m_document.Regenerate();
            var eglassFace2 = SampleBrowserUtils.GetExtrusionFace(glass2, m_rightView, true);
            var iglassFace2 = SampleBrowserUtils.GetExtrusionFace(glass2, m_rightView, false);
            var glassDim2 = m_dimensionCreator.AddDimension(m_rightView, eglassFace2, iglassFace2);
            glassDim2.IsLocked = true;
            var glass2WithSashPlane = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, eglassFace2);
            glass2WithSashPlane.IsLocked = true;

            if (null != m_glassCat)
            {
                glass1.Subcategory = m_glassCat;
                glass2.Subcategory = m_glassCat;
            }

            var id = m_glassMatId;

            glass1.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            glass2.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            subTransaction.Commit();
        }

        public override void CreateMaterial()
        {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();

            var elementCollector = new FilteredElementCollector(m_document);
            elementCollector.WherePasses(new ElementClassFilter(typeof(Material)));
            var materials = elementCollector.ToElements();

            foreach (var materialElement in materials)
            {
                var material = materialElement as Material;
                if (0 == material.Name.CompareTo(Para.SashMat)) m_sashMatId = material.Id;

                if (0 == material.Name.CompareTo(Para.GlassMat)) m_glassMatId = material.Id;
            }

            subTransaction.Commit();
        }

        public override void CombineAndBuild()
        {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            foreach (string type in Para.WinParaTab.Keys)
            {
                var para = Para.WinParaTab[type] as WindowParameter;

                NewFamilyType(para);
            }

            subTransaction.Commit();
        }

        public override bool Creation()
        {
            using (var trans = new Transaction(m_document, "FinishWindowWizard"))
            {
                try
                {
                    trans.Start();
                    CreateMaterial();
                    CreateFrame();
                    CreateSash();
                    CreateGlass();
                    CombineAndBuild();
                    trans.Commit();
                }
                catch (Exception ee)
                {
                    Debug.WriteLine(ee.Message);
                    Debug.WriteLine(ee.StackTrace);
                    return false;
                }
                finally
                {
                    if (trans.HasStarted())
                        trans.RollBack();
                }
            }

            try
            {
                if (File.Exists(Para.PathName))
                    File.Delete(Para.PathName);
                m_document.SaveAs(Para.PathName);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Write to {Para.PathName} Failed");
                Debug.WriteLine(e.Message);
            }

            return true;
        }

        private void CollectTemplateInfo()
        {
            var walls = m_document.GetElements<Wall>().ToList();
            m_wallThickness = walls[0].Width;
            var wallheightPara =
                walls[0].get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
            if (wallheightPara != null) m_wallHeight = wallheightPara.AsDouble();

            var location = walls[0].Location as LocationCurve;
            m_wallWidth = location.Curve.Length;

            m_windowInset = m_wallThickness / 10;
            var type = m_familyManager.CurrentType;
            var heightPara = m_familyManager.get_Parameter(BuiltInParameter.WINDOW_HEIGHT);
            var widthPara = m_familyManager.get_Parameter(BuiltInParameter.WINDOW_WIDTH);
            var sillHeightPara = m_familyManager.get_Parameter("Default Sill Height");
            if (type.HasValue(heightPara))
                switch (heightPara.StorageType)
                {
                    case StorageType.Double:
                        m_height = type.AsDouble(heightPara).Value;
                        break;
                    case StorageType.Integer:
                        m_height = type.AsInteger(heightPara).Value;
                        break;
                }

            if (type.HasValue(widthPara))
                switch (widthPara.StorageType)
                {
                    case StorageType.Double:
                        m_width = type.AsDouble(widthPara).Value;
                        break;
                    case StorageType.Integer:
                        m_width = type.AsDouble(widthPara).Value;
                        break;
                }

            if (type.HasValue(sillHeightPara))
                switch (sillHeightPara.StorageType)
                {
                    case StorageType.Double:
                        m_sillHeight = type.AsDouble(sillHeightPara).Value;
                        break;
                    case StorageType.Integer:
                        m_sillHeight = type.AsDouble(sillHeightPara).Value;
                        break;
                }

            m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_HEIGHT),
                m_height);
            m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_WIDTH),
                m_width);
            m_familyManager.Set(m_familyManager.get_Parameter("Default Sill Height"), m_sillHeight);

            var elementCollector = new FilteredElementCollector(m_document);
            elementCollector.WherePasses(new ElementClassFilter(typeof(Material)));
            var materials = elementCollector.ToElements();

            foreach (var materialElement in materials)
            {
                var material = materialElement as Material;
                Para.GlassMaterials.Add(material.Name);
                Para.FrameMaterials.Add(material.Name);
            }

            var categories = m_document.Settings.Categories;
            var category = categories.get_Item(BuiltInCategory.OST_Windows);

            m_frameCat = categories.get_Item(BuiltInCategory.OST_WindowsFrameMullionProjection);
            m_glassCat = categories.get_Item(BuiltInCategory.OST_WindowsGlassProjection);

            var planes = m_document.GetElements<Autodesk.Revit.DB.ReferencePlane>();
            foreach (var p in planes)
            {
                switch (p.Name)
                {
                    case "Sash":
                        m_sashPlane = p;
                        break;
                    case "Exterior":
                        m_exteriorPlane = p;
                        break;
                    case "Center (Front/Back)":
                        m_centerPlane = p;
                        break;
                    case "Top":
                    case "Head":
                        m_topPlane = p;
                        break;
                    case "Sill":
                    case "Bottom":
                        m_sillPlane = p;
                        break;
                }
            }
        }

        private bool NewFamilyType(WindowParameter para) 
        {
            var dbhungPara = para as DoubleHungWinPara;
            var typeName = dbhungPara.Type;
            var height = dbhungPara.Height;
            var width = dbhungPara.Width;
            var sillHeight = dbhungPara.SillHeight;
            var windowInset = dbhungPara.Inset;
            switch (m_document.DisplayUnitSystem)
            {
                case DisplayUnit.METRIC:
                    height = SampleBrowserUtils.MetricToImperial(height);
                    width = SampleBrowserUtils.MetricToImperial(width);
                    sillHeight = SampleBrowserUtils.MetricToImperial(sillHeight);
                    windowInset = SampleBrowserUtils.MetricToImperial(windowInset);
                    break;
            }

            try
            {
                var type = m_familyManager.NewType(typeName);
                m_familyManager.CurrentType = type;
                m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_HEIGHT), height);
                m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_WIDTH), width);
                m_familyManager.Set(m_familyManager.get_Parameter("Default Sill Height"), sillHeight);
                m_familyManager.Set(m_familyManager.get_Parameter("Window Inset"), windowInset);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        private FamilyElementVisibility CreateVisibility()
        {
            var familyElemVisibility = new FamilyElementVisibility(FamilyElementVisibilityType.Model)
            {
                IsShownInCoarse = true,
                IsShownInFine = true,
                IsShownInMedium = true,
                IsShownInFrontBack = true,
                IsShownInLeftRight = true,
                IsShownInPlanRCPCut = false
            };
            return familyElemVisibility;
        }

        private void CreateCommon()
        {
            m_dimensionCreator = new CreateDimension(m_application.Application, m_document);
            m_extrusionCreator = new CreateExtrusion(m_application.Application, m_document);
            m_rightView = m_document.GetNamedView("Right");
        }
    }
}
