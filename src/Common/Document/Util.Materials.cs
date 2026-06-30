// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated material helpers

        /// <summary>
        ///     Return a filtered element collector for materials.
        /// </summary>
        internal static FilteredElementCollector FilterForMaterials(
            Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Material));
        }

        /// <summary>
        ///     Replacement for deprecated Face.MaterialElement.
        /// </summary>
        internal static string FaceMaterialName(
            Document doc,
            Face face)
        {
            var id = face.MaterialElementId;
            var m = doc.GetElement(id) as Material;
            return m.Name;
        }

        /// <summary>
        ///     Return family instance element material.
        /// </summary>
        public static Material GetFamilyInstanceMaterial(
            Document doc,
            FamilyInstance fi)
        {
            Material material = null;

            foreach (Parameter p in fi.Parameters)
            {
                var def = p.Definition;

                if (p.StorageType == StorageType.ElementId
                    && GroupTypeId.Materials == def.GetGroupTypeId()
                    && def.GetDataType() == SpecTypeId.Reference.Material)
                {
                    var materialId = p.AsElementId();

                    if (-1 == materialId.Value)
                    {
                        if (null != fi.Category)
                        {
                            material = fi.Category.Material;

                            if (null == material)
                            {
                                var id = Material.Create(doc, "GoodConditionMat");
                                var mat = doc.GetElement(id) as Material;

                                mat.Color = new Autodesk.Revit.DB.Color(255, 0, 0);

                                fi.Category.Material = mat;

                                material = fi.Category.Material;
                            }
                        }
                    }

                    break;
                }
            }

            return material;
        }

        /// <summary>
        ///     Return materials by recursively traversing geometry solids.
        /// </summary>
        public static List<string> GetMaterialsFromGeometry(
            Document doc,
            GeometryElement geo)
        {
            var materials = new List<string>();

            foreach (var o in geo)
                if (o is Solid solid)
                    foreach (Face face in solid.Faces)
                        materials.Add(FaceMaterialName(doc, face));
                else if (o is GeometryInstance instance)
                    materials.AddRange(GetMaterialsFromGeometry(
                        doc, instance.SymbolGeometry));

            return materials;
        }

        #endregion
    }
}
