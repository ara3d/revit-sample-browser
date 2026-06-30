// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    #region Compatibility Methods by Magson Leone

    public static class CompatibilityMethods
    {
        #region Autodesk.Revit.DB.Curve

        public static XYZ GetPoint2(
            this Curve curva,
            int i)
        {
            XYZ value = null;

            var met = curva.GetType().GetMethod(
                    "GetEndPoint",
                    new[] { typeof(int) });

            if (met == null)
                met = curva.GetType().GetMethod(
                    "get_EndPoint",
                    new[] { typeof(int) });

            value = met.Invoke(curva, new object[] { i })
                as XYZ;

            return value;
        }

        #endregion // Autodesk.Revit.DB.Curve

        #region Autodesk.Revit.DB.FamilySymbol

        public static void EnableFamilySymbol2(
            this FamilySymbol fsymbol)
        {
            var met = fsymbol.GetType()
                    .GetMethod("Activate");
            if (met != null) met.Invoke(fsymbol, null);
        }

        #endregion // Autodesk.Revit.DB.FamilySymbol

        #region Autodesk.Revit.DB.InternalDefinition

        public static void VaryGroup2(
            this InternalDefinition def, Document doc)
        {
            var parametros = new object[]
            {
                    doc,
                    true
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            var met = def.GetType()
                    .GetMethod("SetAllowVaryBetweenGroups", tipos);
            if (met != null) met.Invoke(def, parametros);
        }

        #endregion // Autodesk.Revit.DB.InternalDefinition

        #region Autodesk.Revit.DB.Part

        public static ElementId GetSource2(this Part part)
        {
            ElementId value = null;
            var prop = part.GetType()
                    .GetProperty("OriginalDividedElementId");
            if (prop != null)
            {
                value = prop.GetValue(part,
                    null) as ElementId;
            }
            else
            {
                var met = part.GetType()
                        .GetMethod("GetSourceElementIds");
                var temp = met.Invoke(part, null);
                met = temp.GetType()
                    .GetMethod("First");
                temp = met.Invoke(temp, null);
                prop = temp.GetType()
                    .GetProperty("HostElementId");
                value = prop.GetValue(temp,
                    null) as ElementId;
            }

            return value;
        }

        #endregion // Autodesk.Revit.DB.Part

        #region Autodesk.Revit.UI.UIApplication

        public static Rectangle
            GetDrawingArea2(this UIApplication ui)
        {
            var value = WinForms.Screen.PrimaryScreen.Bounds;
            return value;
        }

        #endregion // Autodesk.Revit.UI.UIApplication

        #region Autodesk.Revit.DB.Wall

        public static void FlipWall2(this Wall wall)
        {
            var metodo = "Flip";
            var met = typeof(Wall)
                    .GetMethod(metodo);
            if (met != null)
            {
                met.Invoke(wall, null);
            }
            else
            {
                metodo = "flip";
                met = typeof(Wall).GetMethod(metodo);
                met.Invoke(wall, null);
            }
        }

        #endregion // Autodesk.Revit.DB.Wall

        //#region Autodesk.Revit.DB.Definitions
        //public static Definition Create2(
        //  this Definitions definitions,
        //  Document doc,
        //  string nome,
        //  ParameterType tipo,
        //  bool visibilidade )
        //{
        //  // Does this need updating to check for 
        //  // ExternalDefinitionCreationOptions with
        //  // the additional 'i' in Revit 2016?

        //  Definition value = null;
        //  List<Type> ls = doc.GetType().Assembly
        //  .GetTypes().Where( a => a.IsClass && a
        //  .Name == "ExternalDefinitonCreationOptions" ).ToList();
        //  if( ls.Count > 0 )
        //  {
        //    Type t = ls[ 0 ];
        //    ConstructorInfo c = t
        //    .GetConstructor( new Type[] { typeof(string),
        //            typeof(ParameterType) } );
        //    object ed = c
        //    .Invoke( new object[] { nome, tipo } );
        //    ed.GetType().GetProperty( "Visible" )
        //    .SetValue( ed, visibilidade, null );
        //    value = definitions.GetType()
        //    .GetMethod( "Create", new Type[] { t } ).Invoke( definitions,
        //      new object[] { ed } ) as Definition;
        //  }
        //  {
        //    value = definitions.GetType()
        //    .GetMethod( "Create", new Type[] { typeof(string),
        //            typeof(ParameterType), typeof(bool) } ).Invoke( definitions,
        //      new object[] { nome, tipo,
        //            visibilidade } ) as Definition;
        //  }
        //  return value;
        //}
        //#endregion // Autodesk.Revit.DB.Definitions

        #region Autodesk.Revit.DB.Document

        public static Element GetElement2(
            this Document doc,
            ElementId id)
        {
            Element value = null;
            var met = doc.GetType()
                    .GetMethod("get_Element", new[] { typeof(ElementId) });
            if (met == null)
                met = doc.GetType()
                    .GetMethod("GetElement", new[] { typeof(ElementId) });
            value = met.Invoke(doc,
                new object[] { id }) as Element;
            return value;
        }

        public static Element GetElement2(this Document
            doc, Reference refe)
        {
            Element value = null;
            value = doc.GetElement(refe);
            return value;
        }

        public static Line CreateLine2(
            this Document doc,
            XYZ p1, XYZ p2,
            bool bound = true)
        {
            Line value = null;
            var parametros = new object[]
            {
                    p1,
                    p2
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            var metodo = "CreateBound";
            if (bound == false)
                metodo =
                    "CreateUnbound";
            var met = typeof(Line)
                    .GetMethod(metodo, tipos);
            if (met != null)
            {
                value = met.Invoke(null,
                    parametros) as Line;
            }
            else
            {
                parametros = new object[]
                {
                        p1, p2,
                        bound
                };
                tipos = parametros.Select(a => a
                    .GetType()).ToArray();
                value = doc.Application.Create
                    .GetType().GetMethod("NewLine", tipos).Invoke(doc
                        .Application.Create, parametros) as Line;
            }

            return value;
        }

        public static Wall CreateWall2(
            this Document doc,
            Curve curve, ElementId wallTypeId,
            ElementId levelId, double height,
            double offset, bool flip,
            bool structural)
        {
            Wall value = null;
            var parametros = new object[]
            {
                    doc,
                    curve, wallTypeId, levelId, height, offset, flip,
                    structural
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            var met = typeof(Wall)
                    .GetMethod("Create", tipos);
            if (met != null)
            {
                value = met.Invoke(null,
                    parametros) as Wall;
            }
            else
            {
                parametros = new object[]
                {
                        curve,
                        (WallType) doc.GetElement2(wallTypeId), (Level) doc
                            .GetElement2(levelId),
                        height, offset, flip,
                        structural
                };
                tipos = parametros.Select(a => a
                    .GetType()).ToArray();
                value = doc.Create.GetType()
                    .GetMethod("NewWall", tipos).Invoke(doc.Create,
                        parametros) as Wall;
            }

            return value;
        }

        public static Arc CreateArc2(
            this Document doc,
            XYZ p1, XYZ p2, XYZ p3)
        {
            Arc value = null;
            var parametros = new object[]
            {
                    p1,
                    p2, p3
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            var metodo = "Create";
            var met = typeof(Arc)
                    .GetMethod(metodo, tipos);
            value = met != null
                ? met.Invoke(null,
                    parametros) as Arc
                : doc.Application.Create
                    .GetType().GetMethod("NewArc", tipos).Invoke(doc
                        .Application.Create, parametros) as Arc;
            return value;
        }

        public static char GetDecimalSymbol2(
            this Document doc)
        {
            var valor = ',';
            var met = doc.GetType()
                    .GetMethod("GetUnits");
            if (met != null)
            {
                var temp = met.Invoke(doc, null);
                var prop = temp.GetType()
                        .GetProperty("DecimalSymbol");
                var o = prop.GetValue(temp, null);
                valor = o.ToString() == "Comma" ? ',' : '.';
            }
            else
            {
                var temp = doc.GetType()
                    .GetProperty("ProjectUnit").GetValue(doc, null);
                var prop = temp.GetType()
                        .GetProperty("DecimalSymbolType");
                var o = prop.GetValue(temp, null);
                valor = o.ToString() == "DST_COMMA" ? ',' : '.';
            }

            return valor;
        }

        public static void UnjoinGeometry2(
            this Document doc,
            Element firstElement,
            Element secondElement)
        {
            var ls = doc.GetType().Assembly
                .GetTypes().Where(a => a.IsClass && a
                    .Name == "JoinGeometryUtils").ToList();
            var parametros = new object[]
            {
                    doc,
                    firstElement, secondElement
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            if (ls.Count > 0)
            {
                var t = ls[0];
                var met = t
                        .GetMethod("UnjoinGeometry", tipos);
                met.Invoke(null, parametros);
            }
        }

        public static void JoinGeometry2(
            this Document doc,
            Element firstElement,
            Element secondElement)
        {
            var ls = doc.GetType().Assembly
                .GetTypes().Where(a => a.IsClass && a
                    .Name == "JoinGeometryUtils").ToList();
            var parametros = new object[]
            {
                    doc,
                    firstElement, secondElement
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            if (ls.Count > 0)
            {
                var t = ls[0];
                var met = t
                        .GetMethod("JoinGeometry", tipos);
                met.Invoke(null, parametros);
            }
        }

        public static bool IsJoined2(
            this Document doc,
            Element firstElement,
            Element secondElement)
        {
            var value = false;
            var ls = doc.GetType().Assembly
                .GetTypes().Where(a => a.IsClass && a
                    .Name == "JoinGeometryUtils").ToList();
            var parametros = new object[]
            {
                    doc,
                    firstElement, secondElement
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            if (ls.Count > 0)
            {
                var t = ls[0];
                var met = t
                        .GetMethod("AreElementsJoined", tipos);
                value = (bool)met.Invoke(null,
                    parametros);
            }

            return value;
        }

        public static bool CalculateVolumeArea2(
            this Document doc, bool value)
        {
            var ls = doc.GetType().Assembly
                .GetTypes().Where(a => a.IsClass && a
                    .Name == "AreaVolumeSettings").ToList();
            if (ls.Count > 0)
            {
                var t = ls[0];
                var parametros = new object[]
                {
                        doc
                };
                var tipos = parametros
                        .Select(a => a.GetType()).ToArray();
                var met = t
                        .GetMethod("GetAreaVolumeSettings", tipos);
                var temp = met.Invoke(null,
                    parametros);
                temp.GetType()
                    .GetProperty("ComputeVolumes").SetValue(temp, value, null);
            }
            else
            {
                var prop = doc.Settings
                        .GetType().GetProperty("VolumeCalculationSetting");
                var temp = prop.GetValue(doc
                    .Settings, null);
                prop = temp.GetType()
                    .GetProperty("VolumeCalculationOptions");
                temp = prop.GetValue(temp, null);
                prop = temp.GetType()
                    .GetProperty("VolumeComputationEnable");
                prop.SetValue(temp, value, null);
            }

            return value;
        }

        public static Group CreateGroup2(
            this Document doc,
            List<Element> elementos)
        {
            Group value = null;
            var eleset = new ElementSet();
            foreach (var ele in elementos) eleset.Insert(ele);
            ICollection<ElementId> col = elementos
                .Select(a => a.Id).ToList();
            object obj = doc.Create;
            var met = obj.GetType()
                    .GetMethod("NewGroup", new[] { col.GetType() });
            if (met != null)
            {
                met.Invoke(obj, new object[] { col });
            }
            else
            {
                met = obj.GetType()
                    .GetMethod("NewGroup", new[] { eleset.GetType() });
                met.Invoke(obj,
                    new object[] { eleset });
            }

            return value;
        }

        public static void Delete2(
            this Document doc,
            Element e)
        {
            object obj = doc;
            var m = obj.GetType().GetMethod(
                    "Delete", new[] { typeof(Element) });

            if (m != null)
            {
                m.Invoke(obj, new object[] { e });
            }
            else
            {
                m = obj.GetType().GetMethod(
                    "Delete", new[] { typeof(ElementId) });

                m.Invoke(obj, new object[] { e.Id });
            }
        }

        #endregion // Autodesk.Revit.DB.Document

        #region Autodesk.Revit.DB.Element

        public static Element Level2(this Element ele)
        {
            Element value = null;
            var doc = ele.Document;
            var t = ele.GetType();
            value = t.GetProperty("Level") != null
                ? t.GetProperty("Level")
                    .GetValue(ele, null) as Element
                : doc.GetElement2((ElementId)t
                    .GetProperty("LevelId").GetValue(ele, null));
            return value;
        }

        public static List<Material> Materiais2(
            this Element ele)
        {
            var value = new List<Material>();
            var doc = ele.Document;
            var t = ele.GetType();
            if (t.GetProperty("Materials") != null)
            {
                value = ((IEnumerable)t
                        .GetProperty("Materials").GetValue(ele, null)).Cast<Material>()
                    .ToList();
            }
            else
            {
                var met = t
                        .GetMethod("GetMaterialIds", new[] { typeof(bool) });
                value = ((ICollection<ElementId>)met
                        .Invoke(ele, new object[] { false }))
                    .Select(a => doc.GetElement2(a)).Cast<Material>().ToList();
            }

            return value;
        }

        public static Parameter GetParameter2(
            this Element ele, string nome_paramentro)
        {
            Parameter value = null;
            var t = ele.GetType();
            var met = t
                    .GetMethod("LookupParameter", new[] { typeof(string) });
            if (met == null)
                met = t.GetMethod("get_Parameter",
                    new[] { typeof(string) });
            value = met.Invoke(ele,
                new object[] { nome_paramentro }) as Parameter;
            if (value == null)
            {
                var pas = ele.Parameters
                    .Cast<Parameter>().ToList();
                if (pas.Exists(a => a.Definition
                    .Name.ToLower() == nome_paramentro.Trim().ToLower()))
                    value = pas.First(a => a
                        .Definition.Name.ToLower() == nome_paramentro.Trim()
                        .ToLower());
            }

            return value;
        }

        public static Parameter GetParameter2(
            this Element ele,
            BuiltInParameter builtInParameter)
        {
            Parameter value = null;
            var t = ele.GetType();
            var met = t
                    .GetMethod("LookupParameter", new[] { typeof(BuiltInParameter) });
            if (met == null)
                met = t.GetMethod("get_Parameter",
                    new[] { typeof(BuiltInParameter) });
            value = met.Invoke(ele,
                new object[] { builtInParameter }) as Parameter;
            return value;
        }

        public static double GetMaterialArea2(
            this Element ele, Material m)
        {
            double value = 0;
            var t = ele.GetType();
            var met = t
                    .GetMethod("GetMaterialArea", new[]
                    {
                        typeof(ElementId),
                        typeof(bool)
                    });
            if (met != null)
            {
                value = (double)met.Invoke(ele,
                    new object[] { m.Id, false });
            }
            else
            {
                met = t.GetMethod("GetMaterialArea",
                    new[] { typeof(Element) });
                value = (double)met.Invoke(ele,
                    new object[] { m });
            }

            return value;
        }

        public static double GetMaterialVolume2(
            this Element ele, Material m)
        {
            double value = 0;
            var t = ele.GetType();
            var met = t
                    .GetMethod("GetMaterialVolume", new[]
                    {
                        typeof(ElementId),
                        typeof(bool)
                    });
            if (met != null)
            {
                value = (double)met.Invoke(ele,
                    new object[] { m.Id, false });
            }
            else
            {
                met = t
                    .GetMethod("GetMaterialVolume", new[] { typeof(ElementId) });
                value = (double)met.Invoke(ele,
                    new object[] { m.Id });
            }

            return value;
        }

        public static List<GeometryObject>
            GetGeometricObjects2(this Element ele)
        {
            var value =
                new List<GeometryObject>();
            var op = new Options();
            object obj = ele.get_Geometry(op);
            var prop = obj.GetType()
                    .GetProperty("Objects");
            if (prop != null)
            {
                obj = prop.GetValue(obj, null);
                var arr = obj as IEnumerable;
                foreach (GeometryObject geo in arr) value.Add(geo);
            }
            else
            {
                var geos =
                    obj as IEnumerable<GeometryObject>;
                foreach (var geo in geos) value.Add(geo);
            }

            return value;
        }

        #endregion // Autodesk.Revit.DB.Element

        #region Autodesk.Revit.UI.Selection.Selection

        public static List<Element> GetSelection2(
            this Selection sel, Document doc)
        {
            var value = new List<Element>();
            sel.GetElementIds();
            var t = sel.GetType();
            if (t.GetMethod("GetElementIds") != null)
            {
                var met = t
                        .GetMethod("GetElementIds");
                value = ((ICollection<ElementId>)met
                        .Invoke(sel, null)).Select(a => doc.GetElement2(a))
                    .ToList();
            }
            else
            {
                value = ((IEnumerable)t
                        .GetProperty("Elements").GetValue(sel, null)).Cast<Element>()
                    .ToList();
            }

            return value;
        }

        public static void SetSelection2(
            this Selection sel,
            Document doc,
            ICollection<ElementId> elementos)
        {
            sel.ClearSelection2();
            var parametros = new object[]
            {
                    elementos
            };
            var tipos = parametros.Select(a => a
                    .GetType()).ToArray();
            var met = sel.GetType()
                    .GetMethod("SetElementIds", tipos);
            if (met != null)
            {
                met.Invoke(sel, parametros);
            }
            else
            {
                var prop = sel.GetType()
                        .GetProperty("Elements");
                var temp = prop.GetValue(sel, null);
                if (elementos.Count == 0)
                {
                    met = temp.GetType()
                        .GetMethod("Clear");
                    met.Invoke(temp, null);
                }
                else
                {
                    foreach (var id in elementos)
                    {
                        var elemento = doc
                                .GetElement2(id);
                        parametros = new object[]
                        {
                                elemento
                        };
                        tipos = parametros
                            .Select(a => a.GetType()).ToArray();
                        met = temp.GetType()
                            .GetMethod("Add", tipos);
                        met.Invoke(temp, parametros);
                    }
                }
            }
        }

        public static void ClearSelection2(
            this Selection sel)
        {
            var prop = sel.GetType()
                    .GetProperty("Elements");
            if (prop != null)
            {
                var obj = prop.GetValue(sel, null);
                var met = obj.GetType()
                        .GetMethod("Clear");
                met.Invoke(obj, null);
            }
            else
            {
                ICollection<ElementId> ids
                    = [];
                var met = sel.GetType().GetMethod(
                        "SetElementIds", new[] { ids.GetType() });
                met.Invoke(sel, new object[] { ids });
            }
        }

        #endregion // Autodesk.Revit.UI.Selection.Selection

        #region Autodesk.Revit.DB.View

        public static ElementId Duplicate2(this View view)
        {
            ElementId value = null;
            var doc = view.Document;
            var ls = doc.GetType().Assembly.GetTypes()
                .Where(a => a.IsEnum
                            && a.Name == "ViewDuplicateOption")
                .ToList();
            if (ls.Count > 0)
            {
                var t = ls[0];
                object obj = view;
                var met = view.GetType().GetMethod(
                        "Duplicate", new[] { t });
                if (met != null)
                    value = met.Invoke(obj,
                        new object[] { 2 }) as ElementId;
            }

            return value;
        }

        public static void SetOverlayView2(
            this View view,
            List<ElementId> ids,
            Autodesk.Revit.DB.Color cor = null,
            int espessura = -1)
        {
            var doc = view.Document;
            var ls = doc.GetType().Assembly
                .GetTypes().Where(
                    a => a.IsClass
                         && a.Name == "OverrideGraphicSettings")
                .ToList();
            if (ls.Count > 0)
            {
                var t = ls[0];
                var construtor = t
                        .GetConstructor(new Type[] { });
                construtor.Invoke(new object[] { });
                var obj = construtor.Invoke(new object[] { });
                var met = obj.GetType()
                        .GetMethod("SetProjectionLineColor",
                            new[] { cor.GetType() });
                met.Invoke(obj, new object[] { cor });
                met = obj.GetType()
                    .GetMethod("SetProjectionLineWeight",
                        new[] { espessura.GetType() });
                met.Invoke(obj, new object[] { espessura });
                met = view.GetType()
                    .GetMethod("SetElementOverrides",
                        new[]
                        {
                                typeof(ElementId),
                                obj.GetType()
                        });
                foreach (var id in ids) met.Invoke(view, new[] { id, obj });
            }
            else
            {
                var met = view.GetType()
                        .GetMethod("set_ProjColorOverrideByElement",
                            new[]
                            {
                                typeof(ICollection<ElementId>),
                                typeof(Autodesk.Revit.DB.Color)
                            });
                met.Invoke(view, new object[] { ids, cor });
                met = view.GetType()
                    .GetMethod("set_ProjLineWeightOverrideByElement",
                        new[]
                        {
                                typeof(ICollection<ElementId>),
                                typeof(int)
                        });
                met.Invoke(view, new object[] { ids, espessura });
            }
        }

        #endregion // Autodesk.Revit.DB.View

        #region Autodesk.Revit.DB.Viewplan

        public static ElementId GetViewTemplateId2(
            this ViewPlan view)
        {
            ElementId value = null;
            var prop = view.GetType()
                    .GetProperty("ViewTemplateId");
            if (prop != null)
                value = prop.GetValue(view,
                    null) as ElementId;
            return value;
        }

        public static void SetViewTemplateId2(
            this ViewPlan view,
            ElementId id)
        {
            var prop = view.GetType()
                    .GetProperty("ViewTemplateId");
            if (prop != null) prop.SetValue(view, id, null);
        }

        #endregion // Autodesk.Revit.DB.Viewplan
    }

    #endregion // Compatibility Methods by Magson Leone

}
