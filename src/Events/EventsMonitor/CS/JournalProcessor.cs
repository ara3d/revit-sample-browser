// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace RevitMultiSample.EventsMonitor.CS
{
    /// <summary>
    ///     This class is implemented to make sample autotest.
    ///     It checks whether the external file exists or not.
    ///     The sample can control the UI's display by this judgement.
    ///     It can dump the seleted event list to file or commandData.Data
    ///     and also can retrieve the list from file and commandData.Data.
    /// </summary>
    public class JournalProcessor
    {
        /// <summary>
        ///     direcotory of xml file.
        /// </summary>
        private readonly string m_directory;

        /// <summary>
        ///     events deserialized from xml file.
        /// </summary>
        private List<string> m_eventsInFile;

        /// <summary>
        ///     using UI or playing journal.
        /// </summary>
        private readonly bool m_isReplay;

        /// <summary>
        ///     xml file name.
        /// </summary>
        private readonly string m_xmlFile;

        /// <summary>
        ///     xml serializer.
        /// </summary>
        private readonly XmlSerializer m_xs;

        /// <summary>
        ///     Constructor without argument.
        /// </summary>
        public JournalProcessor()
        {
            m_xs = new XmlSerializer(typeof(List<string>));
            m_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_xmlFile = Path.Combine(m_directory, "Current.xml");

            // if the external file is exist, it means playing journal now. No Setting Dialog will pop up.
            m_isReplay = CheckFileExistence();

            // get event list from xml file.
            GetEventsListFromFile();
        }

        /// <summary>
        ///     Property to get private member variables to check the stauts is playing or recording.
        /// </summary>
        public bool IsReplay => m_isReplay;

        /// <summary>
        ///     Property to get private member variables of Event list.
        /// </summary>
        public List<string> EventsList => m_eventsInFile;

        /// <summary>
        ///     Check whether the xml file is exist or not.
        /// </summary>
        /// <returns></returns>
        private bool CheckFileExistence()
        {
            return File.Exists(m_xmlFile) ? true : false;
        }

        /// <summary>
        ///     Get the event list from xml file.
        ///     This method is used in ExternalApplication.
        /// </summary>
        private void GetEventsListFromFile()
        {
            if (m_isReplay)
            {
                Stream stream = new FileStream(m_xmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_eventsInFile = (List<string>)m_xs.Deserialize(stream);
                stream.Close();
            }
            else
            {
                m_eventsInFile = new List<string>();
            }
        }

        /// <summary>
        ///     Dump the selected event list to xml file.
        ///     This method is used in ExternalApplication.
        /// </summary>
        /// <param name="eventList"></param>
        public void DumpEventsListToFile(List<string> eventList)
        {
            if (!m_isReplay)
            {
                var fileName = DateTime.Now.ToString("yyyyMMdd") + ".xml";
                var tempFile = Path.Combine(m_directory, fileName);
                Stream stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                m_xs.Serialize(stream, eventList);
                stream.Close();
            }
        }

        /// <summary>
        ///     Get event list from commandData.Data.
        ///     This method is used in ExternalCommmand.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<string> GetEventsListFromJournalData(IDictionary<string, string> data)
        {
            var eventList = new List<string>();
            foreach (var kvp in data) eventList.Add(kvp.Key);
            return eventList;
        }

        /// <summary>
        ///     Dump the selected event list to commandData.Data.
        ///     This method is used in ExternalCommand.
        /// </summary>
        /// <param name="eventList"></param>
        /// <param name="data"></param>
        public void DumpEventListToJournalData(List<string> eventList, ref IDictionary<string, string> data)
        {
            foreach (var eventname in eventList) data.Add(eventname, "1");
        }
    }
}
