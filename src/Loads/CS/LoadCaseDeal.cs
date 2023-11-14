// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    ///     Deal the LoadCase class which give methods to connect Revit and the user operation on the form
    /// </summary>
    public class LoadCaseDeal
    {
        private readonly Loads m_dataBuffer;

        private readonly List<string> m_newLoadNaturesName; //store all the new nature's name that should be added

        // Private Members
        private readonly Application m_revit; // Store the reference of revit application

        // Methods
        /// <summary>
        ///     Default constructor of LoadCaseDeal
        /// </summary>
        public LoadCaseDeal(Loads dataBuffer)
        {
            m_dataBuffer = dataBuffer;
            m_revit = dataBuffer.RevitApplication;
            m_newLoadNaturesName = new List<string>();

            m_newLoadNaturesName.Add("EQ1");
            m_newLoadNaturesName.Add("EQ2");
            m_newLoadNaturesName.Add("W1");
            m_newLoadNaturesName.Add("W2");
            m_newLoadNaturesName.Add("W3");
            m_newLoadNaturesName.Add("W4");
            m_newLoadNaturesName.Add("Other");
        }

        /// <summary>
        ///     prepare data for the dialog
        /// </summary>
        public void PrepareData()
        {
            //Create seven Load Natures first
            CreateLoadNatures();

            //get all the categories of load cases
            var uiapplication = new UIApplication(m_revit);
            var categories = uiapplication.ActiveUIDocument.Document.Settings.Categories;
            var category = categories.get_Item(BuiltInCategory.OST_LoadCases);
            var categoryNameMap = category.SubCategories;
            var iter = categoryNameMap.GetEnumerator();
            iter.Reset();
            while (iter.MoveNext())
            {
                var temp = iter.Current as Category;
                if (null == temp)
                    continue;

                m_dataBuffer.LoadCaseCategories.Add(temp);
            }

            //get all the loadnatures name
            var elements = new FilteredElementCollector(uiapplication.ActiveUIDocument.Document)
                .OfClass(typeof(LoadNature)).ToElements();
            foreach (var e in elements)
            {
                var nature = e as LoadNature;
                if (null != nature)
                {
                    m_dataBuffer.LoadNatures.Add(nature);
                    var newLoadNaturesMap = new LoadNaturesMap(nature);
                    m_dataBuffer.LoadNaturesMap.Add(newLoadNaturesMap);
                }
            }

            elements = new FilteredElementCollector(uiapplication.ActiveUIDocument.Document).OfClass(typeof(LoadCase))
                .ToElements();
            foreach (var e in elements)
            {
                //get all the loadcases
                var loadCase = e as LoadCase;
                if (null != loadCase)
                {
                    m_dataBuffer.LoadCases.Add(loadCase);
                    var newLoadCaseMap = new LoadCasesMap(loadCase);
                    m_dataBuffer.LoadCasesMap.Add(newLoadCaseMap);
                }
            }
        }

        /// <summary>
        ///     create some load case natures named EQ1, EQ2, W1, W2, W3, W4, Other
        /// </summary>
        /// <returns></returns>
        private bool CreateLoadNatures()
        {
            //try to add some new load natures
            try
            {
                var uiapplication = new UIApplication(m_revit);
                foreach (var name in m_newLoadNaturesName)
                    LoadNature.Create(uiapplication.ActiveUIDocument.Document, name);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation += e.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     add a new load nature
        /// </summary>
        /// <param name="index">the selected nature's index in the nature map</param>
        /// <returns></returns>
        public bool AddLoadNature(int index)
        {
            var isUnique = false; // check if the name is unique    
            LoadNaturesMap myLoadNature = null;

            //try to get out the loadnature from the map
            try
            {
                myLoadNature = m_dataBuffer.LoadNaturesMap[index];
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation += e.ToString();
                return false;
            }

            //Can not get the load nature
            if (null == myLoadNature)
            {
                m_dataBuffer.ErrorInformation += "Can't find the nature";
                return false;
            }

            //check if the name is unique
            var natureName = myLoadNature.LoadNaturesName; //the load nature's name to be added
            while (!isUnique)
            {
                natureName += "(1)";
                isUnique = IsNatureNameUnique(natureName);
            }

            //try to create a load nature
            try
            {
                var uiapplication = new UIApplication(m_revit);
                var newLoadNature = LoadNature.Create(uiapplication.ActiveUIDocument.Document, natureName);
                if (null == newLoadNature)
                {
                    m_dataBuffer.ErrorInformation += "Create Failed";
                    return false;
                }

                //add the load nature into the list and maps
                m_dataBuffer.LoadNatures.Add(newLoadNature);
                var newMap = new LoadNaturesMap(newLoadNature);
                m_dataBuffer.LoadNaturesMap.Add(newMap);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation += e.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Duplicate a new load case
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DuplicateLoadCase(int index)
        {
            LoadCasesMap myLoadCase = null;
            var isUnique = false;

            //try to get the load case from the map
            try
            {
                myLoadCase = m_dataBuffer.LoadCasesMap[index];
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation += e.ToString();
                return false;
            }

            //get nothing 
            if (null == myLoadCase)
            {
                m_dataBuffer.ErrorInformation += "Can not find the load case";
                return false;
            }

            //check the name
            var caseName = myLoadCase.LoadCasesName;
            while (!isUnique)
            {
                caseName += "(1)";
                isUnique = IsCaseNameUnique(caseName);
            }

            //get the selected case's nature
            var categoryId = myLoadCase.LoadCasesSubCategoryId;
            var natureId = myLoadCase.LoadCasesNatureId;

            var uiapplication = new UIApplication(m_revit);

            //try to create a load case
            try
            {
                var newLoadCase = LoadCase.Create(uiapplication.ActiveUIDocument.Document, caseName, natureId,
                    categoryId);
                if (null == newLoadCase)
                {
                    m_dataBuffer.ErrorInformation += "Create Load Case Failed";
                    return false;
                }

                //add the new case into list and map
                m_dataBuffer.LoadCases.Add(newLoadCase);
                var newLoadCaseMap = new LoadCasesMap(newLoadCase);
                m_dataBuffer.LoadCasesMap.Add(newLoadCaseMap);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation += e.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        ///     check if the case's name is unique
        /// </summary>
        /// <param name="name">the name to be checked</param>
        /// <returns>true will be returned if the name is unique</returns>
        public bool IsCaseNameUnique(string name)
        {
            //compare the name with the name of each case in the map
            for (var i = 0; i < m_dataBuffer.LoadCasesMap.Count; i++)
            {
                var nameTemp = m_dataBuffer.LoadCasesMap[i].LoadCasesName;
                if (name == nameTemp) return false;
            }

            return true;
        }

        /// <summary>
        ///     check if the nature's name is unique
        /// </summary>
        /// <param name="name">the name to be checked</param>
        /// <returns>true will be returned if the name is unique</returns>
        public bool IsNatureNameUnique(string name)
        {
            //compare the name with the name of each nature in the map
            for (var i = 0; i < m_dataBuffer.LoadNatures.Count; i++)
            {
                var nameTemp = m_dataBuffer.LoadNaturesMap[i].LoadNaturesName;
                if (name == nameTemp) return false;
            }

            return true;
        }
    }
}
