// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace RevitMultiSample.SharedCoordinateSystem.CS
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

        /// <summary>
        ///     constructor of form
        /// </summary>
        private CoordinateSystemDataForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     override constructor
        /// </summary>
        /// <param name="data">a instance of CoordinateSystemData class</param>
        public CoordinateSystemDataForm(CoordinateSystemData data, CitySet citySet, SiteLocation siteLocation)
        {
            m_data = data;
            m_currentName = null;

            //create new members about place information
            m_placeInfo = new PlaceInfo(citySet);
            m_siteLocation = siteLocation;
            m_currentCityInfo = new CityInfo();
            InitializeComponent();
        }

        /// <summary>
        ///     get and set the new location's name
        /// </summary>
        public string NewLocationName { get; set; }

        /// <summary>
        ///     display the location information on the form
        /// </summary>
        private void DisplayInformation()
        {
            //initialize the listbox
            locationListBox.Items.Clear();
            foreach (var itemName in m_data.LocationNames)
                if (itemName == m_data.LocationName)
                {
                    m_currentName = itemName + " (current)"; //indicate the current project location
                    locationListBox.Items.Add(m_currentName);
                }
                else
                {
                    locationListBox.Items.Add(itemName);
                }

            //set the selected item to current location
            for (var i = 0; i < locationListBox.Items.Count; i++)
            {
                var itemName = locationListBox.Items[i].ToString();
                if (itemName.Contains("(current)")) locationListBox.SelectedIndex = i;
            }

            //get the offset values of the selected item 
            var selecteName = locationListBox.SelectedItem.ToString();
            m_data.GetOffset(selecteName);
            ShowOffsetValue();

            //set control in placeTabPage
            //convert values get from API and set them to controls
            var cityInfo = new CityInfo(m_siteLocation.Latitude, m_siteLocation.Longitude);
            var cityInfoString = UnitConversion.ConvertFrom(cityInfo);

            //set Text of Latitude and Longitude TextBox
            latitudeTextBox.Text = cityInfoString.Latitude;
            longitudeTextBox.Text = cityInfoString.Longitude;

            //set DataSource of CitiesName ComboBox and TimeZones ComboBox
            cityNameComboBox.DataSource = m_placeInfo.CitiesName;
            timeZoneComboBox.DataSource = m_placeInfo.TimeZones;

            //try use Method DoTextBoxChanged to Set CitiesName ComboBox
            DoTextBoxChanged();
            m_isFormLoading = false;

            //get timezone from double value and set control
            var timeZoneString = m_placeInfo.TryGetTimeZoneString(m_siteLocation.TimeZone);

            //set selectItem of TimeZones ComboBox
            timeZoneComboBox.SelectedItem = timeZoneString;
            timeZoneComboBox.Enabled = false;
        }

        /// <summary>
        ///     load the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoordinateSystemDataForm_Load(object sender, EventArgs e)
        {
            DisplayInformation();

            CheckSelecteCurrent();
        }

        /// <summary>
        ///     display the duplicate form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateButton_Click(object sender, EventArgs e)
        {
            using (var duplicateForm = new DuplicateForm(m_data,
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

        /// <summary>
        ///     when the selected item is the current location,make the button to disable
        /// </summary>
        private void CheckSelecteCurrent()
        {
            if (locationListBox.SelectedItem.ToString() == m_currentName)
                makeCurrentButton.Enabled = false;
            else
                makeCurrentButton.Enabled = true;
            //get the offset values of the selected item 
            var selecteName = locationListBox.SelectedItem.ToString();
            m_data.GetOffset(selecteName);
            ShowOffsetValue();
        }

        /// <summary>
        ///     show the offset values on the form
        /// </summary>
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

        /// <summary>
        ///     the function will be invoked when the selected item changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelecteCurrent();
        }

        /// <summary>
        ///     close the form and return true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!CheckModify()) return;
            SaveSiteLocation();
            DialogResult = DialogResult.OK; // set dialog result
            Close(); // close the form
        }

        /// <summary>
        ///     set the selected item of the listbox to be the current project location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void makeCurrentButton_Click(object sender, EventArgs e)
        {
            var selectIndex = locationListBox.SelectedIndex; //get selected index
            var newCurrentName = locationListBox.SelectedItem.ToString(); //get location name
            m_data.ChangeCurrentLocation(newCurrentName);
            //refresh the form
            DisplayInformation();
            locationListBox.SelectedIndex = selectIndex;
        }

        /// <summary>
        ///     check whether user modify the offset value
        /// </summary>
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

        /// <summary>
        ///     be invoked when SelectedValue of control cityNameComboBox changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cityNameComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //check whether is Focused
            if (!cityNameComboBox.Focused) return;
            DoCityNameChanged();
        }

        /// <summary>
        ///     be invoked when SelectValue of control timeZoneComboBox changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timeZoneComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            //check whether is Focused
            if (!timeZoneComboBox.Focused) return;
            m_currentCityInfo.TimeZone = m_placeInfo.TryGetTimeZoneNumber(
                timeZoneComboBox.SelectedItem as string);
        }

        /// <summary>
        ///     be invoked when text changed in control latitudeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void latitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (latitudeTextBox.Focused) m_isLatitudeChanged = true;
        }

        /// <summary>
        ///     be invoked when text changed in control longitudeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void longitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (longitudeTextBox.Focused) m_isLongitudeChanged = true;
        }

        /// <summary>
        ///     be invoked when focus leave control latitudeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     be invoked when focus leave control longitudeTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        ///     deal with decimal number
        /// </summary>
        /// <param name="value">string wanted to deal with</param>
        /// <returns>result dealing with</returns>
        private string DealDecimalNumber(string value)
        {
            string result;
            double doubleValue;
            //try to get double value from string
            if (!UnitConversion.StringToDouble(value, ValueType.Angle, out doubleValue))
            {
                var degree = ((char)0xb0).ToString();
                if (!value.Contains(degree))
                {
                    result = value + degree;
                    return result;
                }
            }

            //try to convert double into string
            result = UnitConversion.DoubleToString(doubleValue, ValueType.Angle);
            return result;
        }

        /// <summary>
        ///     call by CitiesNameSelectedChanged,when CitiesName ComboBox selected changed
        /// </summary>
        private void DoCityNameChanged()
        {
            //disable timezone ComboBox
            timeZoneComboBox.Enabled = false;
            var cityInfoString = new CityInfoString();

            //get new CityInfoString
            if (GetCityInfo(cityNameComboBox.SelectedItem as string, out cityInfoString))
            {
                //use new CityInfoString to set TextBox and ComboBox
                latitudeTextBox.Text = cityInfoString.Latitude;
                longitudeTextBox.Text = cityInfoString.Longitude;

                //set control timeZonesComboBox
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

        /// <summary>
        ///     called by some functions to do same operation
        /// </summary>
        private void DoTextBoxChanged()
        {
            //enable timezone ComboBox
            timeZoneComboBox.Enabled = true;
            var cityInfoString = new CityInfoString(latitudeTextBox.Text, longitudeTextBox.Text);
            string cityName;
            string timeZone;

            //get new CityName and TimeZone
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
                        var cityInfo = UnitConversion.ConvertTo(cityInfoString);
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

        /// <summary>
        ///     used when city information changed
        /// </summary>
        /// <param name="cityInfoString">city information which changed</param>
        /// <param name="cityName">city name want to get according to information</param>
        /// <param name="timeZone">city time zone gotten according to information</param>
        private void GetCityNameTimeZone(CityInfoString cityInfoString,
            out string cityName, out string timeZone)
        {
            var cityInfo = UnitConversion.ConvertTo(cityInfoString);
            string tempName;
            double tempTime;

            //try to get city name and timezone according to cityInfo
            if (m_placeInfo.TryGetCityNameTimeZone(cityInfo, out tempName, out tempTime))
            {
                cityName = tempName;

                //try to get string representing timezone according to a number 
                timeZone = m_placeInfo.TryGetTimeZoneString(tempTime);

                //set current CityInfo
                m_currentCityInfo.Latitude = cityInfo.Latitude;
                m_currentCityInfo.Longitude = cityInfo.Longitude;
                m_currentCityInfo.TimeZone = tempTime;
                m_currentCityInfo.CityName = tempName;
            }
            else
            {
                //set current CityInfo
                cityName = null;
                timeZone = null;
                m_currentCityInfo.Latitude = cityInfo.Latitude;
                m_currentCityInfo.Longitude = cityInfo.Longitude;
                m_currentCityInfo.CityName = null;
            }
        }

        /// <summary>
        ///     used when city name changed
        /// </summary>
        /// <param name="cityName">city name which changed</param>
        /// <param name="cityInfoString">city information want to get according to city name</param>
        /// <returns>check whether is successful</returns>
        private bool GetCityInfo(string cityName, out CityInfoString cityInfoString)
        {
            var cityInfo = new CityInfo();

            //try to get CityInfo according to cityName
            if (m_placeInfo.TryGetCityInfo(cityName, out cityInfo))
            {
                //do conversion from CityInfo to CityInfoString
                cityInfoString = UnitConversion.ConvertFrom(cityInfo);

                //do TimeZone conversion from double to string
                cityInfoString.TimeZone = m_placeInfo.TryGetTimeZoneString(cityInfo.TimeZone);

                //set current CityInfo
                m_currentCityInfo = cityInfo;
                m_currentCityInfo.CityName = cityName;
                return true;
            }

            //if failed, also set current CityInfo            
            m_currentCityInfo.CityName = null;
            cityInfoString = new CityInfoString();
            return false;
        }

        /// <summary>
        ///     save siteLocation to Revit
        /// </summary>
        private void SaveSiteLocation()
        {
            if (null == m_siteLocation) return;

            //change SiteLocation of Revit          
            m_siteLocation.Latitude = m_currentCityInfo.Latitude;
            m_siteLocation.Longitude = m_currentCityInfo.Longitude;
            m_siteLocation.TimeZone = m_currentCityInfo.TimeZone;
        }

        /// <summary>
        ///     check the format of the user's input and add a degree symbol behind the angle value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void angleTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                //check is there any symbol exist in the behind of the value
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
