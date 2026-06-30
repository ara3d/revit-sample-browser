// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.Events.ProgressNotifier.CS
{
    public class ProgressItem
    {
        public ProgressItem(string name, int lower, int upper, int position, ProgressStage stage)
        {
            Name = name;
            Lower = lower;
            Upper = upper;
            Position = position;
            IsDone = false;
            Stage = stage;
        }

        public string Name
        {
            get => string.IsNullOrEmpty(field) || field == " " ? "<None>" : (field);
            set;
        }

        public bool IsDone { get; set; }

        public int Lower { get; set; }

        public int Upper { get; set; }

        public int Position { get; set; }

        public ProgressStage Stage { get; set; }

        public double PercentDone()
        {
            return Position / (double)(Upper - Lower) * 100;
        }

        public override string ToString()
        {
            return $"Name: {Name}, Stage: {Stage}, Percent Done: {PercentDone():F}, Upper: {Upper}, Position: {Position}";
        }
    }
}
