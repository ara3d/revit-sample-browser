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
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.BeamAndSlabNewParameter.CS
{
    /// <summary>
    /// Display how to add a parameter to an element and set value to the parameter.
    /// the class  supports the IExternalCommand interface
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        Autodesk.Revit.UI.UIApplication m_revit; // application of Revit
        ElementSet m_elements; // correspond to elements parameter in Execute(...)

        /// <summary>
        /// Implement this method as an external command for Revit.
        /// </summary>
        /// <param name="revit">An object that is passed to the external application 
        /// which contains data related to the command, 
        /// such as the application object and active view.</param>
        /// <param name="message">A message that can be set by the external application 
        /// which will be displayed if a failure or cancellation is returned by 
        /// the external command.</param>
        /// <param name="elements">A set of elements to which the external application 
        /// can add elements that are to be highlighted in case of failure or cancellation.</param>
        /// <returns>Return the status of the external command. 
        /// A result of Succeeded means that the API external method functioned as expected. 
        /// Cancelled can be used to signify that the user cancelled the external operation 
        /// at some point. Failure should be returned if the application is unable to proceed with 
        /// the operation.</returns>
        public Autodesk.Revit.UI.Result Execute(Autodesk.Revit.UI.ExternalCommandData revit, 
                                               ref string message, 
                                               ElementSet elements)
        {
            // Set currently executable application to private variable m_revit
            m_revit = revit.Application;           
            m_elements = elements;

            var tran = new Transaction(m_revit.ActiveUIDocument.Document, "BeamAndSlabNewParameter");
            tran.Start();

            // Show UI
            using (var displayForm = new BeamAndSlabParametersForm(this))
            {
                displayForm.ShowDialog();
            }

            tran.Commit();
            return Autodesk.Revit.UI.Result.Succeeded;
        }

        /// <summary>
        /// Add a new parameter, "Unique ID", to the beams and slabs
        /// The following process should be followed: 
        /// Open the shared parameters file, via the Document.OpenSharedParameterFile method. 
        /// Access an existing group or create a new group, via the DefinitionFile.Groups property. 
        /// Access an existing or create a new external parameter definition, 
        /// via the DefinitionGroup.Definitions property. 
        /// Create a new Binding with the categories to which the parameter will be bound
        /// using an InstanceBinding or a TypeBinding.
        /// Finally add the binding and definition to the document
        /// using the Document.ParameterBindings object.
        /// </summary>
        /// <returns>bool type, a value that signifies  if  add parameter was successful</returns>
        public bool SetNewParameterToBeamsAndSlabs ()
        {
            //Open the shared parameters file 
            // via the private method AccessOrCreateExternalSharedParameterFile
            var informationFile = AccessOrCreateExternalSharedParameterFile();

            if (null == informationFile)
            {
                return false;
            }
    
            // Access an existing or create a new group in the shared parameters file
            var informationCollections = informationFile.Groups;
            DefinitionGroup  informationCollection  = null;

            informationCollection = informationCollections.get_Item("MyParameters");

            if (null == informationCollection)
            {
                informationCollections.Create("MyParameters");
                informationCollection = informationCollections.get_Item("MyParameters");
            }

            // Access an existing or create a new external parameter definition 
            // belongs to a specific group
            var information = informationCollection.Definitions.get_Item("Unique ID");
                
            if (null == information)
            {
               var ExternalDefinitionCreationOptions = new ExternalDefinitionCreationOptions("Unique ID", Autodesk.Revit.DB.SpecTypeId.String.Text);
               informationCollection.Definitions.Create(ExternalDefinitionCreationOptions);
                information = informationCollection.Definitions.get_Item("Unique ID");
            }

            // Create a new Binding object with the categories to which the parameter will be bound
            var categories              = m_revit.Application.Create.NewCategorySet();
            Category structuralFramingCategorie = null;
            Category floorsClassification       = null;

            // use category in instead of the string name to get category 
            structuralFramingCategorie = m_revit.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming);
            floorsClassification = m_revit.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
            categories.Insert(structuralFramingCategorie);
            categories.Insert(floorsClassification);

            var caseTying = m_revit.Application.Create.NewInstanceBinding(categories);
            
            // Add the binding and definition to the document
            var boundResult = m_revit.ActiveUIDocument.Document.ParameterBindings.Insert(information, caseTying);        

            return boundResult;
        }

        /// <summary>
        /// Set value(uuid) to Unique ID parameter
        /// </summary>
        public void SetValueToUniqueIDParameter()
        {
            var beamClassFilter = new ElementClassFilter(typeof(FamilyInstance));
            var slabClassFilter = new ElementClassFilter(typeof(Floor));
            var beamTypeFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            var slabTypeFilter = new ElementCategoryFilter(BuiltInCategory.OST_Floors);

            var beamFilter = new LogicalAndFilter(beamClassFilter,beamTypeFilter);
            var slabFilter = new LogicalAndFilter(slabClassFilter,slabTypeFilter);

            var beamandslabFilter = new LogicalOrFilter(beamFilter, slabFilter);
            var elems = from elem in
                                             new FilteredElementCollector(m_revit.ActiveUIDocument.Document).WherePasses(beamandslabFilter).ToElements()

            select elem;

            foreach (var elem in elems)
            {
                // Find the parameter which is named "Unique ID" 
                // belongs to a specifically beam or slab
                var attributes = elem.Parameters;
                var iter = attributes.GetEnumerator();

                iter.Reset();
                while (iter.MoveNext())
                {
                    var attribute = iter.Current as Autodesk.Revit.DB.Parameter;
                    var information = attribute.Definition;

                    if ((null != information)&&("Unique ID" == information.Name) && (null == attribute.AsString()) )
                    {
                        // The shared parameter "Unique ID" then be set to a UUID
                        var uuid = Guid.NewGuid();
                        attribute.Set(uuid.ToString());
                    }
                }
            }

        }

        /// <summary>
        /// Display the value of Unique ID parameter in a list box
        /// </summary>
        /// <returns></returns>
        public System.Collections.ArrayList SendValueToListBox()
        {
           var elements = new ElementSet();
            foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
            {
               elements.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            // all the elements of current document
            var i = elements.GetEnumerator();  

            var parameterValueArrangeBox = new ArrayList();

            // if the selections include beams and slabs, find out their Unique ID's value for display
            i.Reset();
            var moreElements = i.MoveNext();

            while (moreElements)
            {
                // Get beams and slabs from selections
                var component = i.Current as Autodesk.Revit.DB.Element;

                if (null == component)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (null == component.Category)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (("Structural Framing" != component.Category.Name) &&
                    ("Floors" != component.Category.Name))
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                // Get "Unique ID" parameter and display its value in a list box 
                var attributes = component.Parameters;

                foreach(var o in attributes) 
                {
                    var attribute = o as Parameter;

                    if ("Unique ID" == attribute.Definition.Name)
                    {    
                        if (null == attribute.AsString())
                        {
                            break;
                        }

                        parameterValueArrangeBox.Add(attribute.AsString());
                        break;
                    }            
                }
                moreElements = i.MoveNext();
            }    
            return parameterValueArrangeBox;
        }

        /// <summary>
        /// found the element which using the GUID 
        /// that was assigned to the shared parameter in the shared parameters file.
        /// </summary>
        /// <param name="UniqueIdValue"></param>
        public void FindElement(string UniqueIdValue)
        {
           var seleElements = new ElementSet();
            foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
            {
               seleElements.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            // all the elements of current document
            var i = seleElements.GetEnumerator();


            // if the selections include beams and slabs, 
            // find out the element using the select value for display
            i.Reset();
            var moreElements = i.MoveNext();

            while (moreElements)
            {
                // Get beams and slabs from selections
                var component = i.Current as Autodesk.Revit.DB.Element;

                if (null == component)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (null == component.Category)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (("Structural Framing" != component.Category.Name) &&
                    ("Floors" != component.Category.Name))
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                // Get "Unique ID" parameter
                var attributes = component.Parameters;

                foreach (var o in attributes)
                {
                    var attribute = o as Parameter;

                    if ("Unique ID" == attribute.Definition.Name)
                    {
                        if (null == attribute.AsString())
                        {
                            break;
                        }

                        // compare if the parameter's value is the same as the selected value.
                        // Clear the SelElementSet and add the found element into it. 
                        // So this element will highlight in Revit UI
                        if (UniqueIdValue == attribute.AsString())
                        {
                            seleElements.Clear();
                            seleElements.Insert(component);
                            return;
                        }

                        break;
                    }
                }

                moreElements = i.MoveNext();
            }

        }

        /// <summary>
        /// Access an existing or create a new shared parameters file
        /// </summary>
        /// <returns>a shared parameters file </returns>
        private DefinitionFile  AccessOrCreateExternalSharedParameterFile()
        {    
            // The Path of Revit.exe
            var currentExecutablePath = System.Windows.Forms.Application.ExecutablePath;
 
            // The path of ourselves shared parameters file
            var sharedParameterFile = Path.GetDirectoryName(currentExecutablePath);

            sharedParameterFile = sharedParameterFile + "\\MySharedParameters.txt";
            
            //Method's return
            DefinitionFile informationFile = null; 

            // Check if the file is exit
            var documentMessage = new FileInfo(sharedParameterFile);
            var fileExist = documentMessage.Exists;

            // Create file for external shared parameter since it does not exist
            if (!fileExist)
            {
                var fileFlow = File.Create(sharedParameterFile);
                fileFlow.Close();    
            }
            
            // Set  ourselves file to  the externalSharedParameterFile 
            m_revit.Application.SharedParametersFilename = sharedParameterFile;
            informationFile = m_revit.Application.OpenSharedParameterFile();

            return informationFile;
        }
    }
}
