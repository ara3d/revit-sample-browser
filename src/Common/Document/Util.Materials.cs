// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated material helpers

        internal static FilteredElementCollector FilterForMaterials(
            Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(Material));
        }

        internal static string FaceMaterialName(
            Document doc,
            Face face)
        {
            var id = face.MaterialElementId;
            var m = doc.GetElement(id) as Material;
            return m.Name;
        }

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

        public static List<string> GetMaterialsFromGeometry(
            Document doc,
            GeometryElement geo)
        {
            List<string> materials = new();

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
