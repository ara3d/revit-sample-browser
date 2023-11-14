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
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.Loads.CS
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Loads : IExternalCommand
    {
        #region Private Data Members
        // Mainly used data definition
        LoadCombinationDeal m_combinationDeal;  // the deal class on load combination page

        // Define the data mainly used in LoadCombinationDeal class
        // Store all the Load Combination information include the user add.

        // Define the data mainly used in LoadCaseDeal class             

        #endregion
        
        #region Properties
        /// <summary>
        /// Used as the dataSource of load cases DataGridView control,
        /// and the information which support load case creation also.
        /// </summary>
        public List<LoadCasesMap> LoadCasesMap { get; }

        /// <summary>
        /// Used as the dataSource of load natures DataGridView control,
        /// and the information which support load nature creation also.
        /// </summary>
        public List<LoadNaturesMap> LoadNaturesMap { get; }

        /// <summary>
        /// save all loadnature object in current project
        /// </summary>
        public List<LoadNature> LoadNatures { get; }

        /// <summary>
        /// save all loadcase object in current project
        /// </summary>
        public List<LoadCase> LoadCases { get; }

        /// <summary>
        /// save all load cases category in current project
        /// </summary>
        public List<Category> LoadCaseCategories { get; }

        /// <summary>
        /// object which do add, delete and update command on load related objects
        /// </summary>
        public LoadCaseDeal LoadCasesDeal { get; private set; }

        /// <summary>
        /// Store the reference of revit
        /// </summary>
        public Autodesk.Revit.ApplicationServices.Application RevitApplication { get; private set; }

        /// <summary>
        /// LoadUsageNames property, used to store all the usage names in current document
        /// </summary>
        public List<string> LoadUsageNames { get; }

        /// <summary>
        /// Used to store all the load usages in current document, include the user add
        /// </summary>
        public List<LoadUsage> LoadUsages { get; }

        /// <summary>
        /// LoadCombinationNames property, used to store all the combination names in current document
        /// </summary>
        public List<string> LoadCombinationNames { get; }

        /// <summary>
        /// Show the error information while contact with revit
        /// </summary>
        public string ErrorInformation { get; set; }

        /// <summary>
        /// Used as the dataSource of load combination DataGridView control,
        /// and the information which support load combination creation also.
        /// </summary>
        public List<LoadCombinationMap> LoadCombinationMap { get; private set; }

        /// <summary>
        /// Store all load combination formula names
        /// </summary>
        public List<FormulaMap> FormulaMap { get; private set; }

        /// <summary>
        /// Store all load usage
        /// </summary>
        public List<UsageMap> UsageMap { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// Default constructor of Loads
        /// </summary>
        public Loads()
        {
            LoadUsageNames = new List<string>();
            LoadCombinationNames = new List<string>();
            LoadCombinationMap = new List<LoadCombinationMap>();
            LoadUsages = new List<LoadUsage>();
            FormulaMap = new List<FormulaMap>();
            UsageMap = new List<UsageMap>();

            LoadCaseCategories = new List<Category>();
            LoadCases = new List<LoadCase>();
            LoadNatures = new List<LoadNature>();
            LoadCasesMap = new List<LoadCasesMap>();
            LoadNaturesMap = new List<LoadNaturesMap>();
        }

        public Result Execute(ExternalCommandData commandData,
                                                    ref string message, ElementSet elements)
        {
            RevitApplication = commandData.Application.Application;
            var documentTransaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");
            documentTransaction.Start();
            // Initialize the helper classes.
            m_combinationDeal = new LoadCombinationDeal(this);
            LoadCasesDeal = new LoadCaseDeal(this);

            // Prepare some data for the form displaying
            PrepareData();


            // Display the form and wait for the user's operate.
            // This class give some public methods to add or delete LoadUsage and delete LoadCombination
            // The form will use these methods to add or delete dynamically.
            // If the user press cancel button, return Cancelled to roll back All the changes.
            using (var displayForm = new LoadsForm(this))
            {
                if (DialogResult.OK != displayForm.ShowDialog())
                {
                    documentTransaction.RollBack();
                    return Result.Cancelled;
                }
            }

            // If everything goes right, return succeeded.
            documentTransaction.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        /// Prepare the data for the form displaying.
        /// </summary>
        void PrepareData()
        {
            // Prepare the data of the LoadCase page on form
            LoadCasesDeal.PrepareData();

            //Prepare the data of the LoadCombination page on form
            m_combinationDeal.PrepareData();
        }

        /// <summary>
        /// Create new Load Combination
        /// </summary>
        /// <param name="name">The new Load Combination name</param>
        /// <param name="typeId">The index of new Load Combination Type</param>
        /// <param name="stateId">The index of new Load Combination State</param>
        /// <returns>true if the creation was successful; otherwise, false</returns>
        public bool NewLoadCombination(string name, int typeId, int stateId)
        {
            // In order to refresh the combination DataGridView,
            // We should do like as follow
            LoadCombinationMap = new List<LoadCombinationMap>(LoadCombinationMap);

            // Just go to run NewLoadCombination method of LoadCombinationDeal class
            return m_combinationDeal.NewLoadCombination(name, typeId, stateId);
        }

        /// <summary>
        /// Delete the selected Load Combination
        /// </summary>
        /// <param name="index">The selected index in the DataGridView</param>
        /// <returns>true if the delete operation was successful; otherwise, false</returns>
        public bool DeleteCombination(int index)
        {
            // Just go to run DeleteCombination method of LoadCombinationDeal class
            return m_combinationDeal.DeleteCombination(index);
        }

        /// <summary>
        /// Create a new load combination usage
        /// </summary>
        /// <param name="usageName">The new Load Usage name</param>
        /// <returns>true if the process is successful; otherwise, false</returns> 
        public bool NewLoadUsage(string usageName)
        {
            // In order to refresh the usage DataGridView,
            // We should do like as follow
            UsageMap = new List<UsageMap>(UsageMap);

            // Just go to run NewLoadUsage method of LoadCombinationDeal class
            return m_combinationDeal.NewLoadUsage(usageName);
        }

        /// <summary>
        /// Delete the selected Load Usage
        /// </summary>
        /// <param name="index">The selected index in the DataGridView</param>
        /// <returns>true if the delete operation was successful; otherwise, false</returns>
        public bool DeleteUsage(int index)
        {
            // Just go to run DeleteUsage method of LoadCombinationDeal class
            if (false == m_combinationDeal.DeleteUsage(index))
            {
                return false;
            }

            // In order to refresh the usage DataGridView,
            // We should do like as follow
            if (0 == UsageMap.Count)
            {
                UsageMap = new List<UsageMap>();
            }
            return true;
        }

        /// <summary>
        /// Change usage name when the user modify it on the form
        /// </summary>
        /// <param name="oldName">The name before modification</param>
        /// <param name="newName">The name after modification</param>
        /// <returns>true if the modification was successful; otherwise, false</returns>
        public bool ModifyUsageName(string oldName, string newName)
        {
            // Just go to run ModifyUsageName method of LoadCombinationDeal class
            return m_combinationDeal.ModifyUsageName(oldName, newName);
        }

        /// <summary>
        /// Add a formula when the user click Add button to new a formula
        /// </summary>
        /// <returns>true if the creation is successful; otherwise, false</returns>
        public bool AddFormula()
        {
            // Get the first member in LoadCases as the Case
            var loadCase = LoadCases[0];
            if (null == loadCase)
            {
                ErrorInformation = "Can't not find a LoadCase.";
                return false;
            }
            var caseName = loadCase.Name;

            // In order to refresh the formula DataGridView,
            // We should do like as follow
            FormulaMap = new List<FormulaMap>(FormulaMap);

            // Run AddFormula method of LoadCombinationDeal class
            return m_combinationDeal.AddFormula(caseName);
        }

        /// <summary>
        /// Delete the selected Load Formula
        /// </summary>
        /// <param name="index">The selected index in the DataGridView</param>
        /// <returns>true if the delete operation was successful; otherwise, false</returns>
        public bool DeleteFormula(int index)
        {
            // Just remove that data.
            try
            {
                FormulaMap.RemoveAt(index);
            }
            catch (Exception e)
            {
                ErrorInformation = e.ToString();
                return false;
            }
            return true;
        }
        #endregion
    }
}
