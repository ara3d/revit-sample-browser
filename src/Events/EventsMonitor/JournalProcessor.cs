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
        private readonly string m_xmlFile;

        private readonly XmlSerializer m_xs;

        public JournalProcessor()
        {
            m_xs = new XmlSerializer(typeof(List<string>));
            m_directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            m_xmlFile = Path.Combine(m_directory, "Current.xml");

            // Current.xml present means journal replay; the settings dialog is skipped.
            IsReplay = CheckFileExistence();

            GetEventsListFromFile();
        }

        public bool IsReplay { get; }

        public List<string> EventsList { get; private set; }

        private bool CheckFileExistence()
        {
            return File.Exists(m_xmlFile);
        }

        private void GetEventsListFromFile()
        {
            if (IsReplay)
            {
                Stream stream = new FileStream(m_xmlFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                EventsList = (List<string>)m_xs.Deserialize(stream);
                stream.Close();
            }
            else
            {
                EventsList = [];
            }
        }

        public void DumpEventsListToFile(List<string> eventList)
        {
            if (!IsReplay)
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
            List<string> eventList = new();
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
