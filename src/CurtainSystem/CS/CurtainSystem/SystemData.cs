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
using System.Collections.Generic;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Revit.SDK.Samples.CurtainSystem.CS.Data;

namespace Revit.SDK.Samples.CurtainSystem.CS.CurtainSystem
{
    /// <summary>
    /// the class to maintain the data and operations of the curtain system
    /// </summary>
    public class SystemData
    {
        // the data of the sample
        MyDocument m_mydocument;

        // the count of the created curtain systems
        static int m_csIndex = -1;

        // all the created curtain systems and their data
        List<SystemInfo> m_curtainSystemInfos;
        /// <summary>
        /// all the created curtain systems and their data
        /// </summary>
        public List<SystemInfo> CurtainSystemInfos
        {
            get
            {
                return m_curtainSystemInfos;
            }
        }

        /// <summary>
        /// occurs only when new curtain system added/removed
        /// the delegate method to handle the curtain system added/removed events
        /// </summary>
        public delegate void CurtainSystemChangedHandler();
        /// <summary>
        /// the event triggered when curtain system added/removed
        /// </summary>
        public event CurtainSystemChangedHandler CurtainSystemChanged;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mydoc">
        /// the document of the sample
        /// </param>
        public SystemData(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            m_curtainSystemInfos = new List<SystemInfo>();
        }

        /// <summary>
        /// create a new curtain system
        /// </summary>
        /// <param name="faceIndices">
        /// the faces to be covered with new curtain system
        /// </param>
        /// <param name="byFaceArray">
        /// indicates whether the curtain system will be created by face array
        /// </param>
        public void CreateCurtainSystem(List<int> faceIndices, bool byFaceArray)
        {
            // just refresh the main UI
            if (null == faceIndices ||
                0 == faceIndices.Count)
            {
                if (null != CurtainSystemChanged)
                {
                    CurtainSystemChanged();
                }
                return;
            }
            var resultInfo = new SystemInfo(m_mydocument);
            resultInfo.ByFaceArray = byFaceArray;
            resultInfo.GridFacesIndices = faceIndices;
            resultInfo.Index = ++m_csIndex;

            //
            // step 1: create the curtain system
            //
            // create the curtain system by face array
            if (true == byFaceArray)
            {
                var faceArray = new FaceArray();
                foreach (var index in faceIndices)
                {
                    faceArray.Append(m_mydocument.MassFaceArray.get_Item(index));
                }

                Autodesk.Revit.DB.CurtainSystem curtainSystem = null;
                var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t.Start();
                try
                {
                    curtainSystem = m_mydocument.Document.Create.NewCurtainSystem(faceArray, m_mydocument.CurtainSystemType);
                }
                catch (System.Exception)
                {
                    m_mydocument.FatalErrorMsg = Properties.Resources.MSG_CreateCSFailed;
                    t.RollBack();
                    return;
                }

                t.Commit();

                resultInfo.CurtainForm = curtainSystem;
            }
            // create the curtain system by reference array
            else
            {
                var refArray = new ReferenceArray();
                foreach (var index in faceIndices)
                {
                    refArray.Append(m_mydocument.MassFaceArray.get_Item(index).Reference);
                }

                ICollection<ElementId> curtainSystems = null;
                var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
                t.Start();
                try
                {
                    curtainSystems = m_mydocument.Document.Create.NewCurtainSystem2(refArray, m_mydocument.CurtainSystemType);
                }
                catch (System.Exception)
                {
                    m_mydocument.FatalErrorMsg = Properties.Resources.MSG_CreateCSFailed;
                    t.RollBack();
                    return;
                }
                t.Commit();

                // internal fatal error, quit the sample
                if (null == curtainSystems ||
                    1 != curtainSystems.Count)
                {
                    m_mydocument.FatalErrorMsg = Properties.Resources.MSG_MoreThan1CSCreated;
                    return;
                }

                // store the curtain system
                foreach (var cs in curtainSystems)
                {
                    resultInfo.CurtainForm = m_mydocument.Document.GetElement(cs) as Autodesk.Revit.DB.CurtainSystem;
                    break;
                }
            }
            //
            // step 2: update the curtain system list in the main UI
            //
            m_curtainSystemInfos.Add(resultInfo);
            if (null != CurtainSystemChanged)
            {
                CurtainSystemChanged();
            }
        }

        /// <summary>
        /// delete the curtain systems
        /// </summary>
        /// <param name="checkedIndices">
        /// the curtain systems to be deleted
        /// </param>
        public void DeleteCurtainSystem(List<int> checkedIndices)
        {
            var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            foreach (var index in checkedIndices)
            {
                var info = m_curtainSystemInfos[index];
                if (null != info.CurtainForm)
                {
                    m_mydocument.Document.Delete(info.CurtainForm.Id);
                    info.CurtainForm = null;
                }
            }
            t.Commit();

            // update the list of created curtain systems
            // remove the "deleted" curtain systems out
            var infos = m_curtainSystemInfos;
            m_curtainSystemInfos = new List<SystemInfo>();

            foreach (var info in infos)
            {
                if (null != info.CurtainForm)
                {
                    m_curtainSystemInfos.Add(info);
                }
            }

            if (null != CurtainSystemChanged)
            {
                CurtainSystemChanged();
            }
        }


    }// end of class

    /// <summary>
    /// the information of a curtain system
    /// </summary>
    public class SystemInfo
    {
        // the data of the sample
        MyDocument m_mydocument;

        // the curtain system
        Autodesk.Revit.DB.CurtainSystem m_curtainSystem;
        /// <summary>
        /// the curtain system
        /// </summary>
        public Autodesk.Revit.DB.CurtainSystem CurtainForm
        {
            get
            {
                return m_curtainSystem;
            }
            set
            {
                m_curtainSystem = value;
            }
        }

        // indicates which faces the curtain system covers
        List<int> m_gridFacesIndices;
        /// <summary>
        /// indicates which faces the curtain system covers
        /// </summary>
        public List<int> GridFacesIndices
        {
            get
            {
                return m_gridFacesIndices;
            }
            set
            {
                m_gridFacesIndices = value;

                // the faces which don't be included will be added to the m_uncoverFacesIndices collection
                for (var i = 0; i < 6; i++)
                {
                    if (false == m_gridFacesIndices.Contains(i))
                    {
                        m_uncoverFacesIndices.Add(i);
                    }
                }
            }
        }

        // the uncovered faces
        List<int> m_uncoverFacesIndices;
        /// <summary>
        /// the uncovered faces
        /// </summary>
        public List<int> UncoverFacesIndices
        {
            get
            {
                return m_uncoverFacesIndices;
            }
        }

        // indicates whether the curtain system is created by face array
        private bool m_byFaceArray;
        /// <summary>
        /// indicates whether the curtain system is created by face array
        /// </summary>
        public bool ByFaceArray
        {
            get
            {
                return m_byFaceArray;
            }
            set
            {
                m_byFaceArray = value;
            }
        }

        // the name of the curtain system, identified by its index
        private string m_name;
        /// <summary>
        /// the name of the curtain system, identified by its index
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        // the index of the curtain systems
        private int m_index;
        /// <summary>
        /// the index of the curtain systems
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
                m_name = "Curtain System " + m_index;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="mydoc">
        /// the document of the sample
        /// </param>
        public SystemInfo(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            m_gridFacesIndices = new List<int>();
            m_uncoverFacesIndices = new List<int>();
            m_byFaceArray = false;
            m_index = 0;
        }

        /// <summary>
        /// add some curtain grids to the curtain system
        /// </summary>
        /// <param name="faceIndices">
        /// the faces to be covered
        /// </param>
        public void AddCurtainGrids(List<int> faceIndices)
        {
            // step 1: find out the faces to be covered
            var refFaces = new List<Reference>();
            foreach (var index in faceIndices)
            {
                refFaces.Add(m_mydocument.MassFaceArray.get_Item(index).Reference);
            }
            // step 2: cover the selected faces with curtain grids
            var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            try
            {
                foreach (var refFace in refFaces)
                {
                    m_curtainSystem.AddCurtainGrid(refFace);
                }
            }
            catch (System.Exception)
            {
                m_mydocument.FatalErrorMsg = Properties.Resources.MSG_AddCGFailed;
                t.RollBack();
                return;
            }
            t.Commit();

            // step 3: update the uncovered faces and curtain grid faces data
            foreach (var i in faceIndices)
            {
                m_uncoverFacesIndices.Remove(i);
                m_gridFacesIndices.Add(i);
            }
        }

        /// <summary>
        /// remove the selected curtain grids
        /// </summary>
        /// <param name="faceIndices">
        /// the curtain grids to be removed
        /// </param>
        public void RemoveCurtainGrids(List<int> faceIndices)
        {
            // step 1: find out the faces to be covered
            var refFaces = new List<Reference>();
            foreach (var index in faceIndices)
            {
                refFaces.Add(m_mydocument.MassFaceArray.get_Item(index).Reference);
            }
            // step 2: remove the selected curtain grids
            var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            try
            {
                foreach (var refFace in refFaces)
                {
                    m_curtainSystem.RemoveCurtainGrid(refFace);
                }
            }
            catch (System.Exception)
            {
                m_mydocument.FatalErrorMsg = Properties.Resources.MSG_RemoveCGFailed;
                t.RollBack();
                return;
            }
            t.Commit();

            // step 3: update the uncovered faces and curtain grid faces data
            foreach (var i in faceIndices)
            {
                m_gridFacesIndices.Remove(i);
                m_uncoverFacesIndices.Add(i);
            }
        }

        /// <summary>
        /// override ToString method
        /// </summary>
        /// <returns>
        /// the string value of the class
        /// </returns>
        public override string ToString()
        {
            return m_name;
        }
    }

    /// <summary>
    /// the information for the curtain grid (which face does it lay on)
    /// </summary>
    public class GridFaceInfo
    {
        // the host face of the curtain grid
        private int m_faceIndex;
        /// <summary>
        /// the host face of the curtain grid
        /// </summary>
        public int FaceIndex
        {
            get
            {
                return m_faceIndex;
            }
            set
            {
                m_faceIndex = value;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="index">
        /// the index of the host face
        /// </param>
        public GridFaceInfo(int index)
        {
            m_faceIndex = index;
        }

        /// <summary>
        /// the string value for the class
        /// </summary>
        /// <returns>
        /// the string value for the class
        /// </returns>
        public override string ToString()
        {
            return "Grid on Face " + m_faceIndex;
        }
    }

    /// <summary>
    /// the information for the faces of the mass
    /// </summary>
    public class UncoverFaceInfo
    {
        // indicates the index for the face
        private int m_faceIndex;
        /// <summary>
        /// indicates the index for the face
        /// </summary>
        public int Index
        {
            get
            {
                return m_faceIndex;
            }
            set
            {
                m_faceIndex = value;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="index">
        /// the index of the face
        /// </param>
        public UncoverFaceInfo(int index)
        {
            m_faceIndex = index;
        }

        /// <summary>
        /// the string value for the class
        /// </summary>
        /// <returns>
        /// the string value for the class
        /// </returns>
        public override string ToString()
        {
            return "Face " + m_faceIndex;
        }
    }
}
