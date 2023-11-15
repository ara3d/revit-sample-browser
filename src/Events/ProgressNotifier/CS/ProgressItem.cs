// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.ProgressNotifier.CS
{
    /// <summary>
    ///     An object containing a progress event name, position, and status.
    /// </summary>
    public class ProgressItem
    {
        /// <summary>
        ///     Flag of progress
        /// </summary>
        private bool m_done;

        /// <summary>
        ///     Lower
        /// </summary>
        private int m_lower;

        /// <summary>
        ///     Name
        /// </summary>
        private string m_name;

        /// <summary>
        ///     Position
        /// </summary>
        private int m_position;

        /// <summary>
        ///     Progress stage
        /// </summary>
        private ProgressStage m_stage;

        /// <summary>
        ///     Upper
        /// </summary>
        private int m_upper;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <param name="position"></param>
        /// <param name="stage"></param>
        public ProgressItem(string name, int lower, int upper, int position, ProgressStage stage)
        {
            m_name = name;
            m_lower = lower;
            m_upper = upper;
            m_position = position;
            m_done = false;
            m_stage = stage;
        }

        /// <summary>
        ///     Name property
        /// </summary>
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

        /// <summary>
        ///     IsDone property
        /// </summary>
        public bool IsDone
        {
            get => m_done;
            set => m_done = value;
        }

        /// <summary>
        ///     Lower property
        /// </summary>
        public int Lower
        {
            get => m_lower;
            set => m_lower = value;
        }

        /// <summary>
        ///     Upper property
        /// </summary>
        public int Upper
        {
            get => m_upper;
            set => m_upper = value;
        }

        /// <summary>
        ///     Position property
        /// </summary>
        public int Position
        {
            get => m_position;
            set => m_position = value;
        }

        /// <summary>
        ///     progress stage property
        /// </summary>
        public ProgressStage Stage
        {
            get => m_stage;
            set => m_stage = value;
        }

        /// <summary>
        ///     percent of progress
        /// </summary>
        /// <returns></returns>
        public double PercentDone()
        {
            return m_position / (double)(m_upper - m_lower) * 100;
        }

        /// <summary>
        ///     ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Name: " + Name + ", Stage: " + m_stage + ", Percent Done: " + PercentDone().ToString("F") +
                   ", Upper: " + m_upper + ", Position: " + m_position;
        }
    }
}
