// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.ComponentModel;
using System.Drawing.Design;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Data
{
    /// <summary>
    ///     This class contains the data for hosted sweep modification.
    /// </summary>
    public class ModificationData
    {
        private readonly HostedSweep m_elemToModify;

        private readonly Document m_rvtDoc;

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
            AddOrRemoveSegments = creationData;

            m_transaction = new Transaction(m_rvtDoc, "External Tool");

            AddOrRemoveSegments.EdgeAdded +=
                m_creationData_EdgeAdded;
            AddOrRemoveSegments.EdgeRemoved +=
                m_creationData_EdgeRemoved;
            AddOrRemoveSegments.SymbolChanged +=
                m_creationData_SymbolChanged;
        }

        public string CreatorName => AddOrRemoveSegments.Creator.Name;

        /// <summary>
        ///     Name will be displayed in property grid.
        /// </summary>
        [Category("Identity Data")]
        public string Name
        {
            get
            {
                var result = $"[Id:{m_elemToModify.Id}] ";
                return result + m_elemToModify.Name;
            }
        }

        [Category("Profile")]
        public string Angle
        {
            get
            {
                var angle = GetParameter("Angle");
                return angle != null ? angle.AsValueString() : m_elemToModify.Angle.ToString();
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
        public CreationData AddOrRemoveSegments { get; }

        [Category("Dimensions")]
        public string Length
        {
            get
            {
                var length = GetParameter("Length");
                return length != null ? length.AsValueString() : m_elemToModify.Length.ToString();
            }
        }

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
