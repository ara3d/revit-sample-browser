// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    public class MyDocument
    {
        public delegate void MessageChangedHandler();


        // the length unit type for the active Revit document


        public MyDocument(ExternalCommandData commandData)
        {
            if (null != commandData.Application.ActiveUIDocument)
            {
                CommandData = commandData;
                UiDocument = CommandData.Application.ActiveUIDocument;
                Document = UiDocument.Document;
                Views = [];
                WallTypes = [];
                WallGeometry = new WallGeometry(this);
                WallCreated = false;
                GridGeometry = new GridGeometry(this);

                // get all the wall types and all the view plans
                InitializeData();

                GetLengthUnitType();

                // initialize the curtain grid operation type
                ActiveOperation = new LineOperation(LineOperationType.Waiting);
            }
        }

        // object which contains reference of Revit Application
        public ExternalCommandData CommandData { get; }

        // the active document of Revit
        public UIDocument UiDocument { get; }

        // the active document of Revit
        public Document Document { get; }

        /// <summary>
        ///     stores all the Curtain WallTypes in the active Revit document
        /// </summary>
        public List<WallType> WallTypes { get; private set; }

        public List<View> Views { get; private set; }

        public WallGeometry WallGeometry { get; }

        /// <summary>
        ///     stores the curtain wall created
        /// </summary>
        public Wall CurtainWall { get; set; }

        /// <summary>
        ///     indicates whether the curtain wall has been created
        /// </summary>
        public bool WallCreated { get; set; }

        /// <summary>
        ///     store the grid information of the created curtain wall
        /// </summary>
        public GridGeometry GridGeometry { get; }

        public LineOperation ActiveOperation { get; set; }

        public ForgeTypeId LengthUnit { get; private set; }

        public KeyValuePair<string /*msgText*/, bool /*is warningOrError*/> Message
        {
            get;
            set
            {
                field = value;
                MessageChanged?.Invoke();
            }
        }

        public event MessageChangedHandler MessageChanged;

        private void GetLengthUnitType()
        {
            var specTypeId = SpecTypeId.Length;
            var projectUnit = Document.GetUnits();
            try
            {
                var formatOption = projectUnit.GetFormatOptions(specTypeId);
                LengthUnit = formatOption.GetUnitTypeId();
            }
            catch (Exception /*e*/)
            {
                LengthUnit = UnitTypeId.Feet;
            }
        }

        /// <summary>
        ///     get all the wall types for curtain wall and all the view plans from the active document
        /// </summary>
        private void InitializeData()
        {
            // get all the wall types
            FilteredElementCollector filteredElementCollector = new(Document);
            filteredElementCollector.OfClass(typeof(WallType));
            // just get all the curtain wall type
            WallTypes = filteredElementCollector.Cast<WallType>().Where(wallType => wallType.Kind == WallKind.Curtain)
                .ToList();

            // sort them alphabetically
            WallTypeComparer wallComp = new();
            WallTypes.Sort(wallComp);

            // get all the ViewPlans
            Views = SkipTemplateViews(GetElements<View>());

            // sort them alphabetically
            ViewComparer viewComp = new();
            Views.Sort(viewComp);

            // get one of the mullion types
            var mullTypes = Document.MullionTypes;
            foreach (MullionType type in mullTypes)
            {
                if (null != type)
                {
                    var bip = BuiltInParameter.ALL_MODEL_FAMILY_NAME;
                    var para = type.get_Parameter(bip);
                    if (null != para)
                    {
                        var name = para.AsString().ToLower();
                        if (name.StartsWith("circular mullion")) GridGeometry.MullionType = type;
                    }
                }
            }
        }

        protected List<T> GetElements<T>() where T : Element
        {
            List<T> returns = new();
            FilteredElementCollector collector = new(Document);
            ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
            foreach (var elem in founds)
            {
                returns.Add(elem as T);
            }

            return returns;
        }

        private List<T> SkipTemplateViews<T>(List<T> views) where T : View
        {
            List<T> returns = new();
            foreach (View curView in views)
            {
                if (null != curView && !curView.IsTemplate)
                    returns.Add(curView as T);
            }

            return returns;
        }
    }
}
