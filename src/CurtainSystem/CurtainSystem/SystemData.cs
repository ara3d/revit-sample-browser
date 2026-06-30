// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Data;
using Ara3D.RevitSampleBrowser.CurtainSystem.CS.Properties;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.CurtainSystem
{
    public class SystemData
    {
        public delegate void CurtainSystemChangedHandler();

        // the count of the created curtain systems
        private static int _csIndex = -1;

        // the data of the sample
        private readonly MyDocument m_mydocument;

        public SystemData(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            CurtainSystemInfos = [];
        }

        // all the created curtain systems and their data
        public List<SystemInfo> CurtainSystemInfos { get; private set; }

        public event CurtainSystemChangedHandler CurtainSystemChanged;

        public void CreateCurtainSystem(List<int> faceIndices, bool byFaceArray)
        {
            // just refresh the main UI
            if (null == faceIndices ||
                0 == faceIndices.Count)
            {
                CurtainSystemChanged?.Invoke();
                return;
            }

            SystemInfo resultInfo = new(m_mydocument)
            {
                ByFaceArray = byFaceArray,
                GridFacesIndices = faceIndices,
                Index = ++_csIndex
            };

            // step 1: create the curtain system
            if (byFaceArray)
            {
                FaceArray faceArray = new();
                foreach (var index in faceIndices)
                {
                    faceArray.Append(m_mydocument.MassFaceArray.get_Item(index));
                }

                Autodesk.Revit.DB.CurtainSystem curtainSystem = null;
                Transaction t = new(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
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
            else
            {
                ReferenceArray refArray = new();
                foreach (var index in faceIndices)
                {
                    refArray.Append(m_mydocument.MassFaceArray.get_Item(index).Reference);
                }

                ICollection<ElementId> curtainSystems = null;
                Transaction t = new(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
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

                foreach (var cs in curtainSystems)
                {
                    resultInfo.CurtainForm = m_mydocument.Document.GetElement(cs) as Autodesk.Revit.DB.CurtainSystem;
                    break;
                }
            }

            // step 2: update the curtain system list in the main UI
            CurtainSystemInfos.Add(resultInfo);
            CurtainSystemChanged?.Invoke();
        }

        public void DeleteCurtainSystem(List<int> checkedIndices)
        {
            Transaction t = new(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
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

            // remove the "deleted" curtain systems out
            var infos = CurtainSystemInfos;
            CurtainSystemInfos = [];

            foreach (var info in infos)
            {
                if (null != info.CurtainForm)
                    CurtainSystemInfos.Add(info);
            }

            CurtainSystemChanged?.Invoke();
        }
    } // end of class

    public class SystemInfo
    {
        private List<int> m_gridFacesIndices;

        // the index of the curtain systems
        private int m_index;

        // the data of the sample
        private readonly MyDocument m_mydocument;

        public SystemInfo(MyDocument mydoc)
        {
            m_mydocument = mydoc;
            m_gridFacesIndices = [];
            UncoverFacesIndices = [];
            ByFaceArray = false;
            m_index = 0;
        }

        // the curtain system
        public Autodesk.Revit.DB.CurtainSystem CurtainForm { get; set; }

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
        public List<int> UncoverFacesIndices { get; }

        public bool ByFaceArray { get; set; }

        // the name of the curtain system, identified by its index
        public string Name { get; private set; }

        public int Index
        {
            get => m_index;
            set
            {
                m_index = value;
                Name = $"Curtain System {m_index}";
            }
        }

        public void AddCurtainGrids(List<int> faceIndices)
        {
            // step 1: find out the faces to be covered
            List<Reference> refFaces = [];
            foreach (var index in faceIndices)
            {
                refFaces.Add(m_mydocument.MassFaceArray.get_Item(index).Reference);
            }

            // step 2: cover the selected faces with curtain grids
            Transaction t = new(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
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

        public void RemoveCurtainGrids(List<int> faceIndices)
        {
            // step 1: find out the faces to be covered
            List<Reference> refFaces = [];
            foreach (var index in faceIndices)
            {
                refFaces.Add(m_mydocument.MassFaceArray.get_Item(index).Reference);
            }

            // step 2: remove the selected curtain grids
            Transaction t = new(m_mydocument.Document, Guid.NewGuid().GetHashCode().ToString());
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

    public class GridFaceInfo
    {
        public GridFaceInfo(int index)
        {
            FaceIndex = index;
        }

        // the host face of the curtain grid
        public int FaceIndex { get; set; }

        public override string ToString()
        {
            return $"Grid on Face {FaceIndex}";
        }
    }

    public class UncoverFaceInfo
    {
        public UncoverFaceInfo(int index)
        {
            Index = index;
        }

        public int Index { get; set; }

        public override string ToString()
        {
            return $"Face {Index}";
        }
    }
}
