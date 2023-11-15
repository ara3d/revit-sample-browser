// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.VisibilityControl.CS
{
    /// <summary>
    ///     An enumerate type listing the types of element select mode.
    /// </summary>
    public enum IsolateMode
    {
        None,
        PickOne,
        WindowSelect
    }

    /// <summary>
    ///     An object control visibility by category
    /// </summary>
    public class VisibilityCtrl
    {
        private readonly Hashtable m_categoriesWithName; // all categories with its name
        private readonly UIDocument m_document; // the active document

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <remarks>
        ///     get all categories of this document. check its visibility for
        ///     the active document, and initialize a hashtable.
        /// </remarks>
        public VisibilityCtrl(UIDocument document)
        {
            if (null == document)
                throw new ArgumentNullException(nameof(document));
            m_document = document;

            // initialize the two table
            AllCategories = new Hashtable();
            m_categoriesWithName = new Hashtable();

            // fill out the two table
            foreach (Category category in m_document.Document.Settings.Categories)
                if (category.get_AllowsVisibilityControl(m_document.Document.ActiveView))
                {
                    AllCategories.Add(category.Name, category.get_Visible(m_document.Document.ActiveView));
                    m_categoriesWithName.Add(category.Name, category);
                }
        }

        /// <summary>
        ///     get all categories name with its visibility
        /// </summary>
        public Hashtable AllCategories { get; }

        /// <summary>
        ///     get and set the mode to select element(s)
        /// </summary>
        public IsolateMode IsolateMode { get; set; }

        /// <summary>
        ///     Set the visibility for the active view
        /// </summary>
        /// <returns>Return true if operation successed, or else, return false.</returns>
        public bool SetVisibility(bool visible, string name)
        {
            try
            {
                var cat = m_categoriesWithName[name] as Category;
                m_document.Document.ActiveView.SetCategoryHidden(cat.Id, !visible);
                //or cat.set_Visible(m_document.ActiveView, visible);
                AllCategories[cat.Name] = visible;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Isolate elements with the same categories that the selected elements belong to
        ///     using PickOne or WindowSelect.
        /// </summary>
        public void Isolate()
        {
            //m_document.Selection.Elements.Clear();
            ICollection<Reference> elements = null;
            switch (IsolateMode)
            {
                case IsolateMode.PickOne:
                    // One more element will be added to modscope 
                    // if user really selects an element which differs from the elements in 
                    // modscope before user's pick one operation.
                    elements.Add(m_document.Selection.PickObject(ObjectType.Element));
                    break;
                case IsolateMode.WindowSelect:
                    // Elements will be added to modscope if possilbe.
                    elements = m_document.Selection.PickObjects(ObjectType.Element);
                    break;
            }

            // ElementSet elements = m_document.Selection.Elements;

            // hide all categories elements
            foreach (Category cat in m_document.Document.Settings.Categories) SetVisibility(false, cat.Name);

            // set the visibility for the selection elements
            foreach (var reference in elements)
            {
                var cat = m_document.Document.GetElement(reference).Category;
                if (null != cat && !string.IsNullOrEmpty(cat.Name)) SetVisibility(true, cat.Name);
            }
        }
    }
}
