// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Properties;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem
{
    /// <summary>
    ///     the class to maintain the data and operations of the curtain system
    /// </summary>
    public class SystemData
    {
        /// <summary>
        ///     occurs only when new curtain system added/removed
        ///     the delegate method to handle the curtain system added/removed events
        /// </summary>
        public delegate void CurtainSystemChangedHandler();

        // the count of the created curtain systems
        private static int _csIndex = -1;

        // the data of the sample
        private readonly MyDocument m_mydocument;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="mydoc">
        ///     the document of the sample
        /// </param>
        public SystemData(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            CurtainSystemInfos = new List<SystemInfo>();
        }

        // all the created curtain systems and their data
        /// <summary>
        ///     all the created curtain systems and their data
        /// </summary>
        public List<SystemInfo> CurtainSystemInfos { get; private set; }

        /// <summary>
        ///     the event triggered when curtain system added/removed
        /// </summary>
        public event CurtainSystemChangedHandler CurtainSystemChanged;

        /// <summary>
        ///     create a new curtain system
        /// </summary>
        /// <param name="faceIndices">
        ///     the faces to be covered with new curtain system
        /// </param>
        /// <param name="byFaceArray">
        ///     indicates whether the curtain system will be created by face array
        /// </param>
        public void CreateCurtainSystem(List<int> faceIndices, bool byFaceArray)
        {
            // just refresh the main UI
            if (null == faceIndices ||
                0 == faceIndices.Count)
            {
                CurtainSystemChanged?.Invoke();
                return;
            }

            var resultInfo = new SystemInfo(m_mydocument)
            {
                ByFaceArray = byFaceArray,
                GridFacesIndices = faceIndices,
                Index = ++_csIndex
            };

            //
            // step 1: create the curtain system
            //
            // create the curtain system by face array
            if (byFaceArray)
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
                    curtainSystem =
                        m_mydocument.Document.Create.NewCurtainSystem(faceArray, m_mydocument.CurtainSystemType);
                }
                catch (Exception)
                {
                    m_mydocument.FatalErrorMsg = Resources.MSG_CreateCSFailed;
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
                    curtainSystems =
                        m_mydocument.Document.Create.NewCurtainSystem2(refArray, m_mydocument.CurtainSystemType);
                }
                catch (Exception)
                {
                    m_mydocument.FatalErrorMsg = Resources.MSG_CreateCSFailed;
                    t.RollBack();
                    return;
                }

                t.Commit();

                // public fatal error, quit the sample
                if (null == curtainSystems ||
                    1 != curtainSystems.Count)
                {
                    m_mydocument.FatalErrorMsg = Resources.MSG_MoreThan1CSCreated;
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
            CurtainSystemInfos.Add(resultInfo);
            CurtainSystemChanged?.Invoke();
        }

        /// <summary>
        ///     delete the curtain systems
        /// </summary>
        /// <param name="checkedIndices">
        ///     the curtain systems to be deleted
        /// </param>
        public void DeleteCurtainSystem(List<int> checkedIndices)
        {
            var t = new Transaction(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
            t.Start();
            foreach (var index in checkedIndices)
            {
                var info = CurtainSystemInfos[index];
                if (null != info.CurtainForm)
                {
                    m_mydocument.Document.Delete(info.CurtainForm.Id);
                    info.CurtainForm = null;
                }
            }

            t.Commit();

            // update the list of created curtain systems
            // remove the "deleted" curtain systems out
            var infos = CurtainSystemInfos;
            CurtainSystemInfos = new List<SystemInfo>();

            foreach (var info in infos)
            {
                if (null != info.CurtainForm)
                    CurtainSystemInfos.Add(info);
            }

            CurtainSystemChanged?.Invoke();
        }
    } // end of class

    /// <summary>
    ///     the information of a curtain system
    /// </summary>
    public class SystemInfo
    {
        // indicates which faces the curtain system covers
        private List<int> m_gridFacesIndices;

        // the index of the curtain systems
        private int m_index;

        // the data of the sample
        private readonly MyDocument m_mydocument;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="mydoc">
        ///     the document of the sample
        /// </param>
        public SystemInfo(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            m_gridFacesIndices = new List<int>();
            UncoverFacesIndices = new List<int>();
            ByFaceArray = false;
            m_index = 0;
        }

        // the curtain system
        /// <summary>
        ///     the curtain system
        /// </summary>
        public Autodesk.Revit.DB.CurtainSystem CurtainForm { get; set; }

        /// <summary>
        ///     indicates which faces the curtain system covers
        /// </summary>
        public List<int> GridFacesIndices
        {
            get => m_gridFacesIndices;
            set
            {
                m_gridFacesIndices = value;

                // the faces which don't be included will be added to the m_uncoverFacesIndices collection
                for (var i = 0; i < 6; i++)
                    if (false == m_gridFacesIndices.Contains(i))
                        UncoverFacesIndices.Add(i);
            }
        }

        // the uncovered faces
        /// <summary>
        ///     the uncovered faces
        /// </summary>
        public List<int> UncoverFacesIndices { get; }

        // indicates whether the curtain system is created by face array
        /// <summary>
        ///     indicates whether the curtain system is created by face array
        /// </summary>
        public bool ByFaceArray { get; set; }

        // the name of the curtain system, identified by its index
        /// <summary>
        ///     the name of the curtain system, identified by its index
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     the index of the curtain systems
        /// </summary>
        public int Index
        {
            get => m_index;
            set
            {
                m_index = value;
                Name = "Curtain System " + m_index;
            }
        }

        /// <summary>
        ///     add some curtain grids to the curtain system
        /// </summary>
        /// <param name="faceIndices">
        ///     the faces to be covered
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
                    CurtainForm.AddCurtainGrid(refFace);
                }
            }
            catch (Exception)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_AddCGFailed;
                t.RollBack();
                return;
            }

            t.Commit();

            // step 3: update the uncovered faces and curtain grid faces data
            foreach (var i in faceIndices)
            {
                UncoverFacesIndices.Remove(i);
                m_gridFacesIndices.Add(i);
            }
        }

        /// <summary>
        ///     remove the selected curtain grids
        /// </summary>
        /// <param name="faceIndices">
        ///     the curtain grids to be removed
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
                    CurtainForm.RemoveCurtainGrid(refFace);
                }
            }
            catch (Exception)
            {
                m_mydocument.FatalErrorMsg = Resources.MSG_RemoveCGFailed;
                t.RollBack();
                return;
            }

            t.Commit();

            // step 3: update the uncovered faces and curtain grid faces data
            foreach (var i in faceIndices)
            {
                m_gridFacesIndices.Remove(i);
                UncoverFacesIndices.Add(i);
            }
        }

        /// <summary>
        ///     override ToString method
        /// </summary>
        /// <returns>
        ///     the string value of the class
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    ///     the information for the curtain grid (which face does it lay on)
    /// </summary>
    public class GridFaceInfo
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="index">
        ///     the index of the host face
        /// </param>
        public GridFaceInfo(int index)
        {
            FaceIndex = index;
        }

        // the host face of the curtain grid
        /// <summary>
        ///     the host face of the curtain grid
        /// </summary>
        public int FaceIndex { get; set; }

        /// <summary>
        ///     the string value for the class
        /// </summary>
        /// <returns>
        ///     the string value for the class
        /// </returns>
        public override string ToString()
        {
            return "Grid on Face " + FaceIndex;
        }
    }

    /// <summary>
    ///     the information for the faces of the mass
    /// </summary>
    public class UncoverFaceInfo
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="index">
        ///     the index of the face
        /// </param>
        public UncoverFaceInfo(int index)
        {
            Index = index;
        }

        // indicates the index for the face
        /// <summary>
        ///     indicates the index for the face
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        ///     the string value for the class
        /// </summary>
        /// <returns>
        ///     the string value for the class
        /// </returns>
        public override string ToString()
        {
            return "Face " + Index;
        }
    }
}
