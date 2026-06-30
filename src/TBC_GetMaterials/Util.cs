using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_GetMaterials sample.</summary>
    internal static partial class Util
    {
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

                                mat.Color = new Color(255, 0, 0);

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
        public static List<string> GetMaterialsFromGeometry1(
            Document doc,
            GeometryElement geo)
        {
            var materials = new List<string>();

            foreach (var o in geo)
                if (o is Solid solid)
                    foreach (Face face in solid.Faces)
                        materials.Add(FaceMaterialName(doc, face));
                else if (o is GeometryInstance instance)
                    materials.AddRange(GetMaterialsFromGeometry1(
                        doc, instance.SymbolGeometry));

            return materials;
        }

        /// <summary>
        ///     Return materials by traversing geometry and instance solids.
        /// </summary>
        public static List<string> GetMaterialsFromGeometry(
            Document doc,
            GeometryElement geo)
        {
            var materials = new List<string>();

            foreach (var o in geo)
                if (o is Solid solid1)
                {
                    if (null != solid1)
                        foreach (Face face in solid1.Faces)
                            materials.Add(FaceMaterialName(doc, face));
                }
                else if (o is GeometryInstance instance)
                {
                    foreach (object geomObj in instance.SymbolGeometry)
                    {
                        var solid = geomObj as Solid;
                        if (solid != null)
                            foreach (Face face in solid.Faces)
                                materials.Add(FaceMaterialName(doc, face));
                    }
                }

            return materials;
        }
    }
}
