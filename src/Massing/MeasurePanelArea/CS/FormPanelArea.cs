// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.MeasurePanelArea.CS
{
    /// <summary>
    ///     The window designed for interactive operations
    /// </summary>
    public partial class FrmPanelArea : Form
    {
        /// <summary>
        ///     Store all the divided surface selected by user or store all the divided surface in the document if user selects
        ///     nothing
        /// </summary>
        private List<DividedSurface> m_dividedSurfaceList = new List<DividedSurface>();

        /// <summary>
        ///     Record how many panels have an area larger than the maximum value
        /// </summary>
        private int m_maxCounter;

        /// <summary>
        ///     record the panel type specified by the user.
        ///     the panel with an area is greater than "m_maxValue" will be changed to this type
        /// </summary>
        private string m_maxType = "";

        /// <summary>
        ///     Record the maximum value of the desired panel area
        /// </summary>
        private double m_maxValue;

        /// <summary>
        ///     record the panel type specified by the user.
        ///     the panel with an area in the range [m_minValue, m_maxValue] will be changed to this type
        /// </summary>
        private string m_midType = "";

        /// <summary>
        ///     Record how many panels have an area smaller than the minimum value
        /// </summary>
        private int m_minCounter;

        /// <summary>
        ///     record the panel type specified by the user.
        ///     the panel with an area is smaller than "m_minValue" will be changed to this type
        /// </summary>
        private string m_minType = "";

        /// <summary>
        ///     Record the minimum value of the desired panel area
        /// </summary>
        private double m_minValue;

        /// <summary>
        ///     Record how many panels have an area in the range [m_minValue, m_maxValue]
        /// </summary>
        private int m_okCounter;

        /// <summary>
        ///     The revit application instance
        /// </summary>
        private readonly UIApplication m_uiApp;

        /// <summary>
        ///     The active Revit document
        /// </summary>
        private readonly UIDocument m_uiDoc;

        /// <summary>
        ///     A stream used to record the panel's element id and area to a text file
        /// </summary>
        private StreamWriter m_writeFile;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        public FrmPanelArea(ExternalCommandData commandData)
        {
            m_uiApp = commandData.Application;
            m_uiDoc = m_uiApp.ActiveUIDocument;

            InitializeComponent();
            BuildPanelTypeList(commandData);
        }

        /// <summary>
        ///     Handle the event triggered when user clicks the "Compute" button:
        ///     1. compute all the panel areas;
        ///     2. compare the areas with the range, mark the panels with different types;
        ///     3. record the result to a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompute_Click(object sender, EventArgs e)
        {
            m_minValue = Convert.ToDouble(txtMin.Text);
            m_maxValue = Convert.ToDouble(txtMax.Text);

            SetPanelTypesFromUi();

            var assemblyName = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyName);
            m_writeFile = new StreamWriter(assemblyDirectory + @"\" + "_PanelArea.txt");
            m_writeFile.WriteLine("Panel Element ID : Area");

            GetDividedSurfaces();

            foreach (var ds in m_dividedSurfaceList) ExamineDividedSurface(ds);
            m_writeFile.WriteLine(m_maxCounter + " panels larger than " + m_maxValue);
            m_writeFile.WriteLine(m_minCounter + " panels smaller than " + m_minValue);
            m_writeFile.WriteLine(m_okCounter + " panels within desired range");
            m_writeFile.Close();
            Close();
        }

        /// <summary>
        ///     Get names of Panel families and populate drop-down lists in the UI
        /// </summary>
        private void BuildPanelTypeList(ExternalCommandData commandData)
        {
            var list = GetElements<FamilyInstance>();
            if (list.Count == 0)
            {
                TaskDialog.Show("Revit", "There are no panel families loaded in your project");
                btnCompute.Enabled = false;
                Close();
                return;
            }

            var fs = commandData.Application.ActiveUIDocument.Document.GetElement(list[0].GetTypeId()) as FamilySymbol;
            var famDelimiter = ":";
            foreach (var famSymbolId in fs.Family.GetFamilySymbolIds())
            {
                var famSymbol = (FamilySymbol)commandData.Application.ActiveUIDocument.Document.GetElement(famSymbolId);
                cboxMax.Items.Add(fs.Family.Name + famDelimiter + famSymbol.Name);
                cboxMin.Items.Add(fs.Family.Name + famDelimiter + famSymbol.Name);
                cboxMid.Items.Add(fs.Family.Name + famDelimiter + famSymbol.Name);
            }

            cboxMax.SelectedIndex = 0;
            cboxMin.SelectedIndex = 0;
            cboxMid.SelectedIndex = 0;
        }

        /// <summary>
        ///     Analyse the panel types set by UI operation
        /// </summary>
        private void SetPanelTypesFromUi()
        {
            //Set the min, mid, and max panel types based on user selections in the UI
            var minFamilyAndType = Convert.ToString(cboxMin.Text);
            var maxFamilyAndType = Convert.ToString(cboxMax.Text);
            var midFamilyAndType = Convert.ToString(cboxMid.Text);

            var delimStr = ":";
            var delimiter = delimStr.ToCharArray();
            var split = minFamilyAndType.Split(delimiter);
            m_minType = split[1];
            split = maxFamilyAndType.Split(delimiter);
            m_maxType = split[1];
            split = midFamilyAndType.Split(delimiter);
            m_midType = split[1];
        }

        /// <summary>
        ///     Populate DividedSurfaceArray with the selected surfaces or all surfaces in the model
        /// </summary>
        private void GetDividedSurfaces()
        {
            // want to compute all the divided surfaces
            if (m_uiDoc.Selection.GetElementIds().Count == 0)
            {
                m_dividedSurfaceList = GetElements<DividedSurface>();
                return;
            }

            // user selects some divided surface
            foreach (var elementId in m_uiDoc.Selection.GetElementIds())
            {
                var element = m_uiDoc.Document.GetElement(elementId);
                if (element is DividedSurface ds) m_dividedSurfaceList.Add(ds);
            }
        }

        /// <summary>
        ///     Compute the area of the curtain panel instance
        /// </summary>
        /// <param name="familyinstance">
        ///     the curtain panel which needs to be computed
        /// </param>
        /// <returns>
        ///     the area of the curtain panel
        /// </returns>
        private double GetAreaOfTileInstance(FamilyInstance familyinstance)
        {
            var panelArea = 0d;
            var opt = m_uiApp.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            var geomElem = familyinstance.get_Geometry(opt);
            //foreach (GeometryObject geomObject1 in geomElem.Objects)
            var objects = geomElem.GetEnumerator();
            while (objects.MoveNext())
            {
                var geomObject1 = objects.Current;

                Solid solid = null;
                switch (geomObject1)
                {
                    // find area of partial border panels
                    case Solid object1:
                    {
                        solid = object1;
                        if (null == solid) continue;
                        break;
                    }
                    // find area of non-partial panels
                    case GeometryInstance geomInst:
                    {
                        //foreach (Object geomObj in geomInst.SymbolGeometry.Objects)
                        var objects1 = geomInst.SymbolGeometry.GetEnumerator();
                        while (objects1.MoveNext())
                        {
                            object geomObj = objects1.Current;

                            solid = geomObj as Solid;
                            if (solid != null)
                                break;
                        }

                        break;
                    }
                }

                if (null == solid.Faces || 0 == solid.Faces.Size) continue;

                // get the area and write the data to a text file
                foreach (Face face in solid.Faces)
                {
                    panelArea = face.Area;
                    m_writeFile.WriteLine(familyinstance.Id + " : " + panelArea);
                }
            }

            return panelArea;
        }

        /// <summary>
        ///     Check all the panels whose areas are below/above/within the range in the divided surface, mark them with different
        ///     symbols
        /// </summary>
        /// <param name="ds">
        ///     The divided surfaces created in the document, it contains the panels for checking
        /// </param>
        private void ExamineDividedSurface(DividedSurface ds)
        {
            var sym = ds.Document.GetElement(ds.GetTypeId()) as ElementType;
            FamilySymbol fsMin = null;
            FamilySymbol fsMax = null;
            FamilySymbol fsMid = null;

            // get the panel types which are used to identify the panels in the divided surface
            var fs = sym as FamilySymbol;
            foreach (var symbolId in fs.Family.GetFamilySymbolIds())
            {
                var symbol = (FamilySymbol)m_uiDoc.Document.GetElement(symbolId);
                if (symbol.Name == m_maxType) fsMax = symbol;
                if (symbol.Name == m_minType) fsMin = symbol;
                if (symbol.Name == m_midType) fsMid = symbol;
            }

            // find all the panels areas and compare with the range
            for (var u = 0; u < ds.NumberOfUGridlines; u++)
            for (var v = 0; v < ds.NumberOfVGridlines; v++)
            {
                var gn = new GridNode(u, v);
                if (false == ds.IsSeedNode(gn)) continue;

                var familyinstance = ds.GetTileFamilyInstance(gn, 0);
                if (familyinstance != null)
                {
                    var panelArea = GetAreaOfTileInstance(familyinstance);
                    // identify the panels drop in different ranges with different types
                    if (panelArea > m_maxValue)
                    {
                        familyinstance.Symbol = fsMax;
                        m_maxCounter++;
                    }
                    else if (panelArea < m_minValue)
                    {
                        familyinstance.Symbol = fsMin;
                        m_minCounter++;
                    }
                    else
                    {
                        familyinstance.Symbol = fsMid;
                        m_okCounter++;
                    }
                }
            }
        }

        protected List<T> GetElements<T>() where T : Element
        {
            var returns = new List<T>();
            var collector = new FilteredElementCollector(m_uiDoc.Document);
            ICollection<Element> founds = collector.OfClass(typeof(T)).ToElements();
            foreach (var elem in founds) returns.Add(elem as T);
            return returns;
        }
    }
}
