// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Properties;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Utility;
using Autodesk.Revit.DB;
using GeoVector4 = Ara3D.RevitSampleBrowser.Common.Geometry.Vector4;

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Mep;
namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem
{
    /// <summary>
    ///     check whether the selected element is a mass and whether the mass is kind of parallelepiped
    ///     (only mass of parallelepiped supported in this sample)
    /// </summary>
    public class MassChecker
    {
        // the document containing all the data used in the sample
        private readonly MyDocument m_mydocument;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="mydoc">
        ///     the document of the sample
        /// </param>
        public MassChecker(MyDocument mydoc)
        {
            m_mydocument = mydoc;
        }

        /// <summary>
        ///     check whether the selection is a parallelepiped mass
        /// </summary>
        public bool CheckSelectedMass()
        {
            // get the selected mass
            var mass = GetSelectedMass();
            // start the sample without making a parallelepiped mass selected
            if (null == mass)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_InvalidSelection;
                return false;
            }

            // check whether the mass is parallelepiped
            var isMassParallelepiped = IsMassParallelepiped(mass);
            if (false == isMassParallelepiped)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_InvalidSelection;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     check whether the mass is a parallelepiped mass by checking the faces' normals
        ///     (if it's a parallelepiped mass, it will have 3 groups of parallel faces)
        /// </summary>
        /// <param name="mass">
        ///     the mass to be checked
        /// </param>
        /// <returns>
        ///     return true if the mass is parallelepiped; otherwise false
        /// </returns>
        private bool IsMassParallelepiped(FamilyInstance mass)
        {
            var faces = GetMassFaceArray(mass);

            // a parallelepiped always has 6 faces
            if (null == faces ||
                6 != faces.Size)
                return false;

            var isFacesParallel = IsFacesParallel(faces);

            // store the face array
            if (isFacesParallel) m_mydocument.MassFaceArray = faces;

            return isFacesParallel;
        }

        /// <summary>
        ///     check whether the faces of the mass are one-one parallel
        /// </summary>
        /// <param name="faces">
        ///     the 6 faces of the mass
        /// </param>
        /// <returns>
        ///     if the 6 faces are one-one parallel, return true; otherwise false
        /// </returns>
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

            if (normals == null || normals.Count != 6)
                return false;

            return FaceAndSolidGeometry.AreFaceNormalsPaired(normals);
        }

        /// <summary>
        ///     get the faces of the mass
        /// </summary>
        /// <param name="mass">
        ///     the source mass
        /// </param>
        /// <returns>
        ///     the faces of the mass
        /// </returns>
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

        /// <summary>
        ///     get the selected mass
        /// </summary>
        /// <returns>
        ///     return the selected mass; if it's not a mass, return null
        /// </returns>
        private FamilyInstance GetSelectedMass()
        {
            FamilyInstance resultMass = null;

            // check whether a mass was selected before launching this sample
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
