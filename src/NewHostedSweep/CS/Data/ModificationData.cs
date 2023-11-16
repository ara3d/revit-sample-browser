// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using System.Drawing.Design;
using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data
{
    /// <summary>
    ///     This class contains the data for hosted sweep modification.
    /// </summary>
    public class ModificationData
    {
        /// <summary>
        ///     Creation data can be modified.
        /// </summary>
        private readonly CreationData m_creationData;

        /// <summary>
        ///     Element to modify.
        /// </summary>
        private readonly HostedSweep m_elemToModify;

        /// <summary>
        ///     Revit active document.
        /// </summary>
        private readonly Document m_rvtDoc;

        /// <summary>
        ///     Revit UI document.
        /// </summary>
        private readonly UIDocument m_rvtUiDoc;

        /// <summary>
        ///     Sub transaction
        /// </summary>
        private readonly Transaction m_transaction;

        /// <summary>
        ///     Constructor with HostedSweep and CreationData as parameters.
        /// </summary>
        /// <param name="elem">Element to modify</param>
        /// <param name="creationData">CreationData</param>
        public ModificationData(HostedSweep elem, CreationData creationData)
        {
            m_rvtDoc = creationData.Creator.RvtDocument;
            m_rvtUiDoc = creationData.Creator.RvtUiDocument;
            m_elemToModify = elem;
            m_creationData = creationData;

            m_transaction = new Transaction(m_rvtDoc, "External Tool");

            m_creationData.EdgeAdded +=
                m_creationData_EdgeAdded;
            m_creationData.EdgeRemoved +=
                m_creationData_EdgeRemoved;
            m_creationData.SymbolChanged +=
                m_creationData_SymbolChanged;
        }

        /// <summary>
        ///     Name of the Creator.
        /// </summary>
        public string CreatorName => m_creationData.Creator.Name;

        /// <summary>
        ///     Name will be displayed in property grid.
        /// </summary>
        [Category("Identity Data")]
        public string Name
        {
            get
            {
                var result = "[Id:" + m_elemToModify.Id + "] ";
                return result + m_elemToModify.Name;
            }
        }

        /// <summary>
        ///     HostedSweep Angle property.
        /// </summary>
        [Category("Profile")]
        public string Angle
        {
            get
            {
                var angle = GetParameter("Angle");
                if (angle != null)
                    return angle.AsValueString();
                return m_elemToModify.Angle.ToString();
            }
            set
            {
                try
                {
                    StartTransaction();
                    var angle = GetParameter("Angle");
                    if (angle != null)
                        angle.SetValueString(value);
                    else
                        m_elemToModify.Angle = double.Parse(value);
                    CommitTransaction();
                }
                catch
                {
                    RollbackTransaction();
                }
            }
        }

        /// <summary>
        ///     HostedSweep profiles edges, the edges can be removed or added in the
        ///     pop up dialog.
        /// </summary>
        [TypeConverter(typeof(CreationDataTypeConverter))]
        [Editor(typeof(EdgeFormUiTypeEditor), typeof(UITypeEditor))]
        [Category("Profile")]
        [DisplayName("Profile Edges")]
        public CreationData AddOrRemoveSegments => m_creationData;

        /// <summary>
        ///     HostedSweep Length property.
        /// </summary>
        [Category("Dimensions")]
        public string Length
        {
            get
            {
                var length = GetParameter("Length");
                return length != null ? length.AsValueString() : m_elemToModify.Length.ToString();
            }
        }

        /// <summary>
        ///     HostedSweep HorizontalFlipped property.
        /// </summary>
        [Category("Constraints")]
        [DisplayName("Horizontal Profile Flipped")]
        public bool HorizontalFlipped
        {
            get => m_elemToModify.HorizontalFlipped;
            set
            {
                if (value != m_elemToModify.HorizontalFlipped)
                    try
                    {
                        StartTransaction();
                        m_elemToModify.HorizontalFlip();
                        CommitTransaction();
                    }
                    catch
                    {
                        RollbackTransaction();
                    }
            }
        }

        /// <summary>
        ///     HostedSweep HorizontalOffset property.
        /// </summary>
        [Category("Constraints")]
        [DisplayName("Horizontal Profile Offset")]
        public string HorizontalOffset
        {
            get
            {
                var horiOff = GetParameter("Horizontal Profile Offset");
                return horiOff != null ? horiOff.AsValueString() : m_elemToModify.HorizontalOffset.ToString();
            }
            set
            {
                try
                {
                    StartTransaction();
                    var horiOff = GetParameter("Horizontal Profile Offset");
                    if (horiOff != null)
                        horiOff.SetValueString(value);
                    else
                        m_elemToModify.HorizontalOffset = double.Parse(value);
                    CommitTransaction();
                }
                catch
                {
                    RollbackTransaction();
                }
            }
        }

        /// <summary>
        ///     HostedSweep VerticalFlipped property.
        /// </summary>
        [Category("Constraints")]
        [DisplayName("Vertical Profile Flipped")]
        public bool VerticalFlipped
        {
            get => m_elemToModify.VerticalFlipped;
            set
            {
                if (value != m_elemToModify.VerticalFlipped)
                    try
                    {
                        StartTransaction();
                        m_elemToModify.VerticalFlip();
                        CommitTransaction();
                    }
                    catch
                    {
                        RollbackTransaction();
                    }
            }
        }

        /// <summary>
        ///     HostedSweep VerticalOffset property.
        /// </summary>
        [Category("Constraints")]
        [DisplayName("Vertical Profile Offset")]
        public string VerticalOffset
        {
            get
            {
                var vertOff = GetParameter("Vertical Profile Offset");
                return vertOff != null ? vertOff.AsValueString() : m_elemToModify.VerticalOffset.ToString();
            }
            set
            {
                try
                {
                    StartTransaction();
                    var vertOff = GetParameter("Vertical Profile Offset");
                    if (vertOff != null)
                        vertOff.SetValueString(value);
                    else
                        m_elemToModify.VerticalOffset = double.Parse(value);
                    CommitTransaction();
                }
                catch
                {
                    RollbackTransaction();
                }
            }
        }

        /// <summary>
        ///     Change the symbol of the HostedSweep.
        /// </summary>
        /// <param name="sym"></param>
        private void m_creationData_SymbolChanged(ElementType sym)
        {
            try
            {
                StartTransaction();
                m_elemToModify.ChangeTypeId(sym.Id);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
            }
        }

        /// <summary>
        ///     Remove the edge from the HostedSweep.
        /// </summary>
        /// <param name="edge"></param>
        private void m_creationData_EdgeRemoved(Edge edge)
        {
            try
            {
                StartTransaction();
                m_elemToModify.RemoveSegment(edge.Reference);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
            }
        }

        /// <summary>
        ///     Add the edge to the HostedSweep.
        /// </summary>
        /// <param name="edge"></param>
        private void m_creationData_EdgeAdded(Edge edge)
        {
            try
            {
                StartTransaction();
                switch (m_elemToModify)
                {
                    case Fascia fascia:
                        fascia.AddSegment(edge.Reference);
                        break;
                    case Gutter gutter:
                        gutter.AddSegment(edge.Reference);
                        break;
                    case SlabEdge slabEdge:
                        slabEdge.AddSegment(edge.Reference);
                        break;
                }
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
            }
        }

        /// <summary>
        ///     Show the element in a good view.
        /// </summary>
        public void ShowElement()
        {
            try
            {
                StartTransaction();
                m_rvtUiDoc.ShowElements(m_elemToModify);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
            }
        }

        /// <summary>
        ///     Get parameter by given name.
        /// </summary>
        /// <param name="name">name of parameter</param>
        /// <returns>parameter whose definition name is the given name.</returns>
        protected Parameter GetParameter(string name)
        {
            return m_elemToModify.LookupParameter(name);
        }

        public TransactionStatus StartTransaction()
        {
            return m_transaction.Start();
        }

        public TransactionStatus CommitTransaction()
        {
            return m_transaction.Commit();
        }

        public TransactionStatus RollbackTransaction()
        {
            return m_transaction.RollBack();
        }
    }
}
