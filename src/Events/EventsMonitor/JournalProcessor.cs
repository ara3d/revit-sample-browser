// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    /// <summary>Supports journal replay autotest via Current.xml event selection.</summary>
    public class JournalProcessor
    {
        private readonly string m_directory;

        private List<string> m_eventsInFile;

        private readonly bool m_isReplay;

        private readonly string m_xmlFile;

        private readonly XmlSerializer m_xs;

        public JournalProcessor()
        {
            m_xs = new XmlSerializer(typeof(List<string>));
            m_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_xmlFile = Path.Combine(m_directory, "Current.xml");

            // Current.xml present means journal replay; the settings dialog is skipped.
            m_isReplay = CheckFileExistence();

            GetEventsListFromFile();
        }

        public bool IsReplay => m_isReplay;

        public List<string> EventsList => m_eventsInFile;

        private bool CheckFileExistence()
        {
            return File.Exists(m_xmlFile) ? true : false;
        }

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

        public void DumpEventsListToFile(List<string> eventList)
        {
            if (!m_isReplay)
            {
                var fileName = $"{DateTime.Now:yyyyMMdd}.xml";
                var tempFile = Path.Combine(m_directory, fileName);
                Stream stream = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                m_xs.Serialize(stream, eventList);
                stream.Close();
            }
        }

        public List<string> GetEventsListFromJournalData(IDictionary<string, string> data)
        {
            var eventList = new List<string>();
            foreach (var kvp in data)
            {
                eventList.Add(kvp.Key);
            }

            return eventList;
        }

        public void DumpEventListToJournalData(List<string> eventList, ref IDictionary<string, string> data)
        {
            foreach (var eventname in eventList)
            {
                data.Add(eventname, "1");
            }
        }
    }
}
