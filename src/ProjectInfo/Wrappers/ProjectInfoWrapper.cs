// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters;
using Autodesk.Revit.ApplicationServices;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class ProjectInfoWrapper : IWrapper
    {
        private readonly Autodesk.Revit.DB.ProjectInfo m_projectInfo;

        public ProjectInfoWrapper(Autodesk.Revit.DB.ProjectInfo projectInfo)
        {
            m_projectInfo = projectInfo;
        }

        [Category("Energy Analysis")]
        [DisplayName("Energy Settings")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public ICustomTypeDescriptor EnergyDataSettings =>
            new WrapperCustomDescriptor(new EnergyDataSettingsWrapper(m_projectInfo.Document));

        [Category("Other")]
        [DisplayName("Project Issue Data")]
        public string IssueDate
        {
            get => m_projectInfo.IssueDate;
            set => m_projectInfo.IssueDate = value;
        }

        [Category("Other")]
        [DisplayName("Project Status")]
        public string Status
        {
            get => m_projectInfo.Status;
            set => m_projectInfo.Status = value;
        }

        [Category("Other")]
        [DisplayName("Client Name")]
        public string ClientName
        {
            get => m_projectInfo.ClientName;
            set => m_projectInfo.ClientName = value;
        }

        [Category("Other")]
        [DisplayName("Project Address")]
        public string Address
        {
            get => m_projectInfo.Address;
            set => m_projectInfo.Address = value;
        }

        [Category("Other")]
        [DisplayName("Project Number")]
        public string Number
        {
            get => m_projectInfo.Number;
            set => m_projectInfo.Number = value;
        }

        [Browsable(false)]
        public object Handle => m_projectInfo;

        [Category("Other")]
        [DisplayName("Project Name")]
        public string Name
        {
            get => m_projectInfo.Name;
            set => m_projectInfo.Name = value;
        }
    }
}
