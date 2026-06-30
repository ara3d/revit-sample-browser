// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.BeamAndSlabNewParameter.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        private ElementSet m_elements; // correspond to elements parameter in Execute(...)
        private UIApplication m_revit; // application of Revit

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            m_revit = revit.Application;
            m_elements = elements;

            Transaction tran = new(m_revit.ActiveUIDocument.Document, "BeamAndSlabNewParameter");
            tran.Start();

            using (BeamAndSlabParametersForm displayForm = new(this))
            {
                displayForm.ShowDialog();
            }

            tran.Commit();
            return Result.Succeeded;
        }

        public bool SetNewParameterToBeamsAndSlabs()
        {
            // via the private method AccessOrCreateExternalSharedParameterFile
            var informationFile = AccessOrCreateExternalSharedParameterFile();

            if (null == informationFile) return false;

            var informationCollections = informationFile.Groups;

            var informationCollection = informationCollections.get_Item("MyParameters");

            if (null == informationCollection)
            {
                informationCollections.Create("MyParameters");
                informationCollection = informationCollections.get_Item("MyParameters");
            }

            // belongs to a specific group
            var information = informationCollection.Definitions.get_Item("Unique ID");

            if (null == information)
            {
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions =
                    new("Unique ID", SpecTypeId.String.Text);
                informationCollection.Definitions.Create(externalDefinitionCreationOptions);
                information = informationCollection.Definitions.get_Item("Unique ID");
            }

            var categories = m_revit.Application.Create.NewCategorySet();

            // use category in instead of the string name to get category 
            var structuralFramingCategorie =
                m_revit.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming);
            var floorsClassification =
                m_revit.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
            categories.Insert(structuralFramingCategorie);
            categories.Insert(floorsClassification);

            var caseTying = m_revit.Application.Create.NewInstanceBinding(categories);

            var boundResult = m_revit.ActiveUIDocument.Document.ParameterBindings.Insert(information, caseTying);

            return boundResult;
        }

        public void SetValueToUniqueIdParameter()
        {
            ElementClassFilter beamClassFilter = new(typeof(FamilyInstance));
            ElementClassFilter slabClassFilter = new(typeof(Floor));
            ElementCategoryFilter beamTypeFilter = new(BuiltInCategory.OST_StructuralFraming);
            ElementCategoryFilter slabTypeFilter = new(BuiltInCategory.OST_Floors);

            LogicalAndFilter beamFilter = new(beamClassFilter, beamTypeFilter);
            LogicalAndFilter slabFilter = new(slabClassFilter, slabTypeFilter);

            LogicalOrFilter beamandslabFilter = new(beamFilter, slabFilter);
            var elems = from elem in
                    new FilteredElementCollector(m_revit.ActiveUIDocument.Document).WherePasses(beamandslabFilter)
                        .ToElements()
                                         select elem;

            foreach (var elem in elems)
            {
                // belongs to a specifically beam or slab
                var attributes = elem.Parameters;
                var iter = attributes.GetEnumerator();

                iter.Reset();
                while (iter.MoveNext())
                {
                    var attribute = iter.Current as Parameter;
                    var information = attribute.Definition;

                    if (null != information && "Unique ID" == information.Name && null == attribute.AsString())
                    {
                        var uuid = Guid.NewGuid();
                        attribute.Set(uuid.ToString());
                    }
                }
            }
        }

        public ArrayList SendValueToListBox()
        {
            ElementSet elements = new();
            foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
            {
                elements.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            var i = elements.GetEnumerator();

            ArrayList parameterValueArrangeBox = [];

            i.Reset();
            var moreElements = i.MoveNext();

            while (moreElements)
            {

                if (i.Current is not Element component)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (null == component.Category)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (component.Category.Name is not "Structural Framing" and
                    not "Floors")
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                // Get "Unique ID" parameter and display its value in a list box 
                var attributes = component.Parameters;

                foreach (var o in attributes)
                {
                    var attribute = o as Parameter;

                    if ("Unique ID" == attribute.Definition.Name)
                    {
                        if (null == attribute.AsString()) break;

                        parameterValueArrangeBox.Add(attribute.AsString());
                        break;
                    }
                }

                moreElements = i.MoveNext();
            }

            return parameterValueArrangeBox;
        }

        public void FindElement(string uniqueIdValue)
        {
            ElementSet seleElements = new();
            foreach (var elementId in m_revit.ActiveUIDocument.Selection.GetElementIds())
            {
                seleElements.Insert(m_revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            var i = seleElements.GetEnumerator();

            // find out the element using the select value for display
            i.Reset();
            var moreElements = i.MoveNext();

            while (moreElements)
            {

                if (i.Current is not Element component)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (null == component.Category)
                {
                    moreElements = i.MoveNext();
                    continue;
                }

                if (component.Category.Name is not "Structural Framing" and
                    not "Floors")
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
                        if (null == attribute.AsString()) break;

                        // So this element will highlight in Revit UI
                        if (uniqueIdValue == attribute.AsString())
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

        private DefinitionFile AccessOrCreateExternalSharedParameterFile()
        {
            var currentExecutablePath = Application.ExecutablePath;

            var sharedParameterFile = Path.GetDirectoryName(currentExecutablePath);

            sharedParameterFile += "\\MySharedParameters.txt";


            FileInfo documentMessage = new(sharedParameterFile);
            var fileExist = documentMessage.Exists;

            if (!fileExist)
            {
                var fileFlow = File.Create(sharedParameterFile);
                fileFlow.Close();
            }

            m_revit.Application.SharedParametersFilename = sharedParameterFile;
            var informationFile = m_revit.Application.OpenSharedParameterFile();

            return informationFile;
        }
    }
}
