// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.ComponentModel;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.PathReinforcement.CS
{
    /// <summary>
    ///     path reinforcement layout rules parameters.
    /// </summary>
    public enum LayoutRule
    {
        /// <summary>
        ///     fixed number layout rule.
        /// </summary>
        FixedNumber = 2,

        /// <summary>
        ///     maximum spacing layout rule.
        /// </summary>
        MaximumSpacing = 3
    }

    /// <summary>
    ///     path reinforcement face parameters.
    /// </summary>
    public enum Face
    {
        /// <summary>
        ///     floor top or wall exterior.
        /// </summary>
        Top = 0,

        /// <summary>
        ///     floor bottom or wall interior.
        /// </summary>
        Bottom
    }

    /// <summary>
    ///     This class used as PropertyGrid.SelectedObject.
    ///     It stores parameters of path reinforcement.
    /// </summary>
    [DefaultProperty("NumberOfBars")]
    public class PathReinProperties
    {
        /// <summary>
        ///     update selected object of Property grid after enable / disable some properties.
        /// </summary>
        public delegate void UpdateSelectObjEventHandler();

        /// <summary>
        ///     bar spacing
        /// </summary>
        private string m_barSpacing;

        /// <summary>
        ///     face parameter
        /// </summary>
        private Face m_face;

        /// <summary>
        ///     layout rule
        /// </summary>
        private LayoutRule m_layoutRule;

        /// <summary>
        ///     number of bars
        /// </summary>
        private int m_numberOfBars;

        /// <summary>
        ///     cache path reinforcement object.
        /// </summary>
        protected readonly Autodesk.Revit.DB.Structure.PathReinforcement PathRein;

        /// <summary>
        ///     primary bar length
        /// </summary>
        private string m_primaryBarLength;

        /// <summary>
        ///     primary bar type
        /// </summary>
        private ElementId m_primaryBarType;

        /// <summary>
        ///     constructor of class
        /// </summary>
        /// <param name="pathRein"></param>
        public PathReinProperties(Autodesk.Revit.DB.Structure.PathReinforcement pathRein)
        {
            PathRein = pathRein;
            m_layoutRule = (LayoutRule)GetParameter("Layout Rule").AsInteger();
            m_face = (Face)GetParameter("Face").AsInteger();
            m_numberOfBars = PathRein.get_Parameter(
                BuiltInParameter.PATH_REIN_NUMBER_OF_BARS).AsInteger();
            m_barSpacing = PathRein.get_Parameter(
                BuiltInParameter.PATH_REIN_SPACING).AsValueString();
            m_primaryBarLength = PathRein.get_Parameter(
                BuiltInParameter.PATH_REIN_LENGTH_1).AsValueString();
            m_primaryBarType = GetParameter("Primary Bar - Type").AsElementId();
        }

        /// <summary>
        ///     Bar numbers of path reinforcement,read only property.
        ///     the property is read-only by default
        /// </summary>
        [Category("Layers")]
        [DisplayName("Number of Bars")]
        [ReadOnly(true)]
        public int NumberOfBars
        {
            get => m_numberOfBars;
            set => m_numberOfBars = value;
        }

        /// <summary>
        ///     Layout rule of path reinforcement,get/set property.
        /// </summary>
        [Category("Construction")]
        [DisplayName("Layout Rule")]
        [ReadOnly(false)]
        public LayoutRule LayoutRule
        {
            get => m_layoutRule;
            set
            {
                if (m_layoutRule == value) return;
                m_layoutRule = value;

                switch (m_layoutRule)
                {
                    // set BarSpacing and NumberOfBars readonly dynamically when:
                    // When set LayoutRule to "Fixed Number", BarSpacing should be read only
                    // When set to "Maximum Spacing", Number Of Bars should be read only
                    case LayoutRule.FixedNumber:
                        SetPropertyReadOnly("BarSpacing", true);
                        SetPropertyReadOnly("NumberOfBars", false);
                        break;
                    case LayoutRule.MaximumSpacing:
                        SetPropertyReadOnly("BarSpacing", false);
                        SetPropertyReadOnly("NumberOfBars", true);
                        break;
                }

                // update the selected object is necessary
                UpdateSelectObjEvent?.Invoke();
            }
        }

        /// <summary>
        ///     Bar spacing of path reinforcement,get/set property.
        /// </summary>
        [Category("Layers")]
        [DisplayName("Bar Spacing")]
        [ReadOnly(false)]
        public string BarSpacing
        {
            get => m_barSpacing;
            set
            {
                if (!ValidateInch(value)) throw new Exception("Invalid value.");
                m_barSpacing = value;
            }
        }

        /// <summary>
        ///     Primary bar type of path reinforcement,get/set property.
        /// </summary>
        [Category("Layers")]
        [TypeConverter(typeof(BartypeConverter))]
        [DisplayName("Primary Bar - Type")]
        public ElementId PrimaryBarType
        {
            get => m_primaryBarType;
            set => m_primaryBarType = value;
        }

        /// <summary>
        ///     Primary bar length of path reinforcement,get/set property.
        /// </summary>
        [Category("Layers")]
        [DisplayName("Primary Bar - Length")]
        public string PrimaryBarLength
        {
            get => m_primaryBarLength;
            set
            {
                if (!ValidateInch(value)) throw new Exception("Invalid value.");
                m_primaryBarLength = value;
            }
        }

        /// <summary>
        ///     Face of path reinforcement, get/set property.
        /// </summary>
        [Category("Layers")]
        [DisplayName("Face")]
        public Face Face
        {
            get => m_face;
            set => m_face = value;
        }

        /// <summary>
        ///     my event object of updating event
        /// </summary>
        public event UpdateSelectObjEventHandler UpdateSelectObjEvent;

        /// <summary>
        ///     update the parameters of path reinforcement.
        /// </summary>
        public void Update()
        {
            try
            {
                GetParameter("Layout Rule").Set((int)m_layoutRule);
                GetParameter("Face").Set((int)m_face);
                GetParameter("Primary Bar - Type").Set(m_primaryBarType);
                PathRein.get_Parameter(
                    BuiltInParameter.PATH_REIN_LENGTH_1).SetValueString(m_primaryBarLength);

                switch (m_layoutRule)
                {
                    // if layout rule is maximum spacing, number of bar will be read only.
                    // In order to update previously modified number of bar, we should change the layout rule
                    // to fixed number, and then set the layout rule back.
                    case LayoutRule.MaximumSpacing:
                        PathRein.get_Parameter(
                            BuiltInParameter.PATH_REIN_SPACING).SetValueString(m_barSpacing);
                        GetParameter("Layout Rule").Set((int)LayoutRule.FixedNumber);
                        PathRein.get_Parameter(
                            BuiltInParameter.PATH_REIN_NUMBER_OF_BARS).Set(m_numberOfBars);
                        GetParameter("Layout Rule").Set((int)m_layoutRule);
                        break;
                    // if layout rule is fixed number, bar spacing will be read only.
                    // In order to update previously modified bar spacing, we should change the layout rule 
                    // to maximum spacing, and then set the layout rule back.
                    case LayoutRule.FixedNumber:
                        PathRein.get_Parameter(
                            BuiltInParameter.PATH_REIN_NUMBER_OF_BARS).Set(m_numberOfBars);
                        GetParameter("Layout Rule").Set((int)LayoutRule.MaximumSpacing);
                        PathRein.get_Parameter(
                            BuiltInParameter.PATH_REIN_SPACING).SetValueString(m_barSpacing);
                        GetParameter("Layout Rule").Set((int)m_layoutRule);
                        break;
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.ToString());
            }
        }

        /// <summary>
        ///     Get parameter by given name.
        /// </summary>
        /// <param name="name">name of parameter</param>
        /// <returns>parameter whose definition name is the given name.</returns>
        protected Parameter GetParameter(string name)
        {
            foreach (Parameter para in PathRein.Parameters)
            {
                if (para.Definition.Name.Equals(name))
                    return para;
            }

            return null;
        }

        /// <summary>
        ///     set some properties to read only or not
        /// </summary>
        /// <param name="propertyName">name of property</param>
        /// <param name="readOnly">read only or not</param>
        private void SetPropertyReadOnly(string propertyName, bool readOnly)
        {
            var strEx = string.Empty;
            try
            {
                var type = typeof(ReadOnlyAttribute);
                var props = TypeDescriptor.GetProperties(this);
                var attrs = props[propertyName].Attributes;
                var fld = type.GetField("isReadOnly",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                fld.SetValue(attrs[type], readOnly);
            }
            catch (ArgumentException ex)
            {
                strEx = ex.ToString();
            }
            catch (FieldAccessException ex)
            {
                strEx = ex.ToString();
            }
            catch (TargetException ex)
            {
                strEx = ex.ToString();
            }
            catch (Exception ex)
            {
                strEx = ex.ToString();
            }

            // exception occurs?
            if (strEx != string.Empty) throw new Exception(strEx);
        }

        /// <summary>
        ///     validate inch like this 124'- 9".
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>true if input string is a valid inch.</returns>
        private bool ValidateInch(string input)
        {
            // check whether the input string is a valid double value.
            // if it's true, return directly, otherwise parse the string.
            try
            {
                double.Parse(input);
                return true;
            }
            catch (Exception)
            {
                // reasonable exception.
                // continue to parse the string in below lines if exception occurs.
            }

            var inputTrim = input.Trim();
            var number = 0; // number [0, 9].
            var sQuotation = 0; // single quotation mark.
            var dQuotation = 0; // double quotation mark.
            var hLine = 0; // char '-' mark

            foreach (var ch in inputTrim)
            {
                if (dQuotation > 0) return false;

                if ('0' <= ch && ch <= '9')
                {
                    number++;
                }
                else switch (ch)
                {
                    case '\'' when sQuotation > 0 || number == 0:
                        return false;
                    case '\'':
                        sQuotation++;
                        number = 0;
                        break;
                    case '\"' when dQuotation > 0 || number == 0:
                        return false;
                    case '\"':
                        dQuotation++;
                        number = 0;
                        break;
                    case '-' when hLine != 0 || sQuotation == 0 || (sQuotation != 0 && number != 0):
                        return false;
                    case '-':
                        hLine++;
                        number = 0;
                        break;
                    case ' ':
                        // skip the white space
                        break;
                    default:
                        return false;
                }
            }

            //check whether the parsed string is valid.
            var length = inputTrim.Length;
            var last = inputTrim[length - 1];
            if (dQuotation > 0 && !last.Equals('\"')) return false;

            return sQuotation <= 0 || dQuotation != 0 || last.Equals('\'');
        }
    }
}
