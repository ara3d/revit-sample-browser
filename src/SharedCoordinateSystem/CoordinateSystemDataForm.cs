// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Units;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;
using Form = System.Windows.Forms.Form;
using FormatValueType = Ara3D.RevitSampleBrowser.Common.Infrastructure.ValueType;
namespace Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS
{
    /// <summary>
    ///     coordinate system data form
    /// </summary>
    public partial class CoordinateSystemDataForm : Form
    {
        private const int DecimalNumber = 3; //number of decimal
        private string m_angle; //the value of angle
        private CityInfo m_currentCityInfo; //current CityInfo
        private string m_currentName; //the current project location's name;
        private readonly CoordinateSystemData m_data; //the reference of the CoordinateSystemData class
        private string m_eastWest; //the value of the east to west offset
        private string m_elevation; //the value of the elevation from ground level
        private bool m_isFormLoading = true; //indicate whether called when Form loading
        private bool m_isLatitudeChanged; //indicate whether user change Latitude value
        private bool m_isLongitudeChanged; //indicate whether user change Longitude value
        private string m_northSouth; //the value of the north to south offset

        private readonly PlaceInfo m_placeInfo; //store all cities' information
        private readonly SiteLocation m_siteLocation; //reference to SiteLocation

        private CoordinateSystemDataForm()
        {
            InitializeComponent();
        }

        public CoordinateSystemDataForm(CoordinateSystemData data, CitySet citySet, SiteLocation siteLocation)
        {
            m_data = data;
            m_currentName = null;

            m_placeInfo = new PlaceInfo(citySet);
            m_siteLocation = siteLocation;
            m_currentCityInfo = new CityInfo();
            InitializeComponent();
        }

        public string NewLocationName { get; set; }

        private void DisplayInformation()
        {
            //initialize the listbox
            locationListBox.Items.Clear();
            foreach (var itemName in m_data.LocationNames)
            {
                if (itemName == m_data.LocationName)
                {
                    m_currentName = $"{itemName} (current)"; //indicate the current project location
                    locationListBox.Items.Add(m_currentName);
                }
                else
                {
                    locationListBox.Items.Add(itemName);
                }
            }

            for (var i = 0; i < locationListBox.Items.Count; i++)
            {
                var itemName = locationListBox.Items[i].ToString();
                if (itemName.Contains("(current)")) locationListBox.SelectedIndex = i;
            }

            //get the offset values of the selected item 
            var selecteName = locationListBox.SelectedItem.ToString();
            m_data.GetOffset(selecteName);
            ShowOffsetValue();

            //convert values get from API and set them to controls
            CityInfo cityInfo = new(m_siteLocation.Latitude, m_siteLocation.Longitude);
            var cityInfoString = ValueFormatting.ConvertFrom(cityInfo);

            latitudeTextBox.Text = cityInfoString.Latitude;
            longitudeTextBox.Text = cityInfoString.Longitude;

            cityNameComboBox.DataSource = m_placeInfo.CitiesName;
            timeZoneComboBox.DataSource = m_placeInfo.TimeZones;

            //try use Method DoTextBoxChanged to Set CitiesName ComboBox
            DoTextBoxChanged();
            m_isFormLoading = false;

            var timeZoneString = m_placeInfo.TryGetTimeZoneString(m_siteLocation.TimeZone);

            timeZoneComboBox.SelectedItem = timeZoneString;
            timeZoneComboBox.Enabled = false;
        }

        private void CoordinateSystemDataForm_Load(object sender, EventArgs e)
        {
            DisplayInformation();

            CheckSelecteCurrent();
        }

        private void duplicateButton_Click(object sender, EventArgs e)
        {
            using (DuplicateForm duplicateForm = new(m_data,
                       this,
                       locationListBox.SelectedItem.ToString()))
            {
                if (DialogResult.OK != duplicateForm.ShowDialog()) return;
            }

            //refresh the form
            locationListBox.Items.Clear();
            m_data.GatData();
            DisplayInformation();

            //make the new project location is the selected item after it was duplicated
            for (var i = 0; i < locationListBox.Items.Count; i++)
                if (NewLocationName == locationListBox.Items[i].ToString())
                    locationListBox.SelectedIndex = i;
        }

        private void CheckSelecteCurrent()
        {
            makeCurrentButton.Enabled = locationListBox.SelectedItem.ToString() != m_currentName;
            //get the offset values of the selected item 
            var selecteName = locationListBox.SelectedItem.ToString();
            m_data.GetOffset(selecteName);
            ShowOffsetValue();
        }

        private void ShowOffsetValue()
        {
            //show the angle value
            var degree = (char)0xb0;
            angleTextBox.Text = m_data.AngleOffset.ToString() + degree;
            m_angle = m_data.AngleOffset.ToString();

            //show the value of the east to west offset
            eatWestTextBox.Text = m_data.EastWestOffset.ToString();
            m_eastWest = m_data.EastWestOffset.ToString();

            //show the value of the north to south offset
            northSouthTextBox.Text = m_data.NorthSouthOffset.ToString();
            m_northSouth = m_data.NorthSouthOffset.ToString();

            //show the value of the elevation
            elevationTextBox.Text = m_data.PositionElevation.ToString();
            m_elevation = m_data.PositionElevation.ToString();
        }

        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelecteCurrent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!CheckModify()) return;
            SaveSiteLocation();
            DialogResult = DialogResult.OK;
            Close(); // close the form
        }

        private void makeCurrentButton_Click(object sender, EventArgs e)
        {
            var selectIndex = locationListBox.SelectedIndex;
            var newCurrentName = locationListBox.SelectedItem.ToString();
            m_data.ChangeCurrentLocation(newCurrentName);
            //refresh the form
            DisplayInformation();
            locationListBox.SelectedIndex = selectIndex;
        }

        private bool CheckModify()
        {
            try
            {
                if (m_angle != angleTextBox.Text ||
                    m_eastWest != eatWestTextBox.Text ||
                    m_northSouth != northSouthTextBox.Text ||
                    m_elevation != elevationTextBox.Text)
                {
                    var newValue = angleTextBox.Text;
                    var degree = ((char)0xb0).ToString();
                    if (newValue.Contains(degree))
                    {
                        var index = newValue.IndexOf(degree);
                        newValue = newValue.Substring(0, index);
                    }

                    var newAngle = Convert.ToDouble(newValue);
                    var newEast = Convert.ToDouble(eatWestTextBox.Text);
                    var newNorth = Convert.ToDouble(northSouthTextBox.Text);
                    var newElevation = Convert.ToDouble(elevationTextBox.Text);
                    var positionName = locationListBox.SelectedItem.ToString();
                    m_data.EditPosition(positionName, newAngle, newEast, newNorth, newElevation);
                }
            }
            catch (FormatException)
            {
                // spacing text boxes should only input number information
                TaskDialog.Show("Revit", "Please input double number in TextBox.", TaskDialogCommonButtons.Ok);
                return false;
            }
            catch (Exception ex)
            {
                // if other unexpected error, just show the information
                TaskDialog.Show("Revit", ex.Message, TaskDialogCommonButtons.Ok);
                return false;
            }

            return true;
        }

        private void cityNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!cityNameComboBox.Focused) return;
            DoCityNameChanged();
        }

        private void timeZoneComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!timeZoneComboBox.Focused) return;
            m_currentCityInfo.TimeZone = m_placeInfo.TryGetTimeZoneNumber(
                timeZoneComboBox.SelectedItem as string);
        }

        private void latitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (latitudeTextBox.Focused) m_isLatitudeChanged = true;
        }

        private void longitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (longitudeTextBox.Focused) m_isLongitudeChanged = true;
        }

        private void latitudeTextBox_Leave(object sender, EventArgs e)
        {
            if (m_isLatitudeChanged)
            {
                DoTextBoxChanged();
                var text = DealDecimalNumber(latitudeTextBox.Text);
                latitudeTextBox.Text = text;
                m_isLatitudeChanged = false;
            }
        }

        private void longitudeTextBox_Leave(object sender, EventArgs e)
        {
            if (m_isLongitudeChanged)
            {
                DoTextBoxChanged();
                var text = DealDecimalNumber(longitudeTextBox.Text);
                longitudeTextBox.Text = text;
                m_isLongitudeChanged = false;
            }
        }

        private string DealDecimalNumber(string value)
        {
            string result;
            double doubleValue;
            //try to get double value from string
            if (!SampleBrowserUtils.StringToDouble(value, FormatValueType.Angle, out doubleValue))
            {
                var degree = ((char)0xb0).ToString();
                if (!value.Contains(degree))
                {
                    result = value + degree;
                    return result;
                }
            }

            //try to convert double into string
            result = SampleBrowserUtils.DoubleToString(doubleValue, FormatValueType.Angle);
            return result;
        }

        private void DoCityNameChanged()
        {
            //disable timezone ComboBox
            timeZoneComboBox.Enabled = false;
            CityInfoString cityInfoString = new();

            if (GetCityInfo(cityNameComboBox.SelectedItem as string, out cityInfoString))
            {
                //use new CityInfoString to set TextBox and ComboBox
                latitudeTextBox.Text = cityInfoString.Latitude;
                longitudeTextBox.Text = cityInfoString.Longitude;

                if (null != cityInfoString.TimeZone)
                {
                    timeZoneComboBox.Text = null;
                    timeZoneComboBox.SelectedItem = cityInfoString.TimeZone;
                }
                else
                {
                    timeZoneComboBox.SelectedIndex = -1;
                }
            }
            //if failed, set control with nothing 
            else
            {
                latitudeTextBox.Text = null;
                longitudeTextBox.Text = null;
                timeZoneComboBox.SelectedIndex = -1;
            }
        }

        private void DoTextBoxChanged()
        {
            //enable timezone ComboBox
            timeZoneComboBox.Enabled = true;
            CityInfoString cityInfoString = new(latitudeTextBox.Text, longitudeTextBox.Text);
            string cityName;
            string timeZone;

            GetCityNameTimeZone(cityInfoString, out cityName, out timeZone);

            //use new CityName to set ComboBox
            if (null != cityName)
            {
                cityNameComboBox.Text = null;
                cityNameComboBox.SelectedItem = cityName;
                timeZoneComboBox.Enabled = false;
            }
            else
            {
                if (m_isFormLoading)
                {
                    var userDefinedCity = "User Defined\r";
                    if (!m_placeInfo.CitiesName.Contains(userDefinedCity))
                    {
                        cityNameComboBox.DataSource = null;
                        m_placeInfo.CitiesName.Add(userDefinedCity);
                        m_placeInfo.CitiesName.Sort();
                        cityNameComboBox.DataSource = m_placeInfo.CitiesName;
                        var cityInfo = ValueFormatting.ConvertTo(cityInfoString);
                        cityInfo.CityName = userDefinedCity;
                        cityInfo.TimeZone = m_siteLocation.TimeZone;
                        m_placeInfo.AddCityInfo(cityInfo);
                    }

                    cityNameComboBox.SelectedItem = userDefinedCity;
                }
                else
                {
                    cityNameComboBox.SelectedIndex = -1;
                }
            }

            //after get timeZone,set control timeZonesComboBox
            if (null != timeZone)
            {
                timeZoneComboBox.Text = null;
                timeZoneComboBox.SelectedItem = timeZone;
            }
        }

        private void GetCityNameTimeZone(CityInfoString cityInfoString,
            out string cityName, out string timeZone)
        {
            var cityInfo = ValueFormatting.ConvertTo(cityInfoString);
            string tempName;
            double tempTime;

            //try to get city name and timezone according to cityInfo
            if (m_placeInfo.TryGetCityNameTimeZone(cityInfo, out tempName, out tempTime))
            {
                cityName = tempName;

                //try to get string representing timezone according to a number 
                timeZone = m_placeInfo.TryGetTimeZoneString(tempTime);

                m_currentCityInfo.Latitude = cityInfo.Latitude;
                m_currentCityInfo.Longitude = cityInfo.Longitude;
                m_currentCityInfo.TimeZone = tempTime;
                m_currentCityInfo.CityName = tempName;
            }
            else
            {
                cityName = null;
                timeZone = null;
                m_currentCityInfo.Latitude = cityInfo.Latitude;
                m_currentCityInfo.Longitude = cityInfo.Longitude;
                m_currentCityInfo.CityName = null;
            }
        }

        private bool GetCityInfo(string cityName, out CityInfoString cityInfoString)
        {
            CityInfo cityInfo = new();

            //try to get CityInfo according to cityName
            if (m_placeInfo.TryGetCityInfo(cityName, out cityInfo))
            {
                //do conversion from CityInfo to CityInfoString
                cityInfoString = ValueFormatting.ConvertFrom(cityInfo);

                //do TimeZone conversion from double to string
                cityInfoString.TimeZone = m_placeInfo.TryGetTimeZoneString(cityInfo.TimeZone);

                m_currentCityInfo = cityInfo;
                m_currentCityInfo.CityName = cityName;
                return true;
            }

            //if failed, also set current CityInfo            
            m_currentCityInfo.CityName = null;
            cityInfoString = new CityInfoString();
            return false;
        }

        private void SaveSiteLocation()
        {
            if (null == m_siteLocation) return;

            //change SiteLocation of Revit          
            m_siteLocation.Latitude = m_currentCityInfo.Latitude;
            m_siteLocation.Longitude = m_currentCityInfo.Longitude;
            m_siteLocation.TimeZone = m_currentCityInfo.TimeZone;
        }

        private void angleTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                //and check whether the user's input is number 
                var degree = ((char)0xb0).ToString();
                if (!angleTextBox.Text.Contains(degree))
                {
                    angleTextBox.AppendText(degree);
                }
                else
                {
                    var tempName = angleTextBox.Text;
                    var index = tempName.IndexOf(degree);
                    tempName = tempName.Substring(0, index);
                }
            }
            catch (FormatException)
            {
                //angle text boxes should only input number information
                TaskDialog.Show("Revit", "Please input double number in TextBox.", TaskDialogCommonButtons.Ok);
            }
            catch (Exception ex)
            {
                // if other unexpected error, just show the information
                TaskDialog.Show("Revit", ex.Message, TaskDialogCommonButtons.Ok);
            }
        }
    }
}
