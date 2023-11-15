// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;

namespace Ara3D.RevitSampleBrowser.UIAPI.CS
{
    public enum ApplicationAvailablity
    {
        Always,
        ArchitectureDiscipline,
        StructuralAnalysis,
        Mep
    }

    public class ApplicationOptions
    {
        private static ApplicationOptions _sOptions;
        private ExternalApp m_eApplication;

        private ApplicationOptions()
        {
            Availability = ApplicationAvailablity.Always;
        }

        public ApplicationAvailablity Availability { get; set; }

        public static void Initialize(ExternalApp application)
        {
            _sOptions = new ApplicationOptions
            {
                m_eApplication = application
            };
        }

        public static ApplicationOptions Get()
        {
            if (_sOptions == null)
                throw new Exception("Static options was not initialized");

            return _sOptions;
        }
    }
}
