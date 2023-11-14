// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.DB.Events;

namespace Revit.SDK.Samples.ProgressNotifier.CS
{
    /// <summary>
    ///     A collection of ProgressItem objects arranged in a stack
    /// </summary>
    public class ProgressStack
    {
        /// <summary>
        ///     ProgressItem stack
        /// </summary>
        public readonly Stack<ProgressItem> m_itemStack;

        /// <summary>
        ///     Constructor
        /// </summary>
        public ProgressStack()
        {
            m_itemStack = new Stack<ProgressItem>();
        }

        /// <summary>
        ///     Add event data
        /// </summary>
        /// <param name="progressEvent"></param>
        /// <returns></returns>
        public ProgressItem AddEventData(ProgressChangedEventArgs progressEvent)
        {
            ProgressItem currentProgressItem = null;


            switch (progressEvent.Stage)
            {
                case ProgressStage.Started:
                {
                    var pi = new ProgressItem(progressEvent.Caption, progressEvent.LowerRange, progressEvent.UpperRange,
                        progressEvent.Position, progressEvent.Stage);
                    m_itemStack.Push(pi);
                    currentProgressItem = pi;
                    break;
                }
                case ProgressStage.PositionChanged:
                {
                    var pi = m_itemStack.Peek();
                    if (pi.Name != progressEvent.Caption) Debug.WriteLine("Name not matching?");
                    pi.Position = progressEvent.Position;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.RangeChanged:
                {
                    var pi = m_itemStack.Peek();
                    pi.Upper = progressEvent.UpperRange;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.Finished:
                {
                    var pi = m_itemStack.Pop();
                    pi.IsDone = true;
                    pi.Stage = progressEvent.Stage;
                    currentProgressItem = pi;
                    break;
                }

                case ProgressStage.CaptionChanged:
                {
                    var pi = m_itemStack.Peek();
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


            if (m_itemStack.Count == 0)
                Debug.WriteLine("Stack empty");
            else
                Debug.WriteLine(ToString());

            return currentProgressItem;
        }

        /// <summary>
        ///     ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-ProgressBar Stack-");
            foreach (var pi in m_itemStack) sb.AppendLine(pi.ToString());
            return sb.ToString();
        }


        /// <summary>
        ///     ToStringList
        /// </summary>
        /// <param name="padDepth"></param>
        /// <returns></returns>
        public List<string> ToStringList(int padDepth = 0)
        {
            var itemList = new List<string>();

            if (padDepth != 0)
            {
                var padding = padDepth - m_itemStack.Count;

                for (var index = 0; index != padding; ++index) itemList.Add("");
            }

            foreach (var pi in m_itemStack) itemList.Add(pi.ToString());
            return itemList;
        }
    }
}
