// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.ComponentModel;
using Autodesk.Revit.ApplicationServices;

namespace Revit.SDK.Samples.ProjectInfo.CS
{
    /// <summary>
    ///     Wrapper class for ProjectInfo
    /// </summary>
    public class ProjectInfoWrapper : IWrapper
    {
        /// <summary>
        ///     ProjectInfo
        /// </summary>
        private readonly Autodesk.Revit.DB.ProjectInfo m_projectInfo;

        /// <summary>
        ///     Initializes private variables.
        /// </summary>
        /// <param name="projectInfo">ProjectInfo</param>
        public ProjectInfoWrapper(Autodesk.Revit.DB.ProjectInfo projectInfo)
        {
            m_projectInfo = projectInfo;
        }

        /// <summary>
        ///     Gets gbXMLSettings
        /// </summary>
        [Category("Energy Analysis")]
        [DisplayName("Energy Settings")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public ICustomTypeDescriptor EnergyDataSettings =>
            new WrapperCustomDescriptor(new EnergyDataSettingsWrapper(m_projectInfo.Document));

        /// <summary>
        ///     Gets or sets Project Issue Data
        /// </summary>
        [Category("Other")]
        [DisplayName("Project Issue Data")]
        public string IssueDate
        {
            get => m_projectInfo.IssueDate;
            set => m_projectInfo.IssueDate = value;
        }

        /// <summary>
        ///     Gets or sets Project Status
        /// </summary>
        [Category("Other")]
        [DisplayName("Project Status")]
        public string Status
        {
            get => m_projectInfo.Status;
            set => m_projectInfo.Status = value;
        }

        /// <summary>
        ///     Gets or sets Client Name
        /// </summary>
        [Category("Other")]
        [DisplayName("Client Name")]
        public string ClientName
        {
            get => m_projectInfo.ClientName;
            set => m_projectInfo.ClientName = value;
        }

        /// <summary>
        ///     Gets or sets Project Address
        /// </summary>
        [Category("Other")]
        [DisplayName("Project Address")]
        public string Address
        {
            get => m_projectInfo.Address;
            set => m_projectInfo.Address = value;
        }

        /// <summary>
        ///     Gets or sets Project Number
        /// </summary>
        [Category("Other")]
        [DisplayName("Project Number")]
        public string Number
        {
            get => m_projectInfo.Number;
            set => m_projectInfo.Number = value;
        }


        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        [Browsable(false)]
        public object Handle => m_projectInfo;

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        [Category("Other")]
        [DisplayName("Project Name")]
        public string Name
        {
            get => m_projectInfo.Name;
            set => m_projectInfo.Name = value;
        }
    }
}
