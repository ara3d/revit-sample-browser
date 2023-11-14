// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SpotDimension.CS
{
    /// <summary>
    ///     Store all the views and spot dimensions in Revit.
    /// </summary>
    public class SpotDimensionsData
    {
        private readonly UIApplication m_revit; // Store the reference of the application in Revit

        private readonly List<Autodesk.Revit.DB.SpotDimension> m_spotDimensions =
            new List<Autodesk.Revit.DB.SpotDimension>(); //a list to store all SpotDimensions in the project

        private readonly List<string> m_views = new List<string>();

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData"></param>
        public SpotDimensionsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            GetSpotDimensions();
        }

        /// <summary>
        ///     a list of all the SpotDimensions in the project
        /// </summary>
        public ReadOnlyCollection<Autodesk.Revit.DB.SpotDimension> SpotDimensions =>
            new ReadOnlyCollection<Autodesk.Revit.DB.SpotDimension>(m_spotDimensions);

        /// <summary>
        ///     a list of all the views that have SpotDimentions in the project
        /// </summary>
        public ReadOnlyCollection<string> Views => new ReadOnlyCollection<string>(m_views);

        /// <summary>
        ///     try to find all the SpotDimensions and add them to the list
        /// </summary>
        private void GetSpotDimensions()
        {
            //get the active document 
            var document = m_revit.ActiveUIDocument.Document;

            var elementIterator = new FilteredElementCollector(document)
                .OfClass(typeof(Autodesk.Revit.DB.SpotDimension)).GetElementIterator();
            elementIterator.Reset();

            while (elementIterator.MoveNext())
            {
                //find all the SpotDimensions and views
                var tmpSpotDimension = elementIterator.Current as Autodesk.Revit.DB.SpotDimension;
                if (null != tmpSpotDimension)
                {
                    m_spotDimensions.Add(tmpSpotDimension);
                    if (m_views.Contains(tmpSpotDimension.View.Name) == false) m_views.Add(tmpSpotDimension.View.Name);
                }
            }
        }
    }
}
