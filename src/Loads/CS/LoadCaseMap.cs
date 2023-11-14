// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    ///     A class to store Load Case and it's properties.
    /// </summary>
    public class LoadCasesMap
    {
        private readonly LoadCase m_loadCase;
        private string m_loadCasesName; //Store the load case's name
        private ElementId m_loadCasesNatureId; //Store the Id of the load case's nature
        private string m_loadCasesNumber; //Store the load cases number
        private ElementId m_loadCasesSubcategoryId; //Store the Id of the load case's category

        /// <summary>
        ///     Overload the constructor
        /// </summary>
        /// <param name="loadCase">Load Case</param>
        public LoadCasesMap(LoadCase loadCase)
        {
            m_loadCase = loadCase;
            m_loadCasesName = m_loadCase.Name;
            m_loadCasesNumber = m_loadCase.Number.ToString();
            m_loadCasesNatureId = m_loadCase.NatureId;
            m_loadCasesSubcategoryId = m_loadCase.SubcategoryId;
        }

        /// <summary>
        ///     LoadCasesName
        /// </summary>
        public string LoadCasesName
        {
            get => m_loadCasesName;
            set
            {
                m_loadCasesName = value;
                m_loadCase.Name = m_loadCasesName;
            }
        }

        /// <summary>
        ///     LoadCasesNumber property.
        /// </summary>
        public string LoadCasesNumber => m_loadCase.Number.ToString();

        /// <summary>
        ///     LoadCasesNatureId property.
        /// </summary>
        public ElementId LoadCasesNatureId
        {
            get => m_loadCasesNatureId;
            set
            {
                m_loadCasesNatureId = value;
                m_loadCase.NatureId = m_loadCasesNatureId;
            }
        }

        /// <summary>
        ///     LoadCasesCategoryId property.
        /// </summary>
        public ElementId LoadCasesSubCategoryId
        {
            get => m_loadCasesSubcategoryId;
            set
            {
                m_loadCasesSubcategoryId = value;
                m_loadCase.SubcategoryId = m_loadCasesSubcategoryId;
            }
        }
    }

    /// <summary>
    ///     A class to store Load Nature name
    /// </summary>
    public class LoadNaturesMap
    {
        private readonly LoadNature m_loadNature;
        private string m_loadNaturesName;

        /// <summary>
        ///     constructor of LoadNaturesMap class
        /// </summary>
        /// <param name="loadNature"></param>
        public LoadNaturesMap(LoadNature loadNature)
        {
            m_loadNature = loadNature;
            m_loadNaturesName = loadNature.Name;
        }

        /// <summary>
        ///     Get or set a load nature name.
        /// </summary>
        public string LoadNaturesName
        {
            get => m_loadNaturesName;
            set
            {
                m_loadNaturesName = value;
                m_loadNature.Name = m_loadNaturesName;
            }
        }
    }
}
