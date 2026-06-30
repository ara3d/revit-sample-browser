// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.Events.ProgressNotifier.CS
{
    public class ProgressItem
    {
        private bool m_done;
        private int m_lower;
        private string m_name;
        private int m_position;
        private ProgressStage m_stage;
        private int m_upper;

        public ProgressItem(string name, int lower, int upper, int position, ProgressStage stage)
        {
            m_name = name;
            m_lower = lower;
            m_upper = upper;
            m_position = position;
            m_done = false;
            m_stage = stage;
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_name) || m_name == " ")
                    return "<None>";
                return m_name;
            }
            set => m_name = value;
        }

        public bool IsDone
        {
            get => m_done;
            set => m_done = value;
        }

        public int Lower
        {
            get => m_lower;
            set => m_lower = value;
        }

        public int Upper
        {
            get => m_upper;
            set => m_upper = value;
        }

        public int Position
        {
            get => m_position;
            set => m_position = value;
        }

        public ProgressStage Stage
        {
            get => m_stage;
            set => m_stage = value;
        }

        public double PercentDone() => m_position / (double)(m_upper - m_lower) * 100;

        public override string ToString() =>
            $"Name: {Name}, Stage: {m_stage}, Percent Done: {PercentDone():F}, Upper: {m_upper}, Position: {m_position}";
    }
}
