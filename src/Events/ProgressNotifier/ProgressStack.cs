// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.DB.Events;

namespace Ara3D.RevitSampleBrowser.Events.ProgressNotifier.CS
{
    public class ProgressStack
    {
        public readonly Stack<ProgressItem> ItemStack;

        public ProgressStack()
        {
            ItemStack = new Stack<ProgressItem>();
        }

        public ProgressItem AddEventData(ProgressChangedEventArgs progressEvent)
        {
            ProgressItem currentProgressItem = null;

            switch (progressEvent.Stage)
            {
                case ProgressStage.Started:
                {
                    var pi = new ProgressItem(progressEvent.Caption, progressEvent.LowerRange, progressEvent.UpperRange,
                        progressEvent.Position, progressEvent.Stage);
                    ItemStack.Push(pi);
                    currentProgressItem = pi;
                    break;
                }
                case ProgressStage.PositionChanged:
                {
                    var pi = ItemStack.Peek();
                    if (pi.Name != progressEvent.Caption) Debug.WriteLine("Name not matching?");
                    pi.Position = progressEvent.Position;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.RangeChanged:
                {
                    var pi = ItemStack.Peek();
                    pi.Upper = progressEvent.UpperRange;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.Finished:
                {
                    var pi = ItemStack.Pop();
                    pi.IsDone = true;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.CaptionChanged:
                {
                    var pi = ItemStack.Peek();
                    pi.Name = progressEvent.Caption;
                    pi.Stage = progressEvent.Stage;
                    Debug.WriteLine("Caption Change at top.");
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.Unchanged:
                {
                    Debug.WriteLine("Idle.");
                    currentProgressItem = new ProgressItem(progressEvent.Caption, progressEvent.LowerRange,
                        progressEvent.UpperRange, progressEvent.Position, progressEvent.Stage);
                    break;
                }

                default:
                    throw new Exception("Unknown stage.");
            }

            if (ItemStack.Count == 0)
                Debug.WriteLine("Stack empty");
            else
                Debug.WriteLine(ToString());

            return currentProgressItem;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-ProgressBar Stack-");
            foreach (var pi in ItemStack)
            {
                sb.AppendLine(pi.ToString());
            }

            return sb.ToString();
        }

        public List<string> ToStringList(int padDepth = 0)
        {
            var itemList = new List<string>();

            if (padDepth != 0)
            {
                var padding = padDepth - ItemStack.Count;

                for (var index = 0; index != padding; ++index) itemList.Add("");
            }

            foreach (var pi in ItemStack)
            {
                itemList.Add(pi.ToString());
            }

            return itemList;
        }
    }
}
