//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.WindowWizard.CS
{
    /// <summary>
    ///     Inherited from WindowCreation class
    /// </summary>
    internal class DoubleHungWinCreation : WindowCreation
    {
        /// <summary>
        ///     store the Application
        /// </summary>
        private readonly UIApplication m_application;

        /// <summary>
        ///     store the center referenceplane
        /// </summary>
        private Autodesk.Revit.DB.ReferencePlane m_centerPlane;

        /// <summary>
        ///     store the CreateDimension instance
        /// </summary>
        private CreateDimension m_dimensionCreator;

        /// <summary>
        ///     store the document
        /// </summary>
        private readonly Document m_document;

        /// <summary>
        ///     store the exterior referenceplane
        /// </summary>
        private Autodesk.Revit.DB.ReferencePlane m_exteriorPlane;

        /// <summary>
        ///     store the CreateExtrusion instance
        /// </summary>
        private CreateExtrusion m_extrusionCreator;

        /// <summary>
        ///     store the FamilyManager
        /// </summary>
        private readonly FamilyManager m_familyManager;

        /// <summary>
        ///     store the frame category
        /// </summary>
        private Category m_frameCat;

        /// <summary>
        ///     store the glass category
        /// </summary>
        private Category m_glassCat;

        /// <summary>
        ///     store the glass material ID
        /// </summary>
        private ElementId m_glassMatID;

        /// <summary>
        ///     store the height parameter of wall
        /// </summary>
        private double m_height;

        /// <summary>
        ///     store the right view of the document
        /// </summary>
        private View m_rightView;

        /// <summary>
        ///     store the sash material ID
        /// </summary>
        private ElementId m_sashMatID;

        /// <summary>
        ///     store the sash referenceplane
        /// </summary>
        private Autodesk.Revit.DB.ReferencePlane m_sashPlane;

        /// <summary>
        ///     store the sillheight parameter of wall
        /// </summary>
        private double m_sillHeight;

        /// <summary>
        ///     store the sill referenceplane
        /// </summary>
        private Autodesk.Revit.DB.ReferencePlane m_sillPlane;

        /// <summary>
        ///     store the top referenceplane
        /// </summary>
        private Autodesk.Revit.DB.ReferencePlane m_topPlane;

        /// <summary>
        ///     Store the height value of wall
        /// </summary>
        private double m_wallHeight;

        /// <summary>
        ///     store the thickness parameter of wall
        /// </summary>
        private double m_wallThickness;

        /// <summary>
        ///     Store the width value of wall
        /// </summary>
        private double m_wallWidth;

        /// <summary>
        ///     store the width parameter of wall
        /// </summary>
        private double m_width;

        /// <summary>
        ///     store the windowInset parameter of wall
        /// </summary>
        private double m_windowInset;

        /// <summary>
        ///     constructor of DoubleHungWinCreation
        /// </summary>
        /// <param name="para">WizardParameter</param>
        /// <param name="commandData">ExternalCommandData</param>
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

                para.PathName = Path.GetDirectoryName(para.PathName) + "Double Hung.rfa";

                CreateCommon();

                tran.Commit();
            }
        }

        /// <summary>
        ///     The implementation of CreateFrame()
        /// </summary>
        public override void CreateFrame()
        {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();

            //create sash referenceplane and exterior referenceplane
            var refPlaneCreator = new CreateRefPlane();
            if (m_sashPlane == null)
                m_sashPlane = refPlaneCreator.Create(m_document, m_centerPlane, m_rightView,
                    new XYZ(0, m_wallThickness / 2 - m_windowInset, 0), new XYZ(0, 0, 1), "Sash");
            if (m_exteriorPlane == null)
                m_exteriorPlane = refPlaneCreator.Create(m_document, m_centerPlane, m_rightView,
                    new XYZ(0, m_wallThickness / 2, 0), new XYZ(0, 0, 1), "MyExterior");
            m_document.Regenerate();

            //get the wall in the document and retrieve the exterior face
            var walls = Utility.GetElements<Wall>(m_application, m_document);
            var exteriorWallFace = GeoHelper.GetWallFace(walls[0], m_rightView, true);
            if (exteriorWallFace == null)
                return;

            //add dimension between sash reference plane and wall face,and add parameter "Window Inset",label the dimension with window-inset parameter
            var windowInsetDimension = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, exteriorWallFace);
            var windowInsetPara =
                m_familyManager.AddParameter("Window Inset", new ForgeTypeId(), SpecTypeId.Length, false);
            m_familyManager.Set(windowInsetPara, m_windowInset);
            windowInsetDimension.FamilyLabel = windowInsetPara;

            //create the exterior frame            
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

            //add alignment between wall face and exterior frame face
            exteriorWallFace =
                GeoHelper.GetWallFace(walls[0], m_rightView,
                    true); // Get the face again as the document is regenerated.
            var exteriorExtrusionFace1 = GeoHelper.GetExtrusionFace(extFrame, m_rightView, true);
            var interiorExtrusionFace1 = GeoHelper.GetExtrusionFace(extFrame, m_rightView, false);
            var alignmentCreator = new CreateAlignment(m_document);
            alignmentCreator.AddAlignment(m_rightView, exteriorWallFace, exteriorExtrusionFace1);

            //add dimension between sash referenceplane and exterior frame face and lock the dimension
            var extFrameWithSashPlane =
                m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, interiorExtrusionFace1);
            extFrameWithSashPlane.IsLocked = true;
            m_document.Regenerate();

            //create the interior frame                
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

            //add alignment between interior face of wall and interior frame face
            var interiorWallFace = GeoHelper.GetWallFace(walls[0], m_rightView, false);
            var interiorExtrusionFace2 = GeoHelper.GetExtrusionFace(intFrame, m_rightView, false);
            var exteriorExtrusionFace2 = GeoHelper.GetExtrusionFace(intFrame, m_rightView, true);
            alignmentCreator.AddAlignment(m_rightView, interiorWallFace, interiorExtrusionFace2);

            //add dimension between sash referenceplane and interior frame face and lock the dimension
            var intFrameWithSashPlane =
                m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, exteriorExtrusionFace2);
            intFrameWithSashPlane.IsLocked = true;

            //create the sill frame
            var sillCurs = m_extrusionCreator.CreateRectangle(m_width / 2, -m_width / 2,
                m_sillHeight + frameCurveOffset1, m_sillHeight, 0);
            var sillCurveArray = new CurveArrArray();
            sillCurveArray.Append(sillCurs);
            var sillFrame =
                m_extrusionCreator.NewExtrusion(sillCurveArray, m_sashPlane, -m_windowInset, -m_windowInset - 0.1);
            m_document.Regenerate();

            //add alignment between wall face and sill frame face
            exteriorWallFace =
                GeoHelper.GetWallFace(walls[0], m_rightView,
                    true); // Get the face again as the document is regenerated.
            var sillExtFace = GeoHelper.GetExtrusionFace(sillFrame, m_rightView, false);
            alignmentCreator.AddAlignment(m_rightView, sillExtFace, exteriorWallFace);
            m_document.Regenerate();

            //set subcategories of the frames
            if (m_frameCat != null)
            {
                extFrame.Subcategory = m_frameCat;
                intFrame.Subcategory = m_frameCat;
                sillFrame.Subcategory = m_frameCat;
            }

            subTransaction.Commit();
        }

        /// <summary>
        ///     The implementation of CreateSash(),and creating the Window Sash Solid Geometry
        /// </summary>
        public override void CreateSash()
        {
            var frameCurveOffset1 = 0.075;
            var frameDepth = 7 * m_wallThickness / 12 + m_windowInset;
            var sashCurveOffset = 0.075;
            var sashDepth = (frameDepth - m_windowInset) / 2;

            //get the exterior view and sash referenceplane which are used in this process
            var exteriorView = Utility.GetViewByName("Exterior", m_application, m_document);
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();

            //add a middle reference plane between the top referenceplane and sill referenceplane
            var refPlaneCreator = new CreateRefPlane();
            var middlePlane = refPlaneCreator.Create(m_document, m_topPlane, exteriorView, new XYZ(0, 0, -m_height / 2),
                new XYZ(0, -1, 0), "tempmiddle");
            m_document.Regenerate();

            //add dimension between top, sill, and middle reference plane, make the dimension segment equal
            var dim = m_dimensionCreator.AddDimension(exteriorView, m_topPlane, m_sillPlane, middlePlane);
            dim.AreSegmentsEqual = true;

            //create first sash           
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

            var esashFace1 = GeoHelper.GetExtrusionFace(sash1, m_rightView, true);
            var isashFace1 = GeoHelper.GetExtrusionFace(sash1, m_rightView, false);
            var sashDim1 = m_dimensionCreator.AddDimension(m_rightView, esashFace1, isashFace1);
            sashDim1.IsLocked = true;
            var sashWithPlane1 = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, isashFace1);
            sashWithPlane1.IsLocked = true;
            sash1.SetVisibility(CreateVisibility());

            //create second sash
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

            var esashFace2 = GeoHelper.GetExtrusionFace(sash2, m_rightView, true);
            var isashFace2 = GeoHelper.GetExtrusionFace(sash2, m_rightView, false);
            var sashDim2 = m_dimensionCreator.AddDimension(m_rightView, esashFace2, isashFace2);
            sashDim2.IsLocked = true;
            var sashWithPlane2 = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, isashFace2);
            m_document.Regenerate();
            sashWithPlane2.IsLocked = true;

            //set category of the sash extrusions
            if (m_frameCat != null)
            {
                sash1.Subcategory = m_frameCat;
                sash2.Subcategory = m_frameCat;
            }

            var id = m_sashMatID;
            sash1.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            sash2.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            subTransaction.Commit();
        }

        /// <summary>
        ///     The implementation of CreateGlass(), creating the Window Glass Solid Geometry
        /// </summary>
        public override void CreateGlass()
        {
            var frameCurveOffset1 = 0.075;
            var frameDepth = m_wallThickness - 0.15;
            var sashCurveOffset = 0.075;
            var sashDepth = (frameDepth - m_windowInset) / 2;
            var glassDepth = 0.05;
            var glassOffsetSash = 0.05; //from the exterior of the sash

            //create first glass            
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
            var eglassFace1 = GeoHelper.GetExtrusionFace(glass1, m_rightView, true);
            var iglassFace1 = GeoHelper.GetExtrusionFace(glass1, m_rightView, false);
            var glassDim1 = m_dimensionCreator.AddDimension(m_rightView, eglassFace1, iglassFace1);
            glassDim1.IsLocked = true;
            var glass1WithSashPlane = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, eglassFace1);
            glass1WithSashPlane.IsLocked = true;

            //create the second glass
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
            var eglassFace2 = GeoHelper.GetExtrusionFace(glass2, m_rightView, true);
            var iglassFace2 = GeoHelper.GetExtrusionFace(glass2, m_rightView, false);
            var glassDim2 = m_dimensionCreator.AddDimension(m_rightView, eglassFace2, iglassFace2);
            glassDim2.IsLocked = true;
            var glass2WithSashPlane = m_dimensionCreator.AddDimension(m_rightView, m_sashPlane, eglassFace2);
            glass2WithSashPlane.IsLocked = true;

            //set category
            if (null != m_glassCat)
            {
                glass1.Subcategory = m_glassCat;
                glass2.Subcategory = m_glassCat;
            }

            var id = m_glassMatID;

            glass1.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            glass2.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(id);
            subTransaction.Commit();
        }

        /// <summary>
        ///     The implementation of CreateMaterial()
        /// </summary>
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
                if (0 == material.Name.CompareTo(m_para.SashMat)) m_sashMatID = material.Id;

                if (0 == material.Name.CompareTo(m_para.GlassMat)) m_glassMatID = material.Id;
            }

            subTransaction.Commit();
        }

        /// <summary>
        ///     The implementation of CombineAndBuild() ,defining New Window Types
        /// </summary>
        public override void CombineAndBuild()
        {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            foreach (string type in m_para.WinParaTab.Keys)
            {
                var para = m_para.WinParaTab[type] as WindowParameter;

                newFamilyType(para);
            }

            subTransaction.Commit();
        }

        /// <summary>
        ///     The implementation of Creation(), defining the way to do the whole creation.
        /// </summary>
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
                if (File.Exists(m_para.PathName))
                    File.Delete(m_para.PathName);
                m_document.SaveAs(m_para.PathName);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Write to " + m_para.PathName + " Failed");
                Debug.WriteLine(e.Message);
            }

            return true;
        }

        /// <summary>
        ///     The method is used to collect template information, specifying the New Window Parameters
        /// </summary>
        private void CollectTemplateInfo()
        {
            var walls = Utility.GetElements<Wall>(m_application, m_document);
            m_wallThickness = walls[0].Width;
            var wallheightPara =
                walls[0].get_Parameter(BuiltInParameter
                    .WALL_USER_HEIGHT_PARAM); //paraMap.get_Item("Unconnected Height");
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

            //set the height,width and sillheight parameter of the opening
            m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_HEIGHT),
                m_height);
            m_familyManager.Set(m_familyManager.get_Parameter(BuiltInParameter.WINDOW_WIDTH),
                m_width);
            m_familyManager.Set(m_familyManager.get_Parameter("Default Sill Height"), m_sillHeight);

            //get materials

            var elementCollector = new FilteredElementCollector(m_document);
            elementCollector.WherePasses(new ElementClassFilter(typeof(Material)));
            var materials = elementCollector.ToElements();

            foreach (var materialElement in materials)
            {
                var material = materialElement as Material;
                m_para.GlassMaterials.Add(material.Name);
                m_para.FrameMaterials.Add(material.Name);
            }

            //get categories
            var categories = m_document.Settings.Categories;
            var category = categories.get_Item(BuiltInCategory.OST_Windows);

            m_frameCat = categories.get_Item(BuiltInCategory.OST_WindowsFrameMullionProjection);
            m_glassCat = categories.get_Item(BuiltInCategory.OST_WindowsGlassProjection);

            //get referenceplanes
            var planes = Utility.GetElements<Autodesk.Revit.DB.ReferencePlane>(m_application, m_document);
            foreach (var p in planes)
            {
                if (p.Name.Equals("Sash"))
                    m_sashPlane = p;
                if (p.Name.Equals("Exterior"))
                    m_exteriorPlane = p;
                if (p.Name.Equals("Center (Front/Back)"))
                    m_centerPlane = p;
                if (p.Name.Equals("Top") || p.Name.Equals("Head"))
                    m_topPlane = p;
                if (p.Name.Equals("Sill") || p.Name.Equals("Bottom"))
                    m_sillPlane = p;
            }
        }

        /// <summary>
        ///     the method is used to create new family type
        /// </summary>
        /// <param name="para">WindowParameter</param>
        /// <returns>indicate whether the NewType is successful</returns>
        private bool
            newFamilyType(WindowParameter para) //string typeName, double height, double width, double sillHeight)
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
                    height = Utility.MetricToImperial(height);
                    width = Utility.MetricToImperial(width);
                    sillHeight = Utility.MetricToImperial(sillHeight);
                    windowInset = Utility.MetricToImperial(windowInset);
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

        /// <summary>
        ///     The method is used to create a FamilyElementVisibility instance
        /// </summary>
        /// <returns>FamilyElementVisibility instance</returns>
        private FamilyElementVisibility CreateVisibility()
        {
            var familyElemVisibility = new FamilyElementVisibility(FamilyElementVisibilityType.Model);
            familyElemVisibility.IsShownInCoarse = true;
            familyElemVisibility.IsShownInFine = true;
            familyElemVisibility.IsShownInMedium = true;
            familyElemVisibility.IsShownInFrontBack = true;
            familyElemVisibility.IsShownInLeftRight = true;
            familyElemVisibility.IsShownInPlanRCPCut = false;
            return familyElemVisibility;
        }

        /// <summary>
        ///     The method is used to create common class variables in this class
        /// </summary>
        private void CreateCommon()
        {
            //create common 
            m_dimensionCreator = new CreateDimension(m_application.Application, m_document);
            m_extrusionCreator = new CreateExtrusion(m_application.Application, m_document);
            m_rightView = Utility.GetViewByName("Right", m_application, m_document);
        }
    }
}