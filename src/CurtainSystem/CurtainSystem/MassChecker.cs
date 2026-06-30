// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Mep;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Properties;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using GeoVector4 = Ara3D.RevitSampleBrowser.Common.Geometry.Vector4;
namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem
{
    public class MassChecker
    {
        // the document containing all the data used in the sample
        private readonly MyDocument m_mydocument;

        public MassChecker(MyDocument mydoc)
        {
            m_mydocument = mydoc;
        }

        public bool CheckSelectedMass()
        {
            var mass = GetSelectedMass();
            // start the sample without making a parallelepiped mass selected
            if (null == mass)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_InvalidSelection;
                return false;
            }

            var isMassParallelepiped = IsMassParallelepiped(mass);
            if (false == isMassParallelepiped)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_InvalidSelection;
                return false;
            }

            return true;
        }

        private bool IsMassParallelepiped(FamilyInstance mass)
        {
            var faces = GetMassFaceArray(mass);

            // a parallelepiped always has 6 faces
            if (null == faces ||
                6 != faces.Size)
                return false;

            var isFacesParallel = IsFacesParallel(faces);

            if (isFacesParallel) m_mydocument.MassFaceArray = faces;

            return isFacesParallel;
        }

        private bool IsFacesParallel(FaceArray faces)
        {
            // step1: get the normals of the 6 faces
            var normals = new List<GeoVector4>();
            foreach (Face face in faces)
            {
                var edgeArrayArray = face.EdgeLoops;
                var edges = edgeArrayArray.get_Item(0);

                if (null == edges ||
                    2 > edges.Size)
                    return false;

                // we use the cross product of 2 non-parallel vectors as the normal
                for (var i = 0; i < edges.Size - 1; i++)
                {
                    var edgeA = edges.get_Item(i);
                    var edgeB = edges.get_Item(i + 1);

                    // if edgeA & edgeB are parallel, can't compute  the cross product
                    if (FaceAndSolidGeometry.IsLinesParallel(edgeA, edgeB)) continue;

                    var vec4 = ConnectorHelper.ComputeCrossProduct(edgeA, edgeB);
                    normals.Add(vec4);
                    break;
                }
            }

            return normals != null && normals.Count == 6 && FaceAndSolidGeometry.AreFaceNormalsPaired(normals);
        }

        private FaceArray GetMassFaceArray(FamilyInstance mass)
        {
            // Obtain the gemotry information of the mass
            var opt = m_mydocument.CommandData.Application.Application.Create.NewGeometryOptions();
            opt.DetailLevel = ViewDetailLevel.Fine;
            opt.ComputeReferences = true;
            GeometryElement geoElement = null;
            try
            {
                geoElement = mass.get_Geometry(opt);
            }
            catch (Exception)
            {
                return null;
            }

            if (null == geoElement) return null;

            //GeometryObjectArray objectarray = geoElement.Objects;
            //foreach (GeometryObject obj in objectarray)
            var objects = geoElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var obj = objects.Current;

                var solid = obj as Solid;

                if (null != solid &&
                    null != solid.Faces &&
                    0 != solid.Faces.Size)
                    return solid.Faces;
            }

            return null;
        }

        private FamilyInstance GetSelectedMass()
        {
            FamilyInstance resultMass = null;

            var selection = m_mydocument.UiDocument.Selection;
            var elementSet = new ElementSet();
            foreach (var elementId in selection.GetElementIds())
            {
                elementSet.Insert(m_mydocument.UiDocument.Document.GetElement(elementId));
            }

            if (null == selection ||
                null == elementSet ||
                elementSet.IsEmpty ||
                1 != elementSet.Size)
                //m_mydocument.FatalErrorMsg = Properties.Resources.MSG_InvalidSelection;
                return null;

            foreach (var selElementId in selection.GetElementIds())
            {
                var selElement = m_mydocument.UiDocument.Document.GetElement(selElementId);
                if (selElement is FamilyInstance inst &&
                    "Mass" == inst.Category.Name)
                {
                    resultMass = inst;
                    break;
                }
            }

            // nothing selected or the selected element is not a mass

            return resultMass;
        }
    }
}
