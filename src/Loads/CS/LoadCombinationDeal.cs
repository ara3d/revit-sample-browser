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
using System.Windows.Forms;

using Autodesk;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    /// mainly deal class which give methods to connect Revit and the user operation on the form
    /// </summary>
    public class LoadCombinationDeal
    {
        // Private Members
        Loads m_dataBuffer;                 // Store the reference of Loads
        Autodesk.Revit.ApplicationServices.Application m_revit; // Store the reference of revit
        Autodesk.Revit.DB.Document m_document; // Store the reference of document

        // Methods
        /// <summary>
        /// Default constructor of LoadCombinationDeal
        /// </summary>
        public LoadCombinationDeal(Loads dataBuffer)
        {
            m_dataBuffer = dataBuffer;
            m_revit = dataBuffer.RevitApplication;
            var uiapplication = new UIApplication(m_revit);
            m_document = uiapplication.ActiveUIDocument.Document;
        }

        /// <summary>
        /// Find out all Load Combination and Usage in the existing document.
        /// As specification require, prepare some Load Combination Usages if they are not in document
        /// </summary>
        public void PrepareData()
        {
            // Find out all  Load Combination and Usage in the existing document.
            var elements = (new FilteredElementCollector(m_document)).OfClass(typeof(LoadCombination)).ToElements();
            foreach (var elem in elements)
            {
                var combination = elem as LoadCombination;
                if (null != combination)
                {
                    // Add the Load Combination name.
                    m_dataBuffer.LoadCombinationNames.Add(combination.Name);

                    // Create LoadCombinationMap object.
                    var combinationMap = new LoadCombinationMap(combination);

                    // Add the LoadCombinationMap object to the array list.
                    m_dataBuffer.LoadCombinationMap.Add(combinationMap);
                }
            }

            elements = (new FilteredElementCollector(m_document)).OfClass(typeof(LoadUsage)).ToElements();
            foreach (var elem in elements)
            {
                // Add Load Combination Usage information
                var usage = elem as LoadUsage;
                if (null != usage)
                {
                    // Add the Load Usage name
                    m_dataBuffer.LoadUsageNames.Add(usage.Name);

                    // Add the Load Usage object to a LoadUsageArray
                    m_dataBuffer.LoadUsages.Add(usage);

                    // Add the Load Usage information to UsageMap.
                    var usageMap = new UsageMap(m_dataBuffer, usage.Name);
                    m_dataBuffer.UsageMap.Add(usageMap);
                }
            }

            // As specification require, some Load Combination Usages if they are not in document
            String[] initUsageArray = { "Gravity", "Lateral", "Steel", "Composite", "Concrete" };
            foreach (var s in initUsageArray)
            {
                NewLoadUsage(s);
            }
        }

        /// <summary>
        /// Create new Load Combination
        /// </summary>
        /// <param name="name">The new Load Combination name</param>
        /// <param name="typeIndex">The index of new Load Combination Type</param>
        /// <param name="stateIndex">The index of new Load Combination State</param>
        /// <returns>true if the creation was successful; otherwise, false</returns>
        public Boolean NewLoadCombination(String name, int typeIndex, int stateIndex)
        {
            // Define some data for creation.
            var usageIds = new List<ElementId>();
            var components = new List<LoadComponent>();
            var factorArray = new double[m_dataBuffer.FormulaMap.Count];

            // First check whether the name has been used
            foreach (var s in m_dataBuffer.LoadCombinationNames)
            {
                if (s == name || null == name)
                {
                    m_dataBuffer.ErrorInformation = "the combination name has been used.";
                    return false;
                }
            }

            // Get the usage information.
            foreach (var usageMap in m_dataBuffer.UsageMap)
            {
                if (true == usageMap.Set)
                {
                    var usage = FindUsageByName(usageMap.Name);
                    if (null != usage)
                    {
                        usageIds.Add(usage.Id);
                    }
                }
            }

            // Get the formula information
            for (var i = 0; i < m_dataBuffer.FormulaMap.Count; i++)
            {
                var formulaMap = m_dataBuffer.FormulaMap[i];
                factorArray[i] = formulaMap.Factor;
                var loadCase = FindLoadCaseByName(formulaMap.Case);
                if (null != loadCase)
                {
                    var component = new LoadComponent(loadCase.Id, formulaMap.Factor);
                    components.Add(component);
                }
            }


            // Begin to new a load combination
            try
            {
                var loadCombination = LoadCombination.Create(m_document, name, (LoadCombinationType)typeIndex, (LoadCombinationState)stateIndex);
                if (null == loadCombination)
                {
                    m_dataBuffer.ErrorInformation = "Get null reference after usage creation.";
                    return false;
                }
                loadCombination.SetComponents(components);
                loadCombination.SetUsageIds(usageIds);

                // Store this load combination information for further use
                m_dataBuffer.LoadCombinationNames.Add(loadCombination.Name);
                var combinationMap = new LoadCombinationMap(loadCombination);
                m_dataBuffer.LoadCombinationMap.Add(combinationMap);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation = e.Message;
                return false;
            }

            // If create combination successful, reset the usage check state and clear the formula
            foreach (var usageMap in m_dataBuffer.UsageMap)
            {
                usageMap.Set = false;
            }
            m_dataBuffer.FormulaMap.Clear();
            return true;
        }

        /// <summary>
        /// Delete the selected Load Combination
        /// </summary>
        /// <param name="index">The selected index in the DataGridView</param>
        /// <returns>true if the delete operation was successful; otherwise, false</returns>
        public Boolean DeleteCombination(int index)
        {
            // Get the name of the delete combination
            var combinationName = m_dataBuffer.LoadCombinationNames[index];

            // Find the combination by the name and delete the combination
            LoadCombination combination;
            var elements = (new FilteredElementCollector(m_document)).OfClass(typeof(LoadCombination)).ToElements();
            foreach (var elem in elements)
            {
                combination = elem as LoadCombination;

                if (combinationName == combination.Name)
                {
                    // Begin to delete the combination
                    try
                    {
                        m_document.Delete(combination.Id);
                    }
                    catch (Exception e)
                    {
                        m_dataBuffer.ErrorInformation = e.ToString();
                        return false;
                    }
                    break;
                }
            }

            // If delete is successful, Change the map and the string List
            m_dataBuffer.LoadCombinationMap.RemoveAt(index);
            m_dataBuffer.LoadCombinationNames.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Create a new load combination usage
        /// </summary>
        /// <param name="usageName">The new Load Usage name</param>
        /// <returns>true if the process is successful; otherwise, false</returns> 
        public Boolean NewLoadUsage(String usageName)
        {
            // First check whether the name has been used
            foreach (var s in m_dataBuffer.LoadUsageNames)
            {
                if (usageName == s)
                {
                    m_dataBuffer.ErrorInformation = "the usage name has been used.";
                    return false;
                }
            }

            // Begin to new a load combination usage
            try
            {
                var loadUsage = LoadUsage.Create(m_document, usageName);
                if (null == loadUsage)
                {
                    m_dataBuffer.ErrorInformation = "Get null reference after usage creation.";
                    return false;
                }
                // Store this load usage information for further use.
                m_dataBuffer.LoadUsageNames.Add(loadUsage.Name);
                m_dataBuffer.LoadUsages.Add(loadUsage);

                // Add the Load Usage information to UsageMap.
                var usageMap = new UsageMap(m_dataBuffer, loadUsage.Name);
                m_dataBuffer.UsageMap.Add(usageMap);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation = e.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete the selected Load Usage
        /// </summary>
        /// <param name="index">The selected index in the DataGridView</param>
        /// <returns>true if the delete operation was successful; otherwise, false</returns>
        public Boolean DeleteUsage(int index)
        {
            // Get the delete usage
            var deleteUsage = m_dataBuffer.LoadUsages[index];
            var usageName = deleteUsage.Name;

            // Begin to delete the combination
            try
            {
                m_document.Delete(deleteUsage.Id);
            }
            catch (Exception e)
            {
                m_dataBuffer.ErrorInformation = e.ToString();
                return false;
            }

            // Modify the data to show the delete operation
            m_dataBuffer.LoadUsages.RemoveAt(index);
            m_dataBuffer.LoadUsageNames.RemoveAt(index);
            m_dataBuffer.UsageMap.RemoveAt(index);

            // Need to delete corresponding in Combination
            foreach (var map in m_dataBuffer.LoadCombinationMap)
            {
                var oldUsage = map.Usage;
                var location = oldUsage.IndexOf(usageName);
                if (-1 == location)
                {
                    continue;
                }
                if (oldUsage.Length == usageName.Length)
                {
                    map.Usage = oldUsage.Remove(0);
                    continue;
                }
                if (0 == location)
                {
                    map.Usage = oldUsage.Remove(location, usageName.Length + 1);
                }
                else
                {
                    map.Usage = oldUsage.Remove(location - 1, usageName.Length + 1);
                }
            }

            return true;
        }

        /// <summary>
        /// Change usage name when the user modify it on the form
        /// </summary>
        /// <param name="oldName">The name before modification</param>
        /// <param name="newName">The name after modification</param>
        /// <returns>true if the modification was successful; otherwise, false</returns>
        public Boolean ModifyUsageName(String oldName, String newName)
        {
            // If the name is no change, just return true.
            if (oldName == newName)
            {
                return true;
            }

            // Check whether the name has been used
            foreach (var s in m_dataBuffer.LoadUsageNames)
            {
                if (s == newName)
                {
                    TaskDialog.Show("Revit", "There is a same named usage already.");
                    return false;
                }
            }

            // Begin to modify the name of the usage
            foreach (var usage in m_dataBuffer.LoadUsages)
            {
                if (oldName == usage.Name)
                {
                    usage.get_Parameter(BuiltInParameter.LOAD_USAGE_NAME).Set(newName);
                }
            }
            return true;
        }

        /// <summary>
        /// Add a formula with the load case name
        /// </summary>
        /// <param name="caseName">The name of the load case</param>
        /// <returns>true if the creation is successful; otherwise, false</returns>
        public Boolean AddFormula(String caseName)
        {
            // New a FormulaMap, and add it to m_dataBuffer.FormulaMap
            // Note: the factor of the formula is always set 1 
            var map = new FormulaMap(caseName);
            m_dataBuffer.FormulaMap.Add(map);
            return true;
        }

        /// <summary>
        /// Find a load usage by the load usage name
        /// </summary>
        /// <param name="name">The name of load usage</param>
        /// <returns>The reference of the LoadUsage</returns>
        private LoadUsage FindUsageByName(String name)
        {
            LoadUsage usage = null;
            foreach (var l in m_dataBuffer.LoadUsages)
            {
                if (name == l.Name)
                {
                    usage = l;
                    break;
                }
            }
            return usage;
        }

        /// <summary>
        /// Find a load case by the load case name
        /// </summary>
        /// <param name="name">The name of load case</param>
        /// <returns>The reference of the LoadCase</returns>
        private LoadCase FindLoadCaseByName(String name)
        {
            LoadCase loadCase = null;
            foreach (var l in m_dataBuffer.LoadCases)
            {
                if (name == l.Name)
                {
                    loadCase = l;
                    break;
                }
            }
            return loadCase;
        }
    }
}
