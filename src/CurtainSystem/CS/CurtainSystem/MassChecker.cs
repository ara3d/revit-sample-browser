// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Revit.SDK.Samples.CurtainSystem.CS.Data;
using Revit.SDK.Samples.CurtainSystem.CS.Properties;
using Revit.SDK.Samples.CurtainSystem.CS.Utility;

namespace Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem
{
    /// <summary>
    ///     check whether the selected element is a mass and whether the mass is kind of parallelepiped
    ///     (only mass of parallelepiped supported in this sample)
    /// </summary>
    internal class MassChecker
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
            var normals = new List<Vector4>();
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
                    var isLinesParallel = IsLinesParallel(edgeA, edgeB);

                    if (isLinesParallel) continue;

                    var vec4 = ComputeCrossProduct(edgeA, edgeB);
                    normals.Add(vec4);
                    break;
                }
            }

            // step 2: the 6 normals should be one-one parallel pairs
            if (null == normals ||
                6 != normals.Count)
                return false;

            var matchedList = new bool[6];
            for (var i = 0; i < matchedList.Length; i++) matchedList[i] = false;

            // check whether the normal has another matched parallel normal
            for (var i = 0; i < matchedList.Length; i++)
            {
                if (matchedList[i]) continue;

                var vec4A = normals[i];

                for (var j = 0; j < matchedList.Length; j++)
                {
                    if (j == i ||
                        matchedList[j])
                        continue;

                    var vec4B = normals[j];

                    if (IsLinesParallel(vec4A, vec4B))
                    {
                        matchedList[i] = true;
                        matchedList[j] = true;
                        break;
                    }
                }
            }

            // step 3: check each of the 6 normals has matched parallel normal
            for (var i = 0; i < matchedList.Length; i++)
                if (false == matchedList[i])
                    return false;

            // all the normals have matched parallel normals
            return true;
        }

        /// <summary>
        ///     check whether 2 edges are parallel
        /// </summary>
        /// <param name="edgeA">
        ///     the edge to be checked
        /// </param>
        /// <param name="edgeB">
        ///     the edge to be checked
        /// </param>
        /// <returns>
        ///     if they're parallel, return true; otherwise false
        /// </returns>
        private bool IsLinesParallel(Edge edgeA, Edge edgeB)
        {
            var pointsA = edgeA.Tessellate() as List<XYZ>;
            var pointsB = edgeB.Tessellate() as List<XYZ>;
            var vectorA = pointsA[1] - pointsA[0];
            var vectorB = pointsB[1] - pointsB[0];
            var vec4A = new Vector4(vectorA);
            var vec4B = new Vector4(vectorB);
            return IsLinesParallel(vec4A, vec4B);
        }

        /// <summary>
        ///     check whether 2 vectors are parallel
        /// </summary>
        /// <param name="vec4A">
        ///     the vector to be checked
        /// </param>
        /// <param name="vec4B">
        ///     the vector to be checked
        /// </param>
        /// <returns>
        ///     if they're parallel, return true; otherwise false
        /// </returns>
        private bool IsLinesParallel(Vector4 vec4A, Vector4 vec4B)
        {
            // if 2 vectors are parallel, they should be like the following formula:
            // vec4A.X    vec4A.Y    vec4A.Z
            // ------- == ------- == -------
            // vec4B.X    vec4B.Y    vec4B.Z
            // change to multiply, it's 
            // vec4A.X * vec4B.Y == vec4A.Y * vec4B.X &&
            // vec4A.Y * vec4B.Z == vec4A.Z * vec4B.Y
            var aa = vec4A.X * vec4B.Y;
            var bb = vec4A.Y * vec4B.X;
            var cc = vec4A.Y * vec4B.Z;
            var dd = vec4A.Z * vec4B.Y;
            var ee = vec4A.X * vec4B.Z;
            var ff = vec4A.Z * vec4B.X;

            const double tolerance = 0.0001d;

            if (Math.Abs(aa - bb) < tolerance &&
                Math.Abs(cc - dd) < tolerance &&
                Math.Abs(ee - ff) < tolerance)
                return true;

            return false;
        }

        /// <summary>
        ///     compute the cross product of 2 edges
        /// </summary>
        /// <param name="edgeA">
        ///     the edge for the cross product
        /// </param>
        /// <param name="edgeB">
        ///     the edge for the cross product
        /// </param>
        /// <returns>
        ///     the cross product of 2 edges
        /// </returns>
        private Vector4 ComputeCrossProduct(Edge edgeA, Edge edgeB)
        {
            var pointsA = edgeA.Tessellate() as List<XYZ>;
            var pointsB = edgeB.Tessellate() as List<XYZ>;
            var vectorA = pointsA[1] - pointsA[0];
            var vectorB = pointsB[1] - pointsB[0];
            var vec4A = new Vector4(vectorA);
            var vec4B = new Vector4(vectorB);
            return Vector4.CrossProduct(vec4A, vec4B);
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
            var Objects = geoElement.GetEnumerator();
            while (Objects.MoveNext())
            {
                var obj = Objects.Current;

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
            var selection = m_mydocument.UIDocument.Selection;
            var elementSet = new ElementSet();
            foreach (var elementId in selection.GetElementIds())
                elementSet.Insert(m_mydocument.UIDocument.Document.GetElement(elementId));
            if (null == selection ||
                null == elementSet ||
                elementSet.IsEmpty ||
                1 != elementSet.Size)
                //m_mydocument.FatalErrorMsg = Properties.Resources.MSG_InvalidSelection;
                return null;

            foreach (var selElementId in selection.GetElementIds())
            {
                var selElement = m_mydocument.UIDocument.Document.GetElement(selElementId);
                var inst = selElement as FamilyInstance;
                if (null != inst &&
                    "Mass" == inst.Category.Name)
                {
                    resultMass = inst;
                    break;
                }
            }

            // nothing selected or the selected element is not a mass
            if (null == resultMass)
                //m_mydocument.FatalErrorMsg = Properties.Resources.MSG_InvalidSelection;
                return null;

            return resultMass;
        }
    }
}
