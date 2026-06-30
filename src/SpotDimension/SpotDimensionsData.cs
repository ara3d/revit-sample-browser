// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ara3D.RevitSampleBrowser.SpotDimension.CS
{
    /// <summary>
    ///     Store all the views and spot dimensions in Revit.
    /// </summary>
    public class SpotDimensionsData
    {
        private readonly UIApplication m_revit; // Store the reference of the application in Revit

        private readonly List<Autodesk.Revit.DB.SpotDimension> m_spotDimensions =
            []; //a list to store all SpotDimensions in the project

        private readonly List<string> m_views = [];

        public SpotDimensionsData(ExternalCommandData commandData)
        {
            m_revit = commandData.Application;
            GetSpotDimensions();
        }

        /// <summary>
        ///     a list of all the SpotDimensions in the project
        /// </summary>
        public ReadOnlyCollection<Autodesk.Revit.DB.SpotDimension> SpotDimensions =>
            new(m_spotDimensions);

        /// <summary>
        ///     a list of all the views that have SpotDimentions in the project
        /// </summary>
        public ReadOnlyCollection<string> Views => new(m_views);

        private void GetSpotDimensions()
        {
            var document = m_revit.ActiveUIDocument.Document;

            var elementIterator = new FilteredElementCollector(document)
                .OfClass(typeof(Autodesk.Revit.DB.SpotDimension)).GetElementIterator();
            elementIterator.Reset();

            while (elementIterator.MoveNext())
            {
                //find all the SpotDimensions and views
                if (elementIterator.Current is Autodesk.Revit.DB.SpotDimension tmpSpotDimension)
                {
                    m_spotDimensions.Add(tmpSpotDimension);
                    if (m_views.Contains(tmpSpotDimension.View.Name) == false) m_views.Add(tmpSpotDimension.View.Name);
                }
            }
        }
    }
}
